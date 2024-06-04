﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public partial class FD_SettleReceiptDetail : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public Guid? Guid { get; set; }
        public string EnterpriseID { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string AccoSubjectID { get; set; }
        public string AccoSubjectCode { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        public string MarketID { get; set; }
        public string ProjectID { get; set; }
        public string ProductID { get; set; }
        public string PaymentTypeID { get; set; }
        public string AccountID { get; set; }
        public bool LorR { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Content { get; set; }
        public DateTime? AgingDate { get; set; }
        public int RowNum { get; set; }
        public string OrganizationSortID { get; set; }
        public bool IsCharges { get; set; }
        /// <summary>
        /// 商品名称ID
        /// </summary>
        /// 
        public string ProductGroupID { get; set; }
        /// <summary>
        /// 商品分类ID
        /// </summary>
        public string ClassificationID { get; set; }
        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        #endregion
    }
}