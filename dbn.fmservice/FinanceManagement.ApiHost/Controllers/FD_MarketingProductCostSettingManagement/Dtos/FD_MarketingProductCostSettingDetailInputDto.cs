namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Dtos
{
    public class FD_MarketingProductCostSettingDetailInputDto
    {
        public int RecordID { get; set; }


        public string NumericalOrder { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnitName { get; set; }

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
