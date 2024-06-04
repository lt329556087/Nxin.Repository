using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FA_PigAssetsAccrual : EntityBase
    {
        public FA_PigAssetsAccrual()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID =  "0";
            Details = new List<FA_PigAssetsAccrualDetail>();
        }
        public void Update(string EnterpriseID, DateTime DataDate,  string OwnerID, string Remarks)
        {
            this.EnterpriseID = EnterpriseID;
            this.DataDate = DataDate;
            this.OwnerID = OwnerID;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
        }
        public FA_PigAssetsAccrualDetail AddDetail(FA_PigAssetsAccrualDetail detail)
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

        public List<FA_PigAssetsAccrualDetail> Details { get; set; }
    }

    public class FA_PigAssetsAccrualDetail : EntityBase
    {
        public void Add() { }
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
        /// <summary>
        /// 猪场信息
        /// </summary>
        public string PigfarmID { get; set; }
        public string MarketID { get; set; }
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
        /// <summary>
        /// 资产原值
        /// </summary>
        public decimal OriginalValue { get; set; }
        /// <summary>
        /// 月折旧额
        /// </summary>
        public decimal DepreciationMonthAmount { get; set; }
        /// <summary>
        /// 月折旧率
        /// </summary>
        public decimal DepreciationMonthRate { get; set; }
        /// <summary>
        /// 累计折旧
        /// </summary>
        public decimal DepreciationAccumulated { get; set; }
        /// <summary>
        /// 净值
        /// </summary>
        public decimal NetValue { get; set; }
        /// <summary>
        /// 使用月份
        /// </summary>
        public int UseMonth { get; set; }
        /// <summary>
        /// 已经计提月份
        /// </summary>
        public int AlreadyAccruedMonth { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
        public FA_PigAssetsAccrual Main { get; set; }

    }
}
