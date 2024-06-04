using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_BadDebtExecutionODataEntity
    {

        [Key]
        public int RecordID { get; set; }
        public string AppID { get; set; }

        public string AppName { get; set; }


        public long? Number { get; set; }
        public string NumericalOrder { get; set; }
        public string NumberReceipt { get; set; }
        
        public string NumericalOrderReceipt { get; set; }

        public bool State { get; set; }
        public string StateName { get; set; }
        
        /// <summary>
        /// 生成凭证时间
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 详情信息
        /// </summary>
        public string Remarks { get; set; }

    }
}
