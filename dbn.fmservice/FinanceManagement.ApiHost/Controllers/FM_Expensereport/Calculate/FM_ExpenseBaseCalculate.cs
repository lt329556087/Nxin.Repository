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
    public abstract class FM_ExpenseBaseCalculate
    {
        private IIdentityService _identityService;
        private QlwCrossDbContext _context;
        private EnterprisePeriodUtil _enterpriseperiodUtil;

        #region Properties
        public string EnterPriseID { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        ///// <summary>
        ///// 归集类型
        ///// </summary>
        //public string CollectionType { get; set; }
        ///// <summary>
        ///// 分摊方式
        ///// </summary>
        //public string AllocationType { get; set; }
        /// <summary>
        /// 填报期间
        /// </summary>
        public string ReportPeriod { get; set; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 取数公式
        /// </summary>
        public string DataFormula { get; set; }

        /// <summary>
        /// 辅助项
        /// </summary>
        public List<FmCostprojectExtendEntity> ExtendDetails { get; set; } = new List<FmCostprojectExtendEntity>();
        #endregion

        public FM_ExpenseBaseCalculate(IIdentityService identityService, QlwCrossDbContext context,
                                        EnterprisePeriodUtil enterpriseperiodUtil)
        {
            _identityService = identityService;
            _context = context;
            _enterpriseperiodUtil = enterpriseperiodUtil;
            EnterPriseID = string.IsNullOrEmpty(EnterPriseID) ? identityService.EnterpriseId : "0";
        }

        public virtual string QuerySql()
        {
            #region 参考sql
            //FormattableString sql = @$"SELECT
            //                        SUM(t2.Debit) AS nDebitAmount,SUM(t2.Credit) AS nCreditAmount,t2.AccoSubjectID,t2.AccoSubjectCode
		          //                  FROM   NXin_Qlw_Business.FD_SettleReceipt  t1  INNER JOIN  NXin_Qlw_Business.FD_SettleReceiptDetail t2 
		          //                  ON t1.NumericalOrder=t2.NumericalOrder
            //                        INNER JOIN qlw_nxin_com.`biz_accosubject` c ON t2.AccoSubjectID=c.accoSubjectid
		          //                  WHERE  t1.DataDate  BETWEEN  {BeginDate} AND {EndDate} AND t1.EnterpriseID={EnterPriseID} AND LOCATE(a.`AccoSubjectID`,c.`cAxis`)>0
		          //                  GROUP BY t2.AccoSubjectID,t2.AccoSubjectCode";//c.AccoSubjectCode LIKE  '5101%'  AND  
            #endregion 

            string sql = @$"SELECT
                                    " + CreateSelectSql() + @$",'0' AS MarketID,'0' as PigFarmID,CONVERT(FLOOR(1 + (RAND() * 100000000)), CHAR) AS PrimaryKey
		                            FROM NXin_Qlw_Business.FD_SettleReceipt  t1  INNER JOIN  NXin_Qlw_Business.FD_SettleReceiptDetail t2 
		                            ON t1.NumericalOrder=t2.NumericalOrder
                                    INNER JOIN qlw_nxin_com.`biz_accosubject` c ON t2.AccoSubjectID=c.accoSubjectid
		                            WHERE  t1.DataDate  BETWEEN  '{BeginDate}' AND '{EndDate}' AND t1.EnterpriseID={EnterPriseID} AND LOCATE('{AccoSubjectID}',c.`cAxis`)>0
		                            ";
            sql += CreateCondition();
            return sql;
        }

        /// <summary>
        /// 辅助项条件拼接
        /// </summary>
        /// <returns></returns>
        public virtual string CreateCondition()
        {
            var Option = this.ExtendDetails.Where(_ => !string.IsNullOrEmpty(_.RelatedType)).FirstOrDefault();
            string condition = string.Join(',', this.ExtendDetails.Select(_ => _.RelatedId));
            StringBuilder sb = new StringBuilder();
            switch (Option?.RelatedType)
            {
                case "201904150104402122"://部门
                    sb.Append(@$" AND t2.MarketID in ({condition})");
                    break;
                case "201904150104402123"://人员
                    sb.Append(@$" AND t2.PersonID in ({condition})");
                    break;
                case "201904150104402124"://客户
                    sb.Append(@$" AND t2.CustomerID in ({condition})");
                    break;
                case "201904150104402125"://供应商
                    sb.Append(@$" AND t2.CustomerID in ({condition})");
                    break;
                case "201904150104402126"://项目
                    sb.Append(@$" AND t2.ProjectID in ({condition})");
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }
        /// <summary>
        /// 根据公式拼接获取属性sql
        /// </summary>
        /// <returns></returns>
        public virtual string CreateSelectSql()
        {
            string selectStr = "";
            switch (DataFormula)
            {
                case "202202111355001402"://借方发生额
                    selectStr = "IFNULL(SUM(t2.Debit),0) AS Amount";
                    break;
                case "202202111355001403"://贷方发生额
                    selectStr = "IFNULL(SUM(t2.Credit),0) AS Amount";
                    break;
                case "202202111355001404"://期末余额-凭证发生额，暂无期末
                    selectStr = "IFNULL(SUM(t2.Debit),0) AS Amount";
                    break;
                default:
                    break;
            }
            return selectStr;
        }

        public virtual List<FM_ExpenseCalEntity> GetResults()
        {
            PackageEnterprisePeriod();

            string sql = QuerySql();

            return _context.FM_ExpenseCalSet.FromSqlRaw(sql).ToList();
        }

        #region 取数&归集逻辑
        public virtual List<FM_PigGroupDataEntity> GetPigGroupData(string collectionType,string allocationType)
        {
            PackageEnterprisePeriod();
            string selectsql = @$"";
            string groupsql = $@"";
            #region 归集类型
            switch (collectionType)
            {
                case "202202111355001102"://部门,暂无
                    selectsql = "SELECT CONCAT(a.PigFarmID,'') AS PrimaryKey,CONCAT(a.PigFarmID,'') AS GroupID,CONCAT(a.PigFarmID,'') as PigFarmID";//,SUM(a.Days) AS Days,SUM(a.Days*IFNULL(b.CostCoefficient,1)) AS RateDays
                    groupsql = "GROUP BY a.PigFarmID";
                    break;
                case "202202111355001104"://猪场
                    selectsql = "SELECT CONCAT(a.PigFarmID,'') AS PrimaryKey,CONCAT(a.PigFarmID,'') AS GroupID,CONCAT(a.PigFarmID,'') as PigFarmID";
                    groupsql = "GROUP BY a.PigFarmID";
                    break;
                case "202202111355001105"://栋舍类型
                    selectsql = "SELECT CONCAT(a.PigFarmID,a.PigHouseUnitType) AS PrimaryKey,CONCAT(a.PigHouseUnitType,'') AS GroupID,CONCAT(a.PigFarmID,'') as PigFarmID";
                    groupsql = "GROUP BY a.PigHouseUnitType,a.PigFarmID";
                    break;
                case "202202111355001106"://栋舍
                    selectsql = "SELECT CONCAT(a.PigFarmID,a.PigHouseUnitID) AS PrimaryKey,CONCAT(a.PigHouseUnitID,'') AS GroupID,CONCAT(a.PigFarmID,'') as PigFarmID";
                    groupsql = "GROUP BY a.PigHouseUnitID,a.PigFarmID";
                    break;
                case "202202111355001107"://批次
                    selectsql = "SELECT CONCAT(a.PigFarmID,a.PigID) AS PrimaryKey,CONCAT(a.PigID,'') AS GroupID,CONCAT(a.PigFarmID,'') as PigFarmID";
                    groupsql = "GROUP BY a.PigID,a.PigFarmID";
                    break;
                default:
                    selectsql = "SELECT CONCAT(a.PigFarmID,'') AS PrimaryKey,CONCAT(a.PigFarmID,'') AS GroupID,CONCAT(a.PigFarmID,'') as PigFarmID";
                    groupsql = "GROUP BY a.PigFarmID";
                    break;
            }
            #endregion

            #region 分摊方式
            switch (allocationType)
            {
                case "202202111355001202"://饲养日龄
                    selectsql += ",IFNULL(SUM(IFNULL(a.Days,0)),0) AS RateDays";
                    break;
                case "202202111355001204"://饲养日龄*费用系数
                    selectsql += ",IFNULL(SUM(IFNULL(a.Days,0)*IFNULL(b.CostCoefficient,1)),0) AS RateDays";
                    break;
                default:
                    selectsql += ",IFNULL(SUM(IFNULL(a.Days,0)),0) AS RateDays";
                    break;
            }
            #endregion

            string fromsql = @$" FROM 
                        (
                        SELECT PigFarmID,PigID,PigHouseUnitID,BatchType,PigType,PigHouseUnitType,SUM(CASE WHEN IsIn=1 THEN PigCount ELSE -PigCount END) AS PigCount,-- 头数
                        SUM(PigCount* CASE WHEN IsIn=1 THEN 1+TIMESTAMPDIFF(DAY,DataDate,'{EndDate}') ELSE -TIMESTAMPDIFF(DAY,DataDate,'{EndDate}')-1 END) AS Days -- 日龄
                         FROM (
                        SELECT A.PigFarmID,B.PigID,B.PigHouseUnitID,D.PigHouseUnitType,IFNULL(C.BatchType,A.PigType) AS BatchType,'{BeginDate}' AS DataDate,A.PigType,1 AS IsIn,SUM(CASE WHEN A.IsIn=1 THEN PigCount ELSE -PigCount END) AS PigCount
                         FROM nxin_qlw_zlw.ms_transferhourse A 
                         INNER JOIN nxin_qlw_zlw.ms_transferhoursedetail B ON A.NumericalOrder=B.NumericalOrder AND A.IsIn=B.IsIn AND A.Abstract=B.Abstract
                         INNER JOIN nxin_qlw_zlw.biz_pighouseunit D ON B.PigHouseUnitID=D.PigHouseUnitID 
                         LEFT JOIN nxin_qlw_zlw.biz_batch C ON B.PigID=C.BatchID
                        WHERE A.EnterpriseID={EnterPriseID} AND A.DataDate<'{BeginDate}'
                        GROUP BY A.PigFarmID,B.PigID,C.BatchType,A.PigType,B.PigHouseUnitID
                        UNION ALL
                        SELECT A.PigFarmID,B.PigID,B.PigHouseUnitID,D.PigHouseUnitType,IFNULL(C.BatchType,A.PigType) AS BatchType,DataDate,A.PigType,A.IsIn,PigCount 
                        FROM nxin_qlw_zlw.ms_transferhourse A 
                        INNER JOIN nxin_qlw_zlw.ms_transferhoursedetail B ON A.NumericalOrder=B.NumericalOrder AND A.IsIn=B.IsIn AND A.Abstract=B.Abstract
                        INNER JOIN nxin_qlw_zlw.biz_pighouseunit D ON B.PigHouseUnitID=D.PigHouseUnitID 
                        LEFT JOIN nxin_qlw_zlw.biz_batch C ON B.PigID=C.BatchID
                        WHERE A.EnterpriseID={EnterPriseID} AND A.DataDate BETWEEN '{BeginDate}' AND '{EndDate}') M 
                        GROUP BY PigFarmID,PigID,BatchType,PigType,PigHouseUnitID,PigHouseUnitType
                        ) a
                        LEFT JOIN 
                        (
                        SELECT EndDate,CoefficientType,MaterialCoefficient,CostCoefficient 
                        FROM nxin_qlw_zlw.ms_costcoefficient A 
                        INNER JOIN nxin_qlw_zlw.ms_costcoefficientdetail B ON A.NumericalOrder=B.NumericalOrder
                        LEFT JOIN nxin_qlw_zlw.biz_reviwe C ON A.NumericalOrder=C.NumericalOrder AND C.CheckMark=16
                        WHERE EnterpriseID={EnterPriseID} AND C.`CheckedByID`>0
                        GROUP BY B.CoefficientType 
                        ORDER BY EndDate DESC
                        ) b ON a.BatchType=b.CoefficientType
		                            ";
            string sql = selectsql + fromsql + (collectionType== "202202111355001107"? " WHERE a.PigType=324 " : "") + groupsql;//如果归集类型是批次，只取肥猪类型的批次
            return _context.FM_PigGroupDataSet.FromSqlRaw<FM_PigGroupDataEntity>(sql).ToList();
        }
        #endregion

        #region 问题追溯
        public virtual string QueryDetailLogSql()
        {
            string sql = @$"SELECT
                        CONCAT(t2.`NumericalOrder`,t2.`RecordID`) AS PrimaryKey,
                        CONCAT(t1.`NumericalOrder`,'') AS NumericalOrder,
                        CONCAT(t1.`Number`,'') AS Number,
                        CONCAT(t1.`OwnerID`,'') AS OwnerID,
                        d.`Name` AS OwnerName,
                        c.`AccoSubjectFullName`,
                        '-' AS SubsidiaryOption,
                        IFNULL(t2.`Debit`,0)+IFNULL(t2.`Credit`,0) AS Amount
                        FROM NXin_Qlw_Business.FD_SettleReceipt  t1  
                        INNER JOIN  NXin_Qlw_Business.FD_SettleReceiptDetail t2 
                        ON t1.NumericalOrder=t2.NumericalOrder
                        INNER JOIN qlw_nxin_com.`biz_accosubject` c ON t2.AccoSubjectID=c.accoSubjectid
                        LEFT JOIN nxin_qlw_business.hr_person d ON t1.OwnerID=d.BO_ID
                        WHERE  t1.DataDate  BETWEEN  '{BeginDate}' AND '{EndDate}' AND t1.EnterpriseID={EnterPriseID} AND LOCATE('{AccoSubjectID}',c.`cAxis`)>0
                        AND t2.MarketID = 0";
            sql += CreateCondition();
            return sql;
        }

        public virtual List<FM_ExpenseReportDetailLogsEntity> DetailLogResults()
        {
            PackageEnterprisePeriod();

            string sql = QueryDetailLogSql();

            return _context.FM_ExpenseReportDetailLogsSet.FromSqlRaw(sql).ToList();
        }
        #endregion

        #region Private Method
        private void PackageEnterprisePeriod()
        {
            DateTime dt = DateTime.Parse(this.ReportPeriod.Replace('.', '-'));
            this.BeginDate = new DateTime(dt.Year, dt.Month, 1).ToString("yyyy-MM-dd");
            this.EndDate = new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            var result = _enterpriseperiodUtil.GetEnterperisePeriodList(EnterPriseID, dt.Year, dt.Month);
            if (result != null&&result.Count>0)
            {
                this.BeginDate = result.First().StartDate.ToString("yyyy-MM-dd");
                this.EndDate = result.First().EndDate.ToString("yyyy-MM-dd");
            }
        }
        #endregion
    }
}
