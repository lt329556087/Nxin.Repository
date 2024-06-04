using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Domain.MarketingProductCostSettingManagement
{
    /// <summary>
    /// 销售成本汇总表对应成本数据
    /// </summary>
    public class SalesSummaryCostInfoQueryModel
    {
        public DateTime DataDate { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string MeasureUnitName { get; set; }

        public decimal? SalesCost { get; set; }

        public decimal Quantity { get; set; }

        public string EnterpriseID { get; set; }
    }
}
