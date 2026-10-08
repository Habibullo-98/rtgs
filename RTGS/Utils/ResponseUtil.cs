using Serilog;
using Microsoft.AspNetCore.Mvc;
using RTGS.Models.Controller;

namespace RTGS.Utils;

public abstract class ResponseUtil
{
    public static BadRequestObjectResult InvalidRequest(string errorMessage)
    {
        var response = new Response { Result = -1, Comment = errorMessage };
        Log.Warning("[RESPONSE UTIL] Invalid request: {Error}", errorMessage);
        return new BadRequestObjectResult(response);
    }
    
    public static NotFoundObjectResult TransactionNotFound()
    {
        var response = new Response { Result = 0, Comment = "Transaction not found" };
        Log.Warning("[RESPONSE UTIL] Transaction not found");
        return new NotFoundObjectResult(response);
    }

    public static OkObjectResult InProcess()
    {
        var response = new Response { Result = 1, Comment = "In process" };
        Log.Information("[RESPONSE UTIL] In process");
        return new OkObjectResult(response);
    }

    public static OkObjectResult Successful()
    {
        var response = new Response { Result = 2, Comment = "Successful" };
        Log.Information("[RESPONSE UTIL] Successful");
        return new OkObjectResult(response);
    }

    public static ObjectResult Failed()
    {
        var response = new Response { Result = 3, Comment = "Failed" };
        Log.Warning("[RESPONSE UTIL] Failed");
        return new ObjectResult(response){StatusCode = 500};
    }

    public static ObjectResult Duplicate()
    {
        var response = new Response { Result = -1, Comment = "Duplicate" };
        Log.Warning("[RESPONSE UTIL] Duplicate transaction");
        return new ObjectResult(response){StatusCode = 500};
    }

    public static NotFoundObjectResult PartnerNotFound()
    {
        var response = new Response { Result = -1, Comment = "Partner not found" };
        Log.Warning("[RESPONSE UTIL] Partner not found");
        return new NotFoundObjectResult(response);
    }

    public static UnauthorizedObjectResult Unauthorized()
    {
        var response = new Response { Result = -1, Comment = "Unauthorized" };
        Log.Warning("[RESPONSE UTIL] Unauthorized");
        return new UnauthorizedObjectResult(response);
    }
}