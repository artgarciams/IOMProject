using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOPipeline.ADOHelper.Interfaces
{
    public interface iExplorationHelper
    {
        public void CalculateExplorationValues(WorkItem curOp);
        public void CalculateCSEReadiness(WorkItem curOp);
        public void CalculateCustomerReadiness(WorkItem curOp);
        public void CalculateDomainExpertise(WorkItem curOp);
        public void CalculateEngineeringFit(WorkItem curOp);        
    }
}
