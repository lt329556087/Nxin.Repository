using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;

namespace FinanceManagement.Domain.MarketingProductCostSettingManagement
{
    /// <summary>
    /// 营销商品成本设定详情
    /// </summary>
    public class FD_MarketingProductCostSettingDetail : EntityBase
    {
        public int RecordID { get; set; }

        public string NumericalOrder { get; set; }

        public string ProductId { get; set; }

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
    }
}
