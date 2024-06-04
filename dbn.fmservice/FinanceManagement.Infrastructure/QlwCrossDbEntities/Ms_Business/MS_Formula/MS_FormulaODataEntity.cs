using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class MS_FormulaODataEntity : OneWithManyQueryEntity<MS_FormulaDetailODataEntity>
    {
        public MS_FormulaODataEntity()
        {
            Lines = new List<MS_FormulaDetailODataEntity>();
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
        public string DataDate { get; set; }
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
        public string OwnerName { get; set; }
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
        public string EffectiveBeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EffectiveEndDate { get; set; }
        [NotMapped]
        public List<MS_FormulaExtendODataEntity> Extends { get; set; }
        [NotMapped]
        public List<DictionaryData> UseEnterpriseList { get; set; } = new List<DictionaryData>();
        [NotMapped]
        public List<DictionaryData> UseProductList { get; set; } = new List<DictionaryData>();

    }
    public class DictionaryData
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class MS_FormulaListODataEntity 
    {
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 字号（开票点）
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Number { get; set; }
        public string FormulaName { get; set; }
        public bool IsUse { get; set; }
        public decimal BaseQuantity { get; set; }
        public string PackageRemarks { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string ReviewID { get; set; }
        public string ReviewName { get; set; }
        public int IsCheck { get; set; }
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
        public string UseEnterpriseIds { get; set; }
        public string UseEnterpriseNames { get; set; }
        public string UseProduct { get; set; }
        public string UseProductIds { get; set; }
        public string UseProductNames { get; set; }
        public int IsGroup { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string EffectiveBeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EffectiveEndDate { get; set; }

    }
}
