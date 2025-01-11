using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

public static class SaveToCosmosDB
{
    [FunctionName("SaveToCosmosDB")]
    public static async Task Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "save")] HttpRequest req,
        [CosmosDB(
            databaseName: "myDatabase", 
            collectionName: "myCollection", 
            ConnectionStringSetting = "CosmosDBConnectionString")] IAsyncCollector<Document> documents,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<MyDataModel>(requestBody);

        // Adiciona o documento ao CosmosDB
        await documents.AddAsync(new Document
        {
            Id = Guid.NewGuid().ToString(),
            Data = data
        });
        
        log.LogInformation($"Dado {data} salvo no CosmosDB.");
    }
}

public class MyDataModel
{
    public string Name { get; set; }
    public int Age { get; set; }
}
