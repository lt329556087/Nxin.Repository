using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class MS_FormulaDetailODataEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string Specification { get; set; }
        public string MeasureUnitName { get; set; }
        public string StockType { get; set; }
        public string FormulaTypeID { get; set; }
        public decimal ProportionQuantity { get; set; }
        public decimal Quantity { get; set; }
        public int RowNum { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Cost { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
