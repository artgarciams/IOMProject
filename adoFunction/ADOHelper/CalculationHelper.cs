using ADOPipeline.ADOHelper;
using ADOPipeline.ADOHelper.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdoPipline.ADOHelper
{
    public class CalculationHelper
    {
      
        private ILogger logger;
        private IAdoHelper adoHelper;
        private AdoHelper adoHelper1;

        public CalculationHelper(IAdoHelper adoHelper, ILogger logger)
        {
            this.logger = logger;
            this.adoHelper = adoHelper;
        }


        public async void CalculateScores(WorkItem curOp, Opportunity FieldList)
        {
            try
            {
                logger.LogInformation(" ----- Start calculation ----- ");
                int Id = (int)curOp.Id;

                var WorkItemCurrent = adoHelper.GetOpportunity(Id).Result;


                foreach (var list in FieldList.List)
                {
                    decimal netScore = 0;
                    decimal totScore = 0;
                    int fldCount = 0;

                    Console.WriteLine("Summing to Field : " + list.FieldSum);

                    foreach (var item in list.Values)
                    {
                        switch (item.type)
                        {
                            case "int":
                                Console.WriteLine("Field : " + item.key + "  Type :" + item.type);

                                var Fld = curOp.Fields.Where(f => f.Key == item.key).FirstOrDefault();
                                float cFldscore = 0;
                                if (Fld.Value != null)
                                {
                                    cFldscore = float.Parse(Fld.Value.ToString()[..1]);
                                }
                                netScore += (decimal)cFldscore;
                                logger.LogInformation("---Field to calculate :{0} with a value of {1} and a field type of {2}", item.key, Fld.Value, item.type);
                                break;

                            case "boolean":
                                Console.WriteLine("Field : " + item.key + "  Type :" + item.type);

                                var FldBool = curOp.Fields.Where(f => f.Key == item.key).FirstOrDefault();
                                if (FldBool.Value != null && bool.Parse(FldBool.Value.ToString()) == true)
                                {
                                    netScore += (decimal)item.TrueValue;
                                }
                                else
                                {
                                    netScore += (decimal)item.FalseValue;
                                }
                                logger.LogInformation("---Field to calculate :{0} with a value of {1} and a field type of {2}", item.key, FldBool.Value.ToString(), item.type);
                                break;

                            case "decimal":
                                // this is a summary field. this field calculates the sum of previously calculated fields
                                var sumFld = curOp.Fields.Where(f => f.Key == item.key).FirstOrDefault();
                                decimal sumField = 0;
                                if (sumFld.Value != null)
                                {
                                    sumField = decimal.Parse(sumFld.Value.ToString());
                                }
                                netScore += sumField;
                                logger.LogInformation("---Summary Field to calculate :{0} with a value of {1} and a field type of {2}", item.key, sumFld.Value, item.type);

                                break;


                        }
                        fldCount++;

                    }

                    // get average for section
                    totScore = netScore;
                    netScore = netScore / fldCount;
                    netScore = Math.Round(netScore, 2);

                    // update workitem 
                    // if omitZero is true, the field will be set to "" if there the value is 0.
                    // this way it will not appear on the card as card has been set to not show blank values
                    if(list.omitZero is null)
                    {
                        var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, list.FieldSum + "", netScore.ToString()).Result;
                    }
                    else
                    {
                        if (list.omitZero.Trim().ToLower() == "true")
                        {
                            if (netScore == 0)
                            {
                                var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, list.FieldSum + "", "").Result;
                            }else
                            {
                                var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, list.FieldSum + "", netScore.ToString()).Result;
                            }
                        }

                    }

                    // check if this will be a field to check if its changed deltaField 
                    if (list.deltaField is not null)
                    {
                        if (list.deltaField.Trim().ToLower() == "true")
                        {
                            decimal current = (decimal)(double)WorkItemCurrent.Fields[list.FieldSum];
                            if (current != netScore)
                            {
                                string text = "";
                                if (WorkItemCurrent.Fields[FieldList.ChangeField] != null)
                                {
                                    text = WorkItemCurrent.Fields[FieldList.ChangeField].ToString().Replace("(--)", "").Replace("(++)", "");
                                }


                                if (current > netScore)
                                {
                                    var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, FieldList.ChangeField, " (--) " + text).Result;
                                }
                                if (current < netScore)
                                {
                                    var updatedWI = adoHelper.UpdateOpportunity((int)curOp.Id, FieldList.ChangeField, " (++) " + text).Result;
                                }

                                // future use add comment about what changed
                                //string projectGuid = curOp.Url.Split('/')[4];
                                //var updatedWI = adoHelper.AddComment((int)curOp.Id, list.FieldSum +  " value changed", projectGuid ).Result;
                            }

                        }
                    }

                    logger.LogInformation(string.Format("Opportunity Id: {0} has a score of : {1} with : {2} questions counted with total score of : {3}", curOp.Id.ToString(), netScore,fldCount,totScore));
                }

                logger.LogInformation("----- calculation complete -----");

            }
            catch (Exception ex)
            {
                logger.LogError("***** ERROR *****" + ex.Message + " Trace : " + ex.StackTrace);
            }
        }


    }
}
