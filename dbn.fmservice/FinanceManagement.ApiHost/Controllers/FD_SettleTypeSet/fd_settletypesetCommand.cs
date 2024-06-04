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
    public class fd_settletypesetSaveCommand : fd_settletypeset, IRequest<Result>
    {

    }
    public class fd_settletypesetRemoveCommand : fd_settletypeset, IRequest<Result>
    {

    }
    public class fd_settletypeSaveCommand : biz_datadict, IRequest<Result>
    {
        /// <summary>
        /// true = 走删除
        /// </summary>
        public bool IsDelete { get; set; } = false;
    }
}
