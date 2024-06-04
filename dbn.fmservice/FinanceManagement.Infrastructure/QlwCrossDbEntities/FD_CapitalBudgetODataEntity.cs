using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_CapitalBudgetODataEntity : OneWithManyQueryEntity<FD_CapitalBudgetDetailODataEntity>
    {

        [Key]
        public string NumericalOrder { get; set; }

        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>		
        public string DataDate { get; set; }

        /// <summary>
        /// TicketedPointID
        /// </summary>		
        public string TicketedPointID { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// CapitalBudgetType
        /// </summary>		
        public string CapitalBudgetType { get; set; }

        /// <summary>
        /// CapitalBudgetAbstract
        /// </summary>		
        public string CapitalBudgetAbstract { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>		
        public string StartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>		
        public string EndDate { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }
        public string OwnerName { get; set; }

        public string CheckedByID { get; set; }

        public string CheckedByName { get; set; }

        public string FinanceID { get; set; }

        public string FinanceName { get; set; }

        public decimal Amount { get; set; }
        /// <summary>
        /// 预算单类别
        /// </summary>
        public string AbstractName { get; set; }

        public string EnterpriseName { get; set; }
        /// <summary>
        /// 期间
        /// </summary>
        public string Duration { get; set; }

        public string MarketID { get; set; }

        public string MarketName { get; set; }
    }
    public class FD_CapitalBudgetDetailODataEntity
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
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// 摘要ID
        /// </summary>		
        public string ReceiptAbstractID { get; set; }
        public string ReceiptAbstractName { get; set; }

        /// <summary>
        /// 对象名称
        /// </summary>
        public string PaymentObjectName { get; set; }

        /// <summary>
        /// 对象ID（字典）
        /// </summary>		
        public string PaymentObjectID { get; set; }


        /// <summary>
        /// 支出金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal ReceiptAmount { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        public string SettleSummaryGroupCode { get; set; }
    }

    public class AuditODataEntity
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseName { get; set; }
        public string DataDate { get; set; }
        public string ExpenseTypeName { get; set; }
        public string ExpenseType { get; set; }
        public string ReceiptAbstractName { get; set; }
        public decimal Amount { get; set; }
        public int ApprovalState { get; set; }

        public string ApprovalStateName { get; set; }

        public string OwnerName { get; set; }
    }

}
