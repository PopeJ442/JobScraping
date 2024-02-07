using HtmlAgilityPack;
using NabsGroup.Data;
using NabsGroup.Models;
using NabsGroup.Services;

namespace NabsGroup.Service
{
    public class Service : IService
    {
        private readonly ApplicationDbContext dbContext;

        public Service(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Main()
        {
            string baseUrl = "https://jobs.nabsgroupgh.com/browse-jobs-without-map/";
            int totalPages = GetTotalPages(baseUrl);

            for (int currentPage = 1; currentPage <= 2; currentPage++)
            {
                string url = $"{baseUrl}page/{currentPage}";

                var htmlWeb = new HtmlWeb();
                var htmlDocument = htmlWeb.Load(url);

                string xpath = "//li[contains(@class, 'job-listing')]";

                var jobListingNodes = htmlDocument.DocumentNode.SelectNodes(xpath);

                if (jobListingNodes != null)
                {
                    foreach (var jobListingNode in jobListingNodes)
                    {
                        string title = jobListingNode.SelectSingleNode(".//h3[@class='job-listing-title']")?.InnerText.Trim();
                        string location = jobListingNode.SelectSingleNode(".//li[@class='job-listing-footer-location']")?.InnerText.Trim();
                        string jobUrl = jobListingNode.SelectSingleNode(".//a[@class='workscout-grid-job-link-handler']")?.GetAttributeValue("href", "");

                        // Check if title and location combination already exists in the database
                        bool jobExists = dbContext.Staging_NABSGROUP.Any(j => j.JobTitle == title && j.Location == location);

                        // If job doesn't exist, add it to the database
                        if (!jobExists && !string.IsNullOrEmpty(title))
                        {
                            string jobDescription = jobListingNode.SelectSingleNode(".//div[@class='job-listing-description']")?.InnerText.Trim();

                            var job = new Job
                            {
                                Id = Guid.NewGuid().ToString(),
                                JobTitle = title,
                                JobUrl = jobUrl,
                                JobDescription = jobDescription,
                                Location = location,
                                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                            };

                            dbContext.Staging_NABSGROUP.Add(job);

                            Console.WriteLine("Title: " + title);
                            Console.WriteLine("Location: " + location);
                            Console.WriteLine("Description: " + jobDescription);
                            Console.WriteLine("CreatedAt: " + DateOnly.FromDateTime(DateTime.Now));
                            Console.WriteLine("URL: " + jobUrl);
                            Console.WriteLine("----------------------------------------");
                        }
                        else
                        {
                            Console.WriteLine($"Job with title '{title}' and location '{location}' already exists. Skipping...");
                        }
                    }

                    dbContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine("No job listings found on the page.");
                }
            }
        }

        // Helper method to get the total number of pages
        private int GetTotalPages(string baseUrl)
        {
            var htmlWeb = new HtmlWeb();
            var htmlDocument = htmlWeb.Load(baseUrl);

            var totalPagesNode = htmlDocument.DocumentNode.SelectSingleNode("//ul[@class='page-numbers']/li[last()-1]/a");

            if (totalPagesNode != null && int.TryParse(totalPagesNode.InnerText.Trim(), out int totalPages))
            {
                return totalPages;
            }

            return 1; // Default to 1 page if total pages cannot be determined
        }
    }
}
