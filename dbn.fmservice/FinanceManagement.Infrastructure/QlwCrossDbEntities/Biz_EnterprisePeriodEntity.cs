using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class Biz_EnterprisePeriodODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public bool IsCheck { get; set; }
    }
}
