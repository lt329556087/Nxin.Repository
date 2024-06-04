using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsContract
{

    public class FA_AssetsContractAddCommand : FA_AssetsContractCommand, IRequest<Result>
    {

    }

    public class FA_AssetsContractDeleteCommand : FA_AssetsContractCommand, IRequest<Result>
    {

    }

    public class FA_AssetsContractModifyCommand : FA_AssetsContractCommand, IRequest<Result>
    {
    }

    public class FA_AssetsContractCopyCommand : FA_AssetsContractCommand, IRequest<Result>
    {

    }
    public class FA_AssetsContractCommand : MutilLineCommand<FA_AssetsContractDetailCommand>
    {
        public FA_AssetsContractCommand()
        {
            this.Lines = new List<FA_AssetsContractDetailCommand>();
        }
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        public string ContractName { get; set; }
        public string ContractNumber { get; set; }

        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }

        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        public string ContractTemplate { get; set; }
        public string ContractTemplateName { get; set; }
        public string OwnerID { get; set; }
        public string Remarks { get; set; }
        public string UpDataInfo { get; set; }
        public string ContractClause { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

    }

    public class FA_AssetsContractDetailCommand : CommonOperate
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrderInput { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产类别
        /// </summary>
        public string AssetsTypeID { get; set; }
        public string AssetsTypeName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnit { get; set; }
        public string MeasureUnitName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 含税单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }

    public class MutilLineCommand<TLineCommand1> : CommonOperate where TLineCommand1 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
    }

}
