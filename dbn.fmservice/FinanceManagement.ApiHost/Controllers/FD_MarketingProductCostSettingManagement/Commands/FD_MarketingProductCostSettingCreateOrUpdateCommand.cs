using System;
using System.Collections.Generic;
using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Dtos;
using MediatR;

namespace FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement.Commands
{
    public class FD_MarketingProductCostSettingCreateOrUpdateCommand : IRequest<Result>
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
        /// 账务单位
        /// </summary>		
        public string AccountingEnterpriseID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 详情数据
        /// </summary>
        public List<FD_MarketingProductCostSettingDetailInputDto> Details { get; set; }
    }
}
