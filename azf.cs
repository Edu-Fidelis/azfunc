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
