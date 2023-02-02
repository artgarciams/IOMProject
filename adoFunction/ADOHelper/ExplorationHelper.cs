using ADOPipeline.ADOHelper;
using ADOPipeline.ADOHelper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPipline.ExplorationHelper
{
    public class ExplorationHelper : iExplorationHelper
    {
        private ILogger logger;
        private IAdoHelper adoHelper;
        private AdoHelper adoHelper1;
        private decimal expTotal;

        public ExplorationHelper(IAdoHelper adoHelper, ILogger logger)
        {
            this.logger = logger;
            this.adoHelper = adoHelper;
        }

        public void CalculateExplorationValues(WorkItem curOp)
        {
            expTotal = 0;

            CalculateCSEReadiness(curOp);
            CalculateCustomerReadiness(curOp);
            CalculateDomainExpertise(curOp);
            CalculateEngineeringFit(curOp);

            // update exploration totals
            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.ex_summary" + "", expTotal.ToString()).Result;

        }

        public void CalculateCSEReadiness(WorkItem curOp)
        {
            var SCOPE = curOp.Fields.Where(f => f.Key == "Custom.ex_understandscope").FirstOrDefault();
            var RESOURCE = curOp.Fields.Where(f => f.Key == "Custom.ex_resourcesrequired").FirstOrDefault();

            float cSCOREscore = 0;
            if (SCOPE.Value != null)
            {
                cSCOREscore = int.Parse(SCOPE.Value.ToString()[..1]);
            }

            float cRESOURCEscore = 0;
            if (RESOURCE.Value != null)
            {
                cRESOURCEscore = int.Parse(RESOURCE.Value.ToString()[..1]);
            }

            double netScore = cSCOREscore + cRESOURCEscore / 2 ;
            netScore = Math.Round(netScore, 2);
            
            expTotal =+ (decimal)netScore;

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.ex_csereadiness_value" + "", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a CSE Readiness score of : {1}", curOp.Id.ToString(), netScore));
        }

        public void CalculateCustomerReadiness(WorkItem curOp)
        {

            var SPONSOR = curOp.Fields.Where(f => f.Key == "Custom.ex_sponsortimeline").FirstOrDefault();
            var TIMELINE = curOp.Fields.Where(f => f.Key == "Custom.ex_timeline").FirstOrDefault();
            var RESOURCE = curOp.Fields.Where(f => f.Key == "Custom.ex_resourcesrequired").FirstOrDefault();
            var CODEWITH = curOp.Fields.Where(f => f.Key == "Custom.ex_codewithteam").FirstOrDefault();

            float cSPONSORscore = 0;
            if (SPONSOR.Value != null)
            {
                cSPONSORscore = int.Parse(SPONSOR.Value.ToString()[..1]);
            }


            float cTIMELINEscore = 0;
            if (TIMELINE.Value != null)
            {
                cTIMELINEscore = int.Parse(TIMELINE.Value.ToString()[..1]);
            }

            float cRESOURCEscore = 0;
            if (RESOURCE.Value != null)
            {
                cRESOURCEscore = int.Parse(RESOURCE.Value.ToString()[..1]);
            }


            float cCODEWITHScore = 0;
            if (CODEWITH.Value != null)
            {
                cCODEWITHScore = int.Parse(CODEWITH.Value.ToString()[..1]);
            }

            double netScore = (cSPONSORscore + cTIMELINEscore + cRESOURCEscore + cCODEWITHScore / 4);
            netScore = Math.Round(netScore, 2);

            expTotal = +(decimal)netScore;

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.ex_customer_value" + "", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a Customer Readiness score of : {1}", curOp.Id.ToString(), netScore));
        }

        public void CalculateDomainExpertise(WorkItem curOp)
        {
            var NEED = curOp.Fields.Where(f => f.Key == "Custom.ex_articulate_need").FirstOrDefault();
            var SAMPLE = curOp.Fields.Where(f => f.Key == "Custom.ex_examplestoshare").FirstOrDefault();
            var USERS = curOp.Fields.Where(f => f.Key == "Custom.ex_understand_users").FirstOrDefault();

            float cNEEDScore = 0;
            if (NEED.Value != null)
            {
                cNEEDScore = int.Parse(NEED.Value.ToString()[..1]);
            }

            double netScore = (cNEEDScore + (double)SAMPLE.Value + (double)USERS.Value / 3);
            netScore = Math.Round(netScore, 2);

            expTotal = +(decimal)netScore;

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.ex_domain_value" + "", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a Domain Expertise score of : {1}", curOp.Id.ToString(), netScore));
        }

        public void CalculateEngineeringFit(WorkItem curOp)
        {
            var COMPLEX = curOp.Fields.Where(f => f.Key == "Custom.ex_complexity").FirstOrDefault();
            var UNIQUE = curOp.Fields.Where(f => f.Key == "Custom.ex_project_unique").FirstOrDefault();
            var ALIGN = curOp.Fields.Where(f => f.Key == "Custom.ex_strategic_align").FirstOrDefault();

            double netScore = ((double)COMPLEX.Value + (double)UNIQUE.Value + (double)ALIGN.Value / 3);
            netScore = Math.Round(netScore, 2);

            expTotal = +(decimal)netScore;

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.ex_engineer_value" + "", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a Engineering Fit score of : {1}", curOp.Id.ToString(), netScore));
        }

    }

}
