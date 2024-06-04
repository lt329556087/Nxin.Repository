using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_PigAssetsReset
{

    public class FA_PigAssetsResetAddCommand : FA_PigAssetsResetCommand, IRequest<Result>
    {

    }

    public class FA_PigAssetsResetDeleteCommand : FA_PigAssetsResetCommand, IRequest<Result>
    {

    }

    public class FA_PigAssetsResetModifyCommand : FA_PigAssetsResetCommand, IRequest<Result>
    {
    }

    public class FA_PigAssetsResetCopyCommand : FA_PigAssetsResetCommand, IRequest<Result>
    {

    }
    public class FA_PigAssetsResetCommand : MutilLineCommand<FA_PigAssetsResetDetailCommand>
    {
        public FA_PigAssetsResetCommand()
        {
            this.Lines = new List<FA_PigAssetsResetDetailCommand>();
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
        /// 猪数量取值类型
        /// </summary>
        public string PigNumberTypeId { get; set; }
        public string PigNumberTypeName { get; set; }
        /// <summary>
        /// 猪数量
        /// </summary>
        public decimal PigNumber { get; set; }
        /// <summary>
        /// 重置单价
        /// </summary>		
        public decimal PigPrice { get; set; }
        /// <summary>
        /// 猪场资产重置后原值
        /// </summary>
        public decimal PigOriginalValue { get; set; }
        /// <summary>
        /// 开始重置会计期间
        /// </summary>
        public DateTime BeginAccountPeriodDate { get; set; }
        /// <summary>
        /// 结束重置会计期间
        /// </summary>
        public string EndAccountPeriodDate { get; set; }
        /// <summary>
        /// 设备重置原值取值期间
        /// </summary>
        public DateTime ResetOriginalValueDate { get; set; }
        /// <summary>
        /// 设备重置原值取值类型/重置后原值取值方式
        /// </summary>
        public string ResetOriginalValueType { get; set; }
        public string ResetOriginalValueTypeName { get; set; }
        /// <summary>
        /// 设备分配比
        /// </summary>
        public decimal EquipmentProportion { get; set; }
        /// <summary>
        /// 房屋分配比
        /// </summary>
        public decimal HouseProportion { get; set; }
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
        public bool IsUpdateEndAccountPeriodDate { get; set; }

    }

    public class FA_PigAssetsResetDetailCommand : CommonOperate
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrderInput { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>		
        public string AssetsCode { get; set; }
        /// <summary>
        /// 资产验收单号
        /// </summary>
        public string InspectNumber { get; set; }

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
        /// 使用部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// 资产原值
        /// </summary>
        public decimal OriginalValue { get; set; }
        /// <summary>
        /// 资产净值
        /// </summary>
        public decimal NetValue { get; set; }
        /// <summary>
        /// 原使用期限
        /// </summary>
        public int OriginalUseYear { get; set; }
        /// <summary>
        /// 重置基数
        /// </summary>
        public decimal ResetBase { get; set; }
        /// <summary>
        /// 重置使用期限
        /// </summary>
        public int ResetUseYear { get; set; }
        /// <summary>
        /// 重置后原值
        /// </summary>
        public decimal ResetOriginalValue { get; set; }
        /// <summary>
        /// 1:设备/2:房屋
        /// </summary>
        public int ContentType { get; set; }
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
