using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class MS_FormulaDetail : EntityBase
    {
        public void Add() { }

        public void Update(string ProductID, decimal ProportionQuantity, decimal Quantity, int RowNum, decimal UnitCost, decimal Cost)
        {
            this.ProductID = ProductID;
            this.ProportionQuantity = ProportionQuantity;
            this.Quantity = Quantity;
            this.RowNum = RowNum;
            this.UnitCost = UnitCost;
            this.Cost = Cost;;
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
        public string StockType { get; set; }
        public string FormulaTypeID { get; set; }
        public decimal ProportionQuantity { get; set; }
        public decimal Quantity { get; set; }
        public int RowNum { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Cost { get; set; }
        public DateTime ModifiedDate { get; set; }
        public MS_Formula MS_Formula { get; set; }

    }
}
