using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations.MarketingProductCostSettingManagement
{
    public class FD_MarketingProductCostSettingQueryModel
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public DateTime DataDate { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 账务单位
        /// </summary>		
        public string AccountingEnterpriseID { get; set; }

        public string AccountingEnterpriseName { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>		
        public string OwnerID { get; set; }

        public string OwnerName { get; set; }

        public string AuditUserID { get; set; }

        public string AuditUserName { get; set; }

        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>		
        public DateTime ModifiedDate { get; set; }


        public int RecordID { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnitNanme { get; set; }

        /// <summary>
        /// 预测单位成本
        /// </summary>
        public decimal ForecastUnitCost { get; set; }

        /// <summary>
        /// 当期计算成本
        /// </summary>
        public decimal CurrentCalcCost { get; set; }

        /// <summary>
        /// 差异值
        /// </summary>
        public decimal ForecastAndCurrentDiff { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
