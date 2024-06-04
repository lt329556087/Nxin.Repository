using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FD_BadDebtExecution : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        public string AppID { get; set; }

        public string EnterpriseID { get; set; }

        public string NumericalOrder { get; set; }

        /// <summary>
        /// 凭证流水号
        /// </summary>
        public string NumericalOrderReceipt { get; set; }
        public bool State { get; set; }
        /// <summary>
        /// 生成凭证时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 详情信息
        /// </summary>
        public string Remarks { get; set; }
    }
}
