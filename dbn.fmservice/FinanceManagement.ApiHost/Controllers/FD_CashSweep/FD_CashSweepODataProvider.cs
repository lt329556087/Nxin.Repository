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

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FD_CashSweepODataProvider
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;

        public FD_CashSweepODataProvider(IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
        }
     
        public  Task<List<FD_CashSweepODataEntity>> GetData(ODataQueryOptions<FD_CashSweepODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@"	 SELECT    CONVERT( enter.`EnterpriseId` USING utf8mb4) EnterpriseId , 
                                        enter.`EnterpriseName`,account.`AccountName`,IFNULL(b.SweepBalance,0) SweepBalance,
                                        IFNULL(b.AccountBalance,0) AccountBalance,
                                        IFNULL(b.OtherAccountBalance,0) OtherAccountBalance,
                                        IFNULL(b.AccountBalance+b.OtherAccountBalance,0) TotalBalance,
                                        IFNULL(b.AutoSweepBalance,0) AutoSweepBalance,
                                        IFNULL(b.ManualSweepBalance,0) ManualSweepBalance,
                                        CONVERT(a.`NumericalOrder` USING utf8mb4) NumericalOrder, 
                                         IF(a.ExcuteDate,DATE_FORMAT(a.ExcuteDate,'%Y-%m-%d %H:%i'),'') ExcuteDate 
                                         , CASE WHEN (FIND_IN_SET(0, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '详情' WHEN e.excuteStatusResults is null  THEN '未归集'  
                                        ELSE '归集失败' END ExecuteStatusResult
                                        FROM nxin_qlw_business.`fm_cashsweep` a
                                        INNER JOIN (SELECT b.NumericalOrder,SUM(b.AutoSweepBalance+b.ManualSweepBalance) SweepBalance,   b.AutoSweepBalance,b.ManualSweepBalance,  b.AccountBalance,
                                         OtherAccountBalance FROM `nxin_qlw_business`.`fm_cashsweepdetail` b  GROUP BY b.NumericalOrder) b ON a.NumericalOrder = b.NumericalOrder
                                        LEFT JOIN `nxin_qlw_business`.`fd_account` account ON a.AccountID = account.`AccountID` -- 账户
                                        LEFT JOIN `qlw_nxin_com`.`biz_enterprise` enter ON a.`EnterpriseID` = enter.`EnterpriseID`
                                        LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT faac.results) resultsRESu 
                                        FROM NXin_Qlw_Business.fm_cashsweep a 
                                        INNER JOIN qlw_nxin_com.faauditrecord faac  ON a.NumericalOrder = faac.NumericalOrder
                                        GROUP BY a.`NumericalOrder`
                                        ) audit ON a.NumericalOrder = audit.NumericalOrder -- 审批结果
                                        LEFT JOIN (			
                                        SELECT a.numericalorder,GROUP_CONCAT(DISTINCT b.Status) excuteStatusResults 
                                        FROM NXin_Qlw_Business.fm_cashsweep a 
                                        INNER JOIN `nxin_qlw_business`.`fm_cashsweepdetail` b  ON a.NumericalOrder = b.NumericalOrder
                                        GROUP BY a.`NumericalOrder`
                                        ) e ON a.NumericalOrder = e.NumericalOrder -- 执行状态
                                        WHERE 
                                        enter.PId={_identityservice.GroupId}
                                       and (FIND_IN_SET(1, audit.resultsRESu) AND (LENGTH(audit.resultsRESu) - LENGTH(REPLACE( audit.resultsRESu,',','' ))) = 0)
                                        ORDER BY a.DataDate";
            return _context.CashSweepDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
    }
}
