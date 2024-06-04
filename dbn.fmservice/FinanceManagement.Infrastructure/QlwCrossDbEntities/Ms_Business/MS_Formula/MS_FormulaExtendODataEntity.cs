using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class MS_FormulaExtendODataEntity
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
        public decimal StandardPack { get; set; }
        public string Specification { get; set; }
        public string PackingID { get; set; }
        public string PackingName { get; set; }
        public decimal Quantity { get; set; }
        public bool IsUse { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int RowNum { get; set; }

    }
}
