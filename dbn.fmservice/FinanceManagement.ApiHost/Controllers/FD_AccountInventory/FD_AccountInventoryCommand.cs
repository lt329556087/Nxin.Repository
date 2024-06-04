using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_AccountInventory
{

    public class FD_AccountInventoryAddCommand : FD_AccountInventoryCommand, IRequest<Result>
    {

    }

    public class FD_AccountInventoryDeleteCommand : FD_AccountInventoryCommand, IRequest<Result>
    {

    }

    public class FD_AccountInventoryModifyCommand : FD_AccountInventoryCommand, IRequest<Result>
    {
    }

    public class FD_AccountInventoryCommand : MutilLineCommand<FD_AccountInventoryDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        private DateTime _dataDate;
        /// <summary>
        /// DataDate
        /// </summary>		
        public string DataDate
        {
            get { return _dataDate.ToString("yyyy-MM-dd"); }
            set { _dataDate = Convert.ToDateTime(value); }
        }

        /// <summary>   
        /// TicketedPointID
        /// </summary>		
        public string TicketedPointID { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// 负责人ID
        /// </summary>		
        public string ResponsiblePerson { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string ResponsiblePersonName { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// 流动资金
        /// </summary>
        public decimal SumFlowAmount { get; set; }
        /// <summary>
        /// 定期存款
        /// </summary>
        public decimal SumDepositAmount { get; set; }
        /// <summary>
        /// 不可用资金
        /// </summary>
        public decimal SumFrozeAmount { get; set; }

        public string CheckedByID { get; set; }
        public string CheckedByName { get; set; }
    }

    public class FD_AccountInventoryDetailCommand : CommonOperate
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
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }

        public string AccountName { get; set; }

        /// <summary>
        /// AccoSubjectID
        /// </summary>		
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 科目编码
        /// </summary>
        public string AccoSubjectCode { get; set; }
        public string cAccoSubjectFullName { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>		
        public string cAccoSubjectName { get; set; }
        /// <summary>
        /// FlowAmount
        /// </summary>		
        public decimal FlowAmount { get; set; }

        /// <summary>
        /// DepositAmount
        /// </summary>		
        public decimal DepositAmount { get; set; }

        /// <summary>
        /// FrozeAmount
        /// </summary>		
        public decimal FrozeAmount { get; set; }


        /// <summary>
        /// 期货保证金
        /// </summary>
        public decimal FuturesBond { get; set; }

        /// <summary>
        /// 其他保证金
        /// </summary>
        public decimal OtherBond { get; set; }

        /// <summary>
        /// 银行冻结
        /// </summary>
        public decimal BankFrozen { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public decimal OtherAmount { get; set; }

        /// <summary>
        /// 账面金额
        /// </summary>
        public decimal BookAmount { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        public bool bEnd { get; set; }

        public decimal SumAmount { get; set; }

    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }
}
