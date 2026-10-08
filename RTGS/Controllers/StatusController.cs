using Microsoft.AspNetCore.Mvc;
using RTGS.Models.Controller;
using RTGS.Models.DB;
using RTGS.Utils;
using RTGS.Services.DB;

namespace RTGS.Controllers;

[ApiController]
[Route("/checkstatus")]
public class StatusController(ILogger<StatusController> logger, Db db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response>> GetStatus([FromBody] StatusRequest request)
    {
        logger.LogInformation("[STATUS CONTROLLER] Get status request: {@Request}", request);
        if (!ModelState.IsValid) return ResponseUtil.InvalidRequest(ValidateUtil.GetFirstError(ModelState));
        
        try
        {
            var transaction = await db.GetTransaction(int.Parse(request.Id));
            var partner = await db.GetPartner(request.PartnerId);
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (transaction == null) return ResponseUtil.TransactionNotFound();
            if (!ValidateUtil.IsValidRequest(request.Key, partner, clientIp, out var response)) return response;

            switch (transaction.Status)
            {
                case Status.InProgress: return ResponseUtil.InProcess();
                case Status.Success: return ResponseUtil.Successful();
                case Status.Failure: default: return ResponseUtil.Failed();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[STATUS CONTROLLER] Failed to get status request: err {Error}", ex.Message);
        }
        
        return ResponseUtil.Failed();
    }
}