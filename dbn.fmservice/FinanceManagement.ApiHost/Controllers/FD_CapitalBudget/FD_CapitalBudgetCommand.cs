using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_CapitalBudget
{
    public class FD_CapitalBudgetDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FD_CapitalBudgetAddCommand : FD_CapitalBudgetCommand, IRequest<Result>
    {
    }

    public class FD_CapitalBudgetModifyCommand : FD_CapitalBudgetCommand, IRequest<Result>
    {
    }

    /// <summary>
    /// 表头表体关联
    /// </summary>
    /// <typeparam name="TLineCommand"></typeparam>
    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }

    public class FD_CapitalBudgetCommand : MutilLineCommand<FD_CapitalBudgetDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
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
        /// 单据：支出预算，收入预算，周预算
        /// </summary>		
        public string CapitalBudgetType { get; set; }

        /// <summary>
        /// 预算单类别
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
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }
        public string OwnerName { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }

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

        /// <summary>
        /// 审批人
        /// </summary>
        public string CheckedByID { get; set; }
        public string CheckedByName { get; set; }

        public string MarketID { get; set; }

    }

    public class FD_CapitalBudgetDetailCommand : CommonOperate
    {
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
        /// PaymentObject
        /// </summary>		
        public string PaymentObject { get; set; }

        /// <summary>
        /// Amount
        /// </summary>		
        public decimal Amount { get; set; }

        public decimal PayAmount { get; set; }

        public decimal ReceiptAmount { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        public string PaymentObjectID { get; set; }

        public string SettleSummaryGroupCode { get; set; }
    }


    public class DurationInfo
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public int Sort { get; set; }
    }

}
