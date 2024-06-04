﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Architecture.Seedwork.Domain;

namespace FinanceManagement.Domain
{
    public partial class FA_Inventory : EntityBase
    {
        public FA_Inventory()
        {
            DataDate = CreatedDate = ModifiedDate = DateTime.Now;
            EnterpriseID = NumericalOrder = OwnerID = "0";
            Lines = new List<FA_InventoryDetail>();
        }

        public void AddDetail(FA_InventoryDetail detail)
        {
            Lines.Add(detail);
        }
        public void Update(DateTime DataDate, string FAPlaceID, string UseStateID,string Remarks)
        {
            this.DataDate = DataDate;
            this.FAPlaceID = FAPlaceID;
            this.UseStateID = UseStateID;
            this.Remarks = Remarks;
        }
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime DataDate { get; set; }
        public string Number { get; set; }
        public string FAPlaceID { get; set; }
        public string UseStateID { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<FA_InventoryDetail> Lines { get; set; }
    }
    public partial class FA_InventoryDetail : EntityBase
    {
        public FA_InventoryDetail()
        {
            NumericalOrder = CardID = "0";
        }
        public void Update(string CardID, decimal Quantity,decimal InventoryQuantity,string Remarks,string FileName,string PathUrl)
        {
            this.CardID = CardID;
            this.Quantity = Quantity;
            this.InventoryQuantity = InventoryQuantity;
            this.Remarks=Remarks;
            this.FileName = FileName;
            this.PathUrl = PathUrl;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string CardID { get; set; }
        public decimal Quantity { get; set; }
        public decimal InventoryQuantity { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string PathUrl { get; set; }
        public DateTime ModifiedDate { get; set; }
        [ForeignKey("NumericalOrder")]
        public FA_Inventory FA_Inventory { get; set; }
    }
}