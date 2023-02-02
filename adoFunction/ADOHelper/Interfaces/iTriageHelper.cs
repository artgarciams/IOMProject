using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOPipeline.ADOHelper.Interfaces
{
    public interface ITriageHelper
    {
        public void CalculateBANT(WorkItem curOp);
        public void CalculateCustomerReadiness(WorkItem curOp);
        public void CalculateCSEAllignment(WorkItem curOp);
        public void CalculateSecurityMarkers(WorkItem curOp);
        public void CalculateLegal(WorkItem curOp);
        public void CalculateValueMarkers(WorkItem curOp);

        public void CalculateRiskMarkers(WorkItem curOp);

        public void UpdateTotalCheckListValue(WorkItem curOp);
        public void GetWorkItemHistory(WorkItem curOp);

    }
}
