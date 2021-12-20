using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Interrobang.Content;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Interrobang.Albums
{
    public static class AlbumsHttp
    {
        [Function("AlbumsHttp")]
        public static HttpResponseData GetAlbums([HttpTrigger(AuthorizationLevel.Function, "get", Route ="albums")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SongsHttp");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
            TableServiceClient serviceClient = new TableServiceClient(connectionString);
            TableClient tableClient = serviceClient.GetTableClient("albums");
            var queryResultsLINQ = tableClient.Query<AlbumEntity>(ent => ent.PartitionKey == "IBC");
            var songs = queryResultsLINQ.ToArray<AlbumEntity>();
            string text = JsonSerializer.Serialize(songs);
            response.WriteString(text);
            return response;
        }

        public static HttpResponseData GetAlbumById([HttpTrigger(AuthorizationLevel.Function, "get", Route ="albums/{id}")] HttpRequestData req,
            string id,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SongsHttp");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
            TableServiceClient serviceClient = new TableServiceClient(connectionString);
            TableClient tableClient = serviceClient.GetTableClient("albums");
            var queryResultsLINQ = tableClient.Query<AlbumEntity>(ent => ent.PartitionKey == "IBC" && ent.RowKey == id);
            var songs = queryResultsLINQ.ToArray<AlbumEntity>();
            string text = JsonSerializer.Serialize(songs);
            response.WriteString(text);
            return response;
        }
    }
}
