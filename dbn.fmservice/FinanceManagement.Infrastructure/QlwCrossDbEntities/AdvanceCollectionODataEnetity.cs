using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    /// <summary>
    /// 预收款核销
    /// </summary>
   public class AdvanceCollectionODataEnetity
    {
        [Key]
        public string PrimaryKey { get; set; }
        public string CustomerID { get; set; }
        public string SalesmanID { get; set; }
        public string MarketID { get; set; }
        public decimal CancellationAmount { get; set; }
    }
    /// <summary>
    /// 税费抵扣
    /// </summary>
    public class ExpenseODataEnetity
    {
        [Key]
        public string PrimaryKey { get; set; }
        public string MarketID { get; set; }
        public string SalesmanID { get; set; }
        public int Type { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
