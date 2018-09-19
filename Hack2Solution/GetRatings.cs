
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
using System.Collections;
using System.Collections.Generic;
using Hack2Solution.Models;

namespace Hack2Solution
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "ratings/{userid}")]HttpRequest req,
           [CosmosDB(
                databaseName: "UserRatingsDB",
                collectionName: "Rating",
                ConnectionStringSetting = "CosmosDBConnection",
                 SqlQuery = "select * from Rating c where c.userId={userid}")]IEnumerable<UserRating> userRatings
            , ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            return userRatings != null
                ? (ActionResult)new OkObjectResult(userRatings)
                : new BadRequestObjectResult("No Record Founds");
        }
    }
}
