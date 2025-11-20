using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;

namespace Npruehs.KitaStats
{
    public static class Endpoint
    {
        [Function("UpdateCareLevel")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext context)
        {
            var log = context.GetLogger("UpdateCareLevel");
            log.LogInformation("Function processed a request.");

            if (!string.Equals(req.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteStringAsync("UpdateCareLevel endpoint is up. Send POST with JSON payload to record an event.");
                return ok;
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                log.LogWarning("Empty request body.");
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Request body is empty.");
                return bad;
            }

            Payload payload;
            try
            {
                payload = JsonConvert.DeserializeObject<Payload>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Failed to deserialize request body.");
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Invalid JSON payload.");
                return bad;
            }

            if (payload == null
                || string.IsNullOrWhiteSpace(payload.Company)
                || string.IsNullOrWhiteSpace(payload.Kita)
                || string.IsNullOrWhiteSpace(payload.CareLevel)
                || string.IsNullOrWhiteSpace(payload.UserId))
            {
                log.LogWarning("Missing required fields in payload: {@Payload}", payload);
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Payload must include 'company', 'kita', 'careLevel' and 'userId'.");
                return bad;
            }

            var now = DateTime.UtcNow;

            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                log.LogError("AzureWebJobsStorage connection string is not configured.");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            try
            {
                var tableName = "CareLevels";
                var tableClient = new TableClient(storageConnectionString, tableName);
                await tableClient.CreateIfNotExistsAsync();

                var entity = new TableEntity(payload.Company, Guid.NewGuid().ToString())
                {
                    { "Kita", payload.Kita },
                    { "CareLevel", payload.CareLevel },
                    { "UserId", payload.UserId },
                    { "RecordedAt", now.ToString("o") }
                };

                await tableClient.AddEntityAsync(entity);

                log.LogInformation("Stored care event: Company={Company}, Kita={Kita}, CareLevel={CareLevel}, UserId={UserId}, Time={Time}",
                    payload.Company, payload.Kita, payload.CareLevel, payload.UserId, now);

                var created = req.CreateResponse(HttpStatusCode.Created);
                return created;
            }
            catch (RequestFailedException rfe)
            {
                log.LogError(rfe, "Azure Table storage request failed.");
                return req.CreateResponse(HttpStatusCode.BadGateway);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unexpected error while processing UpdateCareLevel.");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private class Payload
        {
            [JsonProperty("company")] public string Company { get; set; }
            [JsonProperty("kita")] public string Kita { get; set; }
            [JsonProperty("careLevel")] public string CareLevel { get; set; }
            [JsonProperty("userId")] public string UserId { get; set; }
        }
    }
}
