using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FA_PurchaseSettings : EntityBase
    {
        public FA_PurchaseSettings()
        {
            Details = new List<FA_PurchaseSettingsDetail>();
        }
        public void Update()
        {

        }
        public FA_PurchaseSettingsDetail AddDetail(FA_PurchaseSettingsDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
        [Key]
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 可修改字段
        /// </summary>
        public string ModifyFieldID { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FA_PurchaseSettingsDetail> Details { get; set; }
    }

    public class FA_PurchaseSettingsDetail : EntityBase
    {
        public void Add() { }
        public void Update(string AssetsTypeID, decimal BeginRange, decimal EndRange, string FloatingDirectionID, string FloatingTypeID, decimal MaxFloatingValue, DateTime ModifiedDate)
        {
            this.AssetsTypeID = AssetsTypeID;
            this.BeginRange = BeginRange;
            this.EndRange = EndRange;
            this.FloatingDirectionID = FloatingDirectionID;
            this.FloatingTypeID = FloatingTypeID;
            this.MaxFloatingValue = MaxFloatingValue;
            this.ModifiedDate = ModifiedDate;
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 资产类别
        /// </summary>
        public string AssetsTypeID { get; set; }
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
        /// <summary>
        /// 浮动类型
        /// </summary>
        public string FloatingTypeID { get; set; }
        /// <summary>
        /// 最大浮动值
        /// </summary>
        public decimal MaxFloatingValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public FA_PurchaseSettings Main { get; set; }

    }
    public class AssetsTypeModel
    {
        public string AssetsTypeID { get; set; }
        public string AssetsTypeName { get; set; }
    }
}
