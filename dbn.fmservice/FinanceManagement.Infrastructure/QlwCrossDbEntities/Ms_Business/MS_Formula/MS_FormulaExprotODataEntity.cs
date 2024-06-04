using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class MS_FormulaExprotODataEntity
    {
        [Key]
        public string PrimaryKey { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }

        public string FormulaName { get; set; }
        public decimal BaseQuantity { get; set; }
        public string Remarks { get; set; }
        public string PackageRemarks { get; set; }
        public string UseEnterpriseNames { get; set; }
        public string UseProductNames { get; set; }
        public string EffectiveBeginDate { get; set; }
        public string EffectiveEndDate { get; set; }

        public string ProductName { get; set; }
        public string Specification1 { get; set; }
        public string MeasureUnitName { get; set; }
        public decimal ProportionQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Cost { get; set; }
        public string ProductExtendName { get; set; }
        public string Specification2 { get; set; }
        public decimal StandardPack { get; set; }
        public string PackingName { get; set; }
        public decimal QuantityExtend { get; set; }
        public string IsUse { get; set; }
        [NotMapped]
        public string EffectiveDate => this.EffectiveBeginDate + "-" + this.EffectiveEndDate;
    }
}
