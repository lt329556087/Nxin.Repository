using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FA_AssetsMaintain : EntityBase
    {
        public FA_AssetsMaintain()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = EnterpriseID = Number = "0";
            Details = new List<FA_AssetsMaintainDetail>();
        }
        public void Update(string EnterpriseID, DateTime DataDate, string Remarks)
        {
            this.EnterpriseID = EnterpriseID;
            this.DataDate = DataDate;
            this.Remarks = Remarks;
        }
        public FA_AssetsMaintainDetail AddDetail(FA_AssetsMaintainDetail detail)
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
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
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

        public List<FA_AssetsMaintainDetail> Details { get; set; }
    }

    public class FA_AssetsMaintainDetail : EntityBase
    {
        public void Add() { }
        public void Update(string AssetsName, string AssetsCode, string CardID, string MaintainID, DateTime MaintainDate, string Content, decimal Amount, string DepositID, string FileName, string FilePath, string PersonID, string Remarks)
        {
            this.AssetsName = AssetsName;
            this.AssetsCode = AssetsCode;
            this.MaintainID = MaintainID;
            this.MaintainDate = MaintainDate;
            this.Content = Content;
            this.Amount = Amount;
            this.DepositID = DepositID;
            this.FileName = FileName;
            this.FilePath = FilePath;
            this.PersonID = PersonID;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
            this.CardID = CardID;
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 资产ID
        /// </summary>
        public string CardID { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>
        public string AssetsCode { get; set; }
        /// <summary>
        /// 保养方式
        /// </summary>
        public string MaintainID { get; set; }
        /// <summary>
        /// 保养时间
        /// </summary>
        public DateTime MaintainDate { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 存放地点ID
        /// </summary>
        public string DepositID { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string PersonID { get; set; }
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
        public FA_AssetsMaintain Main { get; set; }

    }

}
