using System.Text;
using System.Text.Json;
using RTGS.Models.Abs;
using RTGS.Configs;
using RTGS.Models.DB;

namespace RTGS.Services.Http;

public class Abs(Config config, HttpClient httpClient, ILogger<Abs> logger)
{
    public async Task<bool> Send(Request requestBody)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {config.AbsAuthKey}");
            var request = new HttpRequestMessage(HttpMethod.Get, config.AbsUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                logger.LogInformation("[ABS] Request sent successfully: content = {Content}", responseData);
                return true;
            }
            logger.LogInformation("[ABS] Request failed: code = {StatusCode}", response.StatusCode);
            return false;
        }
        catch (HttpRequestException e)
        {
            logger.LogInformation("[ABS] Request failed: err = {Error}", e.Message);
            return false;
        }
    }

    public async Task<bool> CheckTransactionStatus(Transaction transaction)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {config.AbsAuthKey}");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{config.AbsTransferUrl}{transaction.Id}");
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var absResponse = JsonSerializer.Deserialize<Response>(responseData);
                
                logger.LogInformation("[ABS] Transaction {Id} check sent successfully: content = {Content}", transaction.Id, absResponse);
                if (absResponse == null) return true;
                return absResponse.Type == null || absResponse.Type.ToUpper() != "TECHNICAL";
            }

            logger.LogInformation("[ABS] Transaction {Id} check failed: code = {StatusCode}", transaction.Id, response.StatusCode);
            return false;
        }
        catch (HttpRequestException e)
        {
            logger.LogInformation("[ABS] Transaction {Id} check failed: err = {Error}", transaction.Id, e.Message);
            return false;
        }
    }
}