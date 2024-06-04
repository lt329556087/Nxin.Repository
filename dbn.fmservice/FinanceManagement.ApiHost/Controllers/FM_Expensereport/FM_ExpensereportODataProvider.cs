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

namespace FinanceManagement.ApiHost.Controllers.FM_Expensereport
{
    public class FM_ExpensereportODataProvider : QueryProviderAbstraction<FM_ExpensereportEntity>
    {
        private IIdentityService _identityService;
        private QlwCrossDbContext _context;

        public FM_ExpensereportODataProvider(IIdentityService identityService, QlwCrossDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        /// <summary>
        /// OData 专用
        /// </summary>
        /// <returns></returns>
        public override IQueryable<FM_ExpensereportEntity> GetDatas()
        {
            FormattableString sql = @$"SELECT 
              CONCAT(a.NumericalOrder,'') AS NumericalOrder,
              STR_TO_DATE(a.DataDate,'%Y-%m-%d') AS DataDate,
              a.ReportPeriod,
              CONCAT(a.EnterpriseID,'') AS EnterpriseID,
              a.ExpenseAmount,
              a.Remarks,
              CONCAT(a.OwnerID,'') AS OwnerID,
              b.Name AS OwnerName,
              IF(c.IsCollect='1',1,0) AS IsCollect,
              IF(d.NumericalOrder IS NULL,0,1) AS IsErrorLogs,
              STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate 
            FROM
              nxin_qlw_business.fm_expensereport a
              LEFT JOIN nxin_qlw_business.hr_person b ON a.OwnerID=b.BO_ID
              LEFT JOIN 
              (
		        SELECT a.`NumericalOrder`,GROUP_CONCAT(DISTINCT a.IsCollect ORDER BY a.IsCollect) IsCollect FROM nxin_qlw_business.`fm_expensereportdetail` a
		        GROUP BY a.`NumericalOrder`
              ) c ON a.`NumericalOrder`=c.NumericalOrder
              LEFT JOIN 
              (
		        SELECT a.`NumericalOrder` FROM nxin_qlw_business.`fm_expensereportdetail` a
		        INNER JOIN nxin_qlw_business.`fm_expensereportdetaillog` b ON a.recordid=b.`DetailID`
		        GROUP BY a.`NumericalOrder`
              ) d ON a.`NumericalOrder`=d.NumericalOrder
            WHERE a.EnterpriseID = {_identityService.EnterpriseId} ORDER BY a.ReportPeriod DESC";
            return _context.FM_ExpensereportSet.FromSqlInterpolated(sql);
        }

        public FM_ExpensereportEntity GetSingleData(string numericalOrder, string reportPeriod="")
        {
            var query = GetDatas();
            FM_ExpensereportEntity main = query.Where(_ => _.NumericalOrder == numericalOrder||_.ReportPeriod==reportPeriod).FirstOrDefault();
            if (main == null)
            {
                return CreateNewFM_ExpensereportEntity(reportPeriod);
            }

            #region 获取明细-有记录
            FormattableString sqldetail = $@"SELECT 
              a.RecordID,
              CONCAT(a.NumericalOrder,'') AS NumericalOrder,
              CONCAT(a.CostProjectID,'') AS CostProjectID,
              b.`CostProjectCode`,
              b.`CostProjectName`,
              CONCAT(a.CollectionType,'') AS CollectionType,
              c.cDictName AS CollectionTypeName,
              CONCAT(a.DataSource,'') AS DataSource,
              d.cDictName AS DataSourceName,
              CONCAT(a.AllocationType,'') AS AllocationType,
              e.cDictName AS AllocationTypeName,
              a.ExpenseAmount,
              a.IsCollect,
              STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate  
            FROM
              nxin_qlw_business.fm_expensereportdetail a
              LEFT JOIN nxin_qlw_business.fm_costproject b ON a.CostProjectID=b.CostProjectID
              LEFT JOIN qlw_nxin_com.bsdatadict c ON a.CollectionType=c.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict d ON a.DataSource=d.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict e ON a.AllocationType=e.DictID
              WHERE a.NumericalOrder={main.NumericalOrder}";
            main.DetailList = _context.fm_expensereportdetailSet.FromSqlInterpolated(sqldetail).ToList();

            #region 获取明细的辅助项
            main.DetailList.ForEach(_ => {
                #region 取数明细
                FormattableString sqlextend = $@"SELECT 
                      a.RecordID,
                      a.DetailID,
                      CONCAT(a.PigFarmID,'') AS PigFarmID,
                      b.PigFarmName,
                      CONCAT(a.DeptOrOthersID,'') AS DeptOrOthersID,
                      CASE WHEN c.CollectionType=202202111355001102 THEN d.MarketName 
                      WHEN c.CollectionType=202202111355001103 THEN e.Name 
                      WHEN c.CollectionType=202202111355001104 THEN b.PigFarmName 
                      WHEN c.CollectionType=202202111355001105 THEN g.cDictName 
                      WHEN c.CollectionType=202202111355001106 THEN f.PigHouseUnitName 
                      WHEN c.CollectionType=202202111355001107 THEN h.BatchName
                      ELSE '' END AS DeptOrOthersName,
                      a.ExpenseAmount,
                      a.Remarks,
                      STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
                      STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate 
                    FROM
                      nxin_qlw_business.fm_expensereportextend a
                      LEFT JOIN nxin_qlw_zlw.biz_pigfarm b ON a.PigFarmID=b.PigFarmID
                      LEFT JOIN nxin_qlw_business.fm_expensereportdetail c ON a.DetailID=c.RecordID
                      LEFT JOIN qlw_nxin_com.biz_market d ON a.DeptOrOthersID=d.MarketID
                      LEFT JOIN nxin_qlw_business.hr_person e ON a.DeptOrOthersID=e.BO_ID
                      LEFT JOIN nxin_qlw_zlw.biz_pighouseunit f ON a.DeptOrOthersID=f.PigHouseUnitID
                      LEFT JOIN qlw_nxin_com.bsdatadict g ON a.DeptOrOthersID=g.dictid
                      LEFT JOIN nxin_qlw_zlw.biz_batch h ON a.DeptOrOthersID=h.BatchID
                  WHERE a.DetailID={_.RecordID}";
                _.ExtendList = _context.fm_expensereportextendSet.FromSqlInterpolated(sqlextend).ToList();
                #endregion

                #region 归集明细
                FormattableString sqlextendlist = $@"SELECT 
                  a.RecordID,
                  a.DetailID,
                  CONCAT(a.PigFarmID,'') AS PigFarmID,
                  b.PigFarmName,
                  CONCAT(a.DeptOrOthersID,'') AS DeptOrOthersID,
                  CASE WHEN c.CollectionType=202202111355001102 THEN d.MarketName 
                  WHEN c.CollectionType=202202111355001103 THEN e.Name 
                  WHEN c.CollectionType=202202111355001104 THEN b.PigFarmName 
                  WHEN c.CollectionType=202202111355001105 THEN g.cDictName 
                  WHEN c.CollectionType=202202111355001106 THEN f.PigHouseUnitName 
                  WHEN c.CollectionType=202202111355001107 THEN h.BatchName
                  ELSE '' END AS DeptOrOthersName,
                  a.ExpenseValue,
                  a.ExpenseAmount,
                  a.Remarks,
                  a.CreatedDate,
                  a.ModifiedDate 
                FROM
                  nxin_qlw_business.fm_expensereportextendlist a
                  LEFT JOIN nxin_qlw_zlw.biz_pigfarm b ON a.PigFarmID=b.PigFarmID
                  LEFT JOIN nxin_qlw_business.fm_expensereportdetail c ON a.DetailID=c.RecordID
                  LEFT JOIN qlw_nxin_com.biz_market d ON a.DeptOrOthersID=d.MarketID
                  LEFT JOIN nxin_qlw_business.hr_person e ON a.DeptOrOthersID=e.BO_ID
                  LEFT JOIN nxin_qlw_zlw.biz_pighouseunit f ON a.DeptOrOthersID=f.PigHouseUnitID
                  LEFT JOIN qlw_nxin_com.bsdatadict g ON a.DeptOrOthersID=g.dictid
                  LEFT JOIN nxin_qlw_zlw.biz_batch h ON a.DeptOrOthersID=h.BatchID
                  WHERE a.DetailID={_.RecordID}";
                _.ExtendDetailList = _context.fm_expensereportextendlistSet.FromSqlInterpolated(sqlextendlist).ToList();
                #endregion

                #region 取数日志
                //FormattableString sqldetaillogs = $@"SELECT 
                //      a.RecordID,
                //      a.DetailID,
                //      a.SubsidiaryOption,
                //      a.OccuredAmount,
                //      a.ErrorCode,
                //      a.ErrorMsg,
                //      STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
                //      STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate
                //    FROM
                //      nxin_qlw_business.fm_expensereportdetaillog a
                //  WHERE a.DetailID={_.RecordID}";
                //_.DetailLogList = _context.fm_expensereportdetaillogSet.FromSqlInterpolated(sqldetaillogs).ToList();
                #endregion
            });
            #endregion

            #endregion

            return main;
        }

        /// <summary>
        /// 费用汇总表
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        public List<FM_ExpenseSummaryReportEntity> GetExpenseSummaryReport(string numericalOrder)
        {
            FormattableString sql = @$"SELECT 
                  CONVERT(FLOOR(1 + (RAND() * 100000000)), CHAR) as PrimaryKey,
		          d.CostProjectCode,
                  CONCAT(a.PigFarmID,'') AS PigFarmID,
                  b.PigFarmName,
                  CONCAT(c.CollectionType,'') AS CollectionType,
                  e.cDictName AS CollectionTypeName,
                  CONCAT(c.CostProjectID,'') AS CostProjectID,
                  d.CostProjectName,
                  SUM(IFNULL(a.ExpenseValue,0)) AS ExpenseValue,
                  SUM(IFNULL(a.ExpenseAmount,0)) AS ExpenseAmount
                FROM
	          nxin_qlw_business.`fm_expensereport` m
	          INNER JOIN nxin_qlw_business.`fm_expensereportdetail` c ON m.NumericalOrder=c.NumericalOrder
                  INNER JOIN nxin_qlw_business.fm_expensereportextendlist a ON a.DetailID=c.RecordID
                  LEFT JOIN nxin_qlw_zlw.biz_pigfarm b ON a.PigFarmID=b.PigFarmID
                  LEFT JOIN nxin_qlw_business.`fm_costproject` d ON c.CostProjectID=d.CostProjectID
                  LEFT JOIN qlw_nxin_com.bsdatadict e ON c.CollectionType=e.DictID
                  WHERE m.NumericalOrder={numericalOrder}
                  GROUP BY c.CostProjectID,a.PigFarmID,c.CollectionType";
            return _context.FM_ExpenseSummaryReportSet.FromSqlInterpolated(sql).ToList();
        }

        /// <summary>
        /// 检查报告
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <returns></returns>
        public List<FM_ExpenseReportLogsEntity> GetExpenseReportLogs(string numericalOrder)
        {
            FormattableString sql = @$"SELECT 
                  CONVERT(FLOOR(1 + (RAND() * 100000000)), CHAR) AS PrimaryKey,
                  CONCAT(m.`NumericalOrder`,'') AS NumericalOrder,
                  c.RecordID,
                  m.ReportPeriod,
                  CONCAT(c.AllocationType,'') AS AllocationType,
		          d.CostProjectCode,
                  CONCAT(c.CollectionType,'') AS CollectionType,
                  e.cDictName AS CollectionTypeName,
                  CONCAT(c.CostProjectID,'') AS CostProjectID,
                  d.CostProjectName,
                  a.`SubsidiaryOption`,
                  IFNULL(a.`OccuredAmount`,0) AS OccuredAmount,
                  a.`ErrorMsg`
                FROM
	          nxin_qlw_business.`fm_expensereport` m
	          INNER JOIN nxin_qlw_business.`fm_expensereportdetail` c ON m.NumericalOrder=c.NumericalOrder
                  INNER JOIN nxin_qlw_business.`fm_expensereportdetaillog` a ON a.DetailID=c.RecordID
                  LEFT JOIN nxin_qlw_business.`fm_costproject` d ON c.CostProjectID=d.CostProjectID
                  LEFT JOIN qlw_nxin_com.bsdatadict e ON c.CollectionType=e.DictID
                  WHERE m.NumericalOrder={numericalOrder}
                  GROUP BY c.CostProjectID,c.CollectionType";
            return _context.FM_ExpenseReportLogsSet.FromSqlInterpolated(sql).ToList();
        }

        #region private method
        private FM_ExpensereportEntity CreateNewFM_ExpensereportEntity(string reportPeriod)
        {
            var main = new FM_ExpensereportEntity() { EnterpriseID=_identityService.EnterpriseId,ReportPeriod=string.IsNullOrEmpty(reportPeriod)?DateTime.Now.ToString("yyyy.M"):reportPeriod,ExpenseAmount=0,NumericalOrder="",OwnerID=""};
            
            FormattableString sqldetail = $@"SELECT 
              CONVERT(FLOOR(1 + (RAND() * 100000000)), UNSIGNED INTEGER) as RecordID,
              '' AS NumericalOrder,
              CONCAT(a.CostProjectID,'') AS CostProjectID,
              a.`CostProjectCode`,
              a.`CostProjectName`,
              CONCAT(a.CollectionType,'') AS CollectionType,
              c.cDictName AS CollectionTypeName,
              CONCAT(a.DataSource,'') AS DataSource,
              d.cDictName AS DataSourceName,
              CONCAT(a.AllocationType,'') AS AllocationType,
              e.cDictName AS AllocationTypeName,
              0.0 AS ExpenseAmount,
              0 as IsCollect,
              STR_TO_DATE(a.CreatedDate,'%Y-%m-%d %H:%i:%s') AS CreatedDate,
              STR_TO_DATE(a.ModifiedDate,'%Y-%m-%d %H:%i:%s') AS ModifiedDate  
            FROM
              nxin_qlw_business.fm_costproject a 
              LEFT JOIN qlw_nxin_com.bsdatadict c ON a.CollectionType=c.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict d ON a.DataSource=d.DictID
              LEFT JOIN qlw_nxin_com.bsdatadict e ON a.AllocationType=e.DictID
              WHERE a.EnterpriseID={_identityService.EnterpriseId} AND a.IsUse=1 AND a.CostProjectTypeID=201904121023082104 ";
            main.DetailList = _context.fm_expensereportdetailSet.FromSqlInterpolated(sqldetail).ToList();
            return main;
        }
        #endregion
    }
}
