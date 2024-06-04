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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;
using System.Dynamic;
using NPOI.OpenXmlFormats.Wordprocessing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using NPOI.SS.Formula.Functions;
using Architecture.Common.Util;
using Aspose.Cells;
using NPOI.XSSF.Streaming.Values;
using FinanceManagement.Common.MonthEndCheckout;
using Newtonsoft.Json.Linq;
using System.Collections;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FinanceManagement.ApiHost.Controllers
{
    /// <summary>
    /// 波尔莱特 - 定制化需求
    /// </summary>
    public class CostAnalysisODataProvider
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        private TreeModelODataProvider _treeModel;
        BIZ_DataDictODataProvider _dictProvider;
        FMBaseCommon _baseUnit;
        IHttpContextAccessor _httpContextAccessor;
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        private readonly ILogger<PerformanceIncomeEntity> _logger;

        public CostAnalysisODataProvider(IIdentityService identityservice, ILogger<PerformanceIncomeEntity> logger, FMBaseCommon baseUnit, QlwCrossDbContext context, TreeModelODataProvider treeModel, BIZ_DataDictODataProvider dictProvider, IHttpContextAccessor httpContextAccessor, HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _baseUnit = baseUnit;
            _identityservice = identityservice;
            _context = context;
            _treeModel = treeModel;
            _dictProvider = dictProvider;
            _httpContextAccessor = httpContextAccessor;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
            _logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// model.EnterpriseIds = 单位id
        /// <returns></returns>
        public dynamic GetCostAnalysisData(dynamic model)
        {
            CostAnalysisData result = new CostAnalysisData();
            if (model.IsLastData == null)
            {
                model.IsLastData = false;
            }
            string PersonSql;
            //当期数据
            List<dynamic> Data;
            //同期数据（去年数据）
            List<dynamic> LastData = new List<dynamic>();
            //获取凭证数据
            GetSettleData(model, out PersonSql, out Data);
            model.BeginDate = ((DateTime)Convert.ToDateTime(model.BeginDate)).AddYears(-1).ToString("yyyy-MM-dd");
            model.EndDate = ((DateTime)Convert.ToDateTime(model.EndDate)).AddYears(-1).ToString("yyyy-MM-dd");
            //bool 是否同比 
            if (model.IsLastData != null && !string.IsNullOrEmpty(model.IsLastData.ToString()))
            {
                //获取同期数据
                GetSettleData(model, out PersonSql, out LastData);
            }
            //小计做准备  赋值
            for (int i = 0; i < Data.Count; i++)
            {
                //是否小计
                _context.AddProperty(Data[i], "IsSum", 0);
                //排序
                _context.AddProperty(Data[i], "OrderNum", 0.00);
                //人均费用
                _context.AddProperty(Data[i], "PersonCost", 0.00);
                //追加默认级次 默认五级
                #region 单位组织 部门组织级次追加
                _context.AddProperty(Data[i], "org_en1", "");
                _context.AddProperty(Data[i], "org_en2", "");
                _context.AddProperty(Data[i], "org_en3", "");
                _context.AddProperty(Data[i], "org_en4", "");
                _context.AddProperty(Data[i], "org_en5", "");
                _context.AddProperty(Data[i], "org_market1", "");
                _context.AddProperty(Data[i], "org_market2", "");
                _context.AddProperty(Data[i], "org_market3", "");
                _context.AddProperty(Data[i], "org_market4", "");
                _context.AddProperty(Data[i], "org_market5", "");
                #endregion
                //是否获取同期数据
                if (Convert.ToBoolean(model?.IsLastData))
                {
                    //赋值去年同期费用 下方 汇总计算时会用到
                    _context.AddProperty(Data[i], "LastAmount", 0.00);
                    //增长率 本期/上期-1
                    _context.AddProperty(Data[i], "LastRate", 0.00);
                }
            }
            //获取单位组织
            var enterSort = EnterSortInfo(_identityservice.GroupId);
            var orgSort = OrgSortInfo(_identityservice.GroupId);
            #region 人员数据追加
            var PersonInfos = _context.DynamicSqlQuery(PersonSql);
            foreach (var item in PersonInfos)
            {
                SetSortNameOrganizationName(enterSort, orgSort, item);
            }
            #endregion
            //单位组织，部门组织设置属性
            foreach (var item in Data)
            {
                SetSortNameOrganizationName(enterSort, orgSort, item);
            }
            foreach (var item in LastData)
            {
                SetSortNameOrganizationName(enterSort, orgSort, item);
            }
            //组织过滤
            OrgEnterFilter(model, Data);
            #region 列汇总数据项
            string colunm = GetNameBySummary(model.ColumnSummary.ToString() + ",OrderNum");
            //列汇总数据项
            var ColumnList = new List<dynamic>();
            //Dynamic Linq 重构列汇总方式参数
            var ColumnSummary = GetColumnSummaryHandler(model.ColumnSummary.ToString());
            // 使用 Dynamic Linq 库执行查询

            if (!string.IsNullOrEmpty(model.ColumnSummary.ToString()))
            {
                string order = model.ColumnSummary.ToString();

                ColumnList = Data.AsQueryable().GroupBy($"{ColumnSummary.ToString()}", "it").Select(colunm).OrderBy(order).ToDynamicList();
            }
            else
            {
                //无列汇总方式则全显
                ColumnList = Data;
            }
            #endregion
            #region 行汇总数据项
            //行汇总数据项
            var RowColumnList = new List<dynamic>();
            var RowColumnSummary = GetColumnSummaryHandler(model.RowColumnSummary.ToString());
            string rowColunm = GetNameBySummary(model.RowColumnSummary.ToString());
            // 使用 Dynamic Linq 库执行查询
            if (!string.IsNullOrEmpty(model.RowColumnSummary.ToString()))
            {
                string order = model.RowColumnSummary.ToString();
                RowColumnList = Data.AsQueryable().GroupBy($"{RowColumnSummary.ToString()}", "it").Select(rowColunm).OrderBy(order).ToDynamicList();
            }
            #endregion
            #region 数据
            //存在行汇总方式时
            List<dynamic> List = new List<dynamic>();
            //获取行汇总项目 所选项
            foreach (object item in RowColumnList)
            {
                //获取行名称
                string propName = model.RowColumnSummary.ToString();
                var value = item.GetType().GetProperty(propName).GetValue(item);
                //填充到数据集标识头
                dynamic temp = new { RowName = value, Details = new List<dynamic>() };
                string order = model.RowColumnSummary.ToString();
                //获取列项目字符串
                string colSummary = model.ColumnSummary.ToString();
                //获取行项目字符串
                string rowColSummary = model.ColumnSummary.ToString();
                //where 过滤 行项，dynamic LINQ 分组 列数据 得到最终数据
                var Details = Data.AsQueryable().Where($"{propName}.ToString() == \"{value}\"").GroupBy($"{ColumnSummary.ToString()}", "it").ToDynamicList();
                foreach (var group in Details)
                {
                    // 使用 dynamic 类型访问 Grouping<TKey, TElement> 中的 Key 和对应的元素集合
                    dynamic key = group.Key;
                    IEnumerable<dynamic> elements = group;

                    // 使用 dynamic 类型计算 Credit 字段的总和
                    var Credit = elements.Sum(m => (decimal)m.Credit);
                    var Debit = elements.Sum(m => (decimal)m.Debit);
                    //临时变量
                    var Gtemp = elements.FirstOrDefault();
                    Gtemp.Credit = Credit;
                    Gtemp.Debit = Debit;
                    //借-贷
                    Gtemp.Amount = Debit - Credit;
                    #region 追加人员数量 人均费用
                    //部门 > 部门组织 > 单位 > 单位组织 >
                    //科目 = 全量，月付 = 按月取人数
                    if (colSummary.IndexOf("Market") > -1 || colSummary.IndexOf("Market") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty("MarketID") == Gtemp.GetType().GetProperty("MarketID"));
                    }
                    else if (colSummary.IndexOf("org_en") > -1 || colSummary.IndexOf("org_en") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty("org_enid1") == Gtemp.GetType().GetProperty("org_enid1"));
                    }
                    else if (colSummary.IndexOf("org_market") > -1 || colSummary.IndexOf("org_market") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty("org_marketid1") == Gtemp.GetType().GetProperty("org_marketid1"));
                    }

                    else if (colSummary.IndexOf("AccSubject") > -1 || colSummary.IndexOf("AccSubject") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count();
                    }
                    else
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty(propName) == Gtemp.GetType().GetProperty(propName));
                    }
                    Gtemp.PersonCost = Gtemp.Amount / (Gtemp.PersonCount == 0 ? 1 : Gtemp.PersonCount);
                    //同期数据
                    if (Convert.ToBoolean(model?.IsLastData))
                    {
                        SetLastYearData(LastData, ColumnSummary, key, Gtemp, $"{propName}.ToString() == \"{value}\"");
                    }
                    #endregion
                    temp.Details.Add(Gtemp);
                }
                List.Add(temp);
            }
            if (RowColumnList.Count == 0)
            {
                //dynamic LINQ 分组 列数据 得到最终数据
                dynamic temp = new { Details = new List<dynamic>() };
                var Details = Data.AsQueryable().GroupBy($"{ColumnSummary.ToString()}", "it").ToDynamicList();
                //获取列项目字符串
                string colSummary = model.ColumnSummary.ToString();
                //数据金额汇总
                foreach (var group in Details)
                {
                    // 使用 dynamic 类型访问 Grouping<TKey, TElement> 中的 Key 和对应的元素集合
                    dynamic key = group.Key;
                    IEnumerable<dynamic> elements = group;

                    // 使用 dynamic 类型计算 Credit 字段的总和
                    var Credit = elements.Sum(m => (decimal)m.Credit);
                    var Debit = elements.Sum(m => (decimal)m.Debit);
                    //临时变量
                    var Gtemp = elements.FirstOrDefault();
                    Gtemp.Credit = Credit;
                    Gtemp.Debit = Debit;
                    //借-贷
                    Gtemp.Amount = Debit - Credit;
                    #region 追加人员数量  人均费用
                    //部门 > 部门组织 > 单位 > 单位组织 >
                    //科目 = 全量，月付 = 按月取人数
                    if (colSummary.IndexOf("Market") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty("MarketID") == Gtemp.GetType().GetProperty("MarketID"));
                    }
                    else if (colSummary.IndexOf("org_en") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty("org_enid1") == Gtemp.GetType().GetProperty("org_enid1"));
                    }
                    else if (colSummary.IndexOf("org_market") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty("org_marketid1") == Gtemp.GetType().GetProperty("org_marketid1"));
                    }

                    else if (colSummary.IndexOf("AccSubject") > -1)
                    {
                        Gtemp.PersonCount = PersonInfos.Count();
                    }
                    else
                    {
                        Gtemp.PersonCount = PersonInfos.Count(m => m.GetType().GetProperty("Month") == Gtemp.GetType().GetProperty("Month"));
                    }
                    Gtemp.PersonCost = Gtemp.Amount / (Gtemp.PersonCount == 0 ? 1 : Gtemp.PersonCount);
                    //同期数据
                    if (Convert.ToBoolean(model?.IsLastData))
                    {
                        SetLastYearData(LastData, ColumnSummary, key, Gtemp);
                    }
                    #endregion
                    temp.Details.Add(Gtemp);
                }
                List.Add(temp);
            }
            #endregion
            //列汇总项赋值
            result.ColumnList = ColumnList.OrderBy(m => m.OrderNum);
            //行汇总项赋值
            result.RowColumnList = RowColumnList;
            //数据赋值
            result.DataList = List;
            string[] ColumnSummaryList = model.ColumnSummary.ToString().Split(",");
            //最终数据 排序好的数据 替换原始数据
            List<dynamic> ResultDataList = new List<dynamic>();
            #region 补全数据
            //补全数据
            foreach (var item in result.DataList)
            {
                //补全数据
                List<dynamic> Details = item.Details;
                if (Details.Count != ColumnList.Count)
                {
                    foreach (var items in ColumnList)
                    {
                        if (Details.Where(m => m.OrderNum == items.OrderNum).Count() == 0)
                        {
                            //反射
                            object TempItems = items;
                            IDictionary<string, object> keyValuePairs = (IDictionary<string, object>)Details.FirstOrDefault();
                            dynamic newData = new ExpandoObject();
                            //补充 剩余属性 保持一致性
                            //是否赋值
                            foreach (var pitem in keyValuePairs.Keys)
                            {
                                bool IsBreak = false;
                                //过滤掉列汇总属性
                                for (int i = 0; i < TempItems.GetType().GetProperties().Count(); i++)
                                {
                                    var xitem = TempItems.GetType().GetProperties()[i];
                                    //如果当前属性跟汇总方式一样，则赋值汇总方式的值
                                    if (xitem.Name == pitem)
                                    {
                                        var xvalueToSet = TempItems.GetType().GetProperty(xitem.Name).GetValue(TempItems);
                                        _context.AddProperty(newData, pitem, xvalueToSet);
                                        //阻止二次覆盖
                                        IsBreak = true;
                                        break;
                                    }
                                }
                                if (!IsBreak)
                                {
                                    var valueToSet = keyValuePairs[pitem];
                                    _context.AddProperty(newData, pitem, valueToSet);
                                }
                            }
                            Details.Add(newData);
                        }
                    }
                }
                //生成小计
                if (ColumnSummaryList.Count() > 1)
                {
                    var tempSummary = "";
                    //存储小计 算完小计 在追加到 数据中
                    List<dynamic> jList = new List<dynamic>();
                    for (int i = 0; i < ColumnSummaryList.Count() - 1; i++)
                    {
                        if (string.IsNullOrEmpty(tempSummary))
                        {
                            //获取倒数第二个汇总方式，永远不取倒数第一的数据
                            foreach (var citem in ColumnSummaryList)
                            {
                                if (citem == ColumnSummaryList.LastOrDefault())
                                {
                                    break;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(tempSummary))
                                    {
                                        tempSummary = citem;
                                    }
                                    else
                                    {
                                        tempSummary += "," + citem;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var tempArry = tempSummary.Split(',');
                            tempSummary = "";
                            foreach (var citem in tempArry)
                            {
                                if (citem == tempArry.LastOrDefault())
                                {
                                    break;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(tempSummary))
                                    {
                                        tempSummary = citem;
                                    }
                                    else
                                    {
                                        tempSummary += "," + citem;
                                    }
                                }
                            }
                        }
                        string tempstrColumn = GetPropertyName(Details.FirstOrDefault());
                        string tempGroupBySummary = GetColumnSummaryHandler(tempSummary);
                        //tempstrColumn = tempstrColumn.Replace("it.FirstOrDefault()." + ColumnSummaryList.LastOrDefault(), $"{ColumnSummaryList.LastOrDefault()} = 小计 ");
                        var tempResult = Details.AsQueryable().GroupBy($"{tempGroupBySummary}", "it").ToDynamicList();
                        if (tempResult.Count > 0)
                        {
                            //小计专用小数点位数
                            foreach (var group in tempResult)
                            {
                                int index = 1;
                                // 使用 dynamic 类型访问 Grouping<TKey, TElement> 中的 Key 和对应的元素集合
                                dynamic key = group.Key;
                                IEnumerable<dynamic> elements = group;
                                var jObj = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(elements.FirstOrDefault()));
                                if (jObj != null)
                                {
                                    #region 重构OrerNum排序 以便于小计排序
                                    foreach (var groupItem in group)
                                    {
                                        if (groupItem.OrderNum == 0)
                                        {
                                            groupItem.OrderNum = Convert.ToDouble(Details.Max(m => m.OrderNum) + 1);
                                        }
                                    }
                                    #endregion
                                    // 使用 dynamic 类型计算 Credit 字段的总和
                                    var Credit = elements.Sum(m => (decimal)m.Credit);
                                    var Debit = elements.Sum(m => (decimal)m.Debit);
                                    _context.SetProperty(jObj, ColumnSummaryList[ColumnSummaryList.Count() - 1 - i], "小计");
                                    jObj.IsSum = 1;
                                    jObj.Credit = elements.Sum(m => (decimal)m.Credit);
                                    jObj.Debit = elements.Sum(m => (decimal)m.Debit);
                                    jObj.Amount = Debit - Credit;
                                    jObj.PersonCost = jObj.Amount / (jObj.PersonCount == 0 ? 1 : jObj.PersonCount);
                                    jObj.OrderNum = Convert.ToDouble(((IGrouping<dynamic, dynamic>)group).LastOrDefault().OrderNum + "." + index);
                                    if (Convert.ToBoolean(model?.IsLastData))
                                    {
                                        jObj.LastAmount = elements.Sum(m => (decimal)m.LastAmount);
                                        //增长率 本期/上期-1
                                        //jObj.LastAmount = elements.Sum(m => (decimal)m.Amount) / (elements.Sum(m => (decimal)m.LastAmount) == 0 ? 1 : elements.Sum(m => (decimal)m.LastAmount)) - 1;
                                    }
                                    jList.Add(jObj);
                                    index++;
                                }
                            }
                        }
                    }
                    //将已经算好的小计 追加到数据中
                    Details.AddRange(jList);
                    //排序
                    ResultDataList.Add(new { RowName = result.RowColumnList.Count == 0 ? "" : item.RowName, Details = ((List<dynamic>)item.Details).OrderBy(m => m.OrderNum).ToDynamicList() });
                }
            }
            #endregion
            #region 增加行合计
            if (ResultDataList.Count > 0)
            {
                foreach (var item in ResultDataList.FirstOrDefault().Details)
                {
                    //行合计明细
                    dynamic keyValuePairs = new ExpandoObject();
                    //追加实体 保持一致
                    foreach (var prop in ((object)item).GetType().GetProperties())
                    {
                        keyValuePairs[prop.Name] = prop.GetValue(item);
                    }
                    if (item.IsSum == 1)
                    {

                    }
                }
                result.DataList = ResultDataList;
            }
            #endregion
            return result;
        }
        /// <summary>
        /// 设置去年数据计算
        /// </summary>
        /// <param name="LastData">同期数据源</param>
        /// <param name="ColumnSummary">列汇总项</param>
        /// <param name="key">键值对</param>
        /// <param name="Gtemp">生成新的数据模型</param>
        /// <param name="where">where 筛选条件 如有行汇总项时 出现</param>
        public void SetLastYearData(List<dynamic> LastData, dynamic ColumnSummary, dynamic key, dynamic Gtemp,string where = "")
        {
            var tempLast = (string.IsNullOrEmpty(where)) ? LastData.AsQueryable().GroupBy($"{ColumnSummary.ToString()}", "it").ToDynamicList() : LastData.AsQueryable().Where(where).GroupBy($"{ColumnSummary.ToString()}", "it").ToDynamicList();
            if (tempLast.Count > 0)
            {
                foreach (var tempItemp in tempLast)
                {
                    if (tempItemp.Key.ToString() == key.ToString())
                    {
                        // 使用 dynamic 类型访问 Grouping<TKey, TElement> 中的 Key 和对应的元素集合
                        IEnumerable<dynamic> tempElements = tempItemp;
                        Gtemp.LastAmount = tempElements.Sum(m => (decimal)m.Debit) - tempElements.Sum(m => (decimal)m.Credit);
                        Gtemp.LastRate = Gtemp.Amount / ((Gtemp.LastAmount == 0 ? 2 : Gtemp.LastAmount == 1 ? 2 : Gtemp.LastAmount) - 1);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 获取凭证数据 动态 部门 科目
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PersonSql"></param>
        /// <param name="Data"></param>
        public void GetSettleData(dynamic model, out string PersonSql, out List<dynamic> Data)
        {
            #region 获取凭证主体数据源（主键）
            string NumSQL = $@" SELECT v.NumericalOrder from nxin_qlw_business.fd_settlereceipt v 
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            INNER JOIN qlw_nxin_com.biz_accosubject acc on acc.accosubjectid = vd.accosubjectid and LEFT(acc.AccoSubjectCode,4) IN ('6601', '6602', '6603', '5101', '5301')
            where v.EnterpriseID IN({model.EnterpriseIds}) and v.DataDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}' ";
            if (!string.IsNullOrEmpty(model.AccoSubjectID.ToString()))
            {
                NumSQL += " AND ( ";
                var index = 0;
                foreach (var item in ((string)model.AccoSubjectID).Split(','))
                {
                    if (index == 0)
                    {
                        NumSQL += @$" acc.AccoSubjectCode LIKE CONCAT({item},'%') ";
                    }
                    else
                    {
                        NumSQL += @$" OR acc.AccoSubjectCode LIKE CONCAT({item},'%') ";
                    }
                    index++;
                }
                NumSQL += " ) ";
            }
            //获取符合条件的流水 作用于WHERE 主键筛查
            var KeyList = _context.DynamicSqlQuery(NumSQL);
            //主键流水逗号分隔
            string Keys = "-10";
            if (KeyList.Count > 0)
            {
                Keys = string.Join(",", KeyList.Select(m => m.NumericalOrder));
            }
            #endregion
            #region 其他字段属性（凭证主体表）列汇总方式
            string column = $@" hr.PersonID,hr.Name as PersonName,v.EnterpriseID,bz.EnterpriseName,m.MarketID,m.MarketName,acc.AccoSubjectID,acc.AccoSubjectName,acc.Rank,m.cFullName MarketName,(IFNULL(vd.Debit,0.00)) Debit,(IFNULL(vd.Credit,0.00)) Credit,LEFT(v.DataDate,7) Month ";
            #endregion
            #region 获取员工数据
            PersonSql = $@"  Select hr_pdd.PersonNumber, hr_ps.personid,  t3.ChildValue as UserID,hr_ps.Name AS PersonName,biz_m.MarketName,biz_m.MarketID,bz.EnterpriseID ,bz.EnterpriseName,tt.PositionName ,
              hrj.JobName AS PostName,hr_p.RankName,hr_p.RankID,tt.PositionID,hrpr.PostID,hrj.JobID,ttt.ID as PositionTypeID ,hr_ps.Gender, hr_pdd.JoinDate,left(hr_pdd.JoinDate,7) JoinMonth,hr_ps.MobilePhone as CellPhone ,hr_ps.Photo
            From qlw_nxin_com.HR_Person hr_ps 
            INNER JOIN   qlw_nxin_com.HR_PostInformation hrpr  on hr_ps.PersonID=hrpr.PersonID and hrpr.IsUse=1
            inner  join qlw_nxin_com.HR_PersonDetail hr_pdd on hr_pdd.PersonID=hr_ps.PersonID   and hr_pdd.PersonType=201610220104402102
            INNER  Join   qlw_nxin_com.BIZ_Enterprise bz on bz.EnterpriseID=hr_pdd.EnterpriseID
            INNER join qlw_nxin_com.BIZ_Related as t3 on hr_ps.PersonID=t3.ParentValue and t3.RelatedType=201610210104402102 and t3.ParentType=201610200104402122 and t3.ChildType=201610200104402102
            Left Join  qlw_nxin_com.HRPersonRank hr_p on hr_p.RankID=hrpr.RankID
            left join qlw_nxin_com.BIZ_Market biz_m on biz_m.MarketID=hrpr.MarketID
            Left Join qlw_nxin_com.HRJob hrj on hrj.JobID=hrpr.PostID
            Left  join qlw_nxin_com.HRPosition tt on tt.PositionID=hrpr.PositionID
            left join qlw_nxin_com.HRPositionType ttt on ttt.ID=tt.PositionTypeID
            WHERE hr_pdd.PersonState <> 201610290104402103 AND hr_pdd.EnterpriseID IN ({model.EnterpriseIds}) AND hr_pdd.JoinDate BETWEEN '{model.BeginDate}' AND '{model.EndDate}' ";
            #endregion
            #region 获取最大部门级次 MarketSql
            var MarketRank = GetMaxMarketRank(model);
            string MarketSql = "";
            //部门 级次 SQL 切割
            for (int i = 1; i <= MarketRank; i++)
            {
                MarketSql += $@" SUBSTRING_INDEX(SUBSTRING_INDEX(m.cFullName, '/', {i}), '/', -1) AS Market_{i},
                               SUBSTRING_INDEX(SUBSTRING_INDEX(m.cAxis, '$', {i+1}), '$', -1) AS Market_{i}ID, ";
            }
            #endregion
            #region 获取最大科目级次 AccSubjectSql
            var AccSubjectRank = GetMaxAccSubjectRank(model);
            string AccSubjectSql = "";
            //部门 级次 SQL 切割
            for (int i = 1; i <= AccSubjectRank; i++)
            {
                AccSubjectSql += $@" SUBSTRING_INDEX(SUBSTRING_INDEX(acc.AccoSubjectFullName, '/', {i}), '/', -1) AS AccSubject_{i},
                                    SUBSTRING_INDEX(SUBSTRING_INDEX(acc.cAxis, '$', {i+1}), '$', -1) AS AccSubject_{i}ID,";
            }
            #endregion
            //最终SQL 部门切割+科目切割+其他字段属性
            string finlaySql = $@"SELECT {MarketSql + AccSubjectSql + column} 
            from nxin_qlw_business.fd_settlereceipt v
            inner join nxin_qlw_business.fd_settlereceiptdetail vd on vd.numericalorder = v.numericalorder
            left join qlw_nxin_com.biz_market m on m.MarketID = vd.MarketID and m.EnterpriseID = v.EnterpriseID
            INNER  Join   qlw_nxin_com.BIZ_Enterprise bz on bz.EnterpriseID=v.EnterpriseID
            inner join qlw_nxin_com.biz_accosubject acc on acc.AccoSubjectID = vd.AccoSubjectID and LEFT(acc.AccoSubjectCode,4) IN ('6601', '6602', '6603', '5101', '5301')
            left join qlw_nxin_com.HR_Person hr on hr.personid = vd.personid 
            where v.NumericalOrder IN ({Keys})
            GROUP BY vd.RecordID 
            order by v.NumericalOrder desc;
            ";
            //主体数据
            Data = _context.DynamicSqlQuery(finlaySql);
        }

        /// <summary>
        /// 读取动态属性 用于 dynamicLINQ 查询列
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetPropertyName(object model)
        {
            if (model == null)
            {
                return "";
            }
            IDictionary<string, object> dictionary = (IDictionary<string, object>)model;
            string str = "new ( ";
            foreach (var item in dictionary.Keys)
            {
                //手动计算 dynamic LINQ 对 dynamic 集合进行 sum 会抛出异常（也可能是写法有问题）
                if (item == "Credit" || item == "Debit")
                {
                    //str += $"it.Sum(x => x.{item}) AS {item}" + ",\n";
                }
                else
                {
                    str += "it.FirstOrDefault()." + item + " AS " + item + ",\n";
                }
            }
            str += " )";
            return str;
        }
        /// <summary>
        /// 通过汇总方式返回对应的属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetNameBySummary(string model)
        {
            string str = "new ( ";
            foreach (var item in model.Split(","))
            {
                str += "it.FirstOrDefault()." + item + " AS " + item + ",\n";
            }
            str += " )";
            return str;
        }
        /// <summary>
        /// 列汇总方式 处理
        /// 用于 dynamic LINQ
        /// 例子：org_en1,org_en2,org_en3 = new (org_en1 as org_en1,org_en2 as org_en2,org_en3 as org_en3)
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetColumnSummaryHandler(string column)
        {
            if (!string.IsNullOrEmpty(column))
            {
                string str = "new ( ";
                foreach (var item in column.Split(','))
                {
                    str += item + " as " + item + " ,\n";
                }
                str += " ) ";
                return str;
            }
            return " new (org_en3 as org_en3) ";
        }
        /// <summary>
        /// 获取权限单位最大部门级次
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public dynamic GetMaxMarketRank(dynamic model)
        {
            #region 获取最大部门级次 MarketSql
            string MaxRankMarket = $@"SELECT MAX(Rank) Rank from qlw_nxin_com.biz_market where EnterpriseID IN ({model.EnterpriseIds})";
            var MarketRank = _context.DynamicSqlQuery(MaxRankMarket).FirstOrDefault().Rank;
            return MarketRank;
            #endregion
        }
        public dynamic GetMaxAccSubjectRank(dynamic model)
        {
            #region 获取最大科目级次 AccSubjectSql
            string MaxRankAccSubject = $@"SELECT MAX(Rank) Rank from qlw_nxin_com.biz_accosubject where EnterpriseID IN ({model.EnterpriseIds})";
            var AccSubjectRank = _context.DynamicSqlQuery(MaxRankAccSubject).FirstOrDefault().Rank;
            return AccSubjectRank;
            #endregion
        }

        /// <summary>
        /// 单位组织，创业单元过滤
        /// </summary>
        /// <param name="search"></param>
        /// <param name="tempList"></param>
        /// <returns></returns>
        private static List<dynamic> OrgEnterFilter(dynamic search, List<dynamic> tempList)
        {
            if (search.EnterSortIds != null)
            {
                if (!string.IsNullOrEmpty(search.EnterSortIds.ToString()))
                {
                    var eSortWhere = ((string)search.EnterSortIds).Split(',').Select(e => Convert.ToInt64(e)).ToList();
                    var TempArry = new List<dynamic>();
                    foreach (var item in eSortWhere)
                    {
                        TempArry.AddRange(tempList.Where(m => m.org_encAxis.Contains(item.ToString())).ToList());
                    }
                    tempList = TempArry;
                }
            }
            if (search.MarketSortIds != null)
            {
                if (!string.IsNullOrEmpty(search.MarketSortIds.ToString()))
                {
                    var mSortWhere = ((string)search.MarketSortIds).Split(',').Select(e => Convert.ToInt64(e)).ToList();
                    var TempArry = new List<dynamic>();
                    foreach (var item in mSortWhere)
                    {
                        TempArry.AddRange(tempList.Where(m => m.bs_encAxis.Contains(item.ToString())).ToList());
                    }
                    tempList = TempArry;
                }
            }
            return tempList;
        }

        /// <summary>
        /// 设置单位组织名称，创业单元
        /// 动态追加属性
        /// </summary>
        /// <param name="enSort">单位组织</param>
        /// <param name="maSort">创业单元</param>
        /// <param name="item">要处理的数据</param>
        private void SetSortNameOrganizationName(List<SortInfo> enSort, List<SortInfo> maSort, dynamic item)
        {
            //单位组织
            {
                var endata = enSort.FirstOrDefault(m => m.SortId.ToString() == item.EnterpriseID);
                item.org_en = endata?.cfullname;
                item.org_encAxis = endata?.cAxis;
                if (string.IsNullOrEmpty(item.org_encAxis))
                {
                    item.org_encAxis = "";
                }
                if (!string.IsNullOrEmpty(item.org_en) && item.org_en.Contains('/'))
                {
                    var esplit = item.org_en.Split('/');
                    if (!string.IsNullOrEmpty(endata.cAxis) && endata.cAxis.Substring(0, 1) == "$")
                    {
                        endata.cAxis = endata.cAxis.Remove(0, 1);
                        endata.cAxis = endata.cAxis.Remove(endata.cAxis.Length - 1, 1);
                    }
                    var esplitid = endata.cAxis.Split('$');
                    for (int i = 0, n = esplit.Length; i < n; i++)
                    {
                        _context.AddProperty(item, "org_en" + (i + 1), esplit[i]);
                        _context.AddProperty(item, "org_enid" + (i + 1), esplitid[i]);
                    }
                }
                else
                {
                    for (int i = 0, n = 10; i < n; i++)
                    {
                        _context.AddProperty(item, "org_en" + (i + 1), "");
                        _context.AddProperty(item, "org_enid" + (i + 1), i);
                    }
                }
            }
            //部门组织↓
            {
                var sortdata = maSort.FirstOrDefault(m => m.Id.ToString() == item.MarketID);
                item.SortName = sortdata?.cfullname;
                item.bs_encAxis = sortdata?.cAxis;
                item.OrganizationSortID = sortdata?.SortId.ToString();
                if (string.IsNullOrEmpty(item.bs_encAxis))
                {
                    item.bs_encAxis = "";
                }
                if (!string.IsNullOrEmpty(item.SortName) && item.SortName.Contains('/'))
                {
                    var msplit = item.SortName.Split('/');
                    if (!string.IsNullOrEmpty(sortdata.cAxis) && sortdata.cAxis.Substring(0, 1) == "$")
                    {
                        sortdata.cAxis = sortdata.cAxis.Remove(0, 1);
                        sortdata.cAxis = sortdata.cAxis.Remove(sortdata.cAxis.Length - 1, 1);
                    }
                    var msplitid = sortdata.cAxis.Split('$');
                    for (int i = 0, n = msplit.Length; i < n; i++)
                    {
                        _context.AddProperty(item, "org_market" + (i + 1), msplit[i]);
                        _context.AddProperty(item, "org_marketid" + (i + 1), msplitid[i]);
                    }
                }
                else
                {
                    for (int i = 0, n = 10; i < n; i++)
                    {
                        _context.AddProperty(item, "org_market" + (i + 1), "");
                        _context.AddProperty(item, "org_marketid" + (i + 1), i);
                    }
                }
            }
        }
        /// <summary>
        /// 获取单位组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> EnterSortInfo(string GroupId = "")
        {
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterpriseId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    InheritanceId = "636346361736274263",
                    ScopeData = new int[] { 1,2},
                    IsUse = 1,
                    IsTop = 1,
                    IsMaster = 0,
                    IsExpand = 1
                }).Result;
            return data.data;
        }
        /// <summary>
        /// 获取部门组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> OrgSortInfo(string GroupId = "")
        {
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterpriseId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    InheritanceId = "636181287875447931",
                    IsUse = 1,
                    IsTop = 1,
                    IsMaster = 0,
                    IsExpand = 1
                }).Result;
            foreach (var item in data.data)
            {
                if (string.IsNullOrEmpty(item.Id))
                {
                    item.Id = "-1";
                }
            }
            return data.data.Where(m=>m.sortRank == 1).ToList();
        }
    }
}
