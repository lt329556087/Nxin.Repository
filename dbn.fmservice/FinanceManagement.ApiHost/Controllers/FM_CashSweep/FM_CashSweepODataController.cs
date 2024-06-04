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

namespace FinanceManagement.ApiHost.Controllers.FM_CashSweep
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FM_CashSweepODataController : ODataController
    {
        FM_CashSweepODataProvider _prodiver;
        private QlwCrossDbContext _dbContext;
        private IIdentityService _identityService;
        HttpClientUtil _httpClientUtil1;
        HostConfiguration _hostCongfiguration;
        private readonly ILogger<FM_CashSweepODataController> _logger;

        public FM_CashSweepODataController(FM_CashSweepODataProvider prodiver, IIdentityService identityService, HttpClientUtil httpClientUtil1, HostConfiguration hostCongfiguration, ILogger<FM_CashSweepODataController> logger)
        {
            _prodiver = prodiver;
            _identityService = identityService;
            _httpClientUtil1 = httpClientUtil1;
            _hostCongfiguration = hostCongfiguration;
            _logger = logger;
        }

        #region 列表查询
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<IQueryable<FM_CashSweepODataEntity>> Get(ODataQueryOptions<FM_CashSweepODataEntity> odataqueryoptions, Uri uri)
        {
            List<FM_CashSweepODataEntity> list = new List<FM_CashSweepODataEntity>();
            if (odataqueryoptions.Filter == null) { return list.AsQueryable(); }
            var filterparams = odataqueryoptions.Filter.RawValue;
            if (string.IsNullOrEmpty(filterparams)) { return list.AsQueryable(); }
            if (!filterparams.Contains("DataDate")) { return list.AsQueryable(); }
            var dateStr = "";
            var firstIndex = filterparams.IndexOf("(DataDate");
            if (firstIndex < 0) return list.AsQueryable();
            var temIndex = filterparams.IndexOf("le", firstIndex);
            if (temIndex < 0) return list.AsQueryable();
            var lastIndex = filterparams.IndexOf(")", temIndex);
            if (lastIndex < 0 && lastIndex <= firstIndex) return list.AsQueryable();
            dateStr = filterparams.Substring(firstIndex, lastIndex - firstIndex + 1);
            dateStr = dateStr.Replace("ge", ">=");
            dateStr = dateStr.Replace("le", "<=");

            //获取权限单位
            var permissionEnteList = await GetPermissionMenuEnter(new PermissionEnter { EnterpriseID = _identityService.EnterpriseId, Bo_ID = _identityService.UserId, MenuID = _identityService.AppId });
            if (permissionEnteList?.Count > 0)
            {
                FM_CashSweepRequest req = _prodiver.req;
                var array = permissionEnteList.Select(p => p.EnterpriseID).ToArray();
                req.PermissionEnterpriseIDs = string.Join(",", array);
                req.DateStr = dateStr;
                list = _prodiver.GetList(odataqueryoptions, HttpContext.Request.GetNextPageLink(odataqueryoptions.Top.Value))?.ToList();
                if (list == null || list.Count == 0) { return new List<FM_CashSweepODataEntity>().AsQueryable(); }
                EncryptAccount encryptAccount = new EncryptAccount(_hostCongfiguration);

                // 批量获取单据状态
                var workflowStates = await _prodiver.GetWorkflowStatesAsync(String.Join(',', list.Select(p => p.NumericalOrder)));

                foreach (var item in list)
                {
                    _prodiver.GetAuditResult(item, workflowStates);
                    //自动归集+审批通过 获取表体账号 +账号解密
                    if (item.SweepType == "1811191754180000202")//&& item.AuditResult == "1"
                    {
                        var resultNumMain = encryptAccount.AccountNumberDecrypt(item.AccountNumber);
                        item.AccountNumber = resultNumMain?.Item1 == true ? resultNumMain.Item2 : item.AccountNumber;
                        try
                        {
                            item.AccountNumber = EncryptUtils.AesEncrypt(item.AccountNumber, _hostCongfiguration.CashsweepAesKey);
                        }
                        catch { }
                        var detailList = await _prodiver.GetDetaiDatasAsync(long.Parse(item.NumericalOrder));

                        if (detailList?.Count > 0)
                        {
                            foreach (var subItem in detailList)
                            {
                                var resultNumSub = encryptAccount.AccountNumberDecrypt(subItem.AccountNumber);
                                subItem.AccountNumber = resultNumSub?.Item1 == true ? resultNumSub.Item2 : subItem.AccountNumber;
                            }
                            var subList = detailList.Select(p => new { toAcctNo = p.AccountNumber, subEntId = p.EnterpriseID });
                            var detailAccouonts = JsonConvert.SerializeObject(subList);
                            try
                            {
                                item.DetailAccounts = EncryptUtils.AesEncrypt(detailAccouonts, _hostCongfiguration.CashsweepAesKey);
                            }
                            catch { }

                        }
                    }
                    if (item.SweepType == "-1")
                    {
                        item.SweepTypeName = "自定义";
                    }
                    if (!item.IsNew)
                    {
                        //转归集状态 新旧菜单金融接口返回状态值不同，需要转换
                        if(item.TradeResult== "归集成功")
                        {
                            item.TradeResult = "归集失败";
                        }
                        else if (item.TradeResult == "归集失败")
                        {
                            item.TradeResult = "归集成功";
                        }
                    }
                }
            }

            return list.AsQueryable();
        }
        #endregion
        #region 权限单位
        /// <summary>
        /// 获取菜单权限单位
        /// </summary>
        public async Task<List<Biz_EnterpirseEntityODataEntity>> GetPermissionMenuEnter(PermissionEnter model)
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
                _logger.LogError(string.Format("FM_CashSweepOData/GetPermissionMenuEnter:{0},param={1}", ex.ToString(), JsonConvert.SerializeObject(model)));
            }
            return list;
        }
        #endregion


    }
}
