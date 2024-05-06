using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SwimTaykaStreak
{
    /// <summary>
    /// Represents the main class for handling updates of JustGiving fundraising amounts within Streak CRM.
    /// </summary>
    public class SwimTaykaStreak
    {
        // Interfaces for Streak CRM and JustGiving scraping services.
        private readonly IStreakClient _streakClient;
        private readonly IJustGivingScrape _justGivingScrape;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwimTaykaStreak"/> class.
        /// </summary>
        /// <param name="streakClient">The client used to interact with Streak CRM.</param>
        /// <param name="justGivingScrape">The scraper used to obtain fundraising amounts from JustGiving.</param>
        public SwimTaykaStreak(IConfiguration configuration, IStreakClient streakClient, IJustGivingScrape justGivingScrape)
        {
            _streakClient = streakClient;
            _justGivingScrape = justGivingScrape;
        }

        /// <summary>
        /// Azure Function to update the raised amount in Streak based on the JustGiving page.
        /// </summary>
        /// <param name="req">The HTTP request triggering this function.</param>
        /// <param name="log">Logger for tracking the operation.</param>
        /// <returns>An IActionResult that indicates the result of the operation, including error messages if failed.</returns>
        [FunctionName("UpdateJustGivingRaisedAmountInStreak")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("UpdateJustGivingRaisedAmountInStreak Started.");

            // Extract the x-StreakAPI header value
            string streakAPIKey = req.Headers["x-StreakAPI"];

            if (string.IsNullOrEmpty(streakAPIKey))
            {
                return new BadRequestObjectResult("x-StreakAPI header is missing.");
            }

            // Retrieves the 'boxKey' from the query parameters.
            string boxKey = req.Query["boxKey"];
            if (string.IsNullOrEmpty(boxKey))
            {
                return new BadRequestObjectResult("Please provide a 'boxKey' query parameter.");
            }

            // Retrieves the 'boxKey' from the query parameters.
            string fieldId = req.Query["fieldid"];
            if (string.IsNullOrEmpty(fieldId))
            {
                return new BadRequestObjectResult("Please provide a 'fieldId' query parameter.");
            }

            // Retrieves the 'boxKey' from the query parameters.
            string fieldToUpdateId = req.Query["fieldtoupdateid"];
            if (string.IsNullOrEmpty(fieldToUpdateId))
            {
                return new BadRequestObjectResult("Please provide a 'fieldToUpdateId' query parameter.");
            }

            // Fetches all related box keys from Streak CRM.
            var boxKeys = await _streakClient.GetBoxKeysAsync(streakAPIKey, boxKey);
            if (boxKeys == null || !boxKeys.Any())
            {
                return new NotFoundObjectResult("No box keys found for the specified box key.");
            }

            // Updates each box with the new fundraising amount and collects the results.
            var updateTasks = boxKeys.Select(key => UpdateBoxAmountForKey(streakAPIKey, key, fieldId, fieldToUpdateId));

            var results = await Task.WhenAll(updateTasks);

            log.LogInformation("UpdateJustGivingRaisedAmountInStreak Completed.");
            return new OkObjectResult(results.Where(url => !string.IsNullOrEmpty(url)).ToList());
        }

        /// <summary>
        /// Helper method to update a single box's fundraising amount.
        /// </summary>
        /// <param name="key">The key of the box to update.</param>
        /// <returns>The URL of the JustGiving page if updated successfully, null otherwise.</returns>
        private async Task<string> UpdateBoxAmountForKey(string streakAPIKey, string key, string fieldId, string fieldToUpdateId)
        {
            var url = await _streakClient.GetBoxUrlAsync(streakAPIKey, key, fieldId);
            if (!string.IsNullOrEmpty(url))
            {
                // Scrapes the current fundraising amount and updates the corresponding Streak field.
                var amountRaised = await _justGivingScrape.GetRaisedAmount(url);
                await _streakClient.PostStreakField(streakAPIKey, key, amountRaised, fieldToUpdateId);
                return url; // Return URL to indicate which boxes were updated.
            }
            return null;
        }
    }
}
