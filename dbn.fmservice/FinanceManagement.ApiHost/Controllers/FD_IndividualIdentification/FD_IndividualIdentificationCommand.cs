using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_IndividualIdentification
{
    public class FD_IndividualIdentificationDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FD_IndividualIdentificationAddCommand : FD_IndividualIdentificationCommand, IRequest<Result>
    {
    }

    public class FD_IndividualIdentificationModifyCommand : FD_IndividualIdentificationCommand, IRequest<Result>
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

    public class FD_IndividualIdentificationCommand : MutilLineCommand<FD_IndividualIdentificationDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }

        public string EnterpriseID { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }

        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }

    }

    public class FD_IndividualIdentificationDetailCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款）
        /// </summary>
        public int BusiType { get; set; }
        /// <summary>
        /// 认定类型
        /// </summary>
        public string IdentificationType { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CurrentUnit { get; set; }
        /// <summary>
        /// 期末余额
        /// </summary>

        public decimal Amount { get; set; }
        /// <summary>
        /// 坏账准备金额
        /// </summary>

        public decimal AccrualAmount { get; set; }
        public int? DataSourceType { get; set; }
        public string ModifiedDate { get; set; }
        public string RowStatus { get; set; }

        //[NotMapped]
        public List<FD_IndividualIdentificationExtCommand> AgingList { get; set; }
    }


    public class FD_IndividualIdentificationExtCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 业务类型（0：坏账准备 1：账龄区间）
        /// </summary>
        public int BusiType { get; set; }
        /// <summary>
        /// 区间金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 区间名称
        /// </summary>
        public string Name { get; set; }
        public string ModifiedDate { get; set; }
        public string RowStatus { get; set; }
    }
}
