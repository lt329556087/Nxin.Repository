using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain.MarketingProductCostSettingManagement
{
    public class FD_MarketingProductCostSettingListQueryModel
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [Key]
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public DateTimeOffset DataDate { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }


        /// <summary>
        /// 账务单位ID
        /// </summary>
        public string AccountingEnterpriseID { get; set; }

        /// <summary>
        /// 账务单位
        /// </summary>		
        public string AccountingEnterpriseName { get; set; }

        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 审核人ID
        /// </summary>
        public string AuditUserID { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTimeOffset? AuditDate { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int AuditStatus { get; set; }
    }
}
