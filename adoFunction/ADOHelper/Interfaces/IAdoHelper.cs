using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOPipeline.ADOHelper.Interfaces
{
    public interface IAdoHelper
    {
        public Task<List<WorkItem>> GetOpportunities();
        public Task<WorkItem> GetOpportunity(int id);
        public Task<WorkItem> UpdateOpportunity(int Id, string field, string value);
        public int GetWorkItemIdFromQueueMessage(string message);
        public Task<List<WorkItemHistory>> GetWorkItemHistory(int Id);

        public Task<Comment> AddComment(int id, string comment, string projectGuid);

    }
}
