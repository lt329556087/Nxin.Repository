using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFD_ExpenseRepository : IRepository<FD_Expense, string>
    {
    }


    public interface IFD_ExpenseDetailRepository : IRepository<FD_ExpenseDetail, string>
    {
    }
    public interface IFD_ExpenseExtRepository : IRepository<FD_ExpenseExt, string>
    {
    }
}
