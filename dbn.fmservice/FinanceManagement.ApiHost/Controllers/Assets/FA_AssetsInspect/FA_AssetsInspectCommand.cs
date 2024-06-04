using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsInspect
{

    public class FA_AssetsInspectAddCommand : FA_AssetsInspectCommand, IRequest<Result>
    {

    }

    public class FA_AssetsInspectDeleteCommand : FA_AssetsInspectCommand, IRequest<Result>
    {

    }

    public class FA_AssetsInspectModifyCommand : FA_AssetsInspectCommand, IRequest<Result>
    {
    }

    public class FA_AssetsInspectCopyCommand : FA_AssetsInspectCommand, IRequest<Result>
    {

    }
    public class FA_AssetsInspectCommand : MutilLineCommand<FA_AssetsInspectDetailCommand>
    {
        public FA_AssetsInspectCommand()
        {
            this.Lines = new List<FA_AssetsInspectDetailCommand>();
        }
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public DateTime DataDate { get; set; }
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        public string PersonID { get; set; }
        public string PersonName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public string ApplyForms { get; set; }
        public string ContractForms { get; set; }

    }

    public class FA_AssetsInspectDetailCommand : CommonOperate
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
        /// 资产性质
        /// </summary>
        public string AssetsNatureId { get; set; }
        public string AssetsNatureName { get; set; }
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
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
    }

    public class MutilLineCommand<TLineCommand1> : CommonOperate where TLineCommand1 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
    }

}
