using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPipline.ADOHelper
{
    public class Opportunity
    {
        public string ChangeField { get; set; }
         
        public List<FieldList>? List { get; set; }
        public class FieldList
        {
            public string? FieldSum { get; set; }
            public int Order { get; set; }
            public string? omitZero { get; set; }   

            public string? deltaField { get; set; } 

            public List<fieldList>? Values { get; set; }
        }

        public class fieldList
        {
            public string? key { get; set; }
            public string? type { get; set; }
            public int TrueValue { get; set; }
            public int FalseValue { get; set; }

        }
    }

}
