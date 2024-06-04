using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_CashSweepSetting
{
    public class FM_CashSweepSettingDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
    }

    public class FM_CashSweepSettingAddCommand : FM_CashSweepSettingCommand, IRequest<Result>
    {
    }

    public class FM_CashSweepSettingModifyCommand : FM_CashSweepSettingCommand, IRequest<Result>
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

    public class FM_CashSweepSettingCommand : MutilLineCommand<FM_CashSweepSettingDetailCommand>
    {

        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }       
    }

    public class FM_CashSweepSettingDetailCommand : CommonOperate
    {
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 详情流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 归集方向(0：向上归集；1：向下归集)
        /// </summary>
        public int? SweepDirection { get; set; }
        /// <summary>
        /// 调拨类别
        /// </summary>
        public string AccountTransferAbstract { get; set; }
        /// <summary>
        /// 调拨事由
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }

        public DateTime ModifiedDate { get; set; }
        public List<FM_CashSweepSettingExtCommand> Extends { get; set; }
    }

    public class FM_CashSweepSettingExtCommand : CommonOperate
    {
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 详情流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 业务类型(0:付款 1：收款)
        /// </summary>
        public int? BusiType { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 业务单元
        /// </summary>
        public string OrganizationSortID { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

}
