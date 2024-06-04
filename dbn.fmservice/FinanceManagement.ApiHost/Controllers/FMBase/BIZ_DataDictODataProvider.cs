using Architecture.Common.Application.Query;
using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Controllers.FD_VoucherAmortization;
using FinanceManagement.ApiHost.Controllers.PerformanceIncome;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{
    public class BIZ_DataDictODataProvider
    {

        private IIdentityService _identityservice;
        private QlwCrossDbContext _context;
        HttpClientUtil _httpClientUtil;
        HostConfiguration _hostCongfiguration;


        public BIZ_DataDictODataProvider(HostConfiguration hostCongfiguration, HttpClientUtil httpClientUtil, IIdentityService identityservice, QlwCrossDbContext context)
        {
            _identityservice = identityservice;
            _context = context;
            _httpClientUtil = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }
        /// <summary>
        /// 获取字典表数据
        /// </summary>
        /// <returns></returns>
        public Task<List<BIZ_DataDictODataEntity>> GetDataDictAsync(string pid)
        {
            try
            {
                FormattableString sql = $@"SELECT  CONVERT(DataDictID USING utf8mb4) DataDictID,DataDictName,	
                                        CONVERT(DataDictType USING utf8mb4) DataDictType,	
                                        CONVERT(PID USING utf8mb4) PID,	
                                        CONVERT(EnterpriseID USING utf8mb4) EnterpriseID,
                                        CONVERT(CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(ModifiedDate USING utf8mb4) ModifiedDate,'' as cPrivCode
                                        FROM `nxin_qlw_business`.`biz_datadict`
                                        WHERE PID={pid}";
                return _context.BIZ_DataDictDataSet.FromSqlInterpolated(sql).ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取字典表数据
        /// </summary>
        /// <returns></returns>
        public List<BIZ_DataDictODataEntity> GetDataDictAsyncDisposed(string pid)
        {
            try
            {
                FormattableString sql = $@"SELECT  CONVERT(DataDictID USING utf8mb4) DataDictID,DataDictName,	
                                        CONVERT(DataDictType USING utf8mb4) DataDictType,	
                                        CONVERT(PID USING utf8mb4) PID,	
                                        CONVERT(EnterpriseID USING utf8mb4) EnterpriseID,
                                        CONVERT(CreatedDate USING utf8mb4) CreatedDate,
                                        CONVERT(ModifiedDate USING utf8mb4) ModifiedDate,'' as cPrivCode
                                        FROM `nxin_qlw_business`.`biz_datadict`
                                        WHERE PID={pid}";
                return _context.BIZ_DataDictDataSet.FromSqlInterpolated(sql).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取字典表数据
        /// </summary>
        /// <returns></returns>
        public Task<List<BIZ_DataDictODataEntity>> GetDataDictAsyncExtend(string pid)
        {
            FormattableString sql = $@"SELECT  CONVERT(DictID USING utf8mb4) DataDictID,cDictName AS DataDictName,	
                                        CONVERT(cDictType USING utf8mb4) DataDictType,	
                                        CONVERT(Pid USING utf8mb4) PID,	
                                        CONVERT(cPrivCode USING utf8mb4) cPrivCode,
                                        CONVERT(EnterID USING utf8mb4) EnterpriseID,
                                        '' AS  CreatedDate,
                                        '' AS  ModifiedDate
                                        FROM `qlw_nxin_com`.BSDataDict
                                        WHERE PID={pid}";
            return _context.BIZ_DataDictDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        /// <summary>
        /// 获取字典表数据转下拉集合
        /// </summary>
        /// <returns></returns>
        public List<TreeModelODataEntity> GetDataDictConvertDrop(string pid)
        {
            List<TreeModelODataEntity> model = new List<TreeModelODataEntity>();
            FormattableString sql = $@"SELECT  CONVERT(DictID USING utf8mb4) DataDictID,cDictName AS DataDictName,	
                                        CONVERT(cDictType USING utf8mb4) DataDictType,	
                                        CONVERT(Pid USING utf8mb4) PID,	
                                        CONVERT(cPrivCode USING utf8mb4) cPrivCode,
                                        CONVERT(EnterID USING utf8mb4) EnterpriseID,
                                        '' AS  CreatedDate,
                                        '' AS  ModifiedDate
                                        FROM `qlw_nxin_com`.BSDataDict
                                        WHERE PID={pid}";
            var data= _context.BIZ_DataDictDataSet.FromSqlInterpolated(sql).ToList();
            foreach (var item in data)
            {
                model.Add(new TreeModelODataEntity() { Id = item.DataDictID, cName = item.DataDictName });
            }
            return model;
        }
        /// <summary>
        /// 获取字典表数据
        /// </summary>
        /// <returns></returns>
        public List<BIZ_DataDictODataEntity> GetDataDictDisposed(string pid)
        {
            FormattableString sql = $@"SELECT  CONVERT(DictID USING utf8mb4) DataDictID,cDictName AS DataDictName,	
                                        CONVERT(cDictType USING utf8mb4) DataDictType,	
                                        CONVERT(Pid USING utf8mb4) PID,	
                                        CONVERT(cPrivCode USING utf8mb4) cPrivCode,
                                        CONVERT(EnterID USING utf8mb4) EnterpriseID,
                                        '' AS  CreatedDate,
                                        '' AS  ModifiedDate
                                        FROM `qlw_nxin_com`.BSDataDict
                                        WHERE PID={pid}";
            var data= _context.BIZ_DataDictDataSet.FromSqlInterpolated(sql).ToList();
            _context.Dispose();
            return data;
        }
        /// <summary>
        /// 获取集团下所有单位
        /// </summary>
        /// <returns></returns>
        public Task<List<Biz_EnterpirseEntityODataEntity>> GetEnterpriseListAsync(bool isgroup)
        {
            if (isgroup)
            {
                FormattableString sql = $@"SELECT RecordID,
                                        CONVERT(EnterpriseID USING utf8mb4) EnterpriseID,	
                                        CONVERT(EnterpriseName USING utf8mb4) EnterpriseName,	
                                        CONVERT(EnterpriseFullName USING utf8mb4) EnterpriseFullName,	
                                        CONVERT(PID USING utf8mb4) PID,	
                                        CONVERT(AreaID USING utf8mb4) AreaID,	
                                        CONVERT(BeginDate USING utf8mb4) BeginDate,	
                                        CONVERT(Remarks USING utf8mb4) Remarks
                                        FROM `qlw_nxin_com`.`biz_enterprise`
                                        WHERE PID={_identityservice.GroupId} AND IsUse=TRUE";
                return _context.Biz_EnterpirseEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
            }
            else
            {
                FormattableString sql = $@"SELECT RecordID,
                                        CONVERT(EnterpriseID USING utf8mb4) EnterpriseID,	
                                        CONVERT(EnterpriseName USING utf8mb4) EnterpriseName,	
                                        CONVERT(EnterpriseFullName USING utf8mb4) EnterpriseFullName,	
                                        CONVERT(PID USING utf8mb4) PID,	
                                        CONVERT(AreaID USING utf8mb4) AreaID,	
                                        CONVERT(BeginDate USING utf8mb4) BeginDate,	
                                        CONVERT(Remarks USING utf8mb4) Remarks
                                        FROM `qlw_nxin_com`.`biz_enterprise`
                                        WHERE EnterpriseID={_identityservice.EnterpriseId} AND IsUse=TRUE";
                return _context.Biz_EnterpirseEntityDataSet.FromSqlInterpolated(sql).ToListAsync();
            }
            
        }

        /// <summary>
        /// 获取字典表数据-费用预置项目DropList
        /// </summary>
        /// <returns></returns>
        public Task<List<BIZ_DataDictODataEntity>> GetDataDictAsyncForPreSetItem(string pid,string enterpriseId)
        {
            FormattableString sql = $@"SELECT  CONVERT(DictID USING utf8mb4) DataDictID,cDictName AS DataDictName,	
                                        CONVERT(cDictType USING utf8mb4) DataDictType,	
                                        CONVERT(Pid USING utf8mb4) PID,	
                                        CONVERT(cPrivCode USING utf8mb4) cPrivCode,
                                        CONVERT(EnterID USING utf8mb4) EnterpriseID,
                                        '' AS  CreatedDate,
                                        '' AS  ModifiedDate
                                        FROM `qlw_nxin_com`.BSDataDict a
                                        LEFT JOIN nxin_qlw_business.`fm_costproject` b ON b.`PresetItem`=a.`DictID` AND b.`EnterpriseID`={enterpriseId} AND b.`IsUse`=1
                                        WHERE a.`Pid`={pid} AND b.`CostProjectID` IS NULL";
            return _context.BIZ_DataDictDataSet.FromSqlInterpolated(sql).ToListAsync();
        }
        public List<BalanceSheet> GetBalanceSheetReviwe(string enterpriseId,string year,string month)
        {
            string sql = @$"select concat(enterpriseid) EnterpriseId,count(re.Level) Level from nxin_qlw_business.FD_BalanceSheet bs
            inner join nxin_qlw_business.biz_reviwe re on re.NumericalOrder = bs.numericalorder 
            where enterpriseid in ({enterpriseId}) AND YEAR(datadate)={year} AND  MONTH(datadate)={month} and checkmark = 16 Group By bs.enterpriseid ";
            return _context.GetBalanceSheetReview.FromSqlRaw(sql).ToList();
        }
        /// <summary>
        /// 获取单位组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> EnterSortInfo(string GroupId = "")
        {
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuju",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterpriseId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    InheritanceId = "636346361736274263",
                    ScopeData = new int[] { 1, 2 },
                    IsUse = 1,
                    IsTop = 1,
                    IsMaster = 0,
                    IsExpand = 1
                }).Result;
            return data.data;
        }
        /// <summary>
        /// 获取部门组织/创业单元
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<SortInfo> OrgSortInfo(string GroupId = "")
        {
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuju",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterpriseId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    InheritanceId = "636181287875447931",
                    IsUse = 1,
                    IsTop = 0,
                    IsMaster = 1,
                }).Result;
            return data.data;
        }
        /// <summary>
        /// 获取部门组织
        /// 部门id找组织
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="MarketIds">部门id找组织</param>
        /// <returns></returns>
        public List<SortInfo> MarketSortInfo(string GroupId = "", string MarketIds = "")
        {
            if (string.IsNullOrEmpty(MarketIds) || MarketIds == "0")
            {
                MarketIds = "10";
            }
            var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"{_hostCongfiguration.NxinGatewayUrl}api/nxin.qlwbase.getorgusedinfo/1.0?open_req_src=nxin_shuju",
            //var data = _httpClientUtil.PostJsonAsync<dynamic, Result<List<SortInfo>>>($"http://open.i.nxin.com/inner/api/nxin.qlwbase.getorginfo/2.0?open_req_src=nxin_shuzhi",
                new
                {
                    EnterId = (string.IsNullOrEmpty(GroupId) ? _identityservice.GroupId : GroupId),
                    Inheritance = "636181287875447931",
                    Id = MarketIds,
                    IsUse = 1,
                    IsMaster = 1,
                }).Result;
            return data.data;
        }
        /// <summary>
        /// 设置单位组织名称，创业单元
        /// 动态追加属性
        /// </summary>
        /// <param name="enSort">单位组织</param>
        /// <param name="maSort">创业单元</param>
        /// <param name="item">要处理的数据</param>
        public void SetSortNameOrganizationName(List<SortInfo> enSort, List<SortInfo> maSort, dynamic item)
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
            }
        }
        /// <summary>
        /// 单位组织，创业单元过滤
        /// </summary>
        /// <param name="search"></param>
        /// <param name="tempList"></param>
        /// <returns></returns>
        public dynamic OrgEnterFilter(VoucherSearch search, List<dynamic> tempList)
        {
            if (search.EnterSortIds == null)
            {
                return tempList;
            }
            if (!string.IsNullOrEmpty(search.EnterSortIds))
            {
                var eSortWhere = (search.EnterSortIds.Split(',')).Select(e => Convert.ToInt64(e)).ToList();
                var TempArry = new List<dynamic>();
                foreach (var item in eSortWhere)
                {
                    TempArry.AddRange(tempList.Where(m => m.org_encAxis.Contains(item.ToString())).ToList());
                }
                tempList = TempArry;
            }

            if (!string.IsNullOrEmpty(search.MarketSortIds))
            {
                var mSortWhere = (search.MarketSortIds.Split(',')).Select(e => Convert.ToInt64(e)).ToList();
                var TempArry = new List<dynamic>();
                foreach (var item in mSortWhere)
                {
                    TempArry.AddRange(tempList.Where(m => m.bs_encAxis.Contains(item.ToString())).ToList());
                }
                tempList = TempArry;
            }

            return tempList;
        }
    }
}
