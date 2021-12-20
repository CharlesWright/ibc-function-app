using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Interrobang.Media
{
    public static class MediaHttp
    {
        [Function("MediaHttp")]
        public async static Task<HttpResponseData> GetImage([HttpTrigger(AuthorizationLevel.Function, "get", Route="media/{path}/{name}")] HttpRequestData req,
            string path,
            string name,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SongsHttp");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
            BlobServiceClient serviceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient($"{path}/{name}");
            BlobProperties properties = await blobClient.GetPropertiesAsync();
            response.Headers.Add("Content-Type", properties.ContentType);

            var blobStream = await blobClient.OpenReadAsync().ConfigureAwait(false);
            await blobStream.CopyToAsync(response.Body);
            return response;
        }
    }
}
