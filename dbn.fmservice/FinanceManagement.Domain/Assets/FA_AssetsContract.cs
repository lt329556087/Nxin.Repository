using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FA_AssetsContract : EntityBase
    {
        public FA_AssetsContract()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = EnterpriseID = MarketID = SupplierID = ContractTemplate = Number = "0";
            Details = new List<FA_AssetsContractDetail>();
        }
        public void Update(DateTime DataDate, string ContractName, string ContractNumber, string EnterpriseID, string MarketID, string SupplierID,
            string ContractTemplate, string Remarks, string UpDataInfo, string ContractClause, string TicketedPointID)
        {
            this.DataDate = DataDate;
            this.ContractName = ContractName;
            this.ContractNumber = ContractNumber;
            this.MarketID = MarketID;
            this.EnterpriseID = EnterpriseID;
            this.SupplierID = SupplierID;
            this.ContractTemplate = ContractTemplate;
            this.UpDataInfo = UpDataInfo;
            this.ContractClause = ContractClause;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
            this.TicketedPointID = TicketedPointID;
        }
        public FA_AssetsContractDetail AddDetail(FA_AssetsContractDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
        [Key]
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        public string ContractName { get; set; }
        public string ContractNumber { get; set; }

        public string EnterpriseID { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        public string ContractTemplate { get; set; }
        public string OwnerID { get; set; }
        public string Remarks { get; set; }
        public string UpDataInfo { get; set; }
        public string ContractClause { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public List<FA_AssetsContractDetail> Details { get; set; }
    }

    public class FA_AssetsContractDetail : EntityBase
    {
        public void Add() { }
        public void Update(string AssetsName,string AssetsTypeID, string Specification, string Brand, string MeasureUnit, decimal Quantity, decimal UnitPrice, decimal Amount, string Remarks)
        {
            this.AssetsName = AssetsName;
            this.Specification = Specification;
            this.Brand = Brand;
            this.MeasureUnit = MeasureUnit;
            this.Quantity = Quantity;
            this.UnitPrice = UnitPrice;
            this.Amount = Amount;
            this.Remarks = Remarks;
            this.AssetsTypeID = AssetsTypeID;
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
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
        /// 含税单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public FA_AssetsContract Main { get; set; }
    }

}
