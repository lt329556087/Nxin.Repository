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
using System.Text;
using FinanceManagement.Common;
namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FD_ExpenseODataProvider : OneWithManyQueryProvider<FD_ExpenseODataEntity, FD_ExpenseDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        FMBaseCommon _baseUnit;
        public FD_ExpenseODataProvider(IIdentityService identityservice, QlwCrossDbContext context, FMBaseCommon baseUnit)
        {
            _baseUnit = baseUnit;
            _identityservice = identityservice;
            _context = context;
        }

        public IQueryable<FD_ExpenseODataEntity> GetList(ODataQueryOptions<FD_ExpenseODataEntity> odataqueryoptions, Uri uri, string beginDate, string endDate)
        {
            var datas = GetData(odataqueryoptions, beginDate,endDate);
            return datas;
        }

        public IQueryable<FD_ExpenseODataEntity> GetData(ODataQueryOptions<FD_ExpenseODataEntity> odataqueryoptions, string beginDate, string endDate)
        {
            return GetDataList(beginDate,endDate);
        }
        public override IQueryable<FD_ExpenseODataEntity> GetDatas(NoneQuery query = null) { return null; }
        public IQueryable<FD_ExpenseODataEntity> GetDataList(string beginDate,string endDate)
        {
            var strWhere = string.Format(" AND A.DataDate BETWEEN '{0}' AND '{1}' AND ent.PID={2} ", beginDate,endDate,_identityservice.GroupId);
            var strSql=GetBaseSql(strWhere);
           
            return _context.FD_ExpenseDataSet.FromSqlRaw(strSql.ToString()); 
           
        }
       
        private string GetBaseSql(string strWhere)
        {
            return string.Format(@"SELECT 
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.Number USING utf8mb4) Number,
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,
                                        DATE_FORMAT(A.DataDate, '%Y-%m-%d') DataDate,
                                        CONVERT(A.Guid USING utf8mb4) Guid,
                                        CONVERT(A.ExpenseType USING utf8mb4) ExpenseType,
                                        CONVERT(A.ExpenseAbstract USING utf8mb4) ExpenseAbstract,
                                        CONVERT(A.ExpenseSort USING utf8mb4) ExpenseSort,
                                        CONVERT(A.PersonID USING utf8mb4) PersonID,
                                        CONVERT(A.HouldPayDate USING utf8mb4) HouldPayDate,
                                        CONVERT(A.PayDate USING utf8mb4) PayDate,
                                        CONVERT(A.StartDate USING utf8mb4) StartDate,
                                        CONVERT(A.EndDate USING utf8mb4) EndDate,
                                        CONVERT(A.DraweeID USING utf8mb4) DraweeID,
                                        CONVERT(A.Remarks USING utf8mb4) Remarks,
                                        CONVERT(A.OwnerID USING utf8mb4) OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) ModifiedDate,
                                        A.CurrentVerificationAmount,
                                        A.Pressing,
                                        pro.Progress,
                                        (case when A.DraweeID>0 then '已付款' else '未付款' end) PayContent,
                                        CONVERT(A.TicketedPointID USING utf8mb4) TicketedPointID
                                        , ent.EnterpriseName, HP.Name PersonName, HP1.Name OwnerName, SM.cText ExpenseTypeName, d.cDictName ExpenseAbstractName, d1.cDictName ExpenseSortName, HP2.Name DraweeName,t.TicketedPointName
                                        , CASE WHEN (FIND_IN_SET(1, audit.resultsRESu) AND (LENGTH(audit.resultsRESu) - LENGTH(REPLACE( audit.resultsRESu,',','' ))) = 0)   THEN '通过'
                                             WHEN (FIND_IN_SET(0, audit.resultsRESu) AND (LENGTH(audit.resultsRESu) - LENGTH(REPLACE( audit.resultsRESu,',','' ))) = 0)   THEN '未审批'
					                         WHEN (FIND_IN_SET(1, audit.resultsRESu) AND FIND_IN_SET(0, audit.resultsRESu) AND !FIND_IN_SET(2, audit.resultsRESu) AND !FIND_IN_SET(3, audit.resultsRESu)) THEN '审批中'
					                         WHEN (FIND_IN_SET(2, audit.resultsRESu) AND !FIND_IN_SET(3, audit.resultsRESu)) THEN '驳回'
					                         WHEN (FIND_IN_SET(3, audit.resultsRESu)) THEN '拒绝' END AuditResultName,'' AuditResult
					                     ,CASE B.BusinessType WHEN 201611160104402103 THEN HPD.Name WHEN 201611160104402102 THEN bm.MarketName ELSE bc.CustomerName END AS CustomerName 
                                         ,IFNULL(B.Amount,0) Amount,IFNULL(pay.PayAmount,0) PayAmount,IFNULL(hx.CurrentVerificationAmount,0) HadVerificationAmount,
                                        CASE WHEN A.ExpenseType=1803081320210000101 AND hx.CurrentVerificationAmount >0 THEN '已核销' WHEN A.ExpenseType=1803081320210000101 AND IFNULL(hx.CurrentVerificationAmount,0) <=0 THEN '未核销' ELSE '' END AS VerificationStateName
                                        FROM qlw_nxin_com.fd_expense A
                                        LEFT JOIN  nxin_qlw_business.HR_Person HP ON HP.PersonID = A.PersonID
                                        LEFT JOIN  nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID = A.OwnerID
                                        LEFT JOIN  nxin_qlw_business.HR_Person HP2 ON HP2.PersonID = A.DraweeID
                                        LEFT JOIN  qlw_nxin_com.biz_enterprise  ent ON A.EnterpriseID = ent.EnterpriseID
                                        LEFT JOIN qlw_nxin_com.STMenu SM ON SM.MenuID = A.ExpenseType
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d ON d.DictID = A.ExpenseAbstract
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d1 ON d1.DictID = A.ExpenseSort
                                        LEFT JOIN  nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID = A.TicketedPointID
                                        LEFT JOIN qlw_nxin_com.fd_expensedetail B ON A.NumericalOrder=B.NumericalOrder 
                                        LEFT JOIN  nxin_qlw_business.HR_Person HPD ON HPD.PersonID=B.PersonID 
                                        LEFT JOIN  qlw_nxin_com.biz_customer bc ON bc.CustomerID=B.CustomerID 
                                        LEFT JOIN  qlw_nxin_com.biz_market bm ON bm.MarketID=B.MarketID                                       
                                        LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT faac.results) resultsRESu 
					                        FROM qlw_nxin_com.fd_expense a 
					                        INNER JOIN qlw_nxin_com.faauditrecord faac  ON a.NumericalOrder = faac.NumericalOrder
					                        INNER JOIN  qlw_nxin_com.biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
					                        WHERE A.ExpenseType IN (1803081320210000101,1612091318520000101) {0}
					                        GROUP BY a.NumericalOrder
					                        ) audit ON a.NumericalOrder = audit.NumericalOrder -- 审批结果
                                                           LEFT JOIN (SELECT A.numericalorder ,SUM(fd.Amount) PayAmount
					                        FROM qlw_nxin_com.fd_expense A
                                             LEFT JOIN nxin_qlw_business.biz_related r ON r.RelatedType=201610210104402122 AND r.ParentType =1612011058280000101 AND r.ChildType=A.ExpenseType AND r.ChildValue=A.NumericalOrder
                                             LEFT JOIN nxin_qlw_business.fd_paymentreceivablesdetail fd ON fd.NumericalOrder=r.ParentValue
                                             INNER JOIN  qlw_nxin_com.biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
                                              WHERE A.ExpenseType IN (1803081320210000101,1612091318520000101) {0}
					                     GROUP BY a.NumericalOrder
					                     ) pay ON a.NumericalOrder = pay.NumericalOrder -- 实付金额 付款单
					                  LEFT JOIN(SELECT a.NumericalOrder,IFNULL(SUM( CASE WHEN faa.totalLevel = faa.AccepterNum THEN f2.CurrentVerificationAmount ELSE 0 END ) ,0)  CurrentVerificationAmount  
						                FROM qlw_nxin_com.FD_Expense a
						                LEFT JOIN nxin_qlw_business.biz_related b1 ON a.NumericalOrder = b1.ChildValue
						                LEFT JOIN qlw_nxin_com.FD_Expense f2 ON b1.ParentValue = f2.NumericalOrder
						                INNER JOIN  qlw_nxin_com.biz_enterprise  ent ON a.EnterpriseID=ent.EnterpriseID
						                LEFT JOIN (
						                SELECT fa.NumericalOrder,COUNT( fa.NumericalOrder ) AS totalLevel,SUM( IF ( fa.results = 1, 1, 0 ) ) AS AccepterNum 
						                FROM qlw_nxin_com.FD_Expense a
						                LEFT JOIN nxin_qlw_business.biz_related b1 ON a.NumericalOrder = b1.ChildValue
						                LEFT JOIN qlw_nxin_com.FD_Expense f2 ON b1.ParentValue = f2.NumericalOrder
						                INNER JOIN qlw_nxin_com.FAAuditRecord fa ON fa.NumericalOrder = f2.NumericalOrder 
						                INNER JOIN  qlw_nxin_com.biz_enterprise  ent ON a.EnterpriseID=ent.EnterpriseID
						                WHERE fa.PersonID > 0 AND b1.RelatedType = 201610210104402122  AND b1.ParentType = 1803081321400000101  AND b1.ChildType = 1612011058280000101  AND a.ExpenseType = 1803081320210000101 AND f2.ExpenseType = 1803081321400000101 {0}
						                AND ent.PID={1}
						                GROUP BY fa.NumericalOrder 
						                ) faa ON faa.NumericalOrder = f2.NumericalOrder 
						                WHERE b1.RelatedType = 201610210104402122   AND b1.ParentType = 1803081321400000101 AND b1.ChildType = 1612011058280000101 AND a.ExpenseType = 1803081320210000101 AND f2.ExpenseType = 1803081321400000101 {0}
						                GROUP BY a.NumericalOrder ) hx ON hx.NumericalOrder = a.Numericalorder 
                                        LEFT JOIN (
                                            select t1.RecordID,t1.NumericalOrder,t1.MenuID,t1.Level,t1.Auditlevel,t1.Title as AuditTitle,ifnull(t1.Views,'') as Views,t1.Adjust,
                                            t1.Results as ResultsID,t1.PersonID as AuditPersonID,t1.SignID as SignPersonID,t1.EnterID,t1.OwnerID,t1.CreationDate,
                                            ifnull(t2.cDictName,'未审批') as ResultsName
                                            ,ifnull(t4.Name,'') as SignPersonName
                                            , GROUP_CONCAT(CONCAT(ifnull(t3.Name,''),'【',ifnull(t2.cDictName,'未审批'),'】')) as Progress
                                            from qlw_nxin_com.FAAuditRecord as t1
                                            left join  (select cDictCode,cDictName from qlw_nxin_com.BSDataDict where pid=201612220104402001) t2 on t1.Results=t2.cDictCode
                                            left join qlw_nxin_com.BIZ_Related b1 on b1.ChildValue=t1.PersonID and b1.ParentType=201610200104402122  and   b1.ChildType=201610200104402102
                                            left join qlw_nxin_com.HR_Person as t3 on t3.PersonID=b1.ParentValue   
                                            left join qlw_nxin_com.BIZ_Related b2 on b2.ChildValue=t1.SignID and b2.ParentType=201610200104402122  and   b2.ChildType=201610200104402102
                                            left join qlw_nxin_com.HR_Person as t4 on t4.PersonID=b2.ParentValue  
                                            inner join qlw_nxin_com.biz_enterprise ent on ent.enterpriseid = t1.EnterID 
                                            INNER JOIN qlw_nxin_com.fd_expense A on A.Numericalorder = t1.Numericalorder 
                                            and A.ExpenseType IN (1803081320210000101,1612091318520000101) {0} 
                                            GROUP BY NumericalOrder
                                        ) pro on pro.NumericalOrder = A.NumericalOrder
                                        WHERE A.ExpenseType IN (1803081320210000101,1612091318520000101) {0}                           
                                        ", strWhere,_identityservice.GroupId);

        }
        #region 查询SQL
        
    //    private string SelectExpensePayer()
    //    {
    //        return string.Format(@"SELECT 
				//CONVERT(F.NumericalOrder USING utf8mb4) NumericalOrder,
    //                                '' ExpenseType,
    //                                '' ExpenseTypeName,
    //                                '' EnterpriseID,
    //                                '' OwnerID,
    //                                '' Content,
    //                                0.0 Amount,
    //                                '' DataDate
    //                                ,'' OwnerName,
    //                                '' EnterpriseName,
    //                                '' HouldPayDate,
    //                                '' ReceiptAbstractCode,
    //                                '' BusinessType,
    //                                '' CustomerNames,
    //                                '' PersonNames,
    //                                '' MarketNames,
    //                                '' EnterpriseNames,
    //                                -- '' PayerID,
    //                                '' PayerName,
    //                                '' BusinessTypeName,
    //                                0.0 PayAmount
    //                                 FROM  qlw_nxin_com.FD_Expense F
    //                            INNER JOIN qlw_nxin_com.FD_ExpenseDetail FD ON  F.NumericalOrder = FD.NumericalOrder
    //                             LEFT JOIN nxin_qlw_business.HR_Person HP2 ON HP2.PersonID =FD.PersonID    
				//LEFT JOIN qlw_nxin_com.biz_customer ps ON ps.CustomerID =FD.CustomerID                                 
				//LEFT JOIN qlw_nxin_com.biz_market bm ON bm.MarketID =FD.MarketID AND bm.EnterpriseID={0} 
				//LEFT JOIN qlw_nxin_com.biz_enterprise be1 ON be1.EnterpriseID = FD.CustomerID  
				//WHERE (DraweeID IS NULL OR DraweeID=0)  AND F.EnterpriseID={0}  
				//GROUP BY F.NumericalOrder ,FD.NumericalOrder ", _identityservice.EnterpriseId);
    //    }
        
        #endregion
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_ExpenseODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            var strWhere = string.Format(" AND A.Numericalorder={0} ", manyQuery);
            var strSql = GetBaseSql(strWhere);

            return _context.FD_ExpenseDataSet.FromSqlRaw(strSql).FirstOrDefaultAsync();
        }

        public override Task<List<FD_ExpenseDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {           
            var sql = GetDetailSql(manyQuery);
            return _context.FD_ExpenseDetailDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        private FormattableString GetDetailSql(long manyQuery)
        {
            FormattableString sql = $@"SELECT CONVERT(A.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,
                                        CONVERT(A.Guid USING utf8mb4) Guid,
                                        CONVERT(A.ReceiptAbstractID USING utf8mb4) ReceiptAbstractID,
                                        CONVERT(A.BusinessType USING utf8mb4) BusinessType,
                                        CONVERT(A.SettleBusinessType USING utf8mb4) SettleBusinessType,
                                        CONVERT(A.CustomerID USING utf8mb4) CustomerID,
                                        CONVERT(A.PersonID USING utf8mb4) PersonID,
                                        CONVERT(A.MarketID USING utf8mb4) MarketID,
                                        CONVERT(A.ProjectID USING utf8mb4) ProjectID,
                                        CONVERT(A.SettlePayerID USING utf8mb4) SettlePayerID,
                                        A.Amount,
                                        CONVERT(A.Content USING utf8mb4) Content,
                                        CONVERT(A.AccountInformation USING utf8mb4) AccountInformation,
                                        CONVERT(A.ReceiptAbstractDetail USING utf8mb4) ReceiptAbstractDetail
                                        ,CASE A.BusinessType WHEN 201611160104402103 THEN HPD.Name WHEN 201611160104402102 THEN bm.MarketName WHEN 201611160104402105 THEN e.EnterpriseName ELSE bc.CustomerName END AS CustomerName 
                                        ,CASE A.SettleBusinessType WHEN 201611160104402103 THEN HPD1.Name WHEN 201611160104402102 THEN bm1.MarketName WHEN 201611160104402105 THEN e1.EnterpriseName ELSE bc1.CustomerName END AS SettlePayerName 
                                        ,bs.SettleSummaryName ReceiptAbstractName
                                        ,bs1.SettleSummaryName ReceiptAbstractDetailName
                                        ,d.cDictName BusinessTypeName
                                        ,d1.cDictName SettleBusinessTypeName,p.ProjectName
                                        FROM qlw_nxin_com.fd_expensedetail A
                                        LEFT JOIN  nxin_qlw_business.HR_Person HPD ON HPD.PersonID=A.PersonID 
                                        LEFT JOIN  qlw_nxin_com.biz_customer bc ON bc.CustomerID=A.CustomerID 
                                        LEFT JOIN  qlw_nxin_com.biz_market bm ON bm.MarketID=A.MarketID  
                                       LEFT JOIN qlw_nxin_com.biz_enterprise e ON e.EnterpriseID=A.CustomerID
                                        LEFT JOIN  qlw_nxin_com.biz_settlesummary bs ON bs.SettleSummaryID=A.ReceiptAbstractID  
                                        LEFT JOIN  qlw_nxin_com.biz_settlesummary bs1 ON bs1.SettleSummaryID=A.ReceiptAbstractDetail 
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d ON d.DictID=A.BusinessType
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d1 ON d1.DictID=A.SettleBusinessType
                                        LEFT JOIN  nxin_qlw_business.HR_Person HPD1 ON HPD1.PersonID=A.SettlePayerID 
                                        LEFT JOIN  qlw_nxin_com.biz_customer bc1 ON bc1.CustomerID=A.SettlePayerID 
                                        LEFT JOIN  qlw_nxin_com.biz_market bm1 ON bm1.MarketID=A.SettlePayerID 
                                        LEFT JOIN qlw_nxin_com.biz_enterprise e1 ON e1.EnterpriseID=A.SettlePayerID
                                        LEFT JOIN  qlw_nxin_com.ppm_project p ON p.ProjectID=A.ProjectID
                                    where A.NumericalOrder = {manyQuery} ";
            return sql;
        }
        /// <summary>
        /// 预付款申请关联采购合同
        /// </summary>
        /// <param name="ParentType"></param>
        /// <param name="ChildType"></param>
        /// <param name="ParentValue"></param>
        /// <param name="ChildValue"></param>
        /// <returns></returns>
        public List<RelatedODataEntity> GetRelatedDatasAsync(string ParentType,string ChildType,string ParentValue,string ChildValue)
        {
            var sql =string.Format(@"SELECT UUID() Guid,CONVERT(r.ChildValue USING utf8mb4) RelatedNumericalOrder,rd.Paid HavePaidAmount,rd.Payable Amount,rd.Payment,r.RelatedID,CONVERT(r.ParentValue USING utf8mb4) NumericalOrder
                                    ,CONVERT(p.DataDate USING utf8mb4) DataDate, CONVERT(p.ContractType USING utf8mb4) ContractType,p.ContractNumber,d.cDictName ContractTypeName,'' Number,'' ContractNumericalOrder
                                    FROM nxin_qlw_business.biz_related r
                        INNER JOIN nxin_qlw_business.biz_relateddetail rd ON r.RelatedId = rd.RelatedId
                        INNER JOIN nxin_qlw_business.PM_Contract p ON p.NumericalOrder=r.ChildValue
                        LEFT JOIN qlw_nxin_com.bsdatadict d ON d.DictID=P.ContractType
                        WHERE r.RelatedType = 201610210104402122 AND r.ParentType = {0} AND r.ChildType = {1} ", ParentType,ChildType);
            if (!string.IsNullOrEmpty(ChildValue))
            {
                sql+=string.Format(" AND r.ChildValue = {0}", ChildValue);
            }
            if (!string.IsNullOrEmpty(ParentValue))
            {
                sql += string.Format(" AND r.ParentValue = {0}", ParentValue);
            }
            return _context.RelatedDataSet.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 采购付款申请关联采购单
        /// </summary>
        /// <param name="ParentType"></param>
        /// <param name="ChildType"></param>
        /// <param name="ParentValue"></param>
        /// <param name="ChildValue"></param>
        /// <returns></returns>
        public List<RelatedODataEntity> GetRelatedPurchaseDatasAsync(string ParentType, string ChildType, string ParentValue, string ChildValue)
        {
            var sql = string.Format(@"SELECT UUID() Guid,CONVERT(r.ChildValue USING utf8mb4) RelatedNumericalOrder,rd.Paid HavePaidAmount,rd.Payable Amount,rd.Payment,r.RelatedID,CONVERT(r.ParentValue USING utf8mb4) NumericalOrder
                                    ,CONVERT(pp.DataDate USING utf8mb4) DataDate,CONVERT(pp.Number USING utf8mb4) Number,IFNULL(PC.ContractNumber,pc2.ContractNumber) ContractNumber,IFNULL(CONVERT(PC.NumericalOrder USING utf8mb4),CONVERT(pc2.NumericalOrder USING utf8mb4)) ContractNumericalOrder,'' ContractType,'' ContractTypeName
                                    FROM nxin_qlw_business.biz_related r
                        INNER JOIN nxin_qlw_business.biz_relateddetail rd ON r.RelatedId = rd.RelatedId
                        INNER JOIN nxin_qlw_business.PM_Purchase pp ON pp.NumericalOrder=r.ChildValue
                        INNER JOIN nxin_qlw_business.PM_PurchaseDetail AS PPD ON PPD.NumericalOrder = PP.NumericalOrder
                        LEFT JOIN nxin_qlw_business.PM_StuffAcceptanceDetail AS PSAD ON PPD.SourceNum = PSAD.NumericalOrder AND PPD.SourceID = PSAD.RecordID
                        LEFT JOIN nxin_qlw_business.PM_StuffAcceptance AS PSA ON PSA.NumericalOrder = PSAD.NumericalOrder AND PSA.EnterpriseID = PP.EnterpriseID
                        LEFT JOIN nxin_qlw_business.PM_StuffArriveDetail AS PSAD2 ON PSAD2.NumericalOrder = PSAD.SourceNum AND PSAD2.RecordID = PSAD.SourceID
                        LEFT JOIN nxin_qlw_business.PM_StuffArrive AS PSA2 ON PSA2.NumericalOrder = PSAD2.NumericalOrder AND PSA.EnterpriseID = PSA2.EnterpriseID
                        LEFT JOIN nxin_qlw_business.PM_ContractDetail AS PCD ON PSAD2.SourceNum = PCD.NumericalOrder AND PSAD2.SourceID = PCD.RecordID
                        LEFT JOIN nxin_qlw_business.PM_Contract AS PC ON PC.NumericalOrder = PCD.NumericalOrder AND PC.EnterpriseID = PSA2.EnterpriseID
                        LEFT JOIN nxin_qlw_business.PM_ContractDetail AS PCD2 ON PPD.SourceNum = PCD2.NumericalOrder AND PPD.SourceID = PCD2.RecordID
                        LEFT JOIN nxin_qlw_business.PM_Contract AS PC2 ON PC2.NumericalOrder = PCD2.NumericalOrder
                        WHERE r.RelatedType = 201610210104402122 AND r.ParentType = {0} AND r.ChildType = {1} ", ParentType, ChildType);
            if (!string.IsNullOrEmpty(ChildValue))
            {
                sql += string.Format(" AND r.ChildValue = {0}", ChildValue);
            }
            if (!string.IsNullOrEmpty(ParentValue))
            {
                sql += string.Format(" AND r.ParentValue = {0}", ParentValue);
            }
            sql += " GROUP BY r.ChildValue ";
            return _context.RelatedDataSet.FromSqlRaw(sql).ToList();
        }
    }
}
