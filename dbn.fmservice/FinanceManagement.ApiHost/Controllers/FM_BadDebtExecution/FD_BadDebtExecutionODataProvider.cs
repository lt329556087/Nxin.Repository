using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BadDebtExecution
{
    public class FD_BadDebtExecutionODataProvider
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_BadDebtExecutionODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }

        public IEnumerable<FD_BadDebtExecutionODataEntity> GetList(ODataQueryOptions<FD_BadDebtExecutionODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public  IQueryable<FD_BadDebtExecutionODataEntity> GetData(ODataQueryOptions<FD_BadDebtExecutionODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        /// <summary>
        /// 列表`
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public  IQueryable<FD_BadDebtExecutionODataEntity> GetDatas(NoneQuery query = null)
        {
            FormattableString sql = $@" SELECT 
                                        exe.RecordID,
                                        CONVERT(appid USING utf8mb4) AppID,
                                        CONVERT( s.Number USING utf8mb4) NumberReceipt,
                                        m.ctext AppName,
                                        CONVERT( exe.CreateDate USING utf8mb4) CreateDate,
                                        exe.Remarks ,
                                        exe.State,
                                        '' StateName,    
                                        CONVERT( exe.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT( s.NumericalOrder USING utf8mb4) NumericalOrderReceipt,
                                        case when exe.appId = 2110211501330000109 then bp.Number 
                                        when exe.appId = 2110211503260000109     then bo.Number
                                        else  br.Number end Number
                                        from  nxin_qlw_business.fd_baddebtexecution exe
                                        Left join qlw_nxin_com.stmenu m on exe.appId = m.menuId
                                        left join  nxin_qlw_business.fd_settlereceipt s on exe.NumericalOrderReceipt = s.NumericalOrder
                                        Left join nxin_qlw_business.fd_baddebtrecover br on exe.Numericalorder = br.Numericalorder
                                        Left join nxin_qlw_business.fd_baddebtprovision bp on exe.Numericalorder = bp.Numericalorder
                                        left join nxin_qlw_business.fd_baddebtoccur bo on  exe.Numericalorder = bo.Numericalorder
                                    WHERE  exe.EnterpriseID = {_identityservice.EnterpriseId}
                                    ORDER BY CreateDate DESC";
     
            return _context.FD_BadDebtExecutionDataSet.FromSqlInterpolated(sql);
        }

       
    }
}
