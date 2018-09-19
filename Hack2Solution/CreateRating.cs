
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Hack2Solution.Models;
using ServiceStack;

namespace Hack2Solution
{
    public static class CreateRating
    {
        [FunctionName("CreateRatings")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post", Route = null)]HttpRequest req,
              [CosmosDB(
                databaseName: "UserRatingsDB",
                collectionName: "Rating",
                ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector< dynamic> document,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserRating data = JsonConvert.DeserializeObject<UserRating>(requestBody);

            var prodExists = IfProductExists(data.productId);
            if (!prodExists)
                return (ActionResult)new BadRequestObjectResult("Product does not exists");
            var userExists = IfUserExists(data.userId);
            if (!userExists)
                return (ActionResult)new BadRequestObjectResult("User does not exists");
            if (!IfRatingIsValid(data.rating))
                return (ActionResult)new BadRequestObjectResult("Rating is out of the range");

            data.id = Guid.NewGuid().ToString();
            data.timestamp = DateTime.UtcNow;

            await document.AddAsync(data);

            return data != null
                ? (ActionResult)new OkObjectResult(data)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }


        private static bool IfRatingIsValid(int rating)
        {
            if (rating > 0 && rating <= 5)
                return true;

            return false;

        }
        static bool IfProductExists(string prodId)
        {
            string prodApiUrl = "https://serverlessohproduct.trafficmanager.net/api/GetProduct";
            prodApiUrl = prodApiUrl.AddQueryParam("productId", prodId);
            try
            {
                //TODO: how to handle the error situation here
                var apiResponse = prodApiUrl.GetStringFromUrl(prodApiUrl);
                return apiResponse != "Product does not exist" ? true : false;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        static bool IfUserExists(string userId)
        {
            string userApiUrl = "https://serverlessohuser.trafficmanager.net/api/GetUser";
            userApiUrl = userApiUrl.AddQueryParam("userId", userId);
            try
            {
                var apiResponse = userApiUrl.GetStringFromUrl(userApiUrl);
                return apiResponse != "User does not exist" ? true : false;
            }
            catch (Exception)
            {

                return false;
            }

        }
    }
}
