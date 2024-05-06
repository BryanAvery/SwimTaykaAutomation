using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwimTaykaStreak
{
    /// <summary>
    /// Implements the <see cref="IJustGivingScrape"/> interface to scrape fundraising amounts from JustGiving pages using HTTP requests.
    /// </summary>
    public class JustGivingScrape : IJustGivingScrape
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JustGivingScrape> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JustGivingScrape"/> class with specified HttpClient and ILogger.
        /// </summary>
        /// <param name="httpClient">The HttpClient used for making requests.</param>
        /// <param name="logger">The logger used for logging information and errors.</param>
        public JustGivingScrape(ILogger<JustGivingScrape> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Asynchronously scrapes the JustGiving page at the specified URL and extracts the fundraising amount.
        /// </summary>
        /// <param name="url">The URL of the JustGiving page to scrape.</param>
        /// <returns>A <see cref="Task{String}"/> that represents the asynchronous operation, containing the raised amount as a string, or an empty string if the amount cannot be determined.</returns>
        public async Task<string> GetRaisedAmount(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                _logger?.LogWarning("Attempted to scrape a JustGiving page with an empty URL.");
                return string.Empty;
            }

            try
            {
                var response = await _httpClient.GetAsync(url);
                var pageContents = await response.Content.ReadAsStringAsync();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageContents);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'cp-heading-large') and contains(@class, 'branded-text')]");

                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        _logger?.LogDebug($"AmountRaised: {node.InnerText.Trim()}");
                        return node.InnerText.Trim();
                    }
                }
                _logger?.LogWarning("No fundraising amount found on the JustGiving page.");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred while scraping the JustGiving page.");
                return string.Empty;
            }
        }
    }
}
