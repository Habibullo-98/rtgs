using System.Data;
using RTGS.Configs;
using RTGS.Models.DB;
using RTGS.Services.Http;
using RTGS.Services.DB;
using RTGS.Utils;

namespace RTGS.Services.Background;

public class StatusCheck(ILogger<StatusCheck> logger, Abs abs, Db db, Config config) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[STATUS CHECK] Starting status check...");

        while (!cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("[STATUS CHECK] Task executed at: {DateTime}", DateTime.Now);
            await PerformStatusUpdate();
            await Task.Delay(20000, cancellationToken);
        }
        
        logger.LogInformation("[STATUS CHECK] Background status check is stopping");
    }

    private async Task PerformStatusUpdate()
    {
        var transactions = await db.GetTransactions(Status.InProgress);
        foreach (var transaction in transactions)
        {
            await UpdateStatus(transaction);
        }
    }

    private async Task UpdateStatus(Transaction transaction)
    {
        try
        {
            if (await abs.CheckTransactionStatus(transaction))
            {
                transaction.Status = Status.Success;
                FileUtil.Move(Path.Combine(config.TransitDir, transaction.FileName()), config.OutputHistoryDir);
            }
            else
            {
                FileUtil.Move(Path.Combine(config.TransitDir, transaction.FileName()), config.OutputDir);
                transaction.Status = Status.Failure;
            }

            await db.UpdateTransaction(transaction);
            logger.LogError("[STATUS CHECK] Transaction {Id} updated: status = {Status}", transaction.Id, transaction.Status);
        }
        catch (Exception e)
        {
            logger.LogError("[STATUS CHECK] Failed to update transaction {Id} status: err = {Error}", transaction.Id, e.Message);
        }

    }
}