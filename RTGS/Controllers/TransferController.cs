using Microsoft.AspNetCore.Mvc;
using RTGS.Models.Controller;
using RTGS.Utils;
using RTGS.Services.DB;
using RTGS.Configs;

namespace RTGS.Controllers;

[ApiController]
[Route("/transfer")]
public class TransferController(ILogger<TransferController> logger, Config config, Db db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response>> Transfer([FromBody] TransferRequest request)
    {
        logger.LogInformation("[TRANSFER CONTROLLER] Get status request: {@Request}", request);
        if (!ModelState.IsValid) return ResponseUtil.InvalidRequest(ValidateUtil.GetFirstError(ModelState));
        
        try
        {
            var partner = await db.GetPartner(request.PartnerId);
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (!ValidateUtil.IsValidRequest(request.Key, partner, clientIp, out var response)) return response;
            if (db.TransactionExists(int.Parse(request.Id), request.PartnerId)) return ResponseUtil.Duplicate();

            var id = await db.AddTransaction(ModelUtils.ToTransaction(request));

            var document = request.ToDataPdu();
            var docString = XmlUtils.Serialize(document);
            await FileUtil.Save(docString, Path.Combine(config.InputDir, $"RTGS PACS_008 Transfer {id}.xml"));
            await FileUtil.Save(docString, Path.Combine(config.InputHistoryDir, $"RTGS PACS_008 Transfer {id}.xml"));
            
            return ResponseUtil.InProcess();
        }
        catch (Exception ex)
        {
            logger.LogError("[TRANSFER CONTROLLER] Transfer request failed: {Error}", ex.Message);
            return ResponseUtil.Failed();
        }
    }
}