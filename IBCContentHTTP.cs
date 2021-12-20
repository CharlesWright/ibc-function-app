
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;


namespace Interrobang.Content
{
    public static class IBCContentHTTP
    {
        [Function("GetContent")]
        public static async Task<HttpResponseData> GetContent([HttpTrigger(AuthorizationLevel.Function, "get", Route = "content/{name}")] HttpRequestData req,
            string name,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("IBCContentHTTP");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
            BlobServiceClient serviceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(name); 

            string content;
            var blobResponse = await blobClient.DownloadAsync();
            using (var streamReader = new StreamReader(blobResponse.Value.Content)) {
                content = await streamReader.ReadToEndAsync();
            }
            response.WriteString(content);
            return response;
        }

        [Function("GetContentRecord")]
        public static HttpResponseData GetContentRecord([HttpTrigger(AuthorizationLevel.Function, "get", Route = "table-content")] HttpRequestData req,
            string id,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("IBCContentHTTP");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
            TableServiceClient serviceClient = new TableServiceClient(connectionString);
            TableClient tableClient = serviceClient.GetTableClient("content");
            var queryResultsLINQ = tableClient.Query<ContentEntity>(ent => ent.PartitionKey == "IBC" && ent.RowKey == id);
            var entity = queryResultsLINQ.SingleOrDefault<ContentEntity>();
            string text = JsonSerializer.Serialize(entity);
            response.WriteString(text);
            return response;
        }
    }
}
