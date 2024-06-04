using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FA_AssetsApply : EntityBase
    {
        public FA_AssetsApply()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = EnterpriseID=Number = "0";
            Details = new List<FA_AssetsApplyDetail>();
        }
        public void Update(string EnterpriseID,DateTime DataDate,string MarketID,string Remarks,string UpDataInfo,string TicketedPointID)
        {
            this.UpDataInfo = UpDataInfo;
            this.MarketID = MarketID;
            this.EnterpriseID = EnterpriseID;
            this.DataDate = DataDate;
            this.Remarks = Remarks;
            this.TicketedPointID = TicketedPointID;
        }
        public FA_AssetsApplyDetail AddDetail(FA_AssetsApplyDetail detail)
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
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        public string OwnerID { get; set; }
        public string Remarks { get; set; }
        public string UpDataInfo { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FA_AssetsApplyDetail> Details { get; set; }
    }

    public class FA_AssetsApplyDetail : EntityBase
    {
        public void Add() { }
        public void Update(string AssetsName, string AssetsTypeID, string Specification, string Brand, string MeasureUnit, decimal Quantity, decimal TaxRate, decimal UnitPrice, decimal Amount, string SupplierID, string ProjectID, string Remarks)
        {
            this.AssetsName = AssetsName;
            this.AssetsTypeID = AssetsTypeID;
            this.Specification = Specification;
            this.Brand = Brand;
            this.MeasureUnit = MeasureUnit;
            this.Quantity = Quantity;
            this.TaxRate = TaxRate;
            this.UnitPrice = UnitPrice;
            this.Amount = Amount;
            this.SupplierID = SupplierID;
            this.ProjectID = ProjectID;
            this.Remarks = Remarks;
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
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
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public decimal TaxRate { get; set; }
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
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public FA_AssetsApply Main { get; set; }

    }
   
}
