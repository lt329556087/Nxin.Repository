using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class MS_FormulaExtend : EntityBase
    {
        public void Update(string ProductID, string PackingID, decimal Quantity, int RowNum, bool IsUse)
        {
            this.ProductID = ProductID;
            this.PackingID = PackingID;
            this.Quantity = Quantity;
            this.RowNum = RowNum;
            this.IsUse = IsUse;
            this.ModifiedDate = DateTime.Now;
        }
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
        public string PackingID { get; set; }
        public decimal Quantity { get; set; }
        public bool IsUse { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int RowNum { get; set; }
        public MS_Formula MS_Formula { get; set; }

    }
}
