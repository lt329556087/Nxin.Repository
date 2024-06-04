using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceManagement.Infrastructure.Repositories.Interfaces
{
    public interface IMS_FormulaProductPriceRepository : IRepository<MS_FormulaProductPrice, string>
    {

    }



    public interface IMS_FormulaProductPriceDetailRepository : IRepository<MS_FormulaProductPriceDetail, string>
    {
    }


    public interface IMS_FormulaProductPriceExtRepository : IRepository<MS_FormulaProductPriceExt, string>
    {
        Task<List<MS_FormulaProductPriceExt>> GetExtAsync(string id,string detailID, int BusiType);
    }
    
}
