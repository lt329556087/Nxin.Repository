using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.PerformanceIncome;
using FinanceManagement.Common;
using FinanceManagement.Common.NewMakeVoucherCommon;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using MP.MiddlePlatform.Integration.Integaration;
using System.Reflection.Emit;
using Newtonsoft.Json;
using FinanceManagement.Common.MakeVoucherCommon;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Architecture.Seedwork.Core;
using NPOI.XSSF.Streaming.Values;
using FinanceManagement.ApiHost.Controllers.SettleReceiptBalance;
using System.Text;
using FinanceManagement.Common.MonthEndCheckout;
using NPOI.HSSF.Util;
using Prometheus;
using static FinanceManagement.ApiHost.Applications.Queries.FM_CashSweepODataProvider;
using System.Collections;
using NPOI.SS.Formula.Functions;
using Microsoft.Extensions.Primitives;

namespace FinanceManagement.ApiHost.Controllers
{
    /// <summary>
    /// 波尔莱特 - 定制化需求
    /// </summary>
    public class RptAssistRecordODataProvider
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private readonly ILogger<RptAssistRecordODataProvider> _logger;


        public RptAssistRecordODataProvider(IIdentityService identityservice, ILogger<RptAssistRecordODataProvider> logger, FMBaseCommon baseUnit, QlwCrossDbContext context)//, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {           
            _identityservice = identityservice;
            _logger = logger;
            _context = context;           
        }
        #region 明细
        /// <summary>
        /// 获取明细数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public GatewayResultModel GetDetailData(RptAssistRecordRequest req)
        {
            var result = new GatewayResultModel();
            var list = RequestDetailList(req);

            result.totalcount = list?.Count ?? 0;
            result.data = list;
            result.code = 0;
            return result;
        }
        public List<AssistRecordResultEntity> RequestDetailList(RptAssistRecordRequest req)
        {
            var list = new List<AssistRecordResultEntity>();
            var pageSize = 20;
            var total = req.EnableEnterList.Count;
            decimal avg = Convert.ToDecimal(total) / pageSize;
            int num = Convert.ToInt32(Math.Ceiling(avg));
            var wheresql = GetWhereSql(req);
            var joinsql = GetJoinSql(req);
            object lockObject = new object();
            Parallel.For(0, num, n =>
            {
                var enterList = req.EnableEnterList.Skip(n * pageSize).Take(pageSize).ToList();
                var enterids = string.Join(',', req.EnableEnterList);
                lock (lockObject)
                {
                    var tmpList = RequestDetailData(req, enterids, wheresql, joinsql);
                    if (list?.Count() > 0)
                    {
                        //排序：默认按账务单位、日期、凭证类别、凭证字号排序
                        tmpList = tmpList.OrderBy(p => p.EnterpriseName).ThenBy(p => p.DataDate).ThenBy(p => p.SettleReceipTypeName).ThenBy(p => p.TicketedPointName).ThenBy(p => p.Number).ToList();
                    }
                    list.AddRange(tmpList);
                }
            });
            return list;
        }
        private List<AssistRecordResultEntity> RequestDetailData(RptAssistRecordRequest req, string enterids, string wheresql, string joinsql)
        {
            var sql = MakeSqlStr1(req, enterids, wheresql, joinsql);
            return _context.AssistRecordDataSet.FromSqlRaw(sql).ToList();
        }
        private string MakeSqlStr1(RptAssistRecordRequest req, string enterids, string wheresql, string joinsql)
        {
            //账务单位 日期  凭证类别 凭证字号    记账号 摘要  内容 会计科目    客商 部门  员工 项目  归属单位 借方金额    贷方金额 备注
            var sql = string.Format(@$"
    SELECT UUID() UID ,be1.EnterpriseName,
      DATE_FORMAT(a.DataDate,'%Y-%m-%d') DataDate,
      dic.cDictName AS SettleReceipTypeName,
      c.TicketedPointName,
      CONVERT(a.Number USING utf8mb4) Number,
      a.AccountNo,
      d.SettleSummaryName, 
      b.Content,
      km.AccoSubjectFullName,
      e.CustomerName,
      f.cFullName AS MarketFullName,
      h.Name AS PersonName,
      proName.ProjectName,
      be.EnterpriseName AS BelongEnterpriseName,   
      org.cFullName OrgMarketName, 
      b.Debit AS Debit,
      b.Credit AS Credit,   
      a.Remarks AS Remark ,
      km.AccoSubjectCode ,
      CONVERT(a.NumericalOrder USING utf8mb4) NumericalOrder,
      Concat(b.Auxiliary1) Auxiliary1,
      aux1.ProjectName AuxiliaryName1,
      Concat(b.Auxiliary2) Auxiliary2,
      aux2.ProjectName AuxiliaryName2,
      Concat(b.Auxiliary3) Auxiliary3,
      aux3.ProjectName AuxiliaryName3,
      Concat(b.Auxiliary4) Auxiliary4,
      aux4.ProjectName AuxiliaryName4,
      Concat(b.Auxiliary5) Auxiliary5,
      aux5.ProjectName AuxiliaryName5,
      Concat(b.Auxiliary6) Auxiliary6,
      aux6.ProjectName AuxiliaryName6,
      Concat(b.Auxiliary7) Auxiliary7,
      aux7.ProjectName AuxiliaryName7,
      Concat(b.Auxiliary8) Auxiliary8,
      aux8.ProjectName AuxiliaryName8,
      Concat(b.Auxiliary9) Auxiliary9,
      aux9.ProjectName AuxiliaryName9,
      Concat(b.Auxiliary10) Auxiliary10,
      aux10.ProjectName AuxiliaryName10 
      -- ,a.TicketedPointID,b.MarketID,b.CustomerID,b.PersonID,b.ProjectID,b.AccoSubjectID,km.IsCus,km.IsSup,b.ProductID   
      FROM NXin_Qlw_Business.FD_SettleReceipt a
      INNER JOIN NXin_Qlw_Business.FD_SettleReceiptDetail b ON a.NumericalOrder=b.NumericalOrder 
      LEFT JOIN NXin_Qlw_Business.BIZ_TicketedPoint c ON a.TicketedPointID=c.TicketedPointID
      LEFT JOIN qlw_nxin_com.BIZ_SettleSummary d ON b.ReceiptAbstractID=d.SettleSummaryID      
      LEFT JOIN qlw_nxin_com.BIZ_SettleSummaryGroup sg ON sg.SettleSummaryGroupID=b.ReceiptAbstractID
      LEFT JOIN qlw_nxin_com.BIZ_Customer e ON b.CustomerID=e.CustomerID
      LEFT JOIN qlw_nxin_com.BIZ_Market f ON b.MarketID=f.MarketID AND a.EnterpriseID=f.EnterpriseID
      LEFT JOIN qlw_nxin_com.HR_Person h ON b.PersonID=h.PersonID
      LEFT JOIN qlw_nxin_com.BIZ_AccoSubject km ON b.AccoSubjectID=km.AccoSubjectID
      LEFT JOIN qlw_nxin_com.PPM_Project proName  ON b.`ProjectID` = proName.ProjectID
      LEFT JOIN qlw_nxin_com.BSOrganizationSort org ON b.OrganizationSortID=org.SortId
      LEFT JOIN qlw_nxin_com.biz_enterprise be ON be.enterpriseid = b.EnterpriseID
      LEFT JOIN qlw_nxin_com.biz_enterprise be1 ON be1.enterpriseid = a.EnterpriseID
      LEFT JOIN qlw_nxin_com.bsdatadict dic ON a.SettleReceipType=dic.dictID  
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux1 on aux1.ProjectId = b.Auxiliary1
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux2 on aux2.ProjectId = b.Auxiliary2
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux3 on aux3.ProjectId = b.Auxiliary3
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux4 on aux4.ProjectId = b.Auxiliary4
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux5 on aux5.ProjectId = b.Auxiliary5
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux6 on aux6.ProjectId = b.Auxiliary6
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux7 on aux7.ProjectId = b.Auxiliary7
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux8 on aux8.ProjectId = b.Auxiliary8
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux9 on aux9.ProjectId = b.Auxiliary9
      LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux10 on aux10.ProjectId = b.Auxiliary10
      {joinsql}
      WHERE a.DataDate BETWEEN '{req.BeginDate}' AND '{req.EndDate}' AND a.EnterpriseID IN ({enterids}) {wheresql}");
            return sql;
        }
        #endregion

        #region 汇总
        /// <summary>
        /// 获取汇总表数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public GatewayResultModel GetSummaryData(RptAssistRecordRequest req)
        {
            var result = new GatewayResultModel();
            var list = RequestSummaryList(req);
            if (list?.Count() > 0)
            {
                result.totalcount = list.Count;
            }
            result.data = list;
            result.code = 0;
            return result;
        }
        public List<AssistRecordSummaryResultEntity> RequestSummaryList(RptAssistRecordRequest req)
        {
            var list = new List<AssistRecordSummaryResultEntity>();
            var pageSize = 20;
            var total = req.EnableEnterList.Count;
            decimal avg = Convert.ToDecimal(total) / pageSize;
            int num = Convert.ToInt32(Math.Ceiling(avg));
            var sbgroup = new StringBuilder(); var sbselect = new StringBuilder(); var sbjoin = new StringBuilder();
            var wheresql = GetWhereSql(req);
            var joinsql = GetJoinSql(req);
            
            Dictionary<string, bool> tableCacheName = GetCacheTable();
            GetSummaryTypeField(req,ref tableCacheName, ref sbselect, ref sbjoin, ref sbgroup);
            var groupsql = "";
            joinsql = joinsql + sbjoin.ToString();
            object lockObject = new object();
            Parallel.For(0, num, n => {
                var enterList = req.EnableEnterList.Skip(n * pageSize).Take(pageSize).ToList();
                var enterids = string.Join(',', req.EnableEnterList);
                lock (lockObject)
                {
                    var tmpList = RequestSummaryData(req, enterids, wheresql, joinsql, sbselect.ToString(), groupsql);
                    if (tmpList?.Count > 0)
                    {
                        list.AddRange(tmpList);
                    }
                }                         
            });
            //增加小计
            list = list.AddSubTotal((subList) =>
            {
                var obj = new AssistRecordSummaryResultEntity()
                {
                    Debit = subList.Sum(n => n.Debit),
                    Credit = subList.Sum(n => n.Credit)
                };
                return obj;
            }).ToList();
            return list;
        }
        private List<AssistRecordSummaryResultEntity> RequestSummaryData(RptAssistRecordRequest req, string enterids, string wheresql, string joinsql,string selectsql,string groupsql)
        {
            var sql = MakeSqlStr1_Summary(req,enterids,wheresql,joinsql,selectsql,groupsql);
            return _context.AssistRecordSummaryDataSet.FromSqlRaw(sql).ToList();
        }
        private string MakeSqlStr1_Summary(RptAssistRecordRequest req, string enterids, string wheresql, string joinsql, string selectsql, string groupsql)
        {
            var sql = string.Format(@$"SELECT UUID() UID,IFNULL(SUM(b.Debit),0) AS Debit,IFNULL(SUM(b.Credit),0) AS Credit {selectsql}
                         FROM  NXin_Qlw_Business.FD_SettleReceipt a
                        INNER JOIN NXin_Qlw_Business.FD_SettleReceiptDetail b ON a.NumericalOrder=b.NumericalOrder    
                        LEFT JOIN qlw_nxin_com.BIZ_Market f ON b.MarketID=f.MarketID AND a.EnterpriseID=f.EnterpriseID
                        LEFT JOIN qlw_nxin_com.BIZ_AccoSubject km ON b.AccoSubjectID=km.AccoSubjectID
                        {joinsql}
                        WHERE a.DataDate BETWEEN '{req.BeginDate}' AND '{req.EndDate}' AND a.EnterpriseID IN ({enterids}) {wheresql} 
                        GROUP BY SummaryType ");
            return sql;
        }

        private void GetSummaryTypeField(RptAssistRecordRequest req, ref Dictionary<string, bool> tableCacheName, ref StringBuilder sbselect, ref StringBuilder sbjoin, ref StringBuilder sbgroup)
        {
            var param = new SummaryTypeRequest { GroupID = req.GroupID, DataDate = req.EndDate, EnterpriseList = req.EnableEnterList };
            var typelist = GetSummaryTypeList(param);
            if (typelist == null||typelist.Count == 0) { return; }
            var summaryTypeList=req.SummaryTypeList;
            var index = 1;
            var select = new StringBuilder(); var selectField = new StringBuilder(); var selectFieldName = new StringBuilder();
            foreach (var summaryType in summaryTypeList)
            {
                var type = typelist.Where(i => i.SV == summaryType)?.FirstOrDefault();
                var snName = type?.SN;
                var snRank = 1;
                if (type?.Rank > 0)
                {
                    snRank=type.Rank;
                }
                selectFieldName.Append(SetSummary($@"'{snName}'"));
                if (summaryType== "EnterpriseID")
                {
                    select.Append(SetSummary("a.EnterpriseID"));
                    selectField.Append(SetSummary("be.EnterpriseName"));
                    if (!tableCacheName["biz_enterprise"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.biz_enterprise be ON be.enterpriseid = a.EnterpriseID ");
                    }
                    //sbgroup.Append($@" a.EnterpriseID,");
                }
                if (summaryType.Contains("market"))
                {
                    select.Append(SetSummary($@"SUBSTRING_INDEX(f.cFullName, '/', {snRank})"));
                    selectField.Append(SetSummary($@"SUBSTRING_INDEX(f.cFullName, '/', {snRank})"));
                    if (!tableCacheName["biz_market"])
                    {
                        tableCacheName["biz_market"] = true;
                        sbjoin.Append($@" LEFT JOIN qlw_nxin_com.biz_market f ON b.MarketID=f.MarketID AND a.EnterpriseID=f.EnterpriseID ");
                    }
                    //sbgroup.Append($@" SummaryType{index},");
                }
                if (summaryType.Contains("subject"))
                {
                    select.Append(SetSummary($@"SUBSTRING_INDEX(km.AccoSubjectFullName, '/', {snRank})"));
                    selectField.Append(SetSummary($@"SUBSTRING_INDEX(km.AccoSubjectFullName, '/', {snRank})"));
                }
                if (summaryType.Contains("settlesummary"))
                {
                    if (!tableCacheName["biz_settlesummary"])
                    {
                        tableCacheName["biz_settlesummary"] = true;
                        sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.BIZ_SettleSummary d ON b.ReceiptAbstractID=d.SettleSummaryID        
                                            LEFT JOIN qlw_nxin_com.BIZ_SettleSummaryGroup sg ON sg.SettleSummaryGroupID=b.ReceiptAbstractID AND sg.EnterpriseID = {req.GroupID}  ");
                        if (snRank ==1)
                        {
                            select.Append(SetSummary($@"sg0.SettleSummaryGroupID"));
                            selectField.Append(SetSummary($@"sg0.SettleSummaryGroupName"));
                            if (!tableCacheName["biz_settlesummarygroup1"])
                            {
                                tableCacheName["biz_settlesummarygroup1"] = true;
                                sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.BIZ_SettleSummaryGroup sg1 ON (sg1.SettleSummaryGroupID=d.SettleSummaryGroupID OR sg1.SettleSummaryGroupID=sg.SettleSummaryGroupID) AND sg1.rank={snRank} AND sg1.EnterpriseID = {req.GroupID} 
                                                      LEFT JOIN qlw_nxin_com.BIZ_SettleSummaryGroup sg0 ON sg0.SettleSummaryGroupID=sg1.PID AND sg0.rank={snRank} AND sg0.EnterpriseID = {req.GroupID} ");
                            }
                            //sbgroup.Append($@" sg0.SettleSummaryGroupID,");
                        }
                        if (snRank == 2)
                        {
                            select.Append(SetSummary($@"sg2.SettleSummaryGroupID"));
                            selectField.Append(SetSummary($@"sg2.SettleSummaryGroupName"));
                            if (!tableCacheName["biz_settlesummarygroup2"])
                            {
                                tableCacheName["biz_settlesummarygroup2"] = true;
                                sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.BIZ_SettleSummaryGroup sg2 ON (sg2.SettleSummaryGroupID=d.SettleSummaryGroupID OR sg2.SettleSummaryGroupID=sg.SettleSummaryGroupID) AND sg2.rank={snRank} AND sg2.EnterpriseID = {req.GroupID}  ");
                            }
                            //sbgroup.Append($@" SettleSummaryGroupID,");

                        }
                        else
                        {
                            select.Append(SetSummary($@"IFNULL(b.ReceiptAbstractID,sg.SettleSummaryGroupID)"));
                            selectField.Append(SetSummary($@"IFNULL(d.SettleSummaryName,sg.SettleSummaryGroupName)"));
                            //sbgroup.Append($@" b.ReceiptAbstractID,");
                        }
                    }
                }
                if (summaryType.Contains("project"))
                {
                    if(snRank==1)
                    {
                        if (!tableCacheName["ppm_project1"])
                        {
                            sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.ppm_project pro2 ON b.ProjectID = pro2.ProjectID ");
                        }
                        if (!tableCacheName["ppm_project2"])
                        { 
                            sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.ppm_project pro1 ON pro1.ProjectID = pro2.PID ");
                        }
                        select.Append(SetSummary($@"pro1.ProjectID"));
                        selectField.Append(SetSummary($@"pro1.ProjectName"));
                    }
                    else
                    {
                        if (!tableCacheName["ppm_project1"])
                        {                            
                            sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.ppm_project pro2 ON b.ProjectID = pro2.ProjectID ");
                        }
                        select.Append(SetSummary($@"pro2.ProjectID"));
                        selectField.Append(SetSummary($@"pro2.ProjectName"));
                    }
                }
                if(summaryType == "customer")
                {
                    select.Append(SetSummary($@"b.CustomerID"));
                    selectField.Append(SetSummary($@"bc.CustomerName"));
                    if (!tableCacheName["biz_customer"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.biz_customer bc ON bc.CustomerID = b.CustomerID ");
                    }
                }
                if (summaryType == "settlereceiptype")
                {
                    select.Append(SetSummary($@"a.SettleReceipType"));
                    selectField.Append(SetSummary($@"bd.cDictName"));
                    if (!tableCacheName["bsdatadict"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN qlw_nxin_com.bsdatadict bd ON a.SettleReceipType = bd.dictID ");
                    }
                }
                if (summaryType == "ticketedpoint")
                {
                    select.Append(SetSummary($@"a.TicketedPointID"));
                    selectField.Append(SetSummary($@"c.TicketedPointName"));
                    if (!tableCacheName["biz_ticketedpoint"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.biz_ticketedpoint c ON a.TicketedPointID = c.TicketedPointID ");
                    }
                }
                if (summaryType == "month")
                {
                    select.Append(SetSummary($@"DATE_FORMAT(a.DataDate,'%Y-%m')"));
                    selectField.Append(SetSummary($@"DATE_FORMAT(a.DataDate,'%Y-%m')"));
                }
                if (summaryType == "datadate"||summaryType== "number")
                {
                    select.Append(SetSummary($@"a.{summaryType}"));
                    selectField.Append(SetSummary($@"a.{summaryType}"));
                }
                if (summaryType == "Auxiliary1")
                {
                    select.Append(SetSummary($@"b.Auxiliary1"));
                    selectField.Append(SetSummary($@"aux1.ProjectName"));
                    if (!tableCacheName["aux1"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux1 on aux1.ProjectId = b.Auxiliary1 ");
                    }
                }
                if (summaryType == "Auxiliary2")
                {
                    select.Append(SetSummary($@"b.Auxiliary2"));
                    selectField.Append(SetSummary($@"aux2.ProjectName"));
                    if (!tableCacheName["aux2"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux2 on aux2.ProjectId = b.Auxiliary2 ");
                    }
                }
                if (summaryType == "Auxiliary3")
                {
                    select.Append(SetSummary($@"b.Auxiliary3"));
                    selectField.Append(SetSummary($@"aux3.ProjectName"));
                    if (!tableCacheName["aux3"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux3 on aux3.ProjectId = b.Auxiliary3 ");
                    }
                }
                if (summaryType == "Auxiliary4")
                {
                    select.Append(SetSummary($@"b.Auxiliary4"));
                    selectField.Append(SetSummary($@"aux4.ProjectName"));
                    if (!tableCacheName["aux4"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux4 on aux4.ProjectId = b.Auxiliary4 ");
                    }
                }
                if (summaryType == "Auxiliary5")
                {
                    select.Append(SetSummary($@"b.Auxiliary5"));
                    selectField.Append(SetSummary($@"aux5.ProjectName"));
                    if (!tableCacheName["aux5"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux5 on aux5.ProjectId = b.Auxiliary5 ");
                    }
                }
                if (summaryType == "Auxiliary6")
                {
                    select.Append(SetSummary($@"b.Auxiliary6"));
                    selectField.Append(SetSummary($@"aux6.ProjectName"));
                    if (!tableCacheName["aux6"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux6 on aux6.ProjectId = b.Auxiliary6 ");
                    }
                }
                if (summaryType == "Auxiliary7")
                {
                    select.Append(SetSummary($@"b.Auxiliary7"));
                    selectField.Append(SetSummary($@"aux7.ProjectName"));
                    if (!tableCacheName["aux7"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux7 on aux7.ProjectId = b.Auxiliary7 ");
                    }
                }
                if (summaryType == "Auxiliary8")
                {
                    select.Append(SetSummary($@"b.Auxiliary8"));
                    selectField.Append(SetSummary($@"aux8.ProjectName"));
                    if (!tableCacheName["aux8"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux8 on aux8.ProjectId = b.Auxiliary8 ");
                    }
                }
                if (summaryType == "Auxiliary9")
                {
                    select.Append(SetSummary($@"b.Auxiliary9"));
                    selectField.Append(SetSummary($@"aux9.ProjectName"));
                    if (!tableCacheName["aux9"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux9 on aux9.ProjectId = b.Auxiliary9 ");
                    }
                }
                if (summaryType == "Auxiliary10")
                {
                    select.Append(SetSummary($@"b.Auxiliary10"));
                    selectField.Append(SetSummary($@"aux10.ProjectName"));
                    if (!tableCacheName["aux10"])
                    {
                        sbjoin.AppendLine($@" LEFT JOIN nxin_qlw_business.fd_auxiliaryproject aux10 on aux10.ProjectId = b.Auxiliary10 ");
                    }
                }
                sbgroup.Append($@" SummaryType{index},");
                index++;
            }
            sbselect = sbselect.Append($@",{GetRetSummary(select.ToString())} SummaryType,{GetRetSummary(selectField.ToString())} SummaryTypeName,{GetRetSummary(selectFieldName.ToString())} SummaryTypeFieldName");
        }
        private string SetSummary(string field)
        {
            return $@"IFNULL({field},'[null]'),'~',";
        }
        private string GetRetSummary(string field)
        {
           return @"CONCAT("+field.Substring(0,field.Length - 5)+")";
        }
      
        public List<SummaryDataType> GetSummaryTypeList(SummaryTypeRequest req)
        {
            List<SummaryDataType> list = new List<SummaryDataType>();
            //单位、部门级次、会计科目级次、结算摘要级次、项目级次、客商、凭证类别、凭证字、月份、日期、凭证号
            //部门、科目、摘要、项目取集团下最大级次
            list.Add(new SummaryDataType() { SN = "账务单位", SV = "EnterpriseID",  Order = 1000 });
            //部门级次
            var marketLevel = GetSummaryLevel(req, "biz_market");
            list.Add(new SummaryDataType() { SN = "部门", SV = "market", Order = 2000 });
            for (int i = 1; i <= marketLevel; i++)
            {
                var summary = new SummaryDataType() { SN = $"部门{i}级", SV = $"market{i}", Pid = "market", Order = 2000 + i,Rank=i };
                list.Add(summary);
            }
            //科目级次
            var subjectLevel = GetSummaryLevel(req, "biz_accosubject");
            list.Add(new SummaryDataType() { SN = "会计科目", SV = "subject", Order = 3000 });
            for (int i = 1;i<=subjectLevel;i++)
            {
                var summary = new SummaryDataType() { SN = $"科目{i}级", SV = $"subject{i}" , Pid = "subject", Order = 3000+i, Rank = i };
                list.Add(summary);
            }
            //摘要级次
            var settlesummaryLevel = 3;// GetSettlesummaryLevel(req);
            list.Add(new SummaryDataType() { SN = "结算摘要", SV = "settlesummary", Order = 4000 });
            for (int i = 1; i <= settlesummaryLevel; i++)
            {
                var summary = new SummaryDataType() { SN = $"结算摘要{i}级", SV = $"settlesummary{i}", Pid = "settlesummary", Order = 4000 + i, Rank = i };
                list.Add(summary);
            }
            //项目级次
            var projectLevel = 2;// GetProjectLevel(req);
            list.Add(new SummaryDataType() { SN = "项目", SV = "project", Order = 5000 });
            for (int i = 1; i <= projectLevel; i++)
            {
                var summary = new SummaryDataType() { SN = $"项目{i}级", SV = $"project{i}", Pid = "project", Order = 5000 + i };
                list.Add(summary);
            }
            list.Add(new SummaryDataType() { SN = "客商", SV = "customer", Order = 6000 });
            list.Add(new SummaryDataType() { SN = "凭证类别", SV = "settlereceiptype", Order = 7000 });
            list.Add(new SummaryDataType() { SN = "凭证字", SV = "ticketedpoint", Order = 8000 });
            list.Add(new SummaryDataType() { SN = "月份", SV = "month", Order = 9000 });
            list.Add(new SummaryDataType() { SN = "日期", SV = "datadate", Order = 10000 });
            list.Add(new SummaryDataType() { SN = "凭证号", SV = "number", Order = 11000 });
            //辅助项
            list.Add(new SummaryDataType() { SN = "辅助项", SV = "Auxiliary", Order = 12000 });
            var auxList = _context.DynamicSqlQuery($@"SELECT * FROM nxin_qlw_business.fd_auxiliarytype where GroupId = {(string.IsNullOrEmpty(req.GroupID) ? _identityservice.GroupId : req.GroupID)} Order by TypeCode;");
            for (int i = 1; i <= auxList.Count; i++)
            {
                var summary = new SummaryDataType() { SN = $"{auxList[i-1].TypeName}", SV = $"Auxiliary{i}", Pid = "Auxiliary", Order = 12000 + i };
                list.Add(summary);
            }
            //var cacheKey = request.GetCacheKey(this.GetType() + "_GetSummaryTypeList");
            //if (CacheHelper.Exists(cacheKey))
            //{
            //    CacheHelper.Get(cacheKey, out lst);
            //}
            //CacheHelper.Add(cacheKey, lst, DateTimeOffset.Now.AddMinutes(3));

            return list;
        }
        private Dictionary<string, bool> GetCacheTable()
        {
            Dictionary<string, bool> tableCacheName = new Dictionary<string, bool>();
            tableCacheName.Add("biz_enterprise", false);
            tableCacheName.Add("biz_market", false);
            tableCacheName.Add("biz_settlesummary", false);
            tableCacheName.Add("biz_settlesummarygroup1", false);
            tableCacheName.Add("biz_settlesummarygroup2", false);
            tableCacheName.Add("ppm_project1", false);
            tableCacheName.Add("ppm_project2", false);
            tableCacheName.Add("biz_customer", false);
            tableCacheName.Add("bsdatadict", false);
            tableCacheName.Add("biz_ticketedpoint", false);
            tableCacheName.Add("aux1",false);
            tableCacheName.Add("aux2",false);
            tableCacheName.Add("aux3",false);
            tableCacheName.Add("aux4",false);
            tableCacheName.Add("aux5",false);
            tableCacheName.Add("aux6",false);
            tableCacheName.Add("aux7",false);
            tableCacheName.Add("aux8",false);
            tableCacheName.Add("aux9",false);
            tableCacheName.Add("aux10",false);
            return tableCacheName;
        }

        public List<AssistRecordSummaryResultEntity> AddSubTotal(List<AssistRecordSummaryResultEntity> list,List<string> summaryList)
        {
            try
            {
                if (list == null || list.Count == 0||summaryList==null||summaryList.Count==0) return list;
                var length = summaryList.Count;
                var subList = new Dictionary<int, List<AssistRecordSummaryResultEntity>>();
                for (var i=0;i< length; i++)
                {
                    var sub = new List<AssistRecordSummaryResultEntity>();
                    list.GroupBy(p => p.SummaryTypeNameList);
                    subList.Add(i, sub);
                }
            }catch{ }
            return list;
        }
     
        #endregion

        #region 公用
        public string GetWhereSql(RptAssistRecordRequest req)
        {
            var sqlWhere = new StringBuilder();
                       
            if (!string.IsNullOrEmpty(req.SettleSummaryID))
            {
                sqlWhere.Append(@$" AND b.ReceiptAbstractID in( {req.SettleSummaryID}) ");
            }
            if (!string.IsNullOrEmpty(req.PersonID))
            {
                sqlWhere.Append(@$" AND b.PersonID in( {req.PersonID}) ");
            }
            if (!string.IsNullOrEmpty(req.MarketID))
            {
                string marSQl = "";
                foreach (var item in req.MarketID.Split(','))
                {
                    if (string.IsNullOrEmpty(marSQl))
                    {
                        marSQl += @$" AND (f.caxis like '%{item}%' ";
                    }
                    else
                    {
                        marSQl += @$" OR f.caxis like '%{item}%' ";
                    }
                }
                marSQl += " ) ";
                sqlWhere.Append(marSQl);
            }
            if (!string.IsNullOrEmpty(req.ProjectID))
            {
                sqlWhere.Append(@$" AND b.ProjectID in( {req.ProjectID}) ");
            }
            if (!string.IsNullOrEmpty(req.CustomerID))
            {
                sqlWhere.Append(@$" AND b.CustomerID in( {req.CustomerID}) ");
            }
            if (!string.IsNullOrEmpty(req.SettleReceipType))
            {
                sqlWhere.Append(@$" AND a.SettleReceipType in( {req.SettleReceipType}) ");
            }
            if (!string.IsNullOrEmpty(req.TicketedPointID))
            {
                sqlWhere.Append(@$" AND a.TicketedPointID in( {req.TicketedPointID}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary1))
            {
                sqlWhere.Append(@$" AND aux1.ProjectId in( {req.Auxiliary1}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary2))
            {
                sqlWhere.Append(@$" AND aux2.ProjectId in( {req.Auxiliary2}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary3))
            {
                sqlWhere.Append(@$" AND aux3.ProjectId in( {req.Auxiliary3}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary4))
            {
                sqlWhere.Append(@$" AND aux4.ProjectId in( {req.Auxiliary4}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary5))
            {
                sqlWhere.Append(@$" AND aux5.ProjectId in( {req.Auxiliary5}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary6))
            {
                sqlWhere.Append(@$" AND aux6.ProjectId in( {req.Auxiliary6}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary7))
            {
                sqlWhere.Append(@$" AND aux7.ProjectId in( {req.Auxiliary7}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary8))
            {
                sqlWhere.Append(@$" AND aux8.ProjectId in( {req.Auxiliary8}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary9))
            {
                sqlWhere.Append(@$" AND aux9.ProjectId in( {req.Auxiliary9}) ");
            }
            if (!string.IsNullOrEmpty(req.Auxiliary10))
            {
                sqlWhere.Append(@$" AND aux10.ProjectId in( {req.Auxiliary10}) ");
            }
            sqlWhere.Append($@" AND  a.SettleReceipType not in (201610220104402204,201610220104402205,201610220104402206) ");
            if (req.AccoSubjectCodeList?.Count > 0)
            {
                sqlWhere.Append(" AND (");
                var index = 0;
                foreach (var item in req.AccoSubjectCodeList)
                {
                    sqlWhere.AppendFormat(" {1} km.AccoSubjectCode LIKE CONCAT({0},'%') ", item, index == 0 ? "" : "OR");
                    index++;
                }
                sqlWhere.Append(" )");
            }
            return sqlWhere.ToString();
        }
        public string GetJoinSql(RptAssistRecordRequest req)
        {
            var joinsql = new StringBuilder();
            if (req.OnlyCombineEnte == true)
            {
                joinsql.Append(@$" inner join qlw_nxin_com.BIZ_MergeEntity mer on a.EnterpriseID=mer.iMergeEnterpriseID and mer.EnteID={req.GroupID}  AND a.DataDate BETWEEN mer.dBegin AND IFNULL(mer.dEnd, CURDATE())  AND mer.iMergeSort = 420509603625894912 ");
            }
            return joinsql.ToString();
        }

        #region 下拉
        /// <summary>
        /// 集团最大级次
        /// </summary>
        /// <param name="req"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetSummaryLevel(SummaryTypeRequest req,string type)
        {
            var subjectLevel = 0;
            var sql = @$"SELECT CONVERT(b.EnterpriseID USING utf8mb4) EnterpriseId,MAX(b.Rank) Level FROM qlw_nxin_com.{type} b
                        INNER JOIN qlw_nxin_com.biz_versionsetting v ON b.VersionID=v.VersionID
                        INNER JOIN qlw_nxin_com.biz_enterprise be ON be.enterpriseid = b.EnterpriseID
                         WHERE (b.EnterpriseID={req.GroupID} OR be.PID={req.GroupID}) AND '{req.DataDate}' BETWEEN v.dBegin AND v.dEnd";
            var list =_context.DynamicSqlQuery(sql);
            if(list?.Count() > 0)
            {
                if (string.IsNullOrEmpty(list.FirstOrDefault().Level.ToString()))
                {
                    list.FirstOrDefault().Level = "0";
                }
                subjectLevel=Convert.ToInt32(list.FirstOrDefault().Level);
            }
            return subjectLevel;
        }
        /// <summary>
        /// 摘要
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public int GetSettlesummaryLevel(SummaryTypeRequest req)
        {
            var subjectLevel = 0;
            //var enterids = req.GroupID;
            //if (req.EnterpriseList?.Count > 0)
            //{
            //    var tenids = string.Join(',', req.EnterpriseList);
            //    if (!string.IsNullOrEmpty(tenids))
            //    {
            //        enterids += "," + tenids;
            //    }
            //}
            var sql = @$"SELECT CONVERT(b.EnterpriseID USING utf8mb4) EnterpriseId, MAX(b.Rank) Level FROM(
                         SELECT b.EnterpriseID,b.Rank FROM qlw_nxin_com.biz_settlesummarygroup b
                         INNER JOIN qlw_nxin_com.biz_versionsetting v ON b.VersionID=v.VersionID
                          WHERE b.EnterpriseID ={req.GroupID} AND '{req.DataDate}' BETWEEN v.dBegin AND v.dEnd
                          UNION ALL
                         SELECT b.EnterpriseID,b.Rank FROM qlw_nxin_com.biz_settlesummary b
                         INNER JOIN qlw_nxin_com.biz_versionsetting v ON b.VersionID=v.VersionID
                         INNER JOIN qlw_nxin_com.biz_enterprise be ON be.enterpriseid = b.EnterpriseID
                          WHERE be.PID={req.GroupID} AND '{req.DataDate}' BETWEEN v.dBegin AND v.dEnd) b";
            var list = _context.GetBalanceSheetReview.FromSqlRaw(sql);
            if (list?.Count() > 0)
            {
                subjectLevel = list.FirstOrDefault().Level;
            }
            return subjectLevel;
        }
        /// <summary>
        /// 项目
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public int GetProjectLevel(SummaryTypeRequest req)
        {
            var subjectLevel = 0;            
            var sql = @$" SELECT CONVERT(b.EnterpriseID USING utf8mb4) EnterpriseId, MAX(b.Rank) Level FROM qlw_nxin_com.ppm_project b
                          INNER JOIN qlw_nxin_com.biz_enterprise be ON be.enterpriseid = b.EnterpriseID
                           WHERE be.PID={req.GroupID} ";
            var list = _context.GetBalanceSheetReview.FromSqlRaw(sql);
            if (list?.Count() > 0)
            {
                subjectLevel = list.FirstOrDefault().Level;
            }
            return subjectLevel;
        }
        /// <summary>
        /// 单位
        /// </summary>
        /// <returns></returns>
        public List<EnterpriseInfo> GetEnterpriseList(string groupId,string enterids)
        {
            var sql = @$"SELECT CONCAT(EnterpriseId) EnterpriseId,CONCAT(EnterpriseName) EnterpriseName FROM qlw_nxin_com.biz_enterprise
                WHERE PID = {groupId} and EnterpriseId in ({enterids})";
            return _context.EnterpriseInfoDataSet.FromSqlRaw(sql).ToList();
        }
        #endregion
        #endregion
    }
}
