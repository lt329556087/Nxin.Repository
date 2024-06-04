using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FM_Expensereport
{
    public class FM_ExpenseOtherCal : FM_ExpenseBaseCalculate
    {
        public FM_ExpenseOtherCal(IIdentityService identityService, QlwCrossDbContext context,
                                        EnterprisePeriodUtil enterpriseperiodUtil) : base(identityService, context, enterpriseperiodUtil)
        {
        }

    }
}
