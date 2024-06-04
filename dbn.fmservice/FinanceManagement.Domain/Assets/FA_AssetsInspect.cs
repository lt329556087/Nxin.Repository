using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FA_AssetsInspect : EntityBase
    {
        public FA_AssetsInspect()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID =PersonID=TicketedPointID= EnterpriseID = Number = "0";
            Details = new List<FA_AssetsInspectDetail>();
        }
        public void Update(string EnterpriseID, DateTime DataDate, string TicketedPointID, string MarketID, string PersonID, string Remarks)
        {
            this.TicketedPointID = TicketedPointID;
            this.PersonID = PersonID;
            this.MarketID = MarketID;
            this.EnterpriseID = EnterpriseID;
            this.DataDate = DataDate;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
        }
        public FA_AssetsInspectDetail AddDetail(FA_AssetsInspectDetail detail)
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
        public string PersonID { get; set; }
        public string OwnerID { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FA_AssetsInspectDetail> Details { get; set; }
    }

    public class FA_AssetsInspectDetail : EntityBase
    {
        public void Add() { }
        public void Update(string AssetsName, string AssetsTypeID, string Specification, string Brand, string MeasureUnit, string AssetsNatureId, decimal Quantity,  decimal UnitPrice, decimal Amount, string SupplierID, string ProjectID, string Remarks)
        {
            this.AssetsNatureId = AssetsNatureId;
            this.AssetsName = AssetsName;
            this.AssetsTypeID = AssetsTypeID;
            this.Specification = Specification;
            this.Brand = Brand;
            this.MeasureUnit = MeasureUnit;
            this.Quantity = Quantity;
            this.UnitPrice = UnitPrice;
            this.Amount = Amount;
            this.SupplierID = SupplierID;
            this.ProjectID = ProjectID;
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
        public string AssetsNatureId { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnit { get; set; }
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
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
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
        public FA_AssetsInspect Main { get; set; }

    }
}
