using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_Expense
{
    public class FD_ExpenseDeleteCommand : IRequest<Result>
    {
        public string NumericalOrder { get; set; }
    }

    public class FD_ExpenseAddCommand : FD_ExpenseCommand, IRequest<Result>
    {
    }

    public class FD_ExpenseModifyCommand : FD_ExpenseCommand, IRequest<Result>
    {
    }
    public class FD_ExpensePreDeleteCommand : IRequest<Result>
    {
    }

    public class FD_ExpensePreAddCommand : FD_ExpenseCommand, IRequest<Result>
    {
    }

    public class FD_ExpensePreModifyCommand : FD_ExpenseCommand, IRequest<Result>
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

    public class FD_ExpenseCommand : MutilLineCommand<FD_ExpenseDetailCommand>
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 备注
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

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
                
        public Guid Guid { get; set; }
        public string ExpenseType { get; set; }
        public string ExpenseAbstract { get; set; }
        public string ExpenseSort { get; set; }
        public string PersonID { get; set; }
        public DateTime? HouldPayDate { get; set; }
        public DateTime? PayDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DraweeID { get; set; }
        public decimal CurrentVerificationAmount { get; set; }
        public int Pressing { get; set; }
        public string Number { get; set; }
        public string TicketedPointID { get; set; }
        public string UploadInfo { get; set; }
        public List<FD_ExpenseExtCommand> Extends { get; set; }
        /// <summary>
        /// 关联采购单
        /// </summary>
        public List<ExRelatedList> RelatedPurList { get; set; }
        /// <summary>
        /// 关联采购合同
        /// </summary>
        public List<ExRelatedList> RelatedConList { get; set; }
    }

    public class FD_ExpenseDetailCommand : CommonOperate
    {

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        public string RowStatus { get; set; }

        public string NumericalOrderDetail { get; set; }
        public Guid Guid { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string BusinessType { get; set; }
        public string SettleBusinessType { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>		
        public string MarketID { get; set; }
        /// <summary>
        /// 收款对象ID
        /// </summary>		
        public string PayerID { get; set; }
        public string ProjectID { get; set; }
        public string SettlePayerID { get; set; }
        public decimal Amount { get; set; }
        public string Content { get; set; }
        public string AccountInformation { get; set; }
        public string ReceiptAbstractDetail { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankDeposit { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }
        public string CustomerName { get; set; }
    }
    public class FD_ExpenseExtCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string CollectionId { get; set; }
        public string PersonId { get; set; }
        public string AccountName { get; set; }
        public string BankDeposit { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
    }
    /// <summary>
    /// 关联信息
    /// </summary>
    public class ExRelatedList
    {
        /// <summary>
        /// 
        /// </summary>
        public string RelatedNumericalOrder { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        //public decimal Paid { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public decimal Payable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Payment { get; set; }
        public string RelatedID { get; set; }
        public decimal Amount { get; set; }
        public decimal HavePaidAmount { get; set; }
    }

}
