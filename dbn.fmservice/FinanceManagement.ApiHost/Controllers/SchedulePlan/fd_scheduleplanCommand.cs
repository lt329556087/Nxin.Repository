using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class fd_scheduleplanSaveCommand : List<fd_scheduleplan>, IRequest<Result>
    {

    }
    public class fd_scheduleplanCancelCommand : List<fd_scheduleplan>, IRequest<Result>
    {

    }
    public class ResultExpense: Result
    {
        public int count { get; set; }
    }
    public class QueryData
    {
        /// <summary>
        /// 申请日期
        /// </summary>
        public string BeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 第几页
        /// </summary>
        public string PageSize { get; set; }
        /// <summary>
        /// 一页显示多少行
        /// </summary>
        public string PageIndex { get; set; }
        /// <summary>
        /// (排程专用)第几页
        /// </summary>
        public string PlanPageSize { get; set; }
        /// <summary>
        /// （排程专用）一页显示多少行
        /// </summary>
        public string PlanPageIndex { get; set; }
        /// <summary>
        /// 申请单的付款到期日
        /// </summary>
        public string ApplyDeadBeginDate { get; set; }
        public string ApplyDeadEndDate { get; set; }
        public string DeadBeginDate { get; set; } = "1900-01-01";
        /// <summary>
        /// 
        /// </summary>
        public string DeadEndDate { get; set; } = "2110-12-31";
        /// <summary>
        /// 
        /// </summary>
        public string MenuID { get; set; } = "2205231636100000109";
        /// <summary>
        /// 当前登录人ID
        /// </summary>
        public string Bo_ID { get; set; }
        /// <summary>
        /// 当前单位ID
        /// </summary>
        public string EnteID { get; set; }
        /// <summary>
        /// 单位（逗号分隔）
        /// </summary>
        public string EnterpriseIDs { get; set; }
        /// <summary>
        /// 单据类型（逗号分隔）
        /// </summary>
        public string ExpenseTypes { get; set; }
        /// <summary>
        /// 往来单位 模糊匹配（名字）
        /// </summary>
        public string PayerName { get; set; }
        /// <summary>
        /// 是否紧急
        /// </summary>
        public string Pressing  { get; set; }
        /// <summary>
        /// 最大金额（区间查询）
        /// </summary>
        public decimal? MaxAmount { get; set; }
        /// <summary>
        /// 最小金额（区间金额）
        /// </summary>
        public decimal? MinAmount { get; set; }
        /// <summary>
        /// 流水号（申请流水号）
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 只用作列表查询 跟表中的 不是一个东西
        /// 排程状态（0:未排程,1：排程中,2:已排程）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// true = 排程，false = 申请
        /// </summary>
        public bool IsSchedule { get; set; } = false;
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PaymentBeginDate { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PaymentEndDate { get; set; }
        /// <summary>
        /// 付款到期日
        /// </summary>
        public DateTime? PlanDeadLineBeginDate { get; set; }
        /// <summary>
        /// 付款到期日
        /// </summary>
        public DateTime? PlanDeadLineEndDate { get; set; }
        /// <summary>
        /// 级别
        /// </summary>
        public string Level { get; set; }
    }
    /// <summary>
    /// 付款申请列表
    /// </summary>
    public class ExpenseData
    {
        /// <summary>
        /// 
        /// </summary>
        public string PayerId { get; set; }
        /// <summary>
        /// 张志格测用
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceiptAbstractCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExpenseType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsCreate { get; set; }
        /// <summary>
        /// 北京助农
        /// </summary>
        public string PayerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ResultID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 账户资金调拨
        /// </summary>
        public string ExpenseTypeName { get; set; }
        /// <summary>
        /// 其他
        /// </summary>
        public string BusinessTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 北京助农
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 调拨2
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Pressing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AdjustAmount { get; set; }
        public DateTime HouldPayDate { get; set; }
    }
}
