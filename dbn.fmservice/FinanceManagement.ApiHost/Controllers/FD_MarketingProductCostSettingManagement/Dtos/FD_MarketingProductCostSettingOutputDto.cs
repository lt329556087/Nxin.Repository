using System;
using System.Collections.Generic;

namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Dtos
{
    public class FD_MarketingProductCostSettingOutputDto
    {
        /// <summary>
        /// 流水号
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

        /// <summary>
        /// 账务单位名称
        /// </summary>
        public string AccountingEnterpriseName { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// 制单人名称
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        public List<FD_MarketingProductCostSettingDetailOutputDto> Details { get; set; }
    }
}
