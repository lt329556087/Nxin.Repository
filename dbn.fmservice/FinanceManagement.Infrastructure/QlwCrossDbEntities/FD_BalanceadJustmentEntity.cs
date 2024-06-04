using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_BalanceadJustmentODataEntity : OneWithManyQueryEntity<FD_BalanceadJustmentDetailODataEntity>
    {
        public FD_BalanceadJustmentODataEntity()
        {
            Lines = new List<FD_BalanceadJustmentDetailODataEntity>();
        }

        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        /// <summary>
        /// DataDate
        /// </summary>		
        public string DataDate{get;set;}
        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }
        /// <summary>
        /// AccountName
        /// </summary>		
        public string AccountName { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }
        /// <summary>
        /// 企业调整后余额
        /// </summary>
        public decimal EnterProjectAmount { get; set; }
        /// <summary>
        /// 银行调整后余额
        /// </summary>
        public decimal BankProjectAmount { get; set; }
        /// <summary>
        /// 差额
        /// </summary>
        public decimal DiffAmount { get; set; }
        public string OwnerName { get; set; }
        public string CheckedByName { get; set; }

        public string ReviewName { get; set; }
        public string OwnerID { get; set; }
        public string CheckedByID { get; set; }
        public string ReviewID { get; set; }
        public string CreatedDate { get; set; }
    }

    public class FD_BalanceadJustmentDetailODataEntity
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
        /// <summary>
        /// EnterProjectID
        /// </summary>		
        public string EnterProjectID { get; set; }

        public string BankProjectID { get; set; }
        public string EnterProjectName { get; set; }

        public string BankProjectName { get; set; }
        /// <summary>
        /// EnterProjectAmount
        /// </summary>		
        public decimal EnterProjectAmount { get; set; }

        /// <summary>
        /// BankProjectAmount
        /// </summary>		
        public decimal BankProjectAmount { get; set; }
    }
    public class FD_AccountODataEntity
    {
        [Key]
        public string AccountID { get; set; }
        public string AccountNumber { get; set; }
        public string BankID { get; set; }
        public string BankAreaID { get; set; }
        public string BankNumber { get; set; }
        public bool OpenBankEnterConnect { get; set; }
    }
}
