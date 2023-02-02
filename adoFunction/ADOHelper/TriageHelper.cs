using ADOPipeline.ADOHelper;
using ADOPipeline.ADOHelper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPipline.ADOHelper
{
    public class TriageHelper : ITriageHelper
    {

        private ILogger logger;
        private IAdoHelper adoHelper;
        private AdoHelper adoHelper1;

        public TriageHelper(IAdoHelper adoHelper, ILogger logger)
        {
            this.logger = logger;
            this.adoHelper = adoHelper;
        }
       

        public void CalculateCheckListValue(WorkItem curOp)
        {
           

            CalculateBANT(curOp);   
            CalculateCSEAllignment(curOp);
            CalculateCustomerReadiness(curOp);
            CalculateSecurityMarkers(curOp);
            CalculateLegal(curOp);

            CalculateValueMarkers(curOp);
            CalculateRiskMarkers(curOp);

            UpdateTotalCheckListValue(curOp);
            
            GetWorkItemHistory(curOp);                       
           
        }



        public void GetWorkItemHistory(WorkItem curOp)
        {

        }


        public void UpdateTotalCheckListValue(WorkItem curOp)
        {
           var BANT = curOp.Fields.Where(f => f.Key == "Custom.BANTValue").FirstOrDefault();
           var CSEAlign = curOp.Fields.Where(f => f.Key == "Custom.CSEAlignmentValues").FirstOrDefault();
           var CDERead = curOp.Fields.Where(f => f.Key == "Custom.CustomerReadinessValues").FirstOrDefault();
           var Legal = curOp.Fields.Where(f => f.Key == "Custom.LegalValues").FirstOrDefault();


            double netScore = ((double)BANT.Value + (double)CSEAlign.Value + (double)CDERead.Value + (double)Legal.Value) / 4;
    
            netScore = Math.Round(netScore, 2);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.ChecklistSummary" +
                "", netScore.ToString()).Result;
            
            logger.LogInformation(string.Format("Opportunity Id: {0} has a Legal  score of : {1}", curOp.Id.ToString(), netScore));
                      
        }

        public void CalculateBANT(WorkItem curOp)
        {
            var budget = curOp.Fields.Where(f => f.Key == "Custom.Budget").FirstOrDefault();
            var athority = curOp.Fields.Where(f => f.Key == "Custom.Authority").FirstOrDefault();
            var need = curOp.Fields.Where(f => f.Key == "Custom.Need").FirstOrDefault();
            var timeline = curOp.Fields.Where(f => f.Key == "Custom.TimeLine").FirstOrDefault();
            var azureSub = curOp.Fields.Where(f => f.Key == "Custom.AzureSubscription").FirstOrDefault();

            float budgetScore = 0;
            if (budget.Value != null)
            {
                budgetScore = int.Parse(budget.Value.ToString()[..1]);
            }

            float athorityScore = 0;
            if (athority.Value != null)
            {
                athorityScore = int.Parse(athority.Value.ToString()[..1]);
            }

            float needScore = 0;
            if (need.Value != null)
            {
                needScore = int.Parse(need.Value.ToString()[..1]);
            }

            float timelineScore = 0;
            if (timeline.Value != null)
            {
                timelineScore = int.Parse(timeline.Value.ToString()[..1]);
            }

            float azSubScore = 0;
            if (azureSub.Value != null)
            {
                azSubScore = int.Parse(azureSub.Value.ToString()[..1]);
            }


            double netScore = (budgetScore + athorityScore + needScore + timelineScore + azSubScore) / 5;
            netScore = Math.Round(netScore, 2);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.BANTValue", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a BANT score of : {1}", curOp.Id.ToString(), netScore));

        }

        public void CalculateCSEAllignment(WorkItem curOp)
        {
            var strategicAlignment = curOp.Fields.Where(f => f.Key == "Custom.StrategicAlignment").FirstOrDefault();
            var accountTeamReadiness = curOp.Fields.Where(f => f.Key == "Custom.AccountTeamReadiness").FirstOrDefault();

            float strategicAlignmentScore = 0;
            if (strategicAlignment.Value != null)
            {
                strategicAlignmentScore = int.Parse(strategicAlignment.Value.ToString()[..1]);
            }

            float accountTeamReadinessScore = 0;
            if (accountTeamReadiness.Value != null)
            {
                accountTeamReadinessScore = int.Parse(accountTeamReadiness.Value.ToString()[..1]);
            }

            double netScore = (strategicAlignmentScore + accountTeamReadinessScore) / 2;
            netScore = Math.Round(netScore, 2);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.CSEAlignmentValues", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a CSE Strategic Alignment score of : {1}", curOp.Id.ToString(), netScore));
        }

        public void CalculateCustomerReadiness(WorkItem curOp)
        {
            var cSEProcess = curOp.Fields.Where(f => f.Key == "Custom.CSEProcessCode_WithResourcesIdentified").FirstOrDefault();
            var domainExpertise = curOp.Fields.Where(f => f.Key == "Custom.DomainExpertise").FirstOrDefault();
            var alignedSkillset = curOp.Fields.Where(f => f.Key == "Custom.AllignedSkillset").FirstOrDefault();

            float cSEProcessScore = 0;
            if (cSEProcess.Value != null)
            {
                cSEProcessScore = int.Parse(cSEProcess.Value.ToString()[..1]);
            }

            float domainExpertiseScore = 0;
            if (domainExpertise.Value != null)
            {
                domainExpertiseScore = int.Parse(domainExpertise.Value.ToString()[..1]);
            }

            float alignedSkillsetScore = 0;
            if (alignedSkillset.Value != null)
            {
                alignedSkillsetScore = int.Parse(alignedSkillset.Value.ToString()[..1]);
            }


            double netScore = (cSEProcessScore + domainExpertiseScore + alignedSkillsetScore) / 3;
            netScore = Math.Round(netScore, 2);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.CustomerReadinessValues", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a Customer Readiness Values score of : {1}", curOp.Id.ToString(), netScore));
        }

        public void CalculateSecurityMarkers(WorkItem curOp)
        {
            var istentingrequired = curOp.Fields.Where(f => f.Key == "Custom.Tenting").FirstOrDefault();
            var clearancerequired = curOp.Fields.Where(f => f.Key == "Custom.Clearancerequired").FirstOrDefault();
            var citizenshiprequired = curOp.Fields.Where(f => f.Key == "Custom.USCitizenshiprequired").FirstOrDefault();
            var cACCardrequired = curOp.Fields.Where(f => f.Key == "Custom.CACCardrequired").FirstOrDefault();
            var securityPlusrequired = curOp.Fields.Where(f => f.Key == "Custom.SecurityPlusrequired").FirstOrDefault();
            var trainingrequired = curOp.Fields.Where(f => f.Key == "Custom.Trainingrequired").FirstOrDefault();
            var airGaprequired = curOp.Fields.Where(f => f.Key == "Custom.AirGaprequired").FirstOrDefault();

            double netScore = 0;
            if (istentingrequired.Value != null && bool.Parse(istentingrequired.Value.ToString()) == true)
            {
                netScore++;
            }
            if (clearancerequired.Value != null && bool.Parse(clearancerequired.Value.ToString()) == true)
            {
                netScore++;
            }
            if (citizenshiprequired.Value != null && bool.Parse(citizenshiprequired.Value.ToString()) == true)
            {
                netScore++;
            }
            if (cACCardrequired.Value != null && bool.Parse(cACCardrequired.Value.ToString()) == true)
            {
                netScore++;
            }
            if (securityPlusrequired.Value != null && bool.Parse(securityPlusrequired.Value.ToString()) == true)
            {
                netScore++;
            }
            if (trainingrequired.Value != null && bool.Parse(trainingrequired.Value.ToString()) == true)
            {
                netScore++;
            }
            if (airGaprequired.Value != null && bool.Parse(airGaprequired.Value.ToString()) == true)
            {
                netScore++;
            }

            netScore = netScore / 7;
            netScore = Math.Round(netScore, 2);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.SecurityValues", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a CSE Security Values score of : {1}", curOp.Id.ToString(), netScore));

        }

        public void CalculateLegal(WorkItem curOp)
        {
            var nda = curOp.Fields.Where(f => f.Key == "Custom.NDA").FirstOrDefault();
            var cwaa = curOp.Fields.Where(f => f.Key == "Custom.CWAA").FirstOrDefault();

            float ndaScore = 0;
            if (nda.Value != null)
            {
                ndaScore = int.Parse(nda.Value.ToString()[..1]);
            }

            float cwaaScore = 0;
            if (cwaa.Value != null)
            {
                cwaaScore = int.Parse(cwaa.Value.ToString()[..1]);
            }

            double netScore = (ndaScore + cwaaScore) / 2;
            netScore = Math.Round(netScore, 2);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.LegalValues", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a Legal  score of : {1}", curOp.Id.ToString(), netScore));
        }
        
        public void CalculateRiskMarkers(WorkItem curOp)
        {
            var customerAgileCommitment = curOp.Fields.Where(f => f.Key == "Custom.CustomerAgileCommitment").FirstOrDefault();
            var customerDeadlineFlexibility = curOp.Fields.Where(f => f.Key == "Custom.CustomerDeadlineFlexibility").FirstOrDefault();
            var customerReadiness = curOp.Fields.Where(f => f.Key == "Custom.CustomerReadiness").FirstOrDefault();
            var clarityonPathtoProduction = curOp.Fields.Where(f => f.Key == "Custom.ClarityonPathtoProduction").FirstOrDefault();
            var preDeterminedTechnologiespriortoDesign = curOp.Fields.Where(f => f.Key == "Custom.PreDeterminedTechnologiespriortoDesign").FirstOrDefault();

            float customerAgileCommitmentScore = 0;
            if (customerAgileCommitment.Value != null)
            {
                customerAgileCommitmentScore = int.Parse(customerAgileCommitment.Value.ToString()[..1]);
            }

            float customerDeadlineFlexibilityScore = 0;
            if (customerDeadlineFlexibility.Value != null)
            {
                customerDeadlineFlexibilityScore = int.Parse(customerDeadlineFlexibility.Value.ToString()[..1]);
            }

            float customerReadinessScore = 0;
            if (customerReadiness.Value != null)
            {
                customerReadinessScore = int.Parse(customerReadiness.Value.ToString()[..1]);
            }

            float clarityonPathtoProductionScore = 0;
            if (clarityonPathtoProduction.Value != null)
            {
                clarityonPathtoProductionScore = int.Parse(clarityonPathtoProduction.Value.ToString()[..1]);
            }

            float preDeterminedTechnologiespriortoDesignScore = 0;
            if (preDeterminedTechnologiespriortoDesign.Value != null)
            {
                preDeterminedTechnologiespriortoDesignScore = int.Parse(preDeterminedTechnologiespriortoDesign.Value.ToString()[..1]);
            }

            double netScore = (customerAgileCommitmentScore + customerDeadlineFlexibilityScore + customerReadinessScore + clarityonPathtoProductionScore + preDeterminedTechnologiespriortoDesignScore);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.RiskMarkers", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a CSE Risk Markers score of : {1}", curOp.Id.ToString(), netScore));
        }

        public void CalculateValueMarkers(WorkItem curOp)
        {
            var currentAzureCommit = curOp.Fields.Where(f => f.Key == "Custom.CurrentAzureCommit").FirstOrDefault();
            var successIDACREstimate = curOp.Fields.Where(f => f.Key == "Custom.SuccessIDACREstimate").FirstOrDefault();
            var customerDevResourcesAllocated = curOp.Fields.Where(f => f.Key == "Custom.CustomerDevResourcesAllocated").FirstOrDefault();
            var vSAAlignment = curOp.Fields.Where(f => f.Key == "Custom.VSAAlignment").FirstOrDefault();
            var solutionReusability = curOp.Fields.Where(f => f.Key == "Custom.SolutionReusability").FirstOrDefault();

            float currentAzureCommitScore = 0;
            if (currentAzureCommit.Value != null)
            {
                currentAzureCommitScore = int.Parse(currentAzureCommit.Value.ToString()[..1]);
            }

            float successIDACREstimateScore = 0;
            if (successIDACREstimate.Value != null)
            {
                successIDACREstimateScore = int.Parse(successIDACREstimate.Value.ToString()[..1]);
            }

            float customerDevResourcesAllocatedScore = 0;
            if (customerDevResourcesAllocated.Value != null)
            {
                customerDevResourcesAllocatedScore = int.Parse(customerDevResourcesAllocated.Value.ToString()[..1]);
            }

            float vSAAlignmentScore = 0;
            if (vSAAlignment.Value != null)
            {
                vSAAlignmentScore = int.Parse(vSAAlignment.Value.ToString()[..1]);
            }

            float solutionReusabilityScore = 0;
            if (solutionReusability.Value != null)
            {
                solutionReusabilityScore = int.Parse(solutionReusability.Value.ToString()[..1]);
            }

            double netScore = (currentAzureCommitScore + successIDACREstimateScore + customerDevResourcesAllocatedScore + vSAAlignmentScore + solutionReusabilityScore);

            var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, "Custom.ValueMarker", netScore.ToString()).Result;
            logger.LogInformation(string.Format("Opportunity Id: {0} has a CSE Value Marker score of : {1}", curOp.Id.ToString(), netScore));

        }
    }
}
