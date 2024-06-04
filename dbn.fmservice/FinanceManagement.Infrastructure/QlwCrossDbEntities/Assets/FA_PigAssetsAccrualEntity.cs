using Architecture.Common.Application.Query;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FA_PigAssetsAccrualODataEntity : OneWithManyQueryEntity<FA_PigAssetsAccrualDetailODataEntity>
    {
        public FA_PigAssetsAccrualODataEntity()
        {
            Lines = new List<FA_PigAssetsAccrualDetailODataEntity>();
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

    public class FA_PigAssetsAccrualDetailODataEntity
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
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
        public decimal DepreciationMonthAmount { get; set; }
        public decimal DepreciationMonthRate { get; set; }
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
        public string CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }
    }

    public class FA_PigAssetsAccrualListODataEntity 
    {
        /// <summary>
        /// 流水号
        /// </summary>	
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string DataDate { get; set; }
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

        public decimal PigOriginalValue { get; set; }
        public decimal EquipmentProportion { get; set; }
        public decimal HouseProportion { get; set; }
        public string BeginAccountPeriodDate { get; set; }
        public string EndAccountPeriodDate { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string ResultsName { get; set; }
    }
    public class FA_PigAssetsResetByAccrualODataEntity
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 卡片编码
        /// </summary>		
        public string CardCode { get; set; }
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

}
