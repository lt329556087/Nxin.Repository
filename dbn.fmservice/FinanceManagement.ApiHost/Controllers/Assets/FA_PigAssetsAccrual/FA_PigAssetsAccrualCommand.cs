using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_PigAssetsAccrual
{

    public class FA_PigAssetsAccrualAddCommand : FA_PigAssetsAccrualCommand, IRequest<Result>
    {

    }
    public class FA_PigAssetsAccrualDeleteCommand : FA_PigAssetsAccrualCommand, IRequest<Result>
    {

    }

    public class FA_PigAssetsAccrualCommand : MutilLineCommand<FA_PigAssetsAccrualDetailCommand>
    {
        public FA_PigAssetsAccrualCommand()
        {
            this.Lines = new List<FA_PigAssetsAccrualDetailCommand>();
        }
        /// <summary>
        /// 流水号
        /// </summary>		
        /// 
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

    }

    public class FA_PigAssetsAccrualDetailCommand : CommonOperate
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrderInput { get; set; }
        /// <summary>
        /// 猪场性质
        /// </summary>
        public string PigfarmNatureID { get; set; }
        public string PigfarmNatureName { get; set; }
        /// <summary>
        /// 猪场信息
        /// </summary>
        public string PigfarmID { get; set; }
        public string PigfarmName { get; set; }
        /// <summary>
        /// 使用部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// 卡片编码
        /// </summary>
        public string CardCode { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>		
        public string AssetsCode { get; set; }

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
        /// 资产原值
        /// </summary>
        public decimal OriginalValue { get; set; }
        /// <summary>
        /// 月折旧额
        /// </summary>
        public decimal DepreciationMonthAmount { get; set; }
        /// <summary>
        /// 月折旧率
        /// </summary>
        public decimal DepreciationMonthRate { get; set; }
        /// <summary>
        /// 累计折旧
        /// </summary>
        public decimal DepreciationAccumulated { get; set; }
        /// <summary>
        /// 资产净值
        /// </summary>
        public decimal NetValue { get; set; }
        /// <summary>
        /// 原使用期限
        /// </summary>
        public int UseMonth { get; set; }
        /// <summary>
        /// 已经计提月份
        /// </summary>
        public int AlreadyAccruedMonth { get; set; }
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
