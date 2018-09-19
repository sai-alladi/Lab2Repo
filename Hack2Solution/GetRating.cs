
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Hack2Solution.Models;

namespace Hack2Solution
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "rating/{id}")]HttpRequest req,
             [CosmosDB(
                databaseName: "UserRatingsDB",
                collectionName: "Rating",
                ConnectionStringSetting = "CosmosDBConnection",
                 Id ="{id}")]UserRating userRating,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

         

            return userRating != null
                ? (ActionResult)new OkObjectResult(userRating)
                : new BadRequestObjectResult("No Result Found");
        }
    }
}
