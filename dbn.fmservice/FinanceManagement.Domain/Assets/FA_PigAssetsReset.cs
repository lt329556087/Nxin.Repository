using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FA_PigAssetsReset : EntityBase
    {
        public FA_PigAssetsReset()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = PigfarmNatureID = PigfarmID = PigNumberTypeId = Number = ResetOriginalValueType = "0";
            Details = new List<FA_PigAssetsResetDetail>();
        }
        public void Update(string EnterpriseID, DateTime DataDate, string Number, string PigfarmNatureID, string PigfarmID, string PigNumberTypeId,
            decimal PigNumber, decimal PigPrice, decimal PigOriginalValue, DateTime BeginAccountPeriodDate, DateTime ResetOriginalValueDate, string ResetOriginalValueType,
            decimal EquipmentProportion, decimal HouseProportion, string OwnerID, string Remarks,string TicketedPointID, string EndAccountPeriodDate)
        {
            this.EnterpriseID = EnterpriseID;
            this.DataDate = DataDate;
            this.Number = Number;
            this.PigfarmNatureID = PigfarmNatureID;
            this.PigfarmID = PigfarmID;
            this.PigNumberTypeId = PigNumberTypeId;
            this.PigNumber = PigNumber;
            this.PigPrice = PigPrice;
            this.PigOriginalValue = PigOriginalValue;
            this.BeginAccountPeriodDate = BeginAccountPeriodDate;
            this.EndAccountPeriodDate = EndAccountPeriodDate;
            this.ResetOriginalValueDate = ResetOriginalValueDate;
            this.ResetOriginalValueType = ResetOriginalValueType;
            this.EquipmentProportion = EquipmentProportion;
            this.HouseProportion = HouseProportion;
            this.OwnerID = OwnerID;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
            this.TicketedPointID = TicketedPointID;
        }
        public void UpdateEndAccountPeriodDate(string EndAccountPeriodDate)
        {
            this.EndAccountPeriodDate = EndAccountPeriodDate;
        }
        public FA_PigAssetsResetDetail AddDetail(FA_PigAssetsResetDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
        [Key]
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime DataDate { get; set; }
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 猪场性质
        /// </summary>
        public string PigfarmNatureID { get; set; }
        /// <summary>
        /// 猪场信息
        /// </summary>
        public string PigfarmID { get; set; }
        /// <summary>
        /// 猪数量取值类型
        /// </summary>
        public string PigNumberTypeId { get; set; }
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
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FA_PigAssetsResetDetail> Details { get; set; }
    }

    public class FA_PigAssetsResetDetail : EntityBase
    {
        public void Add() { }
        public void Update(string AssetsCode, string InspectNumber, string AssetsName, string AssetsTypeID, string Specification, string Brand, string MeasureUnit, string MarketID, decimal OriginalValue, decimal NetValue, int OriginalUseYear, decimal ResetBase
            , int ResetUseYear, decimal ResetOriginalValue, int ContentType, string Remarks)
        {
            this.AssetsCode = AssetsCode;
            this.InspectNumber = InspectNumber;
            this.AssetsName = AssetsName;
            this.AssetsTypeID = AssetsTypeID;
            this.Specification = Specification;
            this.MeasureUnit = MeasureUnit;
            this.Brand = Brand;
            this.MarketID = MarketID;
            this.OriginalValue = OriginalValue;
            this.NetValue = NetValue;
            this.OriginalUseYear = OriginalUseYear;
            this.ResetBase = ResetBase;
            this.ResetUseYear = ResetUseYear;
            this.ResetOriginalValue = ResetOriginalValue;
            this.ContentType = ContentType;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }

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
        /// <summary>
        /// 使用部门
        /// </summary>
        public string MarketID { get; set; }
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
        public FA_PigAssetsReset Main { get; set; }

    }
}
