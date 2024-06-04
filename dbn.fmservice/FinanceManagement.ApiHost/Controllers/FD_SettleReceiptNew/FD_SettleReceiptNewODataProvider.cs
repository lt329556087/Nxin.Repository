using Architecture.Common.Application.Query;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceipt;
using FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface;
using FinanceManagement.ApiHost.Extension;
using FinanceManagement.Common;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;

namespace FinanceManagement.ApiHost.Controllers
{
    public class FD_SettleReceiptNewODataProvider : OneWithManyQueryProvider<FD_SettleReceiptEntity, FD_SettleReceiptDetailEntity>
    {
        ILogger<FD_SettleReceiptODataProvider> _logger;
        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private readonly Nxin_Qlw_BusinessContext _Qlw_BusinessContext;
        private readonly IMemoryCache _memoryCache;
        FMBaseCommon _baseUnit;
        EnterprisePeriodUtil _enterprisePeriodUtil;

        public FD_SettleReceiptNewODataProvider(EnterprisePeriodUtil enterprisePeriodUtil,FMBaseCommon baseUnit, Nxin_Qlw_BusinessContext Qlw_BusinessContext,IIdentityService identityservice, QlwCrossDbContext context,  ILogger<FD_SettleReceiptODataProvider> logger)
        {
            _identityservice = identityservice;
            _context = context;
            _logger = logger;
            _Qlw_BusinessContext = Qlw_BusinessContext;
            _baseUnit = baseUnit;
            _enterprisePeriodUtil = enterprisePeriodUtil;
        }
        public override IQueryable<FD_SettleReceiptEntity> GetDatas(NoneQuery query = null)
        {
            return null;
        }
        /// <summary>
        /// 获取资金账户期初列表
        /// </summary>
        /// <returns></returns>
        public dynamic GetFundList(string BeginDate,string EndDate)
        {
            return _context.DynamicSqlQuery($@"
            SELECT v.*,hr.Name FROM nxin_qlw_business.fd_settlereceipt v
            left join nxin_qlw_business.hr_person hr on hr.BO_ID = v.OwnerID
            where v.EnterpriseID = {_identityservice.EnterpriseId} and v.DataDate between '{BeginDate}' AND '{EndDate}' AND v.SettleReceipType = 201610220104402205 
            ");
        }
        public List<FD_SettleReceiptEntity> GetDataList(string begindate = "",string enddate = "",string enterpriseId = "",string numericalorders="")
        {
            var sql = GetHeadSql(begindate,enddate,enterpriseId,numericalorders);
          
            var data = _context.FD_SettleReceiptNewDataSet.FromSqlRaw(sql).ToList().Distinct().ToList();
            if (data.Count > 0)
            {
                var headExtend = GetHeadExtend(string.Join(',', data.Select(m => m.NumericalOrder)));
                var query = from r in data
                            join h in headExtend on r.NumericalOrder equals h.NumericalOrder.ToString()
                            select new FD_SettleReceiptEntity()
                            {
                                AccountNo = r.AccountNo,
                                Amount = h.Amount,
                                EnterpriseID = r.EnterpriseID,
                                EnterpriseName = r.EnterpriseName,
                                ApplyNumericalOrder = r.ApplyNumericalOrder,
                                ApplySettleReceipType = r.ApplySettleReceipType,
                                AttachmentNum = r.AttachmentNum,
                                AuditName = r.AuditName,
                                CreatedDate = r.CreatedDate,
                                DataDate = r.DataDate,
                                Guid = r.Guid,
                                line = r.line,
                                Lines = r.Lines,
                                ModifiedDate = r.ModifiedDate,
                                Number = r.Number,
                                NumericalOrder = r.NumericalOrder,
                                OwnerID = r.OwnerID,
                                OwnerName = r.OwnerName,
                                PayReceType = r.PayReceType,
                                ReceiptAbstractID = h.ReceiptAbstractID.ToString(),
                                ReceiptAbstractName = h.ReceiptAbstractName,
                                Remarks = r.Remarks,
                                SettleReceipType = r.SettleReceipType,
                                SettleReceipTypeName = r.SettleReceipTypeName,
                                TicketedPointID = r.TicketedPointID,
                                TicketedPointName = r.TicketedPointName,
                                TicketedPointNumber = r.TicketedPointNumber,
                                UploadInfo = r.UploadInfo,
                                ApplyNumber = r.ApplyNumber,
                                IsGroupPay = r.IsGroupPay,
                                InvocieNumber = r.InvocieNumber,
                                InvocieNumericalOrder = r.InvocieNumericalOrder,
                                MarIsEnd = h.MarIsEnd,
                                AccIsEnd = h.AccIsEnd,
                                SummaryIsEnd = h.SummaryIsEnd
                            };
                #region 排序规则   【79214】【新版会计凭证】列表默认排序展示后端   2023-07-11 14:23:27
                
                {
                    //选项一：
                    //选项ID：20191118103011（集团和单位选项）
                    //开启，会计凭证编号按单据字进行编号； 关闭，会计凭证编号不按单据字进行编号； 默认关闭
                    var optionId1 = "20191118103011";
                    var option1 = GetOptionInfos(optionId1, enterpriseId);
                    //选项二：
                    //选项ID：20191118103100（集团和单位选项）
                    //默认不开启，开启后会计凭证编码只以凭证类别进行编码，例如：收 - 1、付 - 1、转 - 1等开始编码
                    var optionId2 = "20191118103100";
                    var option2 = GetOptionInfos(optionId2, enterpriseId);

                    //场景一：选项二关闭，选项一开启
                    //编号规则：按单据字分类排序，列表排序按照 北京 1 北京 2 上海1 上海2
                    if (!option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)
                    {
                        return query.OrderByDescending(m => Convert.ToInt64(m.TicketedPointID)).ThenByDescending(m => m.Number).ToList();
                    }
                    //场景二：选项二关闭，选项一关闭
                    //编号规则：排序就是1开始，不区分单据字和凭证类别
                    else if (!option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
                    {
                        return query.OrderByDescending(m => m.Number).ToList();
                    }
                    //场景三：选项二开启，选项一关闭
                    //编号规则：按凭证类别分类排序，如收1 收2 收3 付1 付2 付3
                    else if (option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
                    {

                        return query.OrderByDescending(m => Convert.ToInt64(m.SettleReceipType)).ThenByDescending(m => m.Number).ToList();
                    }
                    //场景四：选项二开启，选项一开启
                    //编号规则：按单据字、凭证类别分类编号 列表排序如 北京 收1 北京收2 北京 付 1 北京付2 上海收1 上海收2
                    else if (option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)
                    {
                        return query.OrderByDescending(m => Convert.ToInt64(m.TicketedPointID)).ThenByDescending(m => Convert.ToInt64(m.SettleReceipType)).ThenByDescending(m => m.Number).ToList();
                    }
                }
                #endregion
                return query.ToList()/*.OrderByDescending(m=>m.EnterpriseID).ThenByDescending(m=>m.)*/;
            }
            return new List<FD_SettleReceiptEntity>();
        }
        public List<FD_SettleReceiptHeadExtend> GetHeadExtend(string nums = "0")
        {
            if (string.IsNullOrEmpty(nums))
            {
                nums = "0";
            }
            return _context.FD_SettleReceiptHeadExtendDataSet.FromSqlRaw($@"SELECT SUM(A.Debits) Amount,A.* FROM (
            SELECT (count(summary2.IsEnd) + count(summaryGroup2.IsEnd)) SummaryIsEnd,count(a.IsEnd) as AccIsEnd,count(m.IsEnd) as MarIsEnd,SUM(D.Debit) Debits,A.NumericalOrder,D.`ReceiptAbstractID`,CONCAT(IFNULL(summary.`SettleSummaryName`,summaryGroup.SettleSummaryGroupName),IFNULL(CONCAT('/',D.`Content`),'')) ReceiptAbstractName FROM nxin_qlw_business.fd_settlereceipt A 
            INNER JOIN nxin_qlw_business.fd_settlereceiptdetail D ON D.NumericalOrder = A.NumericalOrder
            LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` summary ON summary.`SettleSummaryID` = D.`ReceiptAbstractID`
            LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` summaryGroup ON summaryGroup.settlesummarygroupid = D.`ReceiptAbstractID`
            LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` summary2 ON summary2.`SettleSummaryID` = D.`ReceiptAbstractID` and summary2.IsEnd = false
            LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` summaryGroup2 ON summaryGroup2.settlesummarygroupid = D.`ReceiptAbstractID` and summaryGroup2.IsEnd = false
            LEFT JOIN  qlw_nxin_com.biz_accosubject a ON D.AccoSubjectID=a.AccoSubjectID and a.IsEnd = false
            LEFT JOIN qlw_nxin_com.biz_market m ON m.MarketID=D.MarketID and a.IsEnd = false
            WHERE A.Numericalorder IN ({nums}) 
            GROUP BY D.RecordId
            ORDER BY D.`RowNum`
            ) A
            GROUP BY A.NumericalOrder ").ToList();
        }
        private string GetHeadSql(string begindate = "", string enddate = "", string enterpriseId = "",string numericalorders="")
        {
            if (string.IsNullOrEmpty(begindate))
            {
                return string.Format($@"SELECT 
                         0 as SummaryIsEnd,
                         0 as AccIsEnd,
                         0 as MarIsEnd,
                         CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                         CONVERT(A.SettleReceipType USING utf8mb4) AS SettleReceipType,
                         CONVERT(A.TicketedPointID USING utf8mb4) AS TicketedPointID,
                         A.Number,	
                         CONCAT(TicketedPointName,'-',A.Number) TicketedPointNumber,	
                         CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                         DATE_FORMAT(A.DataDate,'%Y-%m-%d') DataDate,
                         IFNULL(A.Guid,UUID()) Guid,
                         Concat(A.SettleReceipType) PayReceType,
                         A.Remarks,    
                         '0' line,
                         A.AccountNo,
                         A.AttachmentNum,                                   
                         CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                         CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                         CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                         d.DataDictName SettleReceipTypeName,
                         TicketedPointName,
                         bs.Remarks AS UploadInfo,
                         EnterpriseName,
                         hr.Name OwnerName,
                         hr2.Name AuditName,
                         B.Debit Amount,
                         CONCAT(B.ReceiptAbstractID) ReceiptAbstractID,
                         B.ReceiptAbstractName,
                         IF(pay.NumericalOrder is null,'',(
                         case pay.SettleReceipType 
                         when 201611180104402204 THEN CONCAT('付款汇总单-',pay.Number) 
                         when 201611180104402202 THEN CONCAT('付款单-',pay.Number) 
                         when 201611180104402203 THEN CONCAT('收款汇总单-',pay.Number) 
                         when 201611180104402201 THEN CONCAT('收款单-',pay.Number) END)) ApplyNumber,
                         CONCAT(pay.NumericalOrder) ApplyNumericalOrder,
                         Concat(pay.SettleReceipType) ApplySettleReceipType,
                         IFNULL(CONCAT('采购发票-',invo.Number),'') InvocieNumber,
                         IFNULL(CONCAT(invo.NumericalOrder),'') InvocieNumericalOrder,
                         pay.IsGroupPay
                         FROM  nxin_qlw_business.fd_settlereceipt A 
                         INNER JOIN (
                             SELECT SUM(A.Debits) Debit,A.* FROM (
                             SELECT SUM(D.Debit) Debits,A.NumericalOrder,D.`ReceiptAbstractID`,CONCAT(IFNULL(summary.`SettleSummaryName`,summaryGroup.SettleSummaryGroupName),IFNULL(CONCAT('/',D.`Content`),'')) ReceiptAbstractName FROM nxin_qlw_business.fd_settlereceipt A 
                             INNER JOIN nxin_qlw_business.fd_settlereceiptdetail D ON D.NumericalOrder = A.NumericalOrder
                             LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` summary ON summary.`SettleSummaryID` = D.`ReceiptAbstractID`
                             LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` summaryGroup ON summaryGroup.settlesummarygroupid = D.`ReceiptAbstractID`
                             WHERE A.NumericalOrder IN({numericalorders})
                             GROUP BY D.RecordId
                             ORDER BY D.`RowNum`
                             ) A
                             GROUP BY A.NumericalOrder
                         ) B on B.NumericalOrder = A.NumericalOrder
                         LEFT JOIN  nxin_qlw_business.biz_datadict d ON d.DataDictID=A.SettleReceipType 
                         LEFT JOIN  nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID=A.TicketedPointID 
                         LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = A.`NumericalOrder`
                         LEFT JOIN nxin_qlw_business.hr_person hr on hr.bo_id = A.OwnerId
                         LEFT JOIN `nxin_qlw_business`.`biz_reviwe` audit on audit.NumericalOrder = A.NumericalOrder and audit.Level = 2 AND audit.ReviweType = 1611091727140000101 
                         INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.`EnterpriseID` = A.`EnterpriseID`
                         LEFT JOIN nxin_qlw_business.hr_person hr2 on hr2.bo_id = audit.CheckedByID
                         left join nxin_qlw_business.fd_paymentreceivables pay on pay.numericalorder = A.numericalorder
                         LEFT JOIN nxin_qlw_business.biz_related pv on pv.ChildValue = A.NumericalOrder and pv.ChildType = 1611091727140000101 and ParentType = 1803091529030000101
						 LEFT JOIN nxin_qlw_business.pm_invoice invo on invo.NumericalOrder = pv.ParentValue
                         WHERE A.NumericalOrder IN({numericalorders})
                         ORDER BY A.Number DESC 
                        ");
            }
            else
            {
                var numsSql = $@"SELECT CONCAT(NumericalOrder) NumericalOrder,false IsGroupPay from nxin_qlw_business.fd_settlereceipt A WHERE A.EnterpriseID IN ({enterpriseId}) AND A.DataDate BETWEEN '{begindate}' AND '{enddate}' AND A.SettleReceipType NOT IN (201610220104402204,201610220104402205,201610220104402206) ";
                var nums = _context.NumericalOrderDataSet.FromSqlRaw(numsSql).ToList();
                var numsStr = "";
                if (nums.Count == 0)
                {
                    numsStr = "0";
                }
                else
                {
                    numsStr = string.Join(',', nums.Select(m=>m.NumericalOrder));
                }
                return string.Format($@"SELECT 
                         0 as SummaryIsEnd,
                         0 as AccIsEnd,
                         0 as MarIsEnd,
                         CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                         CONVERT(A.SettleReceipType USING utf8mb4) AS SettleReceipType,
                         CONVERT(A.TicketedPointID USING utf8mb4) AS TicketedPointID,
                         A.Number,	
                         CONCAT(TicketedPointName,'-',A.Number) TicketedPointNumber,	
                         CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                         DATE_FORMAT(A.DataDate,'%Y-%m-%d') DataDate,
                         IFNULL(A.Guid,UUID()) Guid,
                         CONCAT(A.SettleReceipType) PayReceType,
                         A.Remarks,    
                         A.AccountNo,
                         '0' line,
                         A.AttachmentNum,                                   
                         CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                         CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                         CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                         d.DataDictName SettleReceipTypeName,
                         TicketedPointName,
                         bs.Remarks AS UploadInfo,
                         EnterpriseName,
                         hr.Name OwnerName,
                         hr2.Name AuditName,
                         0.00 Amount,
                         '0' ReceiptAbstractID,
                         ''  ReceiptAbstractName,
                         IF(pay.NumericalOrder is null,'',(
                         case pay.SettleReceipType 
                         when 201611180104402204 THEN CONCAT('付款汇总单-',pay.Number) 
                         when 201611180104402202 THEN CONCAT('付款单-',pay.Number) 
                         when 201611180104402203 THEN CONCAT('收款汇总单-',pay.Number) 
                         ELSE CONCAT('收款单-',pay.Number) END)) ApplyNumber,
                         CONCAT(pay.NumericalOrder) ApplyNumericalOrder,
                         Concat(pay.SettleReceipType) ApplySettleReceipType,
                         IFNULL(CONCAT('采购发票-',invo.Number),'') InvocieNumber,
                         IFNULL(CONCAT(invo.NumericalOrder),'') InvocieNumericalOrder,
                         pay.IsGroupPay
                         FROM  nxin_qlw_business.fd_settlereceipt A 
                         LEFT JOIN  nxin_qlw_business.biz_datadict d ON d.DataDictID=A.SettleReceipType 
                         LEFT JOIN  nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID=A.TicketedPointID 
                         LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = A.`NumericalOrder`
                         LEFT JOIN nxin_qlw_business.hr_person hr on hr.bo_id = A.OwnerId
                         LEFT JOIN `nxin_qlw_business`.`biz_reviwe` audit on audit.NumericalOrder = A.NumericalOrder and audit.Level = 2  AND audit.ReviweType = 1611091727140000101 
                         INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.`EnterpriseID` = A.`EnterpriseID`
                         LEFT JOIN nxin_qlw_business.hr_person hr2 on hr2.bo_id = audit.CheckedByID
                         left join nxin_qlw_business.fd_paymentreceivables pay on pay.numericalorder = A.numericalorder
                         LEFT JOIN nxin_qlw_business.biz_related pv on pv.ChildValue = A.NumericalOrder and pv.ChildType = 1611091727140000101 and ParentType = 1803091529030000101
						 LEFT JOIN nxin_qlw_business.pm_invoice invo on invo.NumericalOrder = pv.ParentValue
                         WHERE A.NumericalOrder IN ({numsStr})
                         ORDER BY A.Number DESC 
                        ");
            }
            
        }
        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="manyQuery"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public override Task<FD_SettleReceiptEntity> GetDataAsync(long manyQuery, NoneQuery query = null)
        {
            FormattableString sql = GetDataSql(manyQuery);

            return _context.FD_SettleReceiptNewDataSet.FromSqlInterpolated(sql).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 是否序时
        /// 选项ID：20180330173530 开启后，付款单、付款汇总单、收款单、收款汇总单、会计凭证 需要增加序时控制
        /// 单据或凭证日期不得早于当前会计期间内已存在单据或凭证的最晚日期，允许等于或晚于
        /// 在选择单据或凭证日期时，触发校验，提示：请遵循序时原则
        /// 关闭，不校验序时控制
        /// </summary>
        /// <param name="DataDate">凭证日期</param>
        /// <param name="SettleReceipType">凭证类别</param>
        /// <param name="EnterpriseID">单位</param>
        /// <returns></returns>
        public bool SequentialTime(string DataDate, string EnterpriseID, string SettleReceipType)
        {
            //选项ID：20180330173530 开启后，付款单、付款汇总单、收款单、收款汇总单、会计凭证 需要增加序时控制

            var optionId1 = "20180330173530";
            var option1 = GetOptionInfos(optionId1, EnterpriseID);
            //收付款序时控制
            //201611180104402201  收款    
            //201611180104402202  付款
            //201611180104402203  汇总收款
            //201611180104402204  汇总付款

            if (SettleReceipType == "201611180104402201" || SettleReceipType == "201611180104402202" || SettleReceipType == "201611180104402203" || SettleReceipType == "201611180104402204")
            {
                if (option1.FirstOrDefault().OptionSwitch)
                {
                    string sql = $@" select  CONCAT(IFNULL(max(datadate),'')) MaxNumber from nxin_qlw_business.fd_paymentreceivables WHERE  EnterpriseID = {EnterpriseID} and SettleReceipType = '{SettleReceipType}' and DataDate > '{DataDate}' and month(DataDate) = month('{DataDate}')  and year(DataDate) = year('{DataDate}')  Limit 0,1 ";

                    var data = _context.GetNumbers.FromSqlRaw(sql).FirstOrDefault();
                    if (data != null)
                    {
                        if (!string.IsNullOrEmpty(data.MaxNumber))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            //会计凭证序时逻辑
            else
            {
                if (option1.FirstOrDefault().OptionSwitch)
                {
                    string sql = $@" select  CONCAT(IFNULL(max(datadate),'')) MaxNumber from nxin_qlw_business.FD_SettleReceipt WHERE  EnterpriseID = {EnterpriseID} and SettleReceipType = '{SettleReceipType}' and DataDate > '{DataDate}' and month(DataDate) = month('{DataDate}')  and year(DataDate) = year('{DataDate}')  Limit 0,1 ";

                    var data = _context.GetNumbers.FromSqlRaw(sql).FirstOrDefault();
                    if (data != null)
                    {
                        if (!string.IsNullOrEmpty(data.MaxNumber))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取当前最大凭证号
        /// </summary>
        /// <param name="SettleReceipType"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public Number GetMaxNumber(string SettleReceipType,string EnterpriseID)
        {
            string sql = $@" SELECT IFNULL(CONCAT(MAX(IFNULL(Number,0))+1),1) MaxNumber FROM `nxin_qlw_business`.`fd_settlereceipt` WHERE `SettleReceipType` = {SettleReceipType} AND `EnterpriseID` = {EnterpriseID} ";

            var data = _context.GetNumbers.FromSqlRaw(sql).FirstOrDefault();

            //var myAvatar = _avatarCache.GetOrCreate(data.Result.MaxNumber, () => data.Result.MaxNumber);

            return data;
        }
        /// <summary>
        /// 获取当前最大凭证号
        /// </summary>
        /// <param name="SettleReceipType"></param>
        /// <param name="EnterpriseID"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="TicketPoint"></param>
        /// <returns></returns>
        public Number GetMaxNumberByDate(string SettleReceipType, string EnterpriseID,string BeginDate,string EndDate,string TicketPoint)
        {
            //选项一：
            //选项ID：20191118103011（集团和单位选项）
            //开启，会计凭证编号按单据字进行编号； 关闭，会计凭证编号不按单据字进行编号； 默认关闭
            var optionId1 = "20191118103011";
            var option1 = GetOptionInfos(optionId1, EnterpriseID);
            //选项二：
            //选项ID：20191118103100（集团和单位选项）
            //默认不开启，开启后会计凭证编码只以凭证类别进行编码，例如：收 - 1、付 - 1、转 - 1等开始编码
            var optionId2 = "20191118103100";
            var option2 = GetOptionInfos(optionId2, EnterpriseID);
            //不开启系统选项  无视 凭证类别 自增加1

            //场景二：选项二关闭，选项一关闭
            //编号规则：什么都不区分，编号就是1开始
            if (!option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.EnterpriseID == EnterpriseID && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate) 
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    data = "0";
                }
                //var myAvatar = _avatarCache.GetOrCreate(data.Result.MaxNumber, () => data.Result.MaxNumber);
                return new Number() { MaxNumber = (Convert.ToInt64(data) + 1).ToString() };
            }
            //场景三：选项二开启，选项一关闭
            //编号规则：按凭证类别分类编号
            else if(option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)  
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.SettleReceipType == SettleReceipType && m.EnterpriseID == EnterpriseID && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate) 
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    data = "0";
                }
                //var myAvatar = _avatarCache.GetOrCreate(data.Result.MaxNumber, () => data.Result.MaxNumber);
                return new Number() { MaxNumber = (Convert.ToInt64(data) + 1).ToString() };
            }
            //场景四：选项二开启，选项一开启
            //编号规则：按单据字、凭证类别分类编号 列表排序如 北京 收1 北京收2 北京 付 1 北京付2
            else if(option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.SettleReceipType == SettleReceipType && m.EnterpriseID == EnterpriseID && m.TicketedPointID == TicketPoint && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate)
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    data = "0";
                }
                //var myAvatar = _avatarCache.GetOrCreate(data.Result.MaxNumber, () => data.Result.MaxNumber);
                return new Number() { MaxNumber = (Convert.ToInt64(data) + 1).ToString() };
            }
            //场景一：选项二关闭，选项一开启
            //编号规则：按单据字分类排序，列表排序如 北京 1 北京 2 上海1 上海2
            else /*if (!option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)*/
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.EnterpriseID == EnterpriseID && m.TicketedPointID == TicketPoint && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate)
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    data = "0";
                }
                //var myAvatar = _avatarCache.GetOrCreate(data.Result.MaxNumber, () => data.Result.MaxNumber);
                return new Number() { MaxNumber = (Convert.ToInt64(data) + 1).ToString() };
            }
        }
        /// <summary>
        /// 获取当前凭证号是否存在
        /// </summary>
        /// <param name="SettleReceipType"></param>
        /// <param name="EnterpriseID"></param>
        /// <param name="Number"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="NumericalOrder"></param>
        /// <param name="TicketedPoint"></param>
        /// <param name="TicketedPointID"></param>
        /// <returns></returns>
        public Number GetIsExitNumber(string SettleReceipType, string EnterpriseID, string Number, string BeginDate, string EndDate, string NumericalOrder = "0", string TicketedPoint = "0")
        {
            if (string.IsNullOrEmpty(NumericalOrder))
            {
                NumericalOrder = "0";
            }
            if (string.IsNullOrEmpty(TicketedPoint))
            {
                TicketedPoint = "0";
            }
            //选项一：
            //选项ID：20191118103011（集团和单位选项）
            //开启，会计凭证编号按单据字进行编号； 关闭，会计凭证编号不按单据字进行编号； 默认关闭
            var optionId1 = "20191118103011";
            var option1 = GetOptionInfos(optionId1, EnterpriseID);
            //选项二：
            //选项ID：20191118103100（集团和单位选项）
            //默认不开启，开启后会计凭证编码只以凭证类别进行编码，例如：收 - 1、付 - 1、转 - 1等开始编码
            var optionId2 = "20191118103100";
            var option2 = GetOptionInfos(optionId2, EnterpriseID);
            //不开启系统选项  无视 凭证类别 自增加1

            //场景二：选项二关闭，选项一关闭
            //编号规则：什么都不区分，编号就是1开始
            if (!option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.EnterpriseID == EnterpriseID && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate) && m.NumericalOrder != NumericalOrder && m.Number == Number
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    return new Number() { MaxNumber = "0" };
                }
                else
                {
                    return new Number() { MaxNumber = data };
                }
            }
            //场景三：选项二开启，选项一关闭
            //编号规则：按凭证类别分类编号
            else if (option2.FirstOrDefault().OptionSwitch && !option1.FirstOrDefault().OptionSwitch)
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.SettleReceipType == SettleReceipType && m.EnterpriseID == EnterpriseID && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate) && m.NumericalOrder != NumericalOrder && m.Number == Number 
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    return new Number() { MaxNumber = "0" };
                }
                else
                {
                    return new Number() { MaxNumber = data };
                }
            }
            //场景四：选项二开启，选项一开启
            //编号规则：按单据字、凭证类别分类编号 列表排序如 北京 收1 北京收2 北京 付 1 北京付2
            else if (option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.SettleReceipType == SettleReceipType && m.TicketedPointID == TicketedPoint && m.EnterpriseID == EnterpriseID && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate) && m.NumericalOrder != NumericalOrder && m.Number == Number
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    return new Number() { MaxNumber = "0" };
                }
                else
                {
                    return new Number() { MaxNumber = data };
                }
            }
            //场景一：选项二关闭，选项一开启
            //编号规则：按单据字分类排序，列表排序如 北京 1 北京 2 上海1 上海2
            else /*if (!option2.FirstOrDefault().OptionSwitch && option1.FirstOrDefault().OptionSwitch)*/
            {
                var data = _Qlw_BusinessContext.Set<Domain.FD_SettleReceipt>().Where(m => m.TicketedPointID == TicketedPoint && m.EnterpriseID == EnterpriseID && m.DataDate >= Convert.ToDateTime(BeginDate) && m.DataDate <= Convert.ToDateTime(EndDate) && m.NumericalOrder != NumericalOrder && m.Number == Number
                && m.SettleReceipType != "201610220104402204"
                && m.SettleReceipType != "201610220104402205"
                && m.SettleReceipType != "201610220104402206"
                ).Max(m => m.Number);
                if (string.IsNullOrEmpty(data))
                {
                    return new Number() { MaxNumber = "0" };
                }
                else
                {
                    return new Number() { MaxNumber = data };
                }
            }
        }
        private FormattableString GetDataSql(long manyQuery)
        {
            return @$"SELECT 
                         0 as SummaryIsEnd,
                         0 as AccIsEnd,
                         0 as MarIsEnd,
                         CONVERT(A.NumericalOrder USING utf8mb4) NumericalOrder,	
                         CONVERT(A.SettleReceipType USING utf8mb4) AS SettleReceipType,
                         CONVERT(A.TicketedPointID USING utf8mb4) AS TicketedPointID,
                         A.Number,	
                         CONCAT(TicketedPointName,'-',A.Number) TicketedPointNumber,	
                         CONVERT(A.EnterpriseID USING utf8mb4) EnterpriseID,                                   
                         DATE_FORMAT(A.DataDate,'%Y-%m-%d') DataDate,
                         IFNULL(A.Guid,UUID()) Guid,
                         Concat(A.SettleReceipType) PayReceType,
                         A.Remarks,    
                         A.AccountNo,
                         '0' line,
                         A.AttachmentNum,                                   
                         CONVERT(A.OwnerID USING utf8mb4) AS OwnerID,
                         CONVERT(A.CreatedDate USING utf8mb4) AS CreatedDate,
                         CONVERT(A.ModifiedDate USING utf8mb4) AS ModifiedDate,
                         d.DataDictName SettleReceipTypeName,
                         TicketedPointName,
                         bs.Remarks AS UploadInfo,
                         EnterpriseName,
                         hr.Name OwnerName,
                         hr2.Name AuditName,
                         SUM(B.Debit) Amount,
                         '' ReceiptAbstractID,
                         '' ReceiptAbstractName,
                         IF(pay.NumericalOrder is null,'',(
                         case pay.SettleReceipType 
                         when 201611180104402204 THEN CONCAT('付款汇总单-',pay.Number) 
                         when 201611180104402202 THEN CONCAT('付款单-',pay.Number) 
                         when 201611180104402203 THEN CONCAT('收款汇总单-',pay.Number) 
                         ELSE CONCAT('收款单-',pay.Number) END)) ApplyNumber,
                         CONCAT(pay.NumericalOrder) ApplyNumericalOrder,
                         Concat(pay.SettleReceipType) ApplySettleReceipType,
                         IFNULL(CONCAT('采购发票-',invo.Number),'') InvocieNumber,
                         IFNULL(CONCAT(invo.NumericalOrder),'') InvocieNumericalOrder,
                         pay.IsGroupPay
                         FROM  nxin_qlw_business.fd_settlereceipt A 
                         INNER JOIN nxin_qlw_business.fd_settlereceiptdetail B ON A.NumericalOrder = B.NumericalOrder
                         LEFT JOIN  nxin_qlw_business.biz_datadict d ON d.DataDictID=A.SettleReceipType 
                         LEFT JOIN  nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID=A.TicketedPointID 
                         INNER JOIN  qlw_nxin_com.biz_enterprise  ent ON A.EnterpriseID=ent.EnterpriseID 
                         LEFT JOIN `qlw_nxin_com`.`bsfile` bs ON bs.`NumericalOrder` = A.`NumericalOrder`
                         LEFT JOIN nxin_qlw_business.hr_person hr on hr.bo_id = A.OwnerId
                         LEFT JOIN `nxin_qlw_business`.`biz_reviwe` audit on audit.NumericalOrder = A.NumericalOrder and audit.Level = 2  AND audit.ReviweType = 1611091727140000101
                         LEFT JOIN nxin_qlw_business.hr_person hr2 on hr2.bo_id = audit.CheckedByID
                         left join nxin_qlw_business.fd_paymentreceivables pay on pay.numericalorder = A.numericalorder
                         LEFT JOIN nxin_qlw_business.biz_related pv on pv.ChildValue = A.NumericalOrder and pv.ChildType = 1611091727140000101 and ParentType = 1803091529030000101
						 LEFT JOIN nxin_qlw_business.pm_invoice invo on invo.NumericalOrder = pv.ParentValue
                         WHERE a.NumericalOrder = {manyQuery}
                         GROUP BY A.NumericalOrder
                         ";
        }
        public IQueryable<VoucherHandleInfoEntity> GetVoucherHandleInfoEntities(string entes)
        {
            string sql = $@"
            SELECT CONCAT(A.`NumericalOrder`) NumericalOrder,CONCAT(en.`EnterpriseID`) EnterpriseID,en.`EnterpriseName`,CONCAT(A.`DataDate`) DataDate,CONCAT(P.`SettleReceipType`) SettleReceipType, 
            CONCAT(d.`DataDictName`,'单') SettleReceipTypeName,P.`Number` Number,CONCAT(A.TicketedPointID) TicketedPointID,CONCAT(TicketedPointName,'-',P.Number) TicketedPointNumber,
            CONCAT(PD.`BusinessType`) BusinessType,BD.`DataDictName` BusinessTypeName,
            IFNULL(IFNULL(IFNULL(IFNULL(e2.`EnterpriseName`,e3.`Name`),e4.`CustomerName`),e5.`SupplierName`),e6.`MarketName`) AS CollectionName,
            CONCAT(p3.CollectionId) AS CollectionId,PD.Amount
            FROM `nxin_qlw_business`.`fd_paymentreceivablesvoucher` A
            LEFT JOIN `nxin_qlw_business`.`biz_related` br ON br.`ChildType` = 1611091727140000101 AND br.`ParentType` = 2303061011170000150 AND br.ParentValue = A.`NumericalOrder`
            LEFT JOIN `nxin_qlw_business`.`fd_settlereceipt` S ON S.`NumericalOrder` = A.`NumericalOrder`
            
            LEFT JOIN  nxin_qlw_business.biz_ticketedpoint t ON t.TicketedPointID=A.TicketedPointID 
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.`EnterpriseID` = A.`EnterpriseID`  AND en.`PID` = {_identityservice.GroupId}
            INNER JOIN (
	            SELECT DD.BusinessType,A.`NumericalOrder`,SUM(DD.`Amount`) + SUM(DD.Charges) Amount FROM `nxin_qlw_business`.`fd_paymentreceivablesdetail` DD
	            INNER JOIN nxin_qlw_business.`fd_paymentreceivables` A ON A.`NumericalOrder` = DD.`NumericalOrder`
	            WHERE A.EnterpriseID IN({entes}) AND A.`SettleReceipType` IN (201611180104402201,201611180104402202) 
	            GROUP BY A.`NumericalOrder`
            ) PD ON PD.`NumericalOrder` = A.`NumericalOrder`
            INNER JOIN `nxin_qlw_business`.`fd_paymentreceivables` P ON P.`NumericalOrder` = PD.`NumericalOrder`
            LEFT JOIN  nxin_qlw_business.biz_datadict d ON d.DataDictID=P.SettleReceipType 
            INNER JOIN `nxin_qlw_business`.`fd_paymentextend` P3 ON P3.`NumericalOrder` = P.`NumericalOrder`
            LEFT JOIN `qlw_nxin_com`.`biz_enterprise` e2 ON e2.`EnterpriseID` = p3.`CollectionId` -- 单位
            LEFT JOIN `nxin_qlw_business`.`hr_person` e3 ON e3.`PersonID` = p3.`CollectionId` -- 员工
            LEFT JOIN `nxin_qlw_business`.`sa_customer` e4 ON e4.`CustomerID` = p3.`CollectionId` -- 客户
            LEFT JOIN `nxin_qlw_business`.`pm_supplier` e5 ON e5.`SupplierID` = p3.`CollectionId` -- 供应商
            LEFT JOIN `nxin_qlw_business`.`biz_market` e6 ON e6.`MarketID` = p3.`CollectionId` -- 部门
            LEFT JOIN `nxin_qlw_business`.`biz_datadict` BD ON BD.`DataDictID` = PD.`BusinessType`
            LEFT JOIN `nxin_qlw_business`.`fd_paymentextend` PDE ON PDE.`NumericalOrder` = A.`NumericalOrder`
            WHERE A.EnterpriseID IN({entes}) AND S.`NumericalOrder` IS NULL AND br.`ParentValue` IS NULL  AND P.`SettleReceipType` IN (201611180104402201,201611180104402202)
            GROUP BY A.`NumericalOrder`
            ";
            return _context.VoucherHandleInfoEntityDataSet.FromSqlRaw(sql);
        }
        public override Task<List<FD_SettleReceiptDetailEntity>> GetDetaiDatasAsync(long manyQuery)
        {
            string sql = GetDetailSql(manyQuery);
            return _context.FD_SettleReceiptDetailNewDataSet.FromSqlRaw(sql).ToListAsync();
        }
        /// <summary>
        /// 根据名称获取当前集团下所有单位信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<EnterpriseInfo> GetEnterpriseInfosByName()
        {
            return _context.EnterpriseInfoDataSet.FromSqlRaw(
                @$"SELECT CONCAT(EnterpriseId) EnterpriseId,CONCAT(EnterpriseName) EnterpriseName FROM qlw_nxin_com.biz_enterprise
                WHERE PID = {_identityservice.GroupId}").ToList();
        }
        /// <summary>
        /// 根据集团ID 获取全部单位单据字信息
        /// </summary>
        /// <returns></returns>
        public List<TicketPointInfo> GetTicketPointInfos(string groupId = "")
        {
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = _identityservice.GroupId;
            }
            return _context.TicketPointInfoDataSet.FromSqlRaw($@"SELECT CONCAT(TicketedPointID) TicketedPointId,TicketedPointName,CONCAT(ticket.EnterpriseId) EnterpriseId FROM `nxin_qlw_business`.`biz_ticketedpoint` ticket
            INNER JOIN qlw_nxin_com.`biz_enterprise` ent ON ent.enterpriseid = ticket.enterpriseid AND ent.pid = {groupId} ").ToList();
        }
        /// <summary>
        /// 根据集团ID 获取全部单位摘要信息
        /// 如果想包含集团科目数据  就把第二个sql的单位表去掉 或者再加一个or C.EnterpriseId=957025251000000
        /// NOW() BETWEEN B.`dBegin` AND B.`dEnd`
        /// 版本日期控制
        /// 如果想指定日期 就改 now()
        /// </summary>
        /// <returns></returns>
        public List<SettleSummaryInfo> GetSettleSummaryInfos(string groupId = "")
        {
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = _identityservice.GroupId;
            }
            return _context.SettleSummaryInfoDataSet.FromSqlRaw($@"SELECT CONCAT(A.SettleSummaryID) SettleSummaryId,CONCAT(A.EnterpriseID) EnterpriseId,A.SettleSummaryName, A.Remarks,
            IF(LEFT(A.`SettleSummaryCode`,2)='01','201610220104402201',IF(LEFT(A.`SettleSummaryCode`,2)='02','201610220104402202','201610220104402203')) SettleSummaryType
            FROM qlw_nxin_com.`biz_settlesummary` A 
            INNER JOIN qlw_nxin_com.`biz_versionsetting` B ON A.`VersionID`=B.`VersionID`
            INNER JOIN qlw_nxin_com.`biz_enterprise` C ON A.`EnterpriseID`=C.`EnterpriseID`
            WHERE C.`PID`={groupId} AND  B.`iVersionType`=1712221411210000101 AND B.`EnterpriseID`={groupId} AND NOW() BETWEEN B.`dBegin` AND B.`dEnd`").ToList();
        }
        /// <summary>
        /// 根据集团ID 获取全部单位科目信息
        /// 如果想包含集团科目数据  就把第二个sql的单位表去掉 或者再加一个or C.EnterpriseId=957025251000000
        /// NOW() BETWEEN B.`dBegin` AND B.`dEnd`
        /// 版本日期控制
        /// 如果想指定日期 就改 now()
        /// </summary>
        /// <returns></returns>
        public List<AccoSubjectInfo> GetAccoSubjectInfos(string Date = "",EnterpriseInfo model = null,string groupId = "")
        {
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = _identityservice.GroupId;
            }
            if (model == null)
            {
                return new List<AccoSubjectInfo>();
            }
            _logger.LogInformation("会计凭证导入科目SQL："+ $@"
              SELECT CONCAT(A.AccoSubjectID) AccoSubjectId,CONCAT(A.AccoSubjectCode) AccoSubjectCode,A.AccoSubjectFullName AccoSubjectName,CONCAT(A.EnterpriseID) EnterpriseId,
            `IsTorF`,`IsLorR`,`IsProject`,`IsCus`,`IsPerson`,`IsSup`,`IsDept`,`IsItem`,`IsCash`,`IsBank` FROM qlw_nxin_com.`biz_accosubject` A INNER JOIN qlw_nxin_com.`biz_versionsetting` B ON A.`VersionID`=B.`VersionID`
            INNER JOIN qlw_nxin_com.`biz_enterprise` C ON A.`EnterpriseID`=C.`EnterpriseID`
            WHERE (C.`EnterpriseID`={groupId} OR C.`EnterpriseID`={model.EnterpriseId}) AND  B.`iVersionType`=1712221411430000101 AND B.`EnterpriseID`={groupId} AND '{Date}' BETWEEN B.`dBegin` AND B.`dEnd`AND A.IsEnd = TRUE  AND A.IsUse = TRUE 
            ");
            return _context.AccoSubjectInfoDataSet.FromSqlRaw($@"
              SELECT CONCAT(A.AccoSubjectID) AccoSubjectId,CONCAT(A.AccoSubjectCode) AccoSubjectCode,A.AccoSubjectFullName AccoSubjectName,CONCAT(A.EnterpriseID) EnterpriseId,
            `IsTorF`,`IsLorR`,`IsProject`,`IsCus`,`IsPerson`,`IsSup`,`IsDept`,`IsItem`,`IsCash`,`IsBank` FROM qlw_nxin_com.`biz_accosubject` A INNER JOIN qlw_nxin_com.`biz_versionsetting` B ON A.`VersionID`=B.`VersionID`
            INNER JOIN qlw_nxin_com.`biz_enterprise` C ON A.`EnterpriseID`=C.`EnterpriseID`
            WHERE (C.`EnterpriseID`={groupId} OR C.`EnterpriseID`={model.EnterpriseId}) AND  B.`iVersionType`=1712221411430000101 AND B.`EnterpriseID`={groupId} AND '{Date}' BETWEEN B.`dBegin` AND B.`dEnd`AND A.IsEnd = TRUE  AND A.IsUse = TRUE 
            ").ToList();
        }
        /// <summary>
        /// 根据集团ID 获取全部单位项目信息
        /// </summary>
        /// <returns></returns>
        public List<ProjectInfo> GetProjectInfos()
        {
            return _context.ProjectInfoDataSet.FromSqlRaw($@"SELECT CONCAT(pro.`ProjectID`) ProjectID,pro.`ProjectName`,CONCAT(pro.`EnterpriseID`) EnterpriseID FROM `qlw_nxin_com`.`ppm_project` pro
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON ent.enterpriseid = pro.enterpriseid AND ent.pid = {_identityservice.GroupId}").ToList();
        }
        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <returns></returns>
        public List<MarketInfo> GetMarketInfos(string Date = "")
        {
            if (string.IsNullOrEmpty(Date))
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            return _context.MarketInfoDataSet.FromSqlRaw($@"SELECT CONCAT(MarketId) MarketId,cFullName MarketName,CONCAT(bm.EnterpriseId) EnterpriseId FROM `qlw_nxin_com`.`biz_market` bm
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` ent ON ent.enterpriseid = bm.enterpriseid AND ent.pid = {_identityservice.GroupId}
            INNER JOIN qlw_nxin_com.`biz_versionsetting` B ON bm.`VersionID`=B.`VersionID`
            WHERE '{Date}' BETWEEN B.`dBegin` AND B.`dEnd`AND bm.IsUse = TRUE").ToList();
        }
        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <returns></returns>
        public List<PersonInfo> GetPersonInfos()
        {
            return _context.PersonInfoDataSet.FromSqlRaw($@"SELECT DISTINCT CONCAT(hp.PersonID) PersonID,hp.Name AS PersonName,CONCAT(t.EnterpriseId) EnterpriseId FROM `nxin_qlw_business`.hr_person hp
	        INNER JOIN qlw_nxin_com.hr_persondetail hp1 ON hp.PersonID = hp1.PersonID
	        INNER JOIN qlw_nxin_com.st_users d ON d.bo_id=hp.bo_id
	        INNER JOIN qlw_nxin_com.st_usersdetail t ON t.UserId=d.UserId 
	        INNER JOIN qlw_nxin_com.biz_enterprise be ON t.EnterpriseID = be.EnterpriseID
            WHERE be.PID={_identityservice.GroupId} AND hp1.PersonType=201610220104402102").ToList();
        }
        /// <summary>
        /// 根据单位id 获取集团id
        /// </summary>
        /// <param name="enteId"></param>
        /// <returns></returns>
        public Biz_EnterpriseInfo GetBiz_EnterpriseInfos(string enteId)
        {
            return _context.Biz_EnterpriseInfoDataSet.FromSqlRaw($@"SELECT CONCAT(Pid) Pid,CONCAT(EnterpriseId) EnterpriseId,EnterpriseName FROM qlw_nxin_com.biz_enterprise where enterpriseid = {enteId} ").FirstOrDefault();
        }
        /// <summary>
        /// 获取 客户+供应商 信息
        /// </summary>
        /// <returns></returns>
        public List<CusSupInfo> GetCusSupInfos()
        {
            return _context.CusSupInfoDataSet.FromSqlRaw($@"SELECT CONCAT(pm.SupplierID) AS Id,SupplierName AS NAME,CONCAT(pm.enterpriseid) AS EnterpriseId FROM `nxin_qlw_business`.`pm_supplier` pm
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.enterpriseid = pm.enterpriseid AND en.pid = {_identityservice.GroupId}
            UNION
            SELECT CONCAT(cu.CustomerID) AS Id,cu.CustomerName AS NAME,CONCAT(cu.EnterpriseId) AS EnterpriseId FROM `qlw_nxin_com`.`biz_customer` cu
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.enterpriseid = cu.enterpriseid AND en.pid = {_identityservice.GroupId}").ToList();
        }
        /// <summary>
        /// 获取商品代号
        /// </summary>
        /// <returns></returns>
        public List<ProductsInfo> GetProductInfos()
        {
            return _context.ProductInfoDataSet.FromSqlRaw($@"SELECT CONCAT(bp.ProductId) ProductId,CONCAT(bp.EnterpriseId) EnterpriseId,CONCAT(bp.ProductName) ProductName FROM `nxin_qlw_business`.`biz_productdetail` bp
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.enterpriseid = bp.enterpriseid AND en.pid = {_identityservice.GroupId}").ToList();
        }
        /// <summary>
        /// 获取商品名称
        /// </summary>
        /// <returns></returns>
        public List<ProductsInfo> GetProductGroupInfos()
        {
            return _context.ProductInfoDataSet.FromSqlRaw($@"SELECT CONCAT(bp.ProductGroupId) ProductId,CONCAT(bp.EnterpriseId) EnterpriseId,CONCAT(bp.ProductGroupName) ProductName FROM `qlw_nxin_com`.`biz_productgroup` bp
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.enterpriseid = bp.enterpriseid AND en.pid = {_identityservice.GroupId}").ToList();
        }
        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <returns></returns>
        public List<ClassificationInfo> GetClassificationInfos()
        {
            return _context.ClassificationInfoDataSet.FromSqlRaw($@"SELECT CONCAT(ClassificationID) AS ClassificationID,bp.ClassificationName,CONCAT(bp.EnterpriseId) AS EnterpriseId FROM `qlw_nxin_com`.`biz_productclassification` bp
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.enterpriseid = bp.enterpriseid AND en.pid = {_identityservice.GroupId}").ToList();
        }
        /// <summary>
        /// 获取资金账户信息
        /// </summary>
        /// <returns></returns>
        public List<AccountsInfo> GetAccountInfos()
        {
            return _context.AccountInfoDataSet.FromSqlRaw($@"SELECT CONCAT(AccountID) AccountId,AccountName,CONCAT(bp.EnterpriseId) AS EnterpriseId  FROM `nxin_qlw_business`.`fd_account` bp
            INNER JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.enterpriseid = bp.enterpriseid AND en.pid = {_identityservice.GroupId}").ToList();
        }
        /// <summary>
        /// 获取结算方式
        /// </summary>
        /// <returns></returns>
        public List<PayTypeInfo> GetPayTypeInfos()
        {
            return _context.PayTypeInfoDataSet.FromSqlRaw($@"SELECT DataDictName PayTypeName,CONCAT(DataDictId) PayTypeId FROM `nxin_qlw_business`.`biz_datadict` WHERE pid = 201610140104402001").ToList();
        }
        public List<OptionInfo> GetOptionInfos(string optionId,string groupId)
        {

            //默认为 （系统选项ID20180408150746，开启，审核与制单允许相同，关闭，审核与制单不允许相同）
            string scopeCode = "4";
            if (string.IsNullOrEmpty(optionId))
            {
                optionId = "20180408150746";
            }
            if (!string.IsNullOrEmpty(groupId) && groupId == _identityservice.GroupId)
            {
                //集团ID 变 单位ID  房接口只认可 单位ID 
                groupId = _identityservice.EnterpriseId;
                //scopeCode	String	GET查询	是		选项控制范围（必填） 1系统 2集团 4单位 8个人
                scopeCode = "2";
            }
            var result = _baseUnit.OptionConfigValueNew(optionId, groupId,scopeCode);
            if (result == 1)
            {
                return new List<OptionInfo>() { new OptionInfo() {EnterpriseId = groupId,OptionSwitch = true } };
            }
            else
            {
                return new List<OptionInfo>() { new OptionInfo() { EnterpriseId = groupId, OptionSwitch = false } };
            }
        }
        private string GetDetailSql(long manyQuery)
        {
            return @$"  SELECT 
                          IFNULL(s.IsEnd,sg.IsEnd) SummaryIsEnd,
                          a.IsEnd AccIsEnd,
                          m.IsEnd MarIsEnd,
                          FD.RecordID,
                          CONVERT(FD.NumericalOrder USING utf8mb4) NumericalOrder,
                          FD.Guid,
                          CONVERT(FD.EnterpriseID USING utf8mb4) EnterpriseID,                                     
                          CONVERT(FD.ReceiptAbstractID USING utf8mb4) ReceiptAbstractID,
                          CONVERT(FD.AccoSubjectID USING utf8mb4) AccoSubjectID,
                          CONVERT(FD.AccoSubjectCode USING utf8mb4) AccoSubjectCode,
                          CONVERT(FD.CustomerID USING utf8mb4) CustomerID,
                          CONVERT(FD.PersonID USING utf8mb4) PersonID,                                   
                          CONVERT(FD.MarketID USING utf8mb4) MarketID,                                    
                          CONVERT(FD.ProjectID USING utf8mb4) ProjectID,
                          CONVERT(FD.ProductID USING utf8mb4) ProductID,
                          CONVERT(FD.PaymentTypeID USING utf8mb4) PaymentTypeID,                                   
                          CONVERT(FD.AccountID USING utf8mb4) AccountID,
                          FD.LorR,
                          IFNULL(FD.Debit,0.0) Debit,
                          IFNULL(FD.Credit,0.0) Credit,
                          FD.Content,
                          FD.AgingDate,
                          FD.RowNum,
                          CONVERT(FD.OrganizationSortID USING utf8mb4) OrganizationSortID,
                          FD.IsCharges,
                          ent.EnterpriseName, 
                          IFNULL(s.SettleSummaryName,sg.`SettleSummaryGroupName`) ReceiptAbstractName,
                          sd.FinancialStatementName,
                          a.AccoSubjectName,a.AccoSubjectFullName,a.IsProject bProject,a.IsTorF,a.IsCus bCus,a.IsPerson bPerson,a.IsSup bSup,a.IsDept bDept,a.Auxiliary,
                          IFNULL(bc.CustomerName,sp.SupplierName) CustomerName,
                          IF(bc.CustomerName = NULL ,TRUE,FALSE) IsCus,
                          h.Name PersonName,
                          m.MarketName,m.cFullName MarketFullName,
                          p.ProjectName,
                          bp.ProductName,
                          bd.cDictName PaymentTypeName,
                          fa.AccountName,fa.DepositBank,
                          bo.SortName OrganizationSortName,
                          CONCAT(FD.ProductGroupID) ProductGroupID,
                          bpg.ProductGroupName,
                          CONCAT(FD.ClassificationID) ClassificationID,
                          bpf.ClassificationName,
                          Concat(FD.Auxiliary1) Auxiliary1,
                          aux1.ProjectName AuxiliaryName1,
                          Concat(FD.Auxiliary2) Auxiliary2,
                          aux2.ProjectName AuxiliaryName2,
                          Concat(FD.Auxiliary3) Auxiliary3,
                          aux3.ProjectName AuxiliaryName3,
                          Concat(FD.Auxiliary4) Auxiliary4,
                          aux4.ProjectName AuxiliaryName4,
                          Concat(FD.Auxiliary5) Auxiliary5,
                          aux5.ProjectName AuxiliaryName5,
                          Concat(FD.Auxiliary6) Auxiliary6,
                          aux6.ProjectName AuxiliaryName6,
                          Concat(FD.Auxiliary7) Auxiliary7,
                          aux7.ProjectName AuxiliaryName7,
                          Concat(FD.Auxiliary8) Auxiliary8,
                          aux8.ProjectName AuxiliaryName8,
                          Concat(FD.Auxiliary9) Auxiliary9,
                          aux9.ProjectName AuxiliaryName9,
                          Concat(FD.Auxiliary10) Auxiliary10,
                          aux10.ProjectName AuxiliaryName10 
                          FROM  nxin_qlw_business.fd_settlereceiptdetail FD 
                          LEFT JOIN  qlw_nxin_com.biz_enterprise  ent ON FD.EnterpriseID=ent.EnterpriseID
                          LEFT JOIN  qlw_nxin_com.biz_settlesummary s ON FD.ReceiptAbstractID=s.SettleSummaryID
                          LEFT JOIN qlw_nxin_com.`biz_settlesummarygroup` sg ON sg.`SettleSummaryGroupID` = FD.`ReceiptAbstractID`
                          LEFT JOIN  qlw_nxin_com.biz_financialstatement sd ON sd.FinancialStatementID=s.FinancialStatementID
                          LEFT JOIN  qlw_nxin_com.biz_accosubject a ON FD.AccoSubjectID=a.AccoSubjectID
                          LEFT JOIN nxin_qlw_business.PM_Supplier sp ON sp.SupplierID = fd.CustomerID
                          LEFT JOIN  qlw_nxin_com.biz_customer bc ON bc.CustomerID=fd.CustomerID
                          LEFT JOIN  qlw_nxin_com.hr_person h ON h.PersonID=fd.PersonID
                          LEFT JOIN qlw_nxin_com.biz_market m ON m.MarketID=fd.MarketID
                          LEFT JOIN qlw_nxin_com.ppm_project p ON p.ProjectID=fd.ProjectID
                          LEFT JOIN qlw_nxin_com.biz_product bp ON bp.ProductID=fd.ProductID
                          LEFT JOIN qlw_nxin_com.bsdatadict BD ON BD.DictID=FD.PaymentTypeID 
                          LEFT JOIN nxin_qlw_business.fd_account fa ON fa.AccountID=FD.AccountID
                          LEFT JOIN qlw_nxin_com.bsorganizationsort bo ON bo.SortId=fd.OrganizationSortID
                          LEFT JOIN qlw_nxin_com.`biz_productgroup` bpg on bpg.ProductGroupID = FD.ProductGroupID
                          LEFT JOIN qlw_nxin_com.`biz_productgroupclassification` bpf on bpf.ClassificationID = FD.ClassificationID
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux1 on aux1.ProjectId = FD.Auxiliary1
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux2 on aux2.ProjectId = FD.Auxiliary2
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux3 on aux3.ProjectId = FD.Auxiliary3
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux4 on aux4.ProjectId = FD.Auxiliary4
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux5 on aux5.ProjectId = FD.Auxiliary5
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux6 on aux6.ProjectId = FD.Auxiliary6
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux7 on aux7.ProjectId = FD.Auxiliary7
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux8 on aux8.ProjectId = FD.Auxiliary8
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux9 on aux9.ProjectId = FD.Auxiliary9
                          LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux10 on aux10.ProjectId = FD.Auxiliary10
                          WHERE FD.NumericalOrder =  {manyQuery}
                          GROUP BY fd.recordid  
                          ORDER BY FD.RowNum ,fd.Recordid ";
        }
        public decimal GetKMAmount(SearchKM search)
        {
            string sql = $@"
SELECT 
                                IFNULL(km.IsLorR,True) IsLorR,
                                IFNULL(SUM(CASE WHEN a.SettleReceipType=201610220104402206 AND a.DataDate = qcdate.DataDate AND a.EnterpriseID=qcdate.EnterpriseID THEN Debit ELSE 0 END),0) qcDebit,
                                IFNULL(SUM(CASE WHEN a.SettleReceipType=201610220104402206 AND a.DataDate = qcdate.DataDate AND a.EnterpriseID=qcdate.EnterpriseID THEN Credit ELSE 0 END),0) qcCredit,
                                IFNULL(SUM(CASE WHEN a.DataDate>='2023-01-01' AND a.SettleReceipType NOT IN (201610220104402204,201610220104402205,201610220104402206) THEN Debit ELSE 0 END),0) Debit,
                                IFNULL(SUM(CASE WHEN a.DataDate>='2023-01-01' AND a.SettleReceipType NOT IN (201610220104402204,201610220104402205,201610220104402206) THEN Credit ELSE 0 END),0)  Credit,
                                IFNULL(SUM(CASE WHEN a.DataDate<'2023-01-01' AND a.DataDate >= IFNULL(qcdate.DataDate,'') AND a.EnterpriseID=IFNULL(qcdate.EnterpriseID,a.EnterpriseID) AND a.SettleReceipType NOT IN (201610220104402204,201610220104402205,201610220104402206) THEN Debit ELSE 0 END),0) fsDebit,
                                IFNULL(SUM(CASE WHEN a.DataDate<'2023-01-01' AND a.DataDate >= IFNULL(qcdate.DataDate,'') AND a.EnterpriseID=IFNULL(qcdate.EnterpriseID,a.EnterpriseID) AND a.SettleReceipType NOT IN (201610220104402204,201610220104402205,201610220104402206) THEN Credit ELSE 0 END),0) fsCredit
                                FROM NXin_Qlw_Business.FD_SettleReceipt a 
INNER JOIN NXin_Qlw_Business.FD_SettleReceiptDetail b ON a.NumericalOrder=b.NumericalOrder 
INNER JOIN qlw_nxin_com.BIZ_AccoSubject km ON b.AccoSubjectID=km.AccoSubjectID  
 LEFT JOIN qlw_nxin_com.BIZ_Enterprise be ON a.EnterPriseID=be.EnterpriseID 
 LEFT JOIN (SELECT EnterpriseID,MAX(DataDate) AS DataDate FROM NXin_Qlw_Business.FD_SettleReceipt
                                        WHERE SettleReceipType = 201610220104402206 AND DataDate <= '{search.BeginDate}'
                                        AND EnterpriseID IN ({search.EnterpriseId}) GROUP BY EnterpriseID) qcdate ON a.EnterPriseID=qcdate.EnterpriseID 
  WHERE a.DataDate <= '{search.EndDate}' AND a.EnterpriseID IN ({search.EnterpriseId})  AND km.AccoSubjectType='{search.accType}' 
 AND ('{search.accCode}' <= km.AccoSubjectCode AND km.AccoSubjectCode < '{search.accCode}' OR km.AccoSubjectCode LIKE CONCAT('{search.accCode}','%') )  ";
            var data = _context.GetKMData.FromSqlRaw(sql).FirstOrDefault();
            if (data == null)
            {
                return 0;
            }
            //负债类算法 isLor = true   : fsCredit - fsDebit + qcCredit - qcDebit + Credit - Debit = 3200 - 0 + 3000 - 0 = 6200     贷方
            if (data.IsLorR == true)
            {
                return Convert.ToDecimal(data.fsDebit - data.fsCredit + data.qcDebit - data.qcCredit + data.Debit - data.Credit);
            }
            //资产类算法 isLor = false  : fsDebit - fsCredit + qcDebit - qcCredit + Debit - Credit = 965893.99 - 0 + 214968.50 - 1110930.92   借方
            else
            {
                return Convert.ToDecimal(data.fsCredit - data.fsDebit + data.qcCredit - data.qcDebit + data.Credit - data.Debit);
            }
        }

        /// <summary>
        /// 根据单位ID 获取单据字信息
        /// </summary>
        /// <returns></returns>
        public List<TicketPointInfo> GetTicketPointByEnteId(string enteId = "")
        {
            if (string.IsNullOrEmpty(enteId))
            {
                enteId = _identityservice.EnterpriseId;
            }
            return _context.TicketPointInfoDataSet.FromSqlRaw($@"SELECT CONCAT(TicketedPointID) TicketedPointId,TicketedPointName,CONCAT(ticket.EnterpriseId) EnterpriseId FROM nxin_qlw_business.biz_ticketedpoint ticket
            where ticket.EnterpriseId={enteId} ").ToList();
        }
        /// <summary>        
        /// 摘要
        /// </summary>
        /// <returns></returns>
        public List<SettleSummaryInfo> GetSettleSummaryByDate(string groupId = "", string enteId = "",string date="")
        {
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = _identityservice.GroupId;
            }
            if (string.IsNullOrEmpty(enteId))
            {
                enteId = _identityservice.EnterpriseId;
            }
            if (string.IsNullOrEmpty(date))
            {
                date = DateTime.Now.ToString();
            }
            var sql = $@"SELECT CONCAT(A.SettleSummaryID) SettleSummaryId,CONCAT(A.EnterpriseID) EnterpriseId,A.SettleSummaryName, A.Remarks,
            IF(LEFT(A.`SettleSummaryCode`,2)='01','201610220104402201',IF(LEFT(A.`SettleSummaryCode`,2)='02','201610220104402202','201610220104402203')) SettleSummaryType
            FROM qlw_nxin_com.`biz_settlesummary` A 
            INNER JOIN qlw_nxin_com.`biz_versionsetting` B ON A.`VersionID`=B.`VersionID`
            WHERE A.EnterpriseID in({groupId},{enteId}) AND  B.`iVersionType`=1712221411210000101 AND B.`EnterpriseID`={groupId} AND '{date}' BETWEEN B.`dBegin` AND B.`dEnd`";
            return _context.SettleSummaryInfoDataSet.FromSqlRaw(sql).ToList();
        }
        public List<ExportVoucher> GetVouchersExport(VoucherParam model)
        {
            if (string.IsNullOrEmpty(model.NumericalOrder))
            {
                if (string.IsNullOrEmpty(model.EnterpriseIds))
                {
                    model.EnterpriseIds = "0";
                }
                if (string.IsNullOrEmpty(model.BeginDate))
                {
                    model.BeginDate = DateTime.Now.ToString("yyyy-MM-01");
                }
                if (string.IsNullOrEmpty(model.EndDate))
                {
                    model.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                string sql = $@"SELECT vd.`RecordID`,en.`EnterpriseID`,en.`EnterpriseName`,DATE_FORMAT(v.`DataDate`, '%Y-%m-%d') DataDate,st.`DataDictName` AS SettleReceipTypeName,CONCAT(tick.`TicketedPointName`,v.`Number`) TicketPointNumber,CONCAT(IFNULL(s.`SettleSummaryName`,sg.`SettleSummaryGroupName`)) ReceiptAbstractName,vd.`Content`,
                acc.`AccoSubjectFullName`,hr.`Name`,market.`cFullName` MarketName,IFNULL(cus.`CustomerName`,sup.`SupplierName`) CustomerName,proj.`ProjectName`,bp.`ProductName`,bgp.`ProductGroupName`,
                bpf.`ClassificationName`,(vd.`Debit`) Debit,(vd.`Credit`) Credit,account.`AccountFullName`,bd.cDictName PaymentTypeName,IFNULL(bo.cFullName,envd.`EnterpriseName`) SortName,v.`AttachmentNum`,IFNULL(sd.FinancialStatementName,sdg.FinancialStatementName) FinancialStatementName,v.`Remarks`,ohr.`Name` OwnerName,hr2.Name AuditName
                 FROM `nxin_qlw_business`.`fd_settlereceipt` v
                INNER JOIN `nxin_qlw_business`.`fd_settlereceiptdetail` vd ON v.`NumericalOrder` = vd.`NumericalOrder`
                LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON tick.`TicketedPointID` = v.`TicketedPointID`
                LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` s ON s.`SettleSummaryID` = vd.`ReceiptAbstractID`
                LEFT JOIN  qlw_nxin_com.biz_financialstatement sd ON sd.FinancialStatementID=s.FinancialStatementID
                LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` sg ON sg.`SettleSummaryGroupID` = vd.`ReceiptAbstractID`
                LEFT JOIN  qlw_nxin_com.biz_financialstatement sdg ON sdg.FinancialStatementID=sg.FinancialStatementID
                LEFT JOIN `qlw_nxin_com`.`biz_accosubject` acc ON acc.`AccoSubjectID` = vd.`AccoSubjectID`
                LEFT JOIN `qlw_nxin_com`.`hr_person` hr ON hr.`PersonID` = vd.`PersonID`
                LEFT JOIN `qlw_nxin_com`.`biz_market` market ON market.`MarketID` = vd.`MarketID`
                LEFT JOIN `qlw_nxin_com`.`biz_customer` cus ON cus.`CustomerID` = vd.`CustomerID`
                LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON sup.`SupplierID` = vd.`CustomerID`
                LEFT JOIN `qlw_nxin_com`.ppm_project proj ON proj.`ProjectID` = vd.`ProjectID`
                LEFT JOIN `qlw_nxin_com`.`biz_product` bp ON bp.`ProductID` = vd.`ProductID`
                LEFT JOIN `qlw_nxin_com`.`biz_productgroup` bgp ON bgp.`ProductGroupID` = vd.`ProductGroupID`
                LEFT JOIN qlw_nxin_com.`biz_productgroupclassification` bpf ON bpf.ClassificationID = vd.ClassificationID
                LEFT JOIN `nxin_qlw_business`.`fd_account` account ON account.`AccountID` = vd.`AccountID`
                LEFT JOIN qlw_nxin_com.bsdatadict BD ON BD.DictID=vd.PaymentTypeID 
                LEFT JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.`EnterpriseID` = v.`EnterpriseID`
                LEFT JOIN `qlw_nxin_com`.`biz_enterprise` envd ON envd.`EnterpriseID` = vd.`EnterpriseID`
                LEFT JOIN `nxin_qlw_business`.`hr_person` ohr ON ohr.`BO_ID` = v.`OwnerID`
                LEFT JOIN `nxin_qlw_business`.`biz_reviwe` audit ON audit.NumericalOrder = v.NumericalOrder AND audit.Level = 2  AND audit.ReviweType = 1611091727140000101
                LEFT JOIN nxin_qlw_business.hr_person hr2 ON hr2.bo_id = audit.CheckedByID
                LEFT JOIN `nxin_qlw_business`.`biz_datadict` st ON st.`DataDictID` = v.`SettleReceipType`
                LEFT JOIN qlw_nxin_com.bsorganizationsort bo ON bo.SortId=vd.OrganizationSortID
                WHERE v.`EnterpriseID` IN ({model.EnterpriseIds}) AND v.`DataDate` BETWEEN '{model.BeginDate}' AND '{model.EndDate}' 
                {(!string.IsNullOrEmpty(model.Number) ? $" AND v.Number Like '%{model.Number}%' " : " ")}
                {(!string.IsNullOrEmpty(model.SettleReceipType) ? $" AND v.SettleReceipType = '{model.SettleReceipType}' " : " ")} 
                {(!string.IsNullOrEmpty(model.TicketPoint) ? $" AND v.TicketedPointID = '{model.TicketPoint}' " : " ")}
                {(!string.IsNullOrEmpty(model.OwnerId) ? $" AND v.OwnerID = '{model.OwnerId}' " : " ")}
                GROUP BY vd.`RecordID`
                ORDER BY v.`DataDate` DESC";
                return _context.ExportVoucherDataSet.FromSqlRaw(sql).ToList();
            }
            else
            {
                string sql = $@"SELECT vd.`RecordID`,en.`EnterpriseID`,en.`EnterpriseName`,DATE_FORMAT(v.`DataDate`, '%Y-%m-%d') DataDate,st.`DataDictName` AS SettleReceipTypeName,CONCAT(tick.`TicketedPointName`,v.`Number`) TicketPointNumber,CONCAT(IFNULL(s.`SettleSummaryName`,sg.`SettleSummaryGroupName`),vd.`Content`) ReceiptAbstractName,vd.`Content`,
                acc.`AccoSubjectFullName`,hr.`Name`,market.`cFullName` MarketName,IFNULL(cus.`CustomerName`,sup.`SupplierName`) CustomerName,proj.`ProjectName`,bp.`ProductName`,bgp.`ProductGroupName`,
                bpf.`ClassificationName`,(vd.`Debit`) Debit,(vd.`Credit`) Credit,account.`AccountFullName`,bd.cDictName PaymentTypeName,IFNULL(bo.SortName,envd.`EnterpriseName`) SortName,v.`AttachmentNum`,IFNULL(sd.FinancialStatementName,sdg.FinancialStatementName) FinancialStatementName,v.`Remarks`,ohr.`Name` OwnerName,hr2.Name AuditName
                 FROM `nxin_qlw_business`.`fd_settlereceipt` v
                INNER JOIN `nxin_qlw_business`.`fd_settlereceiptdetail` vd ON v.`NumericalOrder` = vd.`NumericalOrder`
                LEFT JOIN `nxin_qlw_business`.`biz_ticketedpoint` tick ON tick.`TicketedPointID` = v.`TicketedPointID`
                LEFT JOIN `qlw_nxin_com`.`biz_settlesummary` s ON s.`SettleSummaryID` = vd.`ReceiptAbstractID`
                LEFT JOIN  qlw_nxin_com.biz_financialstatement sd ON sd.FinancialStatementID=s.FinancialStatementID
                LEFT JOIN `qlw_nxin_com`.`biz_settlesummarygroup` sg ON sg.`SettleSummaryGroupID` = vd.`ReceiptAbstractID`
                LEFT JOIN  qlw_nxin_com.biz_financialstatement sdg ON sdg.FinancialStatementID=sg.FinancialStatementID
                LEFT JOIN `qlw_nxin_com`.`biz_accosubject` acc ON acc.`AccoSubjectID` = vd.`AccoSubjectID`
                LEFT JOIN `qlw_nxin_com`.`hr_person` hr ON hr.`PersonID` = vd.`PersonID`
                LEFT JOIN `qlw_nxin_com`.`biz_market` market ON market.`MarketID` = vd.`MarketID`
                LEFT JOIN `qlw_nxin_com`.`biz_customer` cus ON cus.`CustomerID` = vd.`CustomerID`
                LEFT JOIN `nxin_qlw_business`.`pm_supplier` sup ON sup.`SupplierID` = vd.`CustomerID`
                LEFT JOIN `qlw_nxin_com`.ppm_project proj ON proj.`ProjectID` = vd.`ProjectID`
                LEFT JOIN `qlw_nxin_com`.`biz_product` bp ON bp.`ProductID` = vd.`ProductID`
                LEFT JOIN `qlw_nxin_com`.`biz_productgroup` bgp ON bgp.`ProductGroupID` = vd.`ProductGroupID`
                LEFT JOIN qlw_nxin_com.`biz_productgroupclassification` bpf ON bpf.ClassificationID = vd.ClassificationID
                LEFT JOIN `nxin_qlw_business`.`fd_account` account ON account.`AccountID` = vd.`AccountID`
                LEFT JOIN qlw_nxin_com.bsdatadict BD ON BD.DictID=vd.PaymentTypeID 
                LEFT JOIN `qlw_nxin_com`.`biz_enterprise` en ON en.`EnterpriseID` = v.`EnterpriseID`
                LEFT JOIN `qlw_nxin_com`.`biz_enterprise` envd ON envd.`EnterpriseID` = vd.`EnterpriseID`
                LEFT JOIN `nxin_qlw_business`.`hr_person` ohr ON ohr.`BO_ID` = v.`OwnerID`
                LEFT JOIN `nxin_qlw_business`.`biz_reviwe` audit ON audit.NumericalOrder = v.NumericalOrder AND audit.Level = 2  AND audit.ReviweType = 1611091727140000101
                LEFT JOIN nxin_qlw_business.hr_person hr2 ON hr2.bo_id = audit.CheckedByID
                LEFT JOIN `nxin_qlw_business`.`biz_datadict` st ON st.`DataDictID` = v.`SettleReceipType`
                LEFT JOIN qlw_nxin_com.bsorganizationsort bo ON bo.SortId=vd.OrganizationSortID
                WHERE v.`NumericalOrder` IN ({model.NumericalOrder}) 
                GROUP BY vd.`RecordID`
                ORDER BY v.`DataDate` DESC";
                return _context.ExportVoucherDataSet.FromSqlRaw(sql).ToList();
            }
            
        }
    }
}
