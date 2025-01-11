using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Collections.Generic;

public static class ListRecordsInCosmosDB
{
    [FunctionName("ListRecordsInCosmosDB")]
    public static async Task<IEnumerable<MyDataModel>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "list")] HttpRequest req,
        [CosmosDB(
            databaseName: "myDatabase",
            collectionName: "myCollection",
            ConnectionStringSetting = "CosmosDBConnectionString")] IDocumentClient client,
        ILogger log)
    {
        var collectionUri = UriFactory.CreateDocumentCollectionUri("myDatabase", "myCollection");

        var query = client.CreateDocumentQuery<MyDataModel>(
            collectionUri,
            "SELECT * FROM c"
        );

        var results = query.ToList();

        log.LogInformation("Listando todos os registros no CosmosDB.");
        return results;
    }
}
