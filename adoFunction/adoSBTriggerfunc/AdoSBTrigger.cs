using AdoPipline.ADOHelper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace ADOPipeline.adoSBTriggerfunc
{
    public class AdoSBTrigger
    {
        private const string V = "Gov Opportunity Pipeline\\Americas";
        private ILogger logger;       
        private CalculationHelper CalculationHelper;


        [FunctionName("adoSBTrigger")]
        public void Run([ServiceBusTrigger("adotestqueue", Connection = "DefaultConnection")]string myQueueItem, ILogger logger)
        {
            this.logger = logger;

            logger.LogInformation($"****C# ServiceBus queue trigger function processed message: {myQueueItem}");

            // get environment variables to access ADO REST API
            string CollectionUrl = Environment.GetEnvironmentVariable("CollectionUri");
            string PATKey = Environment.GetEnvironmentVariable("PATKEY");
             
            // initialize helper class 
            ADOPipeline.ADOHelper.AdoHelper adoHelper = new ADOPipeline.ADOHelper.AdoHelper(CollectionUrl, PATKey, logger);
            
            // get work item id from the queue message
            var wID = adoHelper.GetWorkItemIdFromQueueMessage(myQueueItem);

            // find work item in ado
            var curWorkItem = adoHelper.GetOpportunity(wID).Result;
            var areapath = curWorkItem.Fields.Where(f => f.Key == "System.AreaPath").FirstOrDefault();
            var workItemType = curWorkItem.Fields.Where(f => f.Key == "System.WorkItemType").FirstOrDefault();
                       

            // only process government opportunities for now           
            switch (workItemType.Value.ToString().ToLower().Trim())
            {
                case "FSI Portfolio":

                case "m e opportunity":

                    try
                    {
                        logger.LogInformation("***** M E Opportunity Opportunity found *****");

                        // get formula from blob storage
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));

                        // get reference to blob storage
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                        CloudBlobContainer blobContainer = blobClient.GetContainerReference("opportunitytype");
                        var blob = blobContainer.GetBlobReference(Environment.GetEnvironmentVariable(workItemType.Value.ToString().ToLower()));

                        var serializer = new JsonSerializer();

                        Opportunity FieldList = new Opportunity();

                        using (var stream = new MemoryStream())
                        {
                            blob.DownloadToStreamAsync(stream).Wait();
                            stream.Position = 0;

                            using (var sr = new StreamReader(stream))
                            using (var jsonTextReader = new JsonTextReader(sr))
                            {
                                FieldList = serializer.Deserialize<Opportunity>(jsonTextReader);
                            }
                        }

                        logger.LogInformation("***** Begin calculation of M E Opportunity found *****");

                        // initilaize class and calculate values
                        CalculationHelper = new CalculationHelper(adoHelper, logger);
                        CalculationHelper.CalculateScores(curWorkItem, FieldList);

                        logger.LogInformation("***** End calculation of M E Opportunity found *****");

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, " ** Error encountered ** " + ex.Message + " *** " + ex.StackTrace + " *** ");
                    }

                    logger.LogInformation(string.Format("Work Item Id: {0} Processed", wID.ToString()));
                    break;

                case "government opportunity":
                    // calculate government Opprtunity

                    try
                    {
                        logger.LogInformation("***** Government Opportunity found *****");

                        // get formula from blob storage
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));

                        // get reference to blob storage
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                        CloudBlobContainer blobContainer = blobClient.GetContainerReference("opportunitytype");
                        var blob = blobContainer.GetBlobReference(Environment.GetEnvironmentVariable(workItemType.Value.ToString().ToLower()));
                        //var blob = blobContainer.GetBlobReference("Government.json");

                        var serializer = new JsonSerializer();

                        Opportunity FieldList = new Opportunity();

                        using (var stream = new MemoryStream())
                        {
                            blob.DownloadToStreamAsync(stream).Wait();
                            stream.Position = 0;

                            using (var sr = new StreamReader(stream))
                            using (var jsonTextReader = new JsonTextReader(sr))
                            {
                                FieldList = serializer.Deserialize<Opportunity>(jsonTextReader);
                            }
                        }

                        logger.LogInformation("***** Begin calculation of Government Opportunity found *****");

                        // initilaize class and calculate values
                        CalculationHelper = new CalculationHelper(adoHelper, logger);
                        CalculationHelper.CalculateScores(curWorkItem, FieldList);

                        logger.LogInformation("***** End calculation of Government Opportunity found *****");

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, " ** Error encountered ** " + ex.Message + " *** " + ex.StackTrace + " *** ");
                    }
                    
                    logger.LogInformation(string.Format("Work Item Id: {0} Processed", wID.ToString()));
                    break;

                default:
                    logger.LogInformation(string.Format("Work Item Id: {0} Not Processed. WorkItems of type {1} are not supported in this function", wID.ToString(), workItemType.Value));
                    break;
            }

        }



    }
}
