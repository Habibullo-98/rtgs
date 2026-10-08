using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RTGS.Models.DB;

namespace RTGS.Utils;

public abstract class ValidateUtil
{
    public static bool IsValidRequest(string key, Partner? partner, string? clientIp, out ObjectResult response)
    {
        response = ResponseUtil.Unauthorized();

        if (partner == null)
        {
            response = ResponseUtil.PartnerNotFound();
            return false;
        }

        if (clientIp == null || !partner.Ip.Contains(clientIp)) return false;
        if (key != SecurityUtil.GetMD5Hash(partner.Key + partner.Id)) return false;

        return true;
    }

    public static string GetFirstError(ModelStateDictionary modelState)
    {
        return modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault() ?? "";
    }
}