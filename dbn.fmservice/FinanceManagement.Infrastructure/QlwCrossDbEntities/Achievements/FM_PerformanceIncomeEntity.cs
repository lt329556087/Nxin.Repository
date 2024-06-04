using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_PerformanceIncomeEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string ProductGroupID { get; set; }
        public string PropertyName { get; set; }
      
    }
}
