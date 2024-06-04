using Architecture.Seedwork.Security;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class BusinessOutputProvider
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public BusinessOutputProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }
        public List<dynamic> GetSuppliesOutAmountByMonth(DropSelectSearch model)
        {
            if (string.IsNullOrEmpty(model.EnterpriseID) || string.IsNullOrEmpty(model.DataDate))
            {
                return null;
            }
            string sql = @$"SELECT SUM(nQuantity) AS nQuantity,SUM(nAmount) AS nAmount FROM pm_supplies a
                        LEFT JOIN pm_suppliesdetail b ON a.iNumericalOrder = b.iNumericalOrder
                        WHERE a.iType = 1711231121250000101
                        AND DATE_FORMAT(a.dDate,'%Y-%m')= DATE_FORMAT('{model.DataDate}', '%Y-%m')
                        AND a.EnterpriseID IN({model.EnterpriseID})";
            return _context.DynamicSqlQuery(sql);
        }
        public dynamic GetEnterprise(string EnterpriseId)
        {
            if (string.IsNullOrEmpty(EnterpriseId))
            {
                return null;
            }
            else
            {
                return _context.DynamicSqlQuery(@$"SELECT * FROM QLW_NXIN_COM.BIZ_ENTERPRISE WHERE ENTERPRISEID = {EnterpriseId}");
            }
        }
    }
}
