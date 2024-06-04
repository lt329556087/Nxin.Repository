using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class MS_Formula : EntityBase
    {
        public MS_Formula()
        {
            Details = new List<MS_FormulaDetail>();
            Extends = new List<MS_FormulaExtend>();
        }
        public void Update(DateTime DataDate, string FormulaName, decimal BaseQuantity, string Remarks, string PackageRemarks,  DateTime EffectiveBeginDate, DateTime EffectiveEndDate)
        {
            this.DataDate = DataDate;
            this.FormulaName = FormulaName;
            this.BaseQuantity = BaseQuantity;
            this.Remarks = Remarks;
            this.PackageRemarks = PackageRemarks;
            this.ModifiedDate=DateTime.Now;
            this.EffectiveBeginDate = EffectiveBeginDate;
            this.EffectiveEndDate = EffectiveEndDate;
        }
        public MS_FormulaDetail AddDetail(MS_FormulaDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
        public MS_FormulaExtend AddExtend(MS_FormulaExtend detail)
        {
            Extends.Add(detail);
            return detail;
        }
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 字号（开票点）
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Number { get; set; }
        public string FormulaName { get; set; }
        public bool IsUse { get; set; }
        public decimal BaseQuantity { get; set; }
        public string Remarks { get; set; }
        public string PackageRemarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        public decimal EarlyWarning { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
        public string UseEnterprise { get; set; }
        public string UseProduct { get; set; }
        public int IsGroup { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime EffectiveBeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EffectiveEndDate { get; set; }
        public List<MS_FormulaDetail> Details { get; set; }
        public List<MS_FormulaExtend> Extends { get; set; }
    }
}
