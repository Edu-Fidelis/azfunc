using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Collections.Generic;

public static class FilterRecordsInCosmosDB
{
    [FunctionName("FilterRecordsInCosmosDB")]
    public static async Task<IEnumerable<MyDataModel>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "filter/{age}")] HttpRequest req,
        int age,
        [CosmosDB(
            databaseName: "myDatabase",
            collectionName: "myCollection",
            ConnectionStringSetting = "CosmosDBConnectionString")] IDocumentClient client,
        ILogger log)
    {
        var collectionUri = UriFactory.CreateDocumentCollectionUri("myDatabase", "myCollection");

        var query = client.CreateDocumentQuery<MyDataModel>(
            collectionUri,
            $"SELECT * FROM c WHERE c.Age = {age}"
        );

        var results = query.ToList();

        log.LogInformation($"Registros filtrados com a idade {age}.");
        return results;
    }
}
