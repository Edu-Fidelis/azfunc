# azfunc
 Criando um Gerenciador de Catálogos da Netflix com Azure Functions e Banco de Dados


Aqui está um exemplo de como você pode implementar cada uma das etapas mencionadas para criar uma aplicação em **Azure Functions** que interage com o **Azure Storage** e o **CosmosDB**:

### 1. **Criando a Infraestrutura em Cloud**
Antes de começar, você precisa configurar os seguintes recursos no **Azure**:

- **Storage Account**: Para armazenar arquivos.
- **CosmosDB**: Para armazenar e consultar dados.
- **Azure Functions**: Para orquestrar as operações e interações com o **Storage Account** e **CosmosDB**.

### 2. **Criando uma Azure Function para Salvar Arquivos no Storage Account**

Para isso, você pode criar uma **Azure Function** que recebe um arquivo e o salva no **Blob Storage**.

#### Código da Azure Function para salvar arquivos no **Storage Account**:

```csharp
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;

public static class SaveFileToStorage
{
    [FunctionName("SaveFileToStorage")]
    public static async Task Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload")] HttpRequest req,
        [Blob("files/{rand-guid}", FileAccess.Write)] CloudBlockBlob blob,
        ILogger log)
    {
        // Obter arquivo da requisição
        var file = req.Form.Files[0];
        
        // Salvar o arquivo no Blob Storage
        using (var stream = file.OpenReadStream())
        {
            await blob.UploadFromStreamAsync(stream);
        }
        
        log.LogInformation($"Arquivo {file.FileName} salvo no Blob Storage.");
    }
}
```

- Esta **Azure Function** recebe um arquivo via requisição HTTP `POST` e o salva no **Blob Storage** usando o nome aleatório do arquivo gerado pelo parâmetro `{rand-guid}`.

### 3. **Criando uma Azure Function para Salvar em CosmosDB**

Agora, vamos criar uma **Azure Function** que recebe dados de um formulário e os salva no **CosmosDB**.

#### Código da Azure Function para salvar em **CosmosDB**:

```csharp
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
```

- Este exemplo mostra como salvar dados no **CosmosDB** usando a **Azure Function**. Aqui, um objeto de dados (`MyDataModel`) é recebido e armazenado como um documento no CosmosDB.

### 4. **Criando uma Azure Function para Filtrar Registros no CosmosDB**

Aqui está uma **Azure Function** que consulta o **CosmosDB** com um filtro simples.

#### Código da Azure Function para filtrar registros no **CosmosDB**:

```csharp
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
```

- A função realiza uma consulta simples no **CosmosDB** e filtra os registros com base no valor de idade (`age`).

### 5. **Criando uma Azure Function para Listar Registros no CosmosDB**

Aqui está o código para listar todos os registros do **CosmosDB**.

#### Código da Azure Function para listar registros no **CosmosDB**:

```csharp
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
```

- Esta função consulta todos os documentos armazenados na coleção do **CosmosDB** e retorna uma lista de registros.

---

### **Resumo dos Passos**:

1. **Azure Functions** são criadas para interagir com o **Azure Storage** e o **CosmosDB**.
2. **Azure Function 1**: Recebe arquivos via HTTP e os armazena no **Blob Storage**.
3. **Azure Function 2**: Recebe dados via HTTP e os salva no **CosmosDB**.
4. **Azure Function 3**: Filtra registros no **CosmosDB** com base em parâmetros fornecidos.
5. **Azure Function 4**: Lista todos os registros presentes no **CosmosDB**.

Certifique-se de configurar os serviços do **CosmosDB** e **Storage Account** adequadamente no **Azure** e fornecer as **chaves de conexão** necessárias no arquivo `local.settings.json` ou como configurações de ambiente para as **Azure Functions**.
