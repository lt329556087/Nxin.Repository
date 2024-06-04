using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_PurchaseSettings
{

    public class FA_PurchaseSettingsAddCommand : FA_PurchaseSettingsCommand, IRequest<Result>
    {

    }

    public class FA_PurchaseSettingsDeleteCommand : FA_PurchaseSettingsCommand, IRequest<Result>
    {
        public string[] AssetsTypeIDList { get; set; }
    }

    public class FA_PurchaseSettingsModifyCommand : FA_PurchaseSettingsCommand, IRequest<Result>
    {
    }

    public class FA_PurchaseSettingsCommand : MutilLineCommand<FA_PurchaseSettingsDetailCommand>
    {
        public FA_PurchaseSettingsCommand()
        {
            this.Lines = new List<FA_PurchaseSettingsDetailCommand>();
        }
        public string NumericalOrder { get; set; }
        public string ModifyFieldID { get; set; }
        public List<BIZ_DataDictODataEntity> ModifyFieldList { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }

    }

    public class FA_PurchaseSettingsDetailCommand : CommonOperate
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 资产类别
        /// </summary>
        public string AssetsTypeID { get; set; }
        public List<AssetsTypeModel> AssetsTypeList { get; set; }
        /// <summary>
        /// 开始适用范围
        /// </summary>
        public decimal BeginRange { get; set; }
        /// <summary>
        /// 结束适用范围
        /// </summary>
        public decimal EndRange { get; set; }
        /// <summary>
        /// 浮动方向
        /// </summary>
        public string FloatingDirectionID { get; set; }
        public string FloatingDirectionName { get; set; }
        /// <summary>
        /// 浮动类型
        /// </summary>
        public string FloatingTypeID { get; set; }
        public string FloatingTypeName { get; set; }
        /// <summary>
        /// 最大浮动值
        /// </summary>
        public decimal MaxFloatingValue { get; set; }
    }

    public class MutilLineCommand<TLineCommand1> : CommonOperate where TLineCommand1 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
    }

}
