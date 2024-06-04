using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Applications.Queries
{
    public class FM_CashSweepODataProvider : OneWithManyQueryProvider<FM_CashSweepODataEntity, FM_CashSweepDetailODataEntity>
    {
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private readonly HttpClientUtil _httpClientUtil;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HostConfiguration _hostConfiguration;
        public FM_CashSweepRequest req;
        public FM_CashSweepODataProvider(IIdentityService identityservice, QlwCrossDbContext context, HttpClientUtil httpClientUtil, IHttpContextAccessor httpContextAccessor, HostConfiguration hostConfiguration)
        {
            _identityservice = identityservice;
            _context = context;
            _httpClientUtil = httpClientUtil;
            _httpContextAccessor = httpContextAccessor;
            _hostConfiguration = hostConfiguration;
            req = new FM_CashSweepRequest();
        }

        public IEnumerable<FM_CashSweepODataEntity> GetList(ODataQueryOptions<FM_CashSweepODataEntity> odataqueryoptions, Uri uri)
        {
            var datas = GetData(odataqueryoptions, uri).AsEnumerable();
            return datas;
        }

        public IQueryable<FM_CashSweepODataEntity> GetData(ODataQueryOptions<FM_CashSweepODataEntity> odataqueryoptions, Uri uri)
        {
            return GetDatas();
        }

        public override IQueryable<FM_CashSweepODataEntity> GetDatas(NoneQuery query = null)
        {
            var strWhere = string.Format(@"ent.pid={0} ", _identityservice.GroupId);
            if (!string.IsNullOrEmpty(req.DateStr))
            {
                strWhere += " AND " + req.DateStr;
            }
            if (!string.IsNullOrEmpty(req.PermissionEnterpriseIDs))
            {
                strWhere += " AND a.`EnterpriseID` IN (" + req.PermissionEnterpriseIDs + ") ";
            }
            var sql = string.Format(@"SELECT 
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(A.Number USING utf8mb4) Number,	
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                                        DATE_FORMAT( A.DataDate,'%Y-%m-%d') DataDate,
                                        CONVERT(A.`AccountID` USING utf8mb4) AccountID,
                                        CONVERT(A.`SweepDirectionID` USING utf8mb4) SweepDirectionID,
                                        CONVERT(A.`SweepType` USING utf8mb4) SweepType,  
                                        A.Remarks,                                       
                                        CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        CONVERT(A.`ExcuteDate` USING utf8mb4) AS ExcuteDate,
                                        '' as DetailAccounts,
                                        CONVERT(A.`ExcuterID` USING utf8mb4) AS ExcuterID,
                                        A.CollectionScheme,
                                        A.IsUse ,
                                        ifnull(DATE_FORMAT( A.AutoTime,'%H:%i:%s'),A.AutoTime) AS AutoTime,
                                        A.SchemeType ,
                                        A.SchemeAmount ,
                                        A.Rate ,
                                        A.PlanType,
                                        A.SchemeFormula,
                                        A.SchemeTypeName,
                                        A.IsNew,
                                        ent.EnterpriseName EnterpriseName,
                                        HP1.Name OwnerName,
                                        fa.AccountName,
                                        d.`cDictName` SweepDirectionName,
                                        d1.`cDictName` SweepTypeName,
                                        HP.Name ExcuterName, 
                                        fa.`AccountNumber`,fa.AccountFullName,fa.DepositBank,fa.BankNumber, CONVERT(fa.BankID USING utf8mb4) BankID,                                      
                                        SUM(IFNULL(B.`AutoSweepBalance`,0))  Amount,
                                        '' as UploadInfo,
                                        CASE WHEN (FIND_IN_SET(1, audit.resultsRESu) AND (LENGTH(audit.resultsRESu) - LENGTH(REPLACE( audit.resultsRESu,',','' ))) = 0)   THEN '通过'
                                             WHEN (FIND_IN_SET(0, audit.resultsRESu) AND (LENGTH(audit.resultsRESu) - LENGTH(REPLACE( audit.resultsRESu,',','' ))) = 0)   THEN '未审批'
					                         WHEN (FIND_IN_SET(1, audit.resultsRESu) AND FIND_IN_SET(0, audit.resultsRESu) AND !FIND_IN_SET(2, audit.resultsRESu) AND !FIND_IN_SET(3, audit.resultsRESu)) THEN '审批中'
					                         WHEN (FIND_IN_SET(2, audit.resultsRESu) AND !FIND_IN_SET(3, audit.resultsRESu)) THEN '驳回'
					                         WHEN (FIND_IN_SET(3, audit.resultsRESu)) THEN '拒绝' END AuditResultName,'' AuditResult,
                					    CASE WHEN e.excuteStatusResults IS NULL  THEN '未归集'  
                					         WHEN (FIND_IN_SET(1, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集成功' 
                					         WHEN (FIND_IN_SET(0, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集失败'
                                             WHEN (FIND_IN_SET(4, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集失败'
                					         WHEN (FIND_IN_SET(2, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '处理中'
                					         ELSE '部分归集成功' END TradeResult  
                                        FROM  nxin_qlw_business.FM_CashSweep A 
                                        LEFT JOIN nxin_qlw_business.FM_CashSweepdetail B ON A.NumericalOrder=B.NumericalOrder
                                        LEFT JOIN  nxin_qlw_business.HR_Person HP1 ON HP1.BO_ID=A.OwnerID
                                        LEFT JOIN nxin_qlw_business.fd_account fa ON fa.AccountID=A.AccountID
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d ON d.DictID=A.SweepDirectionID AND d.pid=1811191754180000100
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d1 ON d1.DictID=A.SweepType AND d1.pid=1811191754180000200
                                        LEFT JOIN  nxin_qlw_business.HR_Person HP ON HP.BO_ID=A.ExcuterID
                                        LEFT JOIN  qlw_nxin_com.biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT faac.results) resultsRESu 
					                        	FROM NXin_Qlw_Business.fm_cashsweep a 
					                        	INNER JOIN qlw_nxin_com.faauditrecord faac  ON a.NumericalOrder = faac.NumericalOrder
					                        	LEFT JOIN  qlw_nxin_com.biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
					                        	WHERE {0}
					                        	GROUP BY a.`NumericalOrder`
					                        	) audit ON a.NumericalOrder = audit.NumericalOrder -- 审批结果
					                    LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT b.Status) excuteStatusResults 
					                    		FROM NXin_Qlw_Business.fm_cashsweep a 
					                    		INNER JOIN `nxin_qlw_business`.`fm_cashsweepdetail` b  ON a.NumericalOrder = b.NumericalOrder
					                        	LEFT JOIN  qlw_nxin_com.biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
					                        	WHERE {0}
					                    		GROUP BY a.`NumericalOrder`
					                    	  )e ON A.NumericalOrder = e.NumericalOrder -- 执行状态
					                    WHERE {0} 
                                        GROUP BY A.NumericalOrder
                                ORDER BY A.DataDate DESC,A.Number DESC  ", strWhere);
            return _context.FM_CashSweepEntityODataSet.FromSqlRaw(sql);
        }

        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FM_CashSweepODataEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = GetDataSql(manyQuery);

            return _context.FM_CashSweepEntityODataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        public FM_CashSweepODataEntity GetData(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = GetDataSql(manyQuery);

            var data = _context.FM_CashSweepEntityODataSet.FromSqlInterpolated(sql).FirstOrDefault();
            var workflowStates = GetWorkflowStatesAsync(data.NumericalOrder).Result;
            GetAuditResult(data, workflowStates);
            if (!data.IsNew)
            {
                //转归集状态 新旧菜单金融接口返回状态值不同，需要转换
                if (data.TradeResult == "归集成功")
                {
                    data.TradeResult = "归集失败";
                }
                else if (data.TradeResult == "归集失败")
                {
                    data.TradeResult = "归集成功";
                }
            }
            return data;
        }
        private FormattableString GetDataSql(long manyQuery)
        {
            return @$"SELECT 
                                        CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                                        CONVERT(A.Number USING utf8mb4) Number,	
                                        CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                                        DATE_FORMAT( A.DataDate,'%Y-%m-%d') DataDate,
                                        CONVERT(A.`AccountID` USING utf8mb4) AccountID,
                                        CONVERT(A.`SweepDirectionID` USING utf8mb4) SweepDirectionID,
                                        CONVERT(A.`SweepType` USING utf8mb4) SweepType,  
                                        A.Remarks,                                       
                                        CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                                        CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                                        CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                                        CONVERT(A.`ExcuteDate` USING utf8mb4) AS ExcuteDate,
                                        CONVERT(A.`ExcuterID` USING utf8mb4) AS ExcuterID,
                                        A.CollectionScheme,
                                        A.IsUse ,
                                        ifnull(DATE_FORMAT( A.AutoTime,'%H:%i:%s'),A.AutoTime) AS AutoTime,
                                        A.SchemeType ,
                                        '' as DetailAccounts,
                                        A.SchemeAmount ,
                                        A.Rate ,
                                        A.PlanType,
                                        A.SchemeFormula,
                                        A.SchemeTypeName,
                                        A.IsNew,
                                        ent.EnterpriseName EnterpriseName,
                                        HP1.Name OwnerName,
                                        fa.AccountName,
                                        d.`cDictName` SweepDirectionName,
                                        d1.`cDictName` SweepTypeName,
                                        HP.Name ExcuterName, 
                                        fa.`AccountNumber`, fa.AccountFullName,fa.DepositBank,fa.BankNumber,CONVERT(fa.BankID USING utf8mb4) BankID, 
                                        0.0 Amount,
                                        bs.Remarks as UploadInfo,
                                        CASE WHEN (FIND_IN_SET(1, audit.resultsRESu) AND (LENGTH(audit.resultsRESu) - LENGTH(REPLACE( audit.resultsRESu,',','' ))) = 0)   THEN '通过'
                                             WHEN (FIND_IN_SET(0, audit.resultsRESu) AND (LENGTH(audit.resultsRESu) - LENGTH(REPLACE( audit.resultsRESu,',','' ))) = 0)   THEN '未审批'
					                         WHEN (FIND_IN_SET(1, audit.resultsRESu) AND FIND_IN_SET(0, audit.resultsRESu) AND !FIND_IN_SET(2, audit.resultsRESu) AND !FIND_IN_SET(3, audit.resultsRESu)) THEN '审批中'
					                         WHEN (FIND_IN_SET(2, audit.resultsRESu) AND !FIND_IN_SET(3, audit.resultsRESu)) THEN '驳回'
					                         WHEN (FIND_IN_SET(3, audit.resultsRESu)) THEN '拒绝' END AuditResultName ,'' AuditResult,
                					    CASE WHEN e.excuteStatusResults IS NULL  THEN '未归集'  
                					         WHEN (FIND_IN_SET(1, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集成功' 
                					         WHEN (FIND_IN_SET(0, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集失败'
                					         WHEN (FIND_IN_SET(4, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '归集失败'
                					         WHEN (FIND_IN_SET(2, e.excuteStatusResults) AND (LENGTH(e.excuteStatusResults) - LENGTH(REPLACE( e.excuteStatusResults,',','' ))) = 0)   THEN '处理中'
                					         ELSE '部分归集成功' END TradeResult  
                                        FROM  nxin_qlw_business .FM_CashSweep A 
                                        LEFT JOIN  nxin_qlw_business .HR_Person HP1 ON HP1.BO_ID=A.OwnerID
                                        LEFT JOIN nxin_qlw_business.fd_account fa ON fa.AccountID=A.AccountID
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d ON d.DictID=A.SweepDirectionID AND d.pid=1811191754180000100
                                        LEFT JOIN  qlw_nxin_com.bsdatadict d1 ON d1.DictID=A.SweepType AND d1.pid=1811191754180000200
                                        LEFT JOIN  nxin_qlw_business .HR_Person HP ON HP.BO_ID=A.ExcuterID
                                        LEFT JOIN  qlw_nxin_com . biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID
                                        LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = A.`NumericalOrder`
                                        LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT faac.results) resultsRESu 
					                        	FROM NXin_Qlw_Business.fm_cashsweep a 
					                        	INNER JOIN qlw_nxin_com.faauditrecord faac  ON a.NumericalOrder = faac.NumericalOrder
					                        	WHERE a.NumericalOrder={manyQuery}
					                        	GROUP BY a.`NumericalOrder`
					                        	) audit ON a.NumericalOrder = audit.NumericalOrder -- 审批结果
					                    LEFT JOIN (SELECT a.numericalorder,GROUP_CONCAT(DISTINCT b.Status) excuteStatusResults 
					                    		FROM NXin_Qlw_Business.fm_cashsweep a 
					                    		INNER JOIN `nxin_qlw_business`.`fm_cashsweepdetail` b  ON a.NumericalOrder = b.NumericalOrder
					                        	WHERE a.NumericalOrder={manyQuery}
					                    		GROUP BY a.`NumericalOrder`
					                    	  )e ON A.NumericalOrder = e.NumericalOrder -- 执行状态
					                          WHERE a.NumericalOrder = {manyQuery}";
        }

        public override Task<List<FM_CashSweepDetailODataEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            FormattableString sql = GetDetailSql(manyQuery);
            return _context.FM_CashSweepDetailEntityODataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        public List<FM_CashSweepDetailODataEntity> GetDetaiDatas(long manyQuery)
        {
            FormattableString sql = GetDetailSql(manyQuery);
            return _context.FM_CashSweepDetailEntityODataSet.FromSqlInterpolated(sql).ToList();
        }
        private FormattableString GetDetailSql(long manyQuery)
        {
            return @$" SELECT 
                                    FD.RecordID,
                                    CONVERT(FD.NumericalOrderDetail USING utf8mb4) NumericalOrderDetail,
                                    CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                                    CONVERT(FD.EnterpriseID USING utf8mb4) EnterpriseID,                                    
                                    CONVERT(FD.`AccountID` USING utf8mb4) AccountID,
                                    IFNULL(FD.`AccountBalance`,0.0) AccountBalance,
                                    IFNULL(FD.`OtherAccountBalance`,0.0) OtherAccountBalance,
                                    IFNULL(FD.`TheoryBalance`,0.0) TheoryBalance,
                                    IFNULL(FD.`TransformBalance`,0.0) TransformBalance,
                                    IFNULL(FD.`AutoSweepBalance`,0.0) AutoSweepBalance,
                                    IFNULL(FD.`ManualSweepBalance`,0.0) ManualSweepBalance,
                                    CONVERT(FD.`AutoSweepBalance` USING utf8mb4) AutoSweepBalance_Show, 
                                    FD.`Remark`,
                                    FD.`Status`,
                                    FD.`ExcuteMsg`,
                                    FD.`ModifiedDate`,
                                    ent.EnterpriseName,
                                    fa.AccountName,
                                    fa.`AccountNumber`,fa.AccountFullName,fa.DepositBank,fa.BankNumber,CONVERT(fa.BankID USING utf8mb4) BankID
                                    FROM  nxin_qlw_business.FM_CashSweepdetail FD 
                                    LEFT JOIN  qlw_nxin_com . biz_enterprise  ent ON FD.EnterpriseID=ent.EnterpriseID
                                    LEFT JOIN nxin_qlw_business.fd_account fa ON fa.AccountID=FD.AccountID
                                     WHERE FD.NumericalOrder =  {manyQuery}";
        }
        #region 获取备用金
        public Task<List<RevolvingFundDetailODataEntity>> GetRevolvingFundData(RevolvingFundDetailRequest req)
        {
            var sql = string.Format(@"SELECT fd.RecordID,CONVERT(fd.AccountID USING utf8mb4) AccountID,nMinimum FROM  nxin_qlw_business . biz_revolvingfund  f
                                    INNER JOIN  nxin_qlw_business.biz_revolvingfunddetail fd ON f. iNumericalOrder = fd.iNumericalOrder
                                    INNER JOIN(SELECT fd.EnterpriseID, fd.AccountID , MAX(f.dDate ) dDate FROM nxin_qlw_business . biz_revolvingfund f
                                    LEFT JOIN  nxin_qlw_business.biz_revolvingfunddetail fd ON f. iNumericalOrder = fd.iNumericalOrder
                                    WHERE fd. EnterpriseID = {0} AND f. dDate <= '{1}' AND fd.AccountID = {2}
                                    GROUP BY fd.EnterpriseID,fd.AccountID ) rv ON rv.dDate = f.dDate AND rv.EnterpriseID = fd.EnterpriseID AND rv.AccountID = fd.AccountID
                                    WHERE fd. EnterpriseID = {0}  AND fd.AccountID = {2} ", req.EnterpriseID, req.dDate, req.AccountID);
            return _context.RevolvingFundDetailODataEntity.FromSqlRaw(sql).ToListAsync();
        }

        #endregion

        #region 转化审批状态
        //审批状态： 0：未审批，1：通过，2：驳回 ,3：拒绝 4：审批中
        public FM_CashSweepODataEntity GetAuditResult(FM_CashSweepODataEntity item, List<WorkflowState> workflowStates)
        {
            // 找到单据对应的状态
            var ws = workflowStates.FirstOrDefault(p => p.NumericalOrder == item.NumericalOrder);
            if (ws != null)
            {
                item.AuditResultName = ws.StateName;
            }

            if (item.AuditResultName == "未审批")
            {
                item.AuditResult = "0";
            }
            else if (item.AuditResultName == "通过")
            {
                item.AuditResult = "1";
            }
            else if (item.AuditResultName == "驳回")
            {
                item.AuditResult = "2";
            }
            else if (item.AuditResultName == "拒绝")
            {
                item.AuditResult = "3";
            }
            else if (item.AuditResultName == "审批中")
            {
                item.AuditResult = "4";
            }
            return item;
        }

        /// <summary>
        /// 接口获取新工作流状态
        /// </summary>
        /// <param name="numbericalOrders"></param>
        /// <returns></returns>
        public async Task<List<WorkflowState>> GetWorkflowStatesAsync(string numbericalOrders)
        {
            var list = new List<WorkflowState>();

            if (_hostConfiguration.IsEnableNewWorkflow == "1")
            {

                var url = $"{_hostConfiguration.NewWorkflowHost}/flowinfo/state?numericalOrder={numbericalOrders}";

                AuthenticationHeaderValue authentication;
                bool verification = AuthenticationHeaderValue.TryParse(_httpContextAccessor.HttpContext.Request.GetAuthToken(), out authentication);
                // 获取新流程审批状态
                var result = await _httpClientUtil.GetJsonAsync<Object, Result<List<WorkflowState>>>(url, null, p =>
                {
                    p.Authorization = authentication;
                });
                if (result.Status == 1)
                {
                    return result.Data;
                }
            }
            return list;
        }

        /// <summary>
        /// 返回结果实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Result<T>
        {
            public string Message { get; set; }
            public T Data { get; set; }
            public int Status { get; set; }
        }

        /// <summary>
        /// 流程状态实体
        /// </summary>
        public class WorkflowState
        {
            // 流程状态
            public string State { get; set; }
            // 状态名称
            public string StateName { get; set; }
            // 单据号
            public string NumericalOrder { get; set; }

        }

        #endregion
    }
}
