using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;

namespace Npruehs.KitaStats
{
    public static class Endpoint
    {
        [FunctionName("UpdateCareLevel")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!HttpMethods.IsPost(req.Method))
            {
                // Keep GET support for a simple health check
                return new OkObjectResult("UpdateCareLevel endpoint is up. Send POST with JSON payload to record an event.");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                log.LogWarning("Empty request body.");
                return new BadRequestObjectResult("Request body is empty.");
            }

            Payload payload = null;
            try
            {
                payload = JsonConvert.DeserializeObject<Payload>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Failed to deserialize request body.");
                return new BadRequestObjectResult("Invalid JSON payload.");
            }

            if (payload == null
                || string.IsNullOrWhiteSpace(payload.Company)
                || string.IsNullOrWhiteSpace(payload.Kita)
                || string.IsNullOrWhiteSpace(payload.CareLevel)
                || string.IsNullOrWhiteSpace(payload.UserId))
            {
                log.LogWarning("Missing required fields in payload: {@Payload}", payload);
                return new BadRequestObjectResult("Payload must include 'company', 'kita', 'careLevel' and 'userId'.");
            }

            var now = DateTime.UtcNow;

            // Prepare table client
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                log.LogError("AzureWebJobsStorage connection string is not configured.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            try
            {
                var tableName = "CareLevels";
                var tableClient = new TableClient(storageConnectionString, tableName);
                await tableClient.CreateIfNotExistsAsync();

                // Use Company as PartitionKey and a GUID as RowKey
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

                // 201 Created - resource created
                return new StatusCodeResult(StatusCodes.Status201Created);
            }
            catch (RequestFailedException rfe)
            {
                log.LogError(rfe, "Azure Table storage request failed.");
                return new StatusCodeResult(StatusCodes.Status502BadGateway);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unexpected error while processing UpdateCareLevel.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
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
