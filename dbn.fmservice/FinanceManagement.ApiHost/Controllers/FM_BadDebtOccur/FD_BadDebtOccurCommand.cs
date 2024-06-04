using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtOccur
{

    public class FD_BadDebtOccurAddCommand : FD_BadDebtOccurCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtOccurDeleteCommand : FD_BadDebtOccurCommand, IRequest<Result>
    {

    }

    public class FD_BadDebtOccurModifyCommand : FD_BadDebtOccurCommand, IRequest<Result>
    {
    }

    public class FD_BadDebtOccurCommand : MutilLineCommand<FD_BadDebtOccurDetailCommand>
    {
        [Key]
        public string NumericalOrder { get; set; }

        public string NumericalOrderReceive { get; set; }
        public string Number { get; set; }
        public string DataDate { get; set; }

        public string CreateDate { get; set; }

        public string TicketedPointID { get; set; }
        public string EnterpriseID { get; set; }
        public string CustomerID { get; set; }

        /// <summary>
        /// 坏账科目1
        /// </summary>
        public string CAccoSubjectID { get; set; }

        /// <summary>
        /// 坏账科目1
        /// </summary>
        public string AccoSubjectID1 { get; set; }

        /// <summary>
        /// 坏账科目2
        /// </summary>
        public string AccoSubjectID2 { get; set; }

        public string NumericalOrderSetting { get; set; }

        public string BusinessType { get; set; }

        public string PersonID { get; set; }

        [NotMapped]
        public List<FD_BadDebtOccurDetailCommand> Lines1 { get; set; }

        [NotMapped]
        public List<FD_BadDebtOccurDetailCommand> Lines2 { get; set; }
    }

    public class FD_BadDebtOccurDetailCommand : CommonOperate
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }
        public string MarketID { get; set; }
        public string PersonID { get; set; }

        /// <summary>
        /// 本次坏账发生金额
        /// </summary>
        public decimal CurrentOccurAmount { get; set; }

        /// <summary>
        /// 期末余额
        /// </summary>
        public decimal Amount { get; set; }

    }

    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }
}
