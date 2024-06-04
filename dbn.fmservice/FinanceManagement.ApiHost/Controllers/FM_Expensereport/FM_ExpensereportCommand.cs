using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_Expensereport
{
    #region Dto
    public class FM_ExpensereportDto : fm_expensereport, IRequest<Result>
    {

    }
    public class fm_expensereportdetailDto : fm_expensereportdetail
    { }
    public class fm_expensereportdetaillogDto : fm_expensereportdetaillog
    { }
    public class fm_expensereportextendDto : fm_expensereportextend
    { }
    public class fm_expensereportextendlistDto : fm_expensereportextendlist
    { }
    #endregion

    #region Command
    public class FM_ExpensereportCommand : FM_ExpensereportDto, IRequest<Result>
    {
    }
    public class FM_ExpensereportAddCommand : FM_ExpensereportDto, IRequest<Result>
    {

    }

    public class FM_ExpensereportDeleteCommand : FM_ExpensereportDto, IRequest<Result>
    {

    }

    public class FM_ExpensereportModifyCommand : FM_ExpensereportDto, IRequest<Result>
    {
    }

    public class FM_ExpensereportSummaryCommand : FM_ExpensereportDto, IRequest<Result>
    {
    }

    public class FM_ExpensereportLogsCommand : FM_ExpensereportDto, IRequest<Result>
    { }

    public class FM_ExpensereportFillCommand : fm_expensereportdetailDto, IRequest<Result>
    {
    }

    public class FM_ExpensereportDetailLogsCommand : FM_ExpenseReportLogsEntity, IRequest<Result>
    { }

    /// <summary>
    /// 批量删除
    /// </summary>
    public class FM_ExpensereportBatchDelCommand : List<string>, IRequest<Result>
    {

    }
    #endregion
}
