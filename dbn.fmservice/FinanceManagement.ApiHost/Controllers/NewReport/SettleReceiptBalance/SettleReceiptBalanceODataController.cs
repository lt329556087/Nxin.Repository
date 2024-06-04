using Architecture.Seedwork.Security;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Infrastructure;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using FinanceManagement.Common;
using System.Threading.Tasks;
using FinanceManagement.Util;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Architecture.Common.HttpClientUtil;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using System.Net.Http;
using System.Net.Http.Headers;
using Serilog;

namespace FinanceManagement.ApiHost.Controllers.SettleReceiptBalance
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DropODataController : ODataController
    {
        //FM_CashSweepODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<DropODataController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public DropODataController(IIdentityService identityService, HttpClientUtil httpClientUtil1, HostConfiguration hostCongfiguration, ILogger<DropODataController> logger, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            //_prodiver = prodiver;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil1;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="curtype">类型</param>
        /// <param name="customerName">往来单位名称</param>
        /// <returns></returns>
        [EnableQuery]
        public async Task<IQueryable<DropODataEntity>> Get(int curtype,string customerName)
        {            
            List<DropODataEntity> list = new List<DropODataEntity>();
            try
            {
                if (string.IsNullOrEmpty(customerName)) { return list.AsQueryable(); }
                var enterids = string.Empty;
                //获取权限单位
                var permissionEnteList = await GetPermissionMenuEnter(new PermissionEnter { EnterpriseID = _identityService.EnterpriseId, Bo_ID = _identityService.UserId, MenuID = _identityService.AppId });
                if (permissionEnteList?.Count > 0)
                {
                    var arry = permissionEnteList.Select(x => x.EnterpriseID).ToArray();
                    enterids = string.Join(",", arry);

                    if (string.IsNullOrEmpty(enterids)) { return list.AsQueryable(); }
                    StringValues token = new StringValues();
                    _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
                    String tokenVal = token.First();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Method = HttpMethod.Get;
                    request.Headers.Add("Authorization", tokenVal);
                    var filterStr = $"&$filter=(contains(Name,'{customerName}'))";
                    if (curtype == 1)//客商
                    {
                        var url = $"{_hostCongfiguration._wgUrl}/qlw/customer/odata/CustomerSelectOData?enterIds={enterids}{filterStr}";
                        request.RequestUri = new Uri(url);
                        var requestResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request);
                        if (requestResult?.value != null)
                        {
                            list = JsonConvert.DeserializeObject<List<DropODataEntity>>(requestResult.value.ToString());
                            list?.ForEach(x => { x.name = "客商:" + x.name; x.curtype = "1"; });
                        }
                    }
                    else if (curtype == 2)//员工
                    {
                        var url = $"{_hostCongfiguration._wgUrl}/dbn/person/oq/HrPersonByEnterIds?enterIds={enterids}{filterStr}";
                        request.RequestUri = new Uri(url);
                        var requestResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request);
                        if (requestResult?.value != null)
                        {
                            var personList = JsonConvert.DeserializeObject<List<PersonResult>>(requestResult.value.ToString());
                            if (personList?.Count > 0)
                            {
                                foreach (var person in personList)
                                {
                                    list.Add(new DropODataEntity() { id = person.PersonId, name = "员工:" + person.Name, curtype = "2" });
                                }
                            }
                        }
                    }
                    else//客商+员工
                    {
                        var cusurl = $"{_hostCongfiguration._wgUrl}/qlw/customer/odata/CustomerSelectOData?enterIds={enterids}{filterStr}";
                        request.RequestUri = new Uri(cusurl);
                        var requestResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request);
                        if (requestResult?.value != null)
                        {
                            list = JsonConvert.DeserializeObject<List<DropODataEntity>>(requestResult.value.ToString());
                            list?.ForEach(x => { x.name = "客商:" + x.name; x.curtype = "1"; });
                        }
                        var url = $"{_hostCongfiguration._wgUrl}/dbn/person/oq/HrPersonByEnterIds?enterIds={enterids}{filterStr}";
                        HttpRequestMessage request2 = new HttpRequestMessage();
                        request2.Method = HttpMethod.Get;
                        request2.Headers.Add("Authorization", tokenVal);
                        request2.RequestUri = new Uri(url);
                        var personResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request2);
                        if (personResult?.value != null)
                        {
                            var personList = JsonConvert.DeserializeObject<List<PersonResult>>(personResult.value.ToString());
                            if (personList?.Count > 0)
                            {
                                foreach (var person in personList)
                                {
                                    list.Add(new DropODataEntity() { id = person.PersonId, name = "员工:" + person.Name, curtype = "2" });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("FM_CashSweepOData/Get:{0},param={1},cusomername={2}", ex.ToString(), curtype,customerName));
            }           

            return list.AsQueryable();
        }
        #endregion
        #region old
        ///// <summary>
        ///// 列表查询
        ///// </summary>
        ///// <returns></returns>
        //[EnableQuery]
        //public async Task<IQueryable<DropODataEntity>> Get(ODataQueryOptions<DropODataEntity> odataqueryoptions, Uri uri)
        //{
        //    List<DropODataEntity> list = new List<DropODataEntity>();
        //    try
        //    {
        //        var enterids = string.Empty;
        //        //获取权限单位
        //        var permissionEnteList = await GetPermissionMenuEnter(new PermissionEnter { EnterpriseID = _identityService.EnterpriseId, Bo_ID = _identityService.UserId, MenuID = _identityService.AppId });
        //        if (permissionEnteList?.Count > 0)
        //        {
        //            var arry = permissionEnteList.Select(x => x.EnterpriseID).ToArray();
        //            enterids = string.Join(",", arry);
        //            var pageNum = 100;
        //            if (!string.IsNullOrEmpty(odataqueryoptions.RawValues.Top))
        //            {
        //                pageNum = int.Parse(odataqueryoptions.RawValues.Top);
        //            }

        //            StringValues token = new StringValues();
        //            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
        //            String tokenVal = token.First();
        //            HttpRequestMessage request = new HttpRequestMessage();
        //            request.Method = HttpMethod.Get;
        //            request.Headers.Add("Authorization", tokenVal);
        //            var curtype = 0;
        //            var typeFiledName = "curtype";
        //            if (odataqueryoptions.Filter != null && !string.IsNullOrEmpty(odataqueryoptions.Filter.RawValue))
        //            {
        //                var filterparams = odataqueryoptions.Filter.RawValue;
        //                var firstIndex = filterparams.IndexOf(typeFiledName);
        //                if (firstIndex >= 0)
        //                {
        //                    var subStr = filterparams.Substring(firstIndex, filterparams.Length - firstIndex);
        //                    var nextIndex = subStr.IndexOf("and");
        //                    var typeStr = string.Empty;
        //                    if (nextIndex >= 0)
        //                    {
        //                        typeStr = subStr.Substring(typeFiledName.Length - 1, nextIndex);
        //                    }
        //                    else
        //                    {
        //                        typeStr = subStr.Substring(typeFiledName.Length, subStr.Length - typeFiledName.Length);
        //                    }
        //                    typeStr = typeStr.Replace("eq", "");
        //                    typeStr = typeStr.Replace(")", "");
        //                    typeStr = typeStr.Replace("'", "");
        //                    try
        //                    {
        //                        curtype = int.Parse(typeStr.Trim());
        //                    }
        //                    catch { }

        //                };
        //            }
        //            if (curtype == 1)//客商
        //            {
        //                var url = $"{_hostCongfiguration._wgUrl}/qlw/customer/odata/CustomerSelectOData?enterIds={enterids}&$top={pageNum}&$count=true";
        //                request.RequestUri = new Uri(url);
        //                var requestResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request);
        //                if (requestResult?.value != null)
        //                {
        //                    list = JsonConvert.DeserializeObject<List<DropODataEntity>>(requestResult.value.ToString());
        //                    list?.ForEach(x => { x.name = "客商:" + x.name; x.curtype = "1"; });
        //                }
        //            }
        //            else if (curtype == 2)//员工
        //            {
        //                var url = $"{_hostCongfiguration._wgUrl}/dbn/person/oq/HrPersonByEnterIds?enterIds={enterids}&$top={pageNum}&$count=true";
        //                request.RequestUri = new Uri(url);
        //                var requestResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request);
        //                if (requestResult?.value != null)
        //                {
        //                    var personList = JsonConvert.DeserializeObject<List<PersonResult>>(requestResult.value.ToString());
        //                    if (personList?.Count > 0)
        //                    {
        //                        foreach (var person in personList)
        //                        {
        //                            list.Add(new DropODataEntity() { id = person.PersonId, name = "员工:" + person.Name, curtype = "2" });
        //                        }
        //                    }
        //                }
        //            }
        //            else//客商+员工
        //            {
        //                var cusurl = $"{_hostCongfiguration._wgUrl}/qlw/customer/odata/CustomerSelectOData?enterIds={enterids}&$top={pageNum}&$count=true";
        //                request.RequestUri = new Uri(cusurl);
        //                var requestResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request);
        //                if (requestResult?.value != null)
        //                {
        //                    list = JsonConvert.DeserializeObject<List<DropODataEntity>>(requestResult.value.ToString());
        //                    list?.ForEach(x => { x.name = "客商:" + x.name; x.curtype = "1"; });
        //                }
        //                var url = $"{_hostCongfiguration._wgUrl}/dbn/person/oq/HrPersonByEnterIds?enterIds={enterids}&$top={pageNum}&$count=true";
        //                HttpRequestMessage request2 = new HttpRequestMessage();
        //                request2.Method = HttpMethod.Get;
        //                request2.Headers.Add("Authorization", tokenVal);
        //                request2.RequestUri = new Uri(url);
        //                var personResult = await _httpClientUtil1.GetJsonAsync<ODataResultModel>(request2);
        //                if (personResult?.value != null)
        //                {
        //                    var personList = JsonConvert.DeserializeObject<List<PersonResult>>(personResult.value.ToString());
        //                    if (personList?.Count > 0)
        //                    {
        //                        foreach (var person in personList)
        //                        {
        //                            list.Add(new DropODataEntity() { id = person.PersonId, name = "员工:" + person.Name, curtype = "2" });
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(string.Format("FM_CashSweepOData/Get:{0},param={1}", ex.ToString(), JsonConvert.SerializeObject(odataqueryoptions)));
        //    }

        //    return list.AsQueryable();
        //}
        #endregion
        #region 权限单位
        /// <summary>
        /// 获取菜单权限单位
        /// </summary>
        private async Task<List<Biz_EnterpirseEntityODataEntity>> GetPermissionMenuEnter(PermissionEnter model)
        {
            List<Biz_EnterpirseEntityODataEntity> list = null;
            try
            {
                var url = $"{_hostCongfiguration.NxinGatewayUrl}api/nxin.permission.enterlistbymenuids.list/2.0?open_req_src=nxin_shuju&enterpriseid={model.EnterpriseID}&boid={model.Bo_ID}&menuid={model.MenuID}";
                var requestResult = await _httpClientUtil1.GetJsonAsync<RestfulResult>(url);
                if (requestResult?.code == 0 && requestResult?.data != null)
                {
                    list = JsonConvert.DeserializeObject<List<Biz_EnterpirseEntityODataEntity>>(requestResult.data.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("FM_CashSweepOData/GetPermissionMenuEnter:{0},param={1}", ex.ToString(), JsonConvert.SerializeObject(model)));
            }
            return list;
        }
        #endregion
    }
}
