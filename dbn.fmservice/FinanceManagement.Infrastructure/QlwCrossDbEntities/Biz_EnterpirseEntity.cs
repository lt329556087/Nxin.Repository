using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
   public class Biz_EnterpirseEntityODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string EnterpriseFullName { get; set; }
        public string PID { get; set; }
        public string AreaID { get; set; }
        public string BeginDate { get; set; }
        public string Remarks { get; set; }
    }
}
