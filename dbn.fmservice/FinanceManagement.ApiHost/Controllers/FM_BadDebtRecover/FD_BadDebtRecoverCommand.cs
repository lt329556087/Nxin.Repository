using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtRecover
{

    public class FD_BadDebtRecoverAddCommand : FD_BadDebtRecoverCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtRecoverDeleteCommand : FD_BadDebtRecoverCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtRecoverModifyCommand : FD_BadDebtRecoverCommand, IRequest<Result>
    {
    }

    public class FD_BadDebtRecoverCommand : MutilLineCommand<FD_BadDebtRecoverDetailCommand>
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }

        /// <summary>
        /// 收款单流水号
        /// </summary>
        public string NumericalOrderReceive { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public string DataDate { get; set; }

        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointID { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        ///客户ID
        /// </summary>
        public string CustomerID { get; set; }

        public string PersonID { get; set; }

        public string BusinessType { get; set; }

        public string CAccoSubjectID { get; set; }
        public string AccoSubjectID1 { get; set; }


        public string AccoSubjectID2 { get; set; }

        public string NumericalOrderSetting { get; set; }


        [NotMapped]
        public List<FD_BadDebtRecoverDetailCommand> Lines1 { get; set; }

        [NotMapped]
        public List<FD_BadDebtRecoverDetailCommand> Lines2 { get; set; }
    }

    public class FD_BadDebtRecoverDetailCommand : CommonOperate
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }


        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public string MarketID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string PersonID { get; set; }

        /// <summary>
        /// 本次坏账收回金额
        /// </summary>
        public decimal CurrentRecoverAmount { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }
}
