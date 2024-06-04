using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_VoucherAmortization;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class FD_VoucherAmortizationODataProvider : OneWithManyQueryProvider<FD_VoucherAmortizationODataEntity, FD_VoucherAmortizationDetailODataEntity>
    {
        ILogger<FD_VoucherAmortizationODataProvider> _logger;
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        BIZ_DataDictODataProvider _dataDictODataProvider;

        public FD_VoucherAmortizationODataProvider(BIZ_DataDictODataProvider dataDictODataProvider, IIdentityService identityservice, QlwCrossDbContext context, TreeModelODataProvider treeModel, ILogger<FD_VoucherAmortizationODataProvider> logger)
        {
            _identityservice = identityservice;
            _context = context;
            _logger = logger;
            _treeModel = treeModel;
            _dataDictODataProvider = dataDictODataProvider;
        }

        public IEnumerable<FD_VoucherAmortizationODataEntity> GetList(ODataQueryOptions<FD_VoucherAmortizationODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FD_VoucherAmortizationODataEntity> GetData(ODataQueryOptions<FD_VoucherAmortizationODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }
        public override IQueryable<FD_VoucherAmortizationODataEntity> GetDatas(NoneQuery query = null)
        {
            //当前单位最新结账日期+1个月
            FormattableString accSql = $@"SELECT  CONVERT(A.`NumericalOrder` USING utf8mb4) AS NumericalOrder,
                                        CONVERT(DATE_FORMAT(DATE_ADD(MAX(A.Datadate),INTERVAL 1 MONTH),'%Y-%m') USING utf8mb4)AS  Datadate,
                                        CONVERT(A.StartDate USING utf8mb4) AS StartDate,
                                        CONVERT(A.EndDate USING utf8mb4) AS EndDate,
                                        A.`Remarks`
                                        FROM `nxin_qlw_business`.fm_accocheck A LEFT JOIN `nxin_qlw_business`.fm_accocheckdetail B ON a.NumericalOrder = B.NumericalOrder
                                        LEFT JOIN `nxin_qlw_business`.fm_accocheckextend C ON B.NumericalOrderDetail = C.NumericalOrderDetail
                                        WHERE B.AccoCheckType = 201708010104402105
                                        AND C.MenuID = 6 
                                        AND C.CheckMark =1
                                        AND EnterpriseId = {_identityservice.EnterpriseId} GROUP BY NULL ";

            var accFirst = _context.FM_AccocheckDataSet.FromSqlInterpolated(accSql).FirstOrDefault();
            string sqlWhere = string.Empty;
            if (accFirst != null)//如果为空代表始终没有结账
            {
                sqlWhere = @$" and  DATE_FORMAT( per.`AccountDate`,'%Y-%m')='{accFirst.DataDate}' ";
            }
            else
            {
                sqlWhere = @$" and RowNum=1  ";
            }
            string sql = $@" SELECT                  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                                    CONVERT(t1.Number USING utf8mb4) Number,t1.AmortizationName,
                                                    CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                                    tick.TicketedPointName,
                                                    CONVERT(t1.AbstractID USING utf8mb4) AbstractID,
                                                    se.SettleSummaryName AS AbstractName,	
                                                    CONVERT(t1.Remarks USING utf8mb4) Remarks,	
                                                    CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                                    HP1.Name AS OwnerName,	
                                                    CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                                    ent.EnterpriseName,
                                                    CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                                    CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate,
                                                    CONVERT(DATE_FORMAT(t3.AccountDate,'%Y-%m')  USING utf8mb4)  AS AccountDate,
                                                    COUNT(t2.IsAmort) AS QuantityTotal,
                                                    COUNT(CASE WHEN t2.IsAmort=TRUE THEN 1 ELSE NULL END) AS QuantityAlready,
                                                    COUNT(CASE WHEN t2.IsAmort=FALSE THEN 1 ELSE NULL END) AS QuantityFuture,
                                                    SUM(t2.AmortizationAmount) AS AmountTotal,
                                                    SUM(CASE WHEN t2.IsAmort=TRUE THEN t2.AmortizationAmount ELSE 0 END) AS AmountAlready,
                                                    SUM(CASE WHEN t2.IsAmort=FALSE THEN t2.AmortizationAmount ELSE 0 END) AS AmountFuture,
                                                    ifnull(temp.AmortizationAmount,0.00) as CurrentAmount,
                                                    CASE WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN t2.AmortizationAmount ELSE 0 END))>0  THEN 1 
                                                    WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN t2.AmortizationAmount ELSE 0 END))=0  THEN 2 ELSE 0 END
                                                    AS ImpStateID,
                                                    CASE WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN t2.AmortizationAmount ELSE 0 END))>0  THEN '进行中'
                                                    WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN t2.AmortizationAmount ELSE 0 END))=0  THEN '已关闭' ELSE 0 END
                                                    AS ImpState,
                                                    t1.IsUse,
                                                    CASE WHEN t1.IsUse=false THEN '是' WHEN t1.IsUse=true THEN '否' ELSE '未知' END AS UseState,
                                                    CONVERT(t1.OperatorID USING utf8mb4) OperatorID,
                                                    HP2.Name AS OperatorName
                                                    FROM
                                                    `nxin_qlw_business`.`FD_VoucherAmortization` t1
                                                    LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
                                                    LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` se ON t1.AbstractID=se.SettleSummaryID AND t1.`EnterpriseID`=se.EnterpriseID
                                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
                                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                                    LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                                    LEFT JOIN  `nxin_qlw_business`.FD_VoucherAmortizationPeriodDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                                    LEFT JOIN  (select * from `nxin_qlw_business`.FD_VoucherAmortizationPeriodDetail where rownum=1) t3 ON t1.`NumericalOrder`=t3.`NumericalOrder`
                                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP2 ON HP2.BO_ID=t1.OperatorID
                                                    LEFT JOIN 
						                                (
							                            SELECT NumericalOrder,AmortizationAmount,RowNum,DATE_FORMAT( per.`AccountDate`,'%Y-%m') as AccountDate  FROM `nxin_qlw_business`.FD_VoucherAmortizationPeriodDetail per WHERE  1=1 {sqlWhere} group by NumericalOrder) as temp ON t1.`NumericalOrder`=temp.`NumericalOrder`
						                            WHERE t1.EnterpriseID={_identityservice.EnterpriseId} 
                                                    GROUP BY t1.`NumericalOrder` ";
            var list = _context.FD_VoucherAmortizationDataSet.FromSqlRaw(sql);
            return list;
        }
        /// <summary>
        /// 生成凭证结果列表`
        /// </summary>
        /// <returns></returns>
        public IQueryable<FD_VoucherAmortizationRecordODataEntity> GetRecordList(ODataQueryOptions<FD_VoucherAmortizationRecordODataEntity> odataqueryoptions, Uri uri)
        {
            FormattableString sql = $@" SELECT t1.`RecordID`,
                                        CONVERT(t1.NumericalOrderVoucher USING utf8mb4) NumericalOrderVoucher,	
                                        CONVERT(t1.NumericalOrderSettl USING utf8mb4) NumericalOrderSettl,	
                                        t1.`AmortizationName`,
                                        CONVERT(t1.OwnerID USING utf8mb4) OwnerID,	
                                        HP2.`Name` AS OwnerName,
                                        t1.`ImplementResult`,
                                        t1.`ResultState`,
                                        (CASE WHEN ResultState=TRUE THEN '成功' 
                                              WHEN ResultState=FALSE THEN '失败' ELSE '未知' END) AS ResultStateName,
                                        CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                        CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate,
                                        CONVERT(t2.`Number`  USING utf8mb4) AS Number
                                                                                     FROM 
                                        `nxin_qlw_business`.FM_VoucherAmortizationRecord t1
                                        LEFT JOIN `nxin_qlw_business`.fd_settlereceipt t2 ON t1.`NumericalOrderSettl`=t2.`NumericalOrder`
                                        LEFT JOIN `nxin_qlw_business`.`hr_person` HP2 ON t1.OwnerID=HP2.Bo_ID
                                        WHERE  t1.EnterpriseID = {_identityservice.EnterpriseId} 
                                        ORDER BY t1.CreatedDate DESC ";
            return _context.FD_VoucherAmortizationRecordDataSet.FromSqlInterpolated(sql);
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        public FD_VoucherAmortizationODataEntity GetSingleData(long manyQuery)
        {
            FormattableString sql = $@"SELECT                  CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder,	
                                                    CONVERT(t1.Number USING utf8mb4) Number,t1.AmortizationName,
                                                    CONVERT(t1.TicketedPointID USING utf8mb4) TicketedPointID,	
                                                    tick.TicketedPointName,
                                                    CONVERT(t1.AbstractID USING utf8mb4) AbstractID,
                                                    se.SettleSummaryName AS AbstractName,	
                                                    CONVERT(t1.Remarks USING utf8mb4) Remarks,	
                                                    CONVERT(t1.OwnerID USING utf8mb4) OwnerID,
                                                    HP1.Name AS OwnerName,	
                                                    CONVERT(t1.EnterpriseID USING utf8mb4) EnterpriseID,
                                                    ent.EnterpriseName,
                                                    CONVERT(t1.CreatedDate USING utf8mb4) CreatedDate,
                                                    CONVERT(t1.ModifiedDate USING utf8mb4) ModifiedDate,
                                                    CONVERT(CASE WHEN t2.RowNum=1 THEN DATE_FORMAT(t2.AccountDate,'%Y-%m') END USING utf8mb4)  AS AccountDate,
                                                    COUNT(t2.IsAmort) AS QuantityTotal,
                                                    COUNT(CASE WHEN t2.IsAmort=TRUE THEN 1 ELSE NULL END) AS QuantityAlready,
                                                    COUNT(CASE WHEN t2.IsAmort=FALSE THEN 1 ELSE NULL END) AS QuantityFuture,
                                                    SUM(t2.AmortizationAmount) AS AmountTotal,
                                                    SUM(CASE WHEN t2.IsAmort=TRUE THEN t2.AmortizationAmount ELSE 0 END) AS AmountAlready,
                                                    SUM(CASE WHEN t2.IsAmort=FALSE THEN t2.AmortizationAmount ELSE 0 END) AS AmountFuture,
                                                    0.00 as CurrentAmount,
                                                    CASE WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN AmortizationAmount ELSE 0 END))>0  THEN 1 
                                                    WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN AmortizationAmount ELSE 0 END))=0  THEN 2 ELSE 0 END
                                                    AS ImpStateID,
                                                    CASE WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN AmortizationAmount ELSE 0 END))>0  THEN '进行中'
                                                    WHEN 
                                                    SUM((CASE WHEN t2.IsAmort=FALSE THEN AmortizationAmount ELSE 0 END))=0  THEN '已关闭' ELSE 0 END
                                                    AS ImpState,
                                                    t1.IsUse,
                                                    CASE WHEN t1.IsUse=false THEN '是' WHEN t1.IsUse=true THEN '否' ELSE '未知' END AS UseState,
                                                    CONVERT(t1.OperatorID USING utf8mb4) OperatorID,
                                                    HP2.Name AS OperatorName
                                                    FROM
                                                    `nxin_qlw_business`.`FD_VoucherAmortization` t1
                                                    LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
                                                    LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` se ON t1.AbstractID=se.SettleSummaryID AND t1.`EnterpriseID`=se.EnterpriseID
                                                    LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
                                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
                                                    LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
                                                    LEFT JOIN  `nxin_qlw_business`.FD_VoucherAmortizationPeriodDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
                                                    LEFT JOIN `nxin_qlw_business`.HR_Person HP2 ON HP2.BO_ID=t1.OperatorID
                                                    WHERE t1.NumericalOrder ={manyQuery}
                                                    GROUP BY t1.`NumericalOrder`";
            var main = _context.FD_VoucherAmortizationDataSet.FromSqlInterpolated(sql).FirstOrDefault();
            _logger.LogError("主表" + JsonConvert.SerializeObject(main));
            if (main!=null)
            {
                main.Lines = GetDetailDatas(manyQuery);
                main.PeriodLines = GetPeriodDetailDatas(manyQuery);
            }
            return main;
        }

        public List<FD_VoucherAmortizationDetailODataEntity> GetDetailDatas(long manyQuery)
        {
            FormattableString sql = $@"SELECT B.RecordID,CONVERT(B.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(B.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        
                                        CONVERT(B.AccoSubjectID USING utf8mb4) AccoSubjectID,
                                        acc.AccoSubjectFullName AS AccoSubjectName,
                                        CONVERT(B.AccoSubjectCode USING utf8mb4) AccoSubjectCode,
                                        acc.IsPerson AS IsPerson,
                                        acc.IsCus AS IsCustomer,
                                        acc.IsDept AS IsMarket,
                                        acc.IsSup AS IsSupplier,
                                        CONVERT(B.PersonID USING utf8mb4) PersonID,
                                        hr.Name AS PersonName,
                                        CONVERT(B.CustomerID USING utf8mb4) CustomerID,
                                        cut.CustomerName,
                                        CONVERT(B.MarketID USING utf8mb4) MarketID,
                                        mar.MarketName,
                                        CONVERT(B.SupplierID USING utf8mb4) SupplierID,
                                        sup.SupplierName,
                                        B.ValueNumber,B.IsDebit,
                                        CONVERT(B.ModifiedDate USING utf8mb4) ModifiedDate
                                         FROM `nxin_qlw_business`.FD_VoucherAmortizationDetail  B
                                        INNER JOIN `nxin_qlw_business`.FD_VoucherAmortization  A ON A.NumericalOrder=B.NumericalOrder
                                        LEFT JOIN `qlw_nxin_com`.`biz_accosubject` acc ON B.AccoSubjectID=acc.AccoSubjectID 
                                        LEFT JOIN  `qlw_nxin_com`.`hr_person` hr ON B.PersonID=hr.PersonID
                                        LEFT JOIN  `qlw_nxin_com`.`biz_customer` cut ON B.CustomerID=cut.CustomerID 
                                        LEFT JOIN  `qlw_nxin_com`.`biz_market` mar  ON B.MarketID=mar.MarketID
                                        LEFT JOIN  `NXin_Qlw_Business`.`pm_supplier` sup  ON B.SupplierID=sup.SupplierID AND sup.EnterpriseID=A.EnterpriseID
                                        /* LEFT JOIN  `qlw_nxin_com`.`biz_supplier` sup  ON B.SupplierID=sup.SupplierID*/
                                    WHERE B.NumericalOrder ={manyQuery} ";
            var result = _context.FD_VoucherAmortizationDetailDataSet.FromSqlInterpolated(sql).ToList();
            return result;
        }
        public List<FD_VoucherAmortizationPeriodDetailODataEntity> GetPeriodDetailDatas(long manyQuery)
        {
            FormattableString sql = $@"SELECT B.RecordID,CONVERT(B.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(B.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        B.RowNum,
                                        CONVERT(DATE_FORMAT(B.AccountDate,'%Y-%m') USING utf8mb4)  AS AccountDate,
                                        B.AmortizationAmount,
                                        B.IsAmort,
                                        B.IsLast,
                                        CONVERT(B.ModifiedDate USING utf8mb4) ModifiedDate
                                         FROM `nxin_qlw_business`.FD_VoucherAmortizationPeriodDetail  B
                                    WHERE B.NumericalOrder ={manyQuery}
                                    ORDER BY B.RowNum ";
            var result = _context.FD_VoucherAmortizationPeriodDetailDataSet.FromSqlInterpolated(sql).ToList();
            return result;
        }
        public override Task<FD_VoucherAmortizationODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            throw new NotImplementedException();
        }

        public override Task<List<FD_VoucherAmortizationDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            throw new NotImplementedException();
        }
        public List<FM_VoucherAmortizationRelatedODataEntity> GetRelatedDatas(long manyQuery)
        {
            FormattableString sql = $@"SELECT RecordID,CONVERT(NumericalOrderVoucher USING utf8mb4) NumericalOrderVoucher,
                                CONVERT(NumericalOrderSettl USING utf8mb4) NumericalOrderSettl,
                                CONVERT(NumericalOrderStay USING utf8mb4) NumericalOrderStay,
                                CONVERT(NumericalOrderInto USING utf8mb4) NumericalOrderInto,SUM(VoucherAmount) AS VoucherAmount,
                                CONVERT(CreatedDate USING utf8mb4) CreatedDate,
                                CONVERT(ModifiedDate USING utf8mb4) ModifiedDate
                                FROM `nxin_qlw_business`.FM_VoucherAmortizationRelated  where NumericalOrderVoucher={manyQuery}
                                GROUP BY NumericalOrderStay ";
            var result = _context.FM_VoucherAmortizationRelatedDataSet.FromSqlInterpolated(sql).ToList();
            return result;
        }
        /// <summary>
        /// 凭证摊销
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public dynamic GetViycgerAnirtuzationList(VoucherSearch search)
        {
            string sql = $@"    SELECT           
             t1.EnterpriseID,
             t1.AmortizationName, -- 方案名称
             CONCAT(tick.TicketedPointName,'-',CONVERT(t1.Number USING utf8mb4)) Number , -- 单据字号
             CONVERT(t1.AbstractID USING utf8mb4) AbstractID,
             se.SettleSummaryName AS AbstractName,	-- 摘要 
             MAX(t22.RowNum) AS QuantityTotal, -- 总摊销期间
             CONVERT(CASE WHEN t2.RowNum=1 THEN DATE_FORMAT(t2.AccountDate,'%Y-%m') END USING utf8mb4)  AS BeginAccountDate, -- 开始摊销期间
             CONVERT(DATE_FORMAT(t2.AccountDate,'%Y-%m')  USING utf8mb4)  AS AccountDate, -- 会计期间
             COUNT(CASE WHEN t2.IsAmort=TRUE THEN 1 ELSE NULL END) AS QuantityAlready, -- 已摊销期间
             COUNT(CASE WHEN t2.IsAmort=FALSE THEN 1 ELSE NULL END) AS QuantityFuture, -- 待摊销期间 
             SUM(t3.ValueNumber) AS AmountTotal, -- 总摊销金额
             SUM(CASE WHEN t2.IsAmort=TRUE THEN t2.AmortizationAmount ELSE 0 END) AS AmountAlready, -- 已摊销金额
             SUM(CASE WHEN t2.IsAmort=FALSE THEN t2.AmortizationAmount ELSE 0 END) AS AmountFuture, -- 待摊销金额 
             ifnull(t2.AmortizationAmount,0.00) as CurrentAmount, -- 摊销金额
             t2.modifiedDate  AS ExecuteDate, -- 执行时间
             CONVERT(t1.NumericalOrder USING utf8mb4) NumericalOrder
            FROM
            `nxin_qlw_business`.`FD_VoucherAmortization` t1
            LEFT JOIN nxin_qlw_business.FD_VoucherAmortizationDetail t3 on t3.NumericalOrder = t1.NumericalOrder and t3.IsDebit = 1
            LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON t1.TicketedPointID=tick.TicketedPointID AND t1.EnterpriseID=tick.EnterpriseID
            LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` se ON t1.AbstractID=se.SettleSummaryID AND t1.`EnterpriseID`=se.EnterpriseID
            LEFT JOIN `nxin_qlw_business`.BIZ_Reviwe R1 ON R1.NumericalOrder=t1.NumericalOrder AND R1.CheckMark=65536
            LEFT JOIN `nxin_qlw_business`.HR_Person HP1 ON HP1.BO_ID=R1.CheckedByID
            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` ent ON t1.EnterpriseID=ent.EnterpriseID
            LEFT JOIN  `nxin_qlw_business`.FD_VoucherAmortizationPeriodDetail t2 ON t1.`NumericalOrder`=t2.`NumericalOrder`
            LEFT JOIN `nxin_qlw_business`.HR_Person HP2 ON HP2.BO_ID=t1.OperatorID
            LEFT JOIN  `nxin_qlw_business`.FD_VoucherAmortizationPeriodDetail t22 ON t1.`NumericalOrder`=t22.`NumericalOrder` AND t22.IsLast = 1 
            WHERE t1.EnterpriseID IN ({search.EnterpriseIds}) 
            {(!string.IsNullOrEmpty(search.AmortizationName) ? $" AND t1.AmortizationName LIKE '%{search.AmortizationName}%' " : "")}
            {(!string.IsNullOrEmpty(search.AccountDate) ? $" AND t2.AccountDate <= '{search.AccountDate}-01' " : "")} and t2.isAmort = 1   
            GROUP BY t2.`recordid` 
            order by t1.NumericalOrder,t2.AccountDate asc 
            ";
            var list = _context.DynamicSqlQuery(sql);
            var enSort = _dataDictODataProvider.EnterSortInfo(); //单位组织
            string oldNum = "";
            double countData = 0;   
            double amount = 0;
            //数据过滤 
            foreach (var item in list)
            {
                _dataDictODataProvider.SetSortNameOrganizationName(enSort, new List<SortInfo>(), item);
                if (item.NumericalOrder != oldNum)
                {
                    oldNum = item.NumericalOrder.ToString();
                    countData = 0;
                    amount = item.AmountTotal;
                }
                //累减
                {
                    countData++;
                    item.QuantityAlready = countData;
                    item.QuantityFuture = Convert.ToString(Convert.ToDouble(item.QuantityTotal) - countData);
                    amount -= item.CurrentAmount;
                    item.AmountFuture = amount;
                }
            }
            list = _dataDictODataProvider.OrgEnterFilter(search, list);
            return list.Distinct();
        }
    }
}
