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
    public class FM_ExpenseMarketCal:FM_ExpenseBaseCalculate
    {
        public FM_ExpenseMarketCal(IIdentityService identityService, QlwCrossDbContext context,
                                        EnterprisePeriodUtil enterpriseperiodUtil):base(identityService, context, enterpriseperiodUtil)
        {
        }

        public override string QuerySql()
        {
            //猪场对应部门的关系是1对1的；陆锦荣
            string sql = @$"SELECT
                                    " + CreateSelectSql() + @$",CONCAT(t2.MarketID,'') AS MarketID,CONCAT(IFNULL(e.`PigFarmID`,0),'') as PigFarmID,CONVERT(FLOOR(1 + (RAND() * 100000000)), CHAR) AS PrimaryKey
		                            FROM NXin_Qlw_Business.FD_SettleReceipt  t1  INNER JOIN  NXin_Qlw_Business.FD_SettleReceiptDetail t2 
		                            ON t1.NumericalOrder=t2.NumericalOrder
                                    INNER JOIN qlw_nxin_com.`biz_accosubject` c ON t2.AccoSubjectID=c.accoSubjectid
                                    LEFT JOIN qlw_nxin_com.`biz_market` d ON t2.`MarketID`=d.`MarketID`
                                    LEFT JOIN nxin_qlw_zlw.`biz_pigfarm` e ON e.`EnterpriseID`=t1.`EnterpriseID` AND e.`ManagementId`>0 AND LOCATE(e.`ManagementId`,d.`cAxis`)>0
		                            WHERE  t1.DataDate  BETWEEN  '{BeginDate}' AND '{EndDate}' AND t1.EnterpriseID={EnterPriseID} AND LOCATE('{AccoSubjectID}',c.`cAxis`)>0
		                            ";
            sql += CreateCondition();
            sql += "GROUP BY t2.MarketID";
            return sql;
        }
    }
}
