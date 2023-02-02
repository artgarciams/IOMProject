using ADOPipeline.ADO.Helper;
using ADOPipeline.ADOHelper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;


namespace ADOPipeline.ADOHelper
{
    public class AdoHelper : IAdoHelper
    {

        private string collectionUri;
        private string pat;
        private ILogger logger;
        private WorkItemTrackingHttpClient adoClient;

        private WorkItemHistory adoHistory;

        public AdoHelper(string collectionUri, string patToken, ILogger logger)
        {
            this.collectionUri = collectionUri;
            this.pat = patToken;
            this.logger = logger;
            InitializeClient();
        }

        public AdoHelper(ILogger logger)
        {
            this.logger = logger;
        }

        private void InitializeClient()
        {
            try
            {
                var creds = new VssBasicCredential(String.Empty, pat);

                // Connect to Azure DevOps Services
                var connection = new VssConnection(new Uri(collectionUri), creds);

                // Get a GitHttpClient to talk to the Git endpoints
                this.adoClient = connection.GetClient<WorkItemTrackingHttpClient>();              
            }
            catch (Exception ex)
            {
                logger.LogError(ex, " ** Error encountered ** " + ex.Message + " *** " + ex.StackTrace + " *** ");               
            }
          
        }

        public Task<List<WorkItemHistory>> GetWorkItemHistory(int Id)
        {

            return null;
        }

        public async Task<List<WorkItem>> GetOpportunities()
        {
            // create a wiql object and build our query
            string prject = "Gov Opportunity Pipeline";
            string areapth = "Gov Opportunity Pipeline\\Americas";
            string areapath = " AND [System.AreaPath] = 'Gov Opportunity Pipeline\\Americas'";

        var wiql = new Wiql()
            {              
                Query = "Select [Id] " + 
                "From WorkItems Where [System.WorkItemType]  = 'Government opportunity' " +  areapath

            };
                       
            var result = await adoClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
            var ids = result.WorkItems.Select(item => item.Id).ToArray();

            var workItems = await adoClient.GetWorkItemsAsync(ids);

            return workItems;
        }

        public async Task<WorkItem> GetOpportunity(int id)
        {
            var workItem = await adoClient.GetWorkItemAsync(id);
            return workItem;
        }

        public Task<List<WorkItemUpdate>> GetAllWorkItemUpdates(int id)
        {
          var retrn = adoClient.GetUpdatesAsync(id);
          return Task.FromResult(retrn.Result);

        }

        public int GetWorkItemIdFromQueueMessage(string message)
        {
            int wID = 0;
            var workItemDetails = JsonConvert.DeserializeObject<WorkItemChangeTrigger>(message);
            if (workItemDetails != null)
            {
                wID = workItemDetails.resource.workItemId;
                logger.LogInformation(string.Format("Work Item Id: {0}", wID.ToString()));
            }
            else
            {
                throw new Exception("*** Unable to parse response ***");
            }
            return wID;
        }

        public async Task<WorkItem> UpdateOpportunity(int Id, string field, string value)
        {
            logger.LogInformation(string.Format("Attempting updated of Work Item Id: {0}. Field: {1}, Value: {2}", Id, field, value));
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add(
                                   new JsonPatchOperation()
                                   {
                                       Operation = Operation.Replace,
                                       Path = "/fields/" + field,
                                       Value = value
                                   }
                               );
            var result = await adoClient.UpdateWorkItemAsync(patchDocument, Id);         

            return (result);
        }

        public async Task<Comment> AddComment(int id, string comment, string projectGuid)
        {
            logger.LogInformation(string.Format("Attempting add comment to Work Item Id: {0}. , Value: {1}", id, comment));

            var addComment = new CommentCreate()
            {
                Text = comment

            };
                        
            var result = await adoClient.AddCommentAsync(addComment, projectGuid, id);
            
            return result;

        }


    }
}