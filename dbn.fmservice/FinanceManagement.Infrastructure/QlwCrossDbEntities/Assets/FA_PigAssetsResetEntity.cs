using Architecture.Common.Application.Query;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FA_PigAssetsResetODataEntity : OneWithManyQueryEntity<FA_PigAssetsResetDetailODataEntity>
    {
        public FA_PigAssetsResetODataEntity()
        {
            Lines = new List<FA_PigAssetsResetDetailODataEntity>();
        }


        /// <summary>
        /// 流水号
        /// </summary>		
        /// 
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string DataDate { get; set; }
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
        public string ResultsName { get; set; }
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
        public string BeginAccountPeriodDate { get; set; }
        /// <summary>
        /// 结束重置会计期间
        /// </summary>
        public string EndAccountPeriodDate { get; set; }
        /// <summary>
        /// 设备重置原值取值期间
        /// </summary>
        public string ResetOriginalValueDate { get; set; }
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
        public string CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }

    }

    public class FA_PigAssetsResetDetailODataEntity
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        [NotMapped]
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
        public string CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }
    }

    public class FA_PigAssetsResetListODataEntity 
    {
        /// <summary>
        /// 流水号
        /// </summary>	
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string DataDate { get; set; }
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
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

        public decimal PigOriginalValue { get; set; }
        public decimal EquipmentProportion { get; set; }
        public decimal HouseProportion { get; set; }
        /// <summary>
        /// 结束重置会计期间
        /// </summary>
        public string EndAccountPeriodDate { get; set; }
        public string BeginAccountPeriodDate { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string ResultsName { get; set; }
    }
    public class FA_AssetsInspectMobileODataEntity
    {
        public decimal ResetFixedYears { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>	
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        public string Number { get; set; }
        public string AssetsName { get; set; }
        public string AssetsTypeID { get; set; }
        public string AssetsTypeName { get; set; }
        public string Specification { get; set; }
        public string Brand { get; set; }
        public string AssetsNatureId { get; set; }
        public string AssetsNatureName { get; set; }
        public string MeasureUnit { get; set; }
        public string MeasureUnitName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
    }

    public class FA_AssetsCardMobileODataEntity
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string CardCode { get; set; }
        public decimal ResetFixedYears { get; set; }
        public string AssetsCode { get; set; }
        public string AssetsName { get; set; }
        public string AssetsTypeID { get; set; }
        public string AssetsTypeName { get; set; }
        public string Specification { get; set; }
        public string StartDate { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal NetValue { get; set; }
        public decimal DepreciationAccumulated { get; set; }
        public decimal ResidualValue { get; set; }
        public int UseMonth { get; set; }
        public string MeasureUnit { get; set; }
        public string MeasureUnitName { get; set; }
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        public decimal Quantity { get; set; }
    }
}
