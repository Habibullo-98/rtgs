using RTGS.Configs;
using RTGS.Models.Abs;
using RTGS.Models.DB;
using RTGS.Models.ISO20022;
using RTGS.Services.DB;
using RTGS.Utils;
using RTGS.Services.Http;

namespace RTGS.Services.Background;

public class FileProcessor(ILogger<FileProcessor> logger, IServiceScopeFactory scopeFactory, Config config, Abs abs) : BackgroundService
{
    private FileSystemWatcher? _watcher;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[OUT READER] Starting OutReader");

        await ProcessExistingFiles(stoppingToken);
        _watcher = new FileSystemWatcher(config.OutputDir)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
            EnableRaisingEvents = true,
            Filter = "*.*"
        };
        
        _watcher.Created += async (_, args) => await OnNewFileDetected(args.FullPath, stoppingToken);
    }
    
    private async Task ProcessExistingFiles(CancellationToken stoppingToken)
    {
        var files = Directory.GetFiles(config.OutputDir);

        foreach (var file in files)
        {
            await ProcessFile(file, stoppingToken);
        }
    }

    private async Task OnNewFileDetected(string filePath, CancellationToken stoppingToken)
    {
        logger.LogInformation("[FILE PROCESSOR] New file detected {File}", filePath);
        await ProcessFile(filePath, stoppingToken);
    }

    private async Task ProcessFile(string filePath, CancellationToken cancellationToken)
    {
        const int maxRetries = 5;
        const int delay = 1000;

        logger.LogInformation("-------------------------------------------------------");
        logger.LogInformation("[FILE PROCESSOR] Begin processing file:{File}", filePath);
        
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("[FILE PROCESSOR] Cancellation requested");
                return;
            }

            if (FileUtil.IsFileLocked(filePath))
            {
                logger.LogWarning("[FILE PROCESSOR] File is locked. Attempt {Attempt} of {Retries}", attempt + 1, maxRetries);
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<Db>();

                var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
                await PerformOperation(filePath, fileContent, db);
                break;
            }
            catch (IOException e)
            {
                logger.LogError(
                    "[FILE PROCESSOR] Error processing file {File}: {Message}. Attempt {Attempt} of {Retries}",
                    filePath, e.Message, attempt + 1, maxRetries);
                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError("[FILE PROCESSOR] Error processing file {file}: {Message}. Attempt {Attempt} of {Retries}", filePath, e.Message, attempt + 1, maxRetries);
                break;
            }
        }
    }

    private async Task PerformOperation(string path, string content, Db db)
    {
        var dataPdu = XmlUtils.Deserialize<DataPdu>(content);
        var msgType = dataPdu.Body.AppHdr.MsgDefIdr[..8];
        logger.LogInformation("[FILE PROCESSOR] Processing {Type} file {File}", msgType, path);

        switch (msgType)
        {
            case "pacs.008.001": await ProcessCustomerCreditTransfer(dataPdu, db);
                FileUtil.Move(path, config.TransitDir);
                break;
            case "pacs.009.001": await ProcessInstitutionCreditTransfer(dataPdu, db);
                FileUtil.Move(path, config.TransitDir);
                break;
            case "camt.054.001": await ProcessDebitCreditNotification(dataPdu, db);
                FileUtil.Move(path, config.OutputHistoryDir);
                break;
            case "camt.053.001": await ProcessStatement(dataPdu, db);
                FileUtil.Move(path, config.OutputHistoryDir);
                break;
            case "camt.052.001": await ProcessAccount(dataPdu, db);
                FileUtil.Move(path, config.OutputHistoryDir);
                break;
            case "camt.025.001": await ProcessReceipt(dataPdu);
                FileUtil.Move(path, config.OutputHistoryDir);
                break;
            default: await ProcessUnsupported(dataPdu);
                FileUtil.Move(path, config.OutputHistoryDir);
                break;
        }
    }

    private async Task ProcessStatement(DataPdu dataPdu, Db db)
    {
        if (dataPdu.Body.Statement == null) return;
        //var transactions = dataPdu.Body.Statement.ToTransactionsList();
        //await db.AddTransactions(transactions);
        logger.LogInformation("[FILE PROCESSOR] CAMT.053 SUCCESS");
    }

    private async Task ProcessAccount(DataPdu dataPdu, Db db)
    {
        if (dataPdu.Body.Account == null) return;
        logger.LogInformation("[FILE PROCESSOR] CAMT.052 SUCCESS");
    }

    private async Task ProcessInstitutionCreditTransfer(DataPdu dataPdu, Db db)
    {
        if (dataPdu.Body.InstitutionCreditTransfer == null) return;

        const int maxRetries = 5;
        const int delay = 1000;
        var transaction = dataPdu.Body.InstitutionCreditTransfer.ToTransaction();
        transaction.Id = await db.AddTransaction(transaction);

        var request = new Request
        {
            MessageId = transaction.Id,
            DataType = "RTGS_IN",
            Data = XmlUtils.Serialize(dataPdu)
        };
        
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            if (await abs.Send(request)) return;
            await Task.Delay(delay);
        }
        
        transaction.Status = Status.Failure;
        logger.LogInformation("[FILE PROCESSOR] PACS.009 FAILED TrID:{ID}", transaction.Id);
        await db.UpdateTransaction(transaction);
    }

    private async Task ProcessCustomerCreditTransfer(DataPdu dataPdu, Db db)
    {
        if (dataPdu.Body.CustomerCreditTransfer == null) return;
        
        const int maxRetries = 5;
        const int delay = 1000;
        var transaction = dataPdu.Body.CustomerCreditTransfer.ToTransaction();
        transaction.Id = await db.AddTransaction(transaction);

        var request = new Request
        {
            MessageId = transaction.Id,
            DataType = "RTGS_IN",
            Data = XmlUtils.Serialize(dataPdu)
        };
        
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            if (await abs.Send(request)) return;
            await Task.Delay(delay);
        }
        
        logger.LogInformation("[FILE PROCESSOR] PACS.008 FAILED TrID:{ID}", transaction.Id);
        transaction.Status = Status.Failure;
        await db.UpdateTransaction(transaction);
    }

    private async Task ProcessDebitCreditNotification(DataPdu dataPdu, Db db)
    {
        if (dataPdu.Body.DebitCreditNotification == null) return;
        var id = dataPdu.Body.DebitCreditNotification.BkToCstmrDbtCdtNtfctn.Ntfctn[0].Ntry[0].NtryDtls[0].TxDtls[0].Refs
            .TxId;
        var transaction = await db.GetTransaction(id);

        if (transaction != null)
        {
            transaction.Status = Status.Success;
            await db.UpdateTransaction(transaction);
            logger.LogInformation("[FILE PROCESSOR] CAMT.054 SUCCESS TrID:{Id}", id);
            return;
        }
        
        logger.LogInformation("[FILE PROCESSOR] CAMT.054 FAILED TrID:{Id} not found", id);
    }

    private async Task ProcessReceipt(DataPdu dataPdu)
    {
        if (dataPdu.Body.Receipt == null) return;
        logger.LogWarning("[FILE PROCESSOR] CAMT.025 Receipt ID:{Id} MsgType:{MsgType} Status:{Status}", 
            dataPdu.Body.Receipt.Rct.RctDtls[0].OrgnlMsgId.MsgId, 
            dataPdu.Body.Receipt.Rct.RctDtls[0].OrgnlMsgId.MsgNmId,
            dataPdu.Body.Receipt.Rct.RctDtls[0].ReqHdlg[0].StsCd);
    }

    private Task ProcessUnsupported(DataPdu dataPdu)
    {
        logger.LogWarning("[FILE PROCESSOR] Unsupported format {Format}", dataPdu.Body.AppHdr.MsgDefIdr);
        return Task.CompletedTask;
    }
}