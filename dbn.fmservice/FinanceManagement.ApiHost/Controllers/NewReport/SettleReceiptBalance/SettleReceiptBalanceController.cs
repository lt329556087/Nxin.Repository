using Architecture.Seedwork.Core;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers.FM_CashSweep;
using FinanceManagement.Common;
using FinanceManagement.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Codeless.Report;
using Newtonsoft.Json;
using FinanceManagement.ApiHost.Controllers.SettleReceiptBalance;
using Architecture.Seedwork.Security;
using Architecture.Common.HttpClientUtil;
namespace FinanceManagement.ApiHost.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SettleReceiptBalanceController : ControllerBase
    {
        IMediator _mediator;
        private readonly FinanceTradeUtil _financeTradeUtil;
        private IIdentityService _identityService;
        private HostConfiguration _hostCongfiguration;
        private HttpClientUtil _httpClientUtil1;
        public SettleReceiptBalanceController(IMediator mediator,FinanceTradeUtil financeTradeUtil, IIdentityService identityService, HostConfiguration hostCongfiguration,HttpClientUtil httpClientUtil1)
        {
            _mediator = mediator;
            _financeTradeUtil = financeTradeUtil;
            _identityService = identityService;
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil1 = httpClientUtil1;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("data")]
        //[AppCode(CommonField.MenuId)]
        [PermissionAuthorize(Permission.Retrieve)]
        public async Task<GatewayResultModel> GetData([FromBody] SettleReceiptBalanceQueryCommand request)
        {
            return await _mediator.Send(request, HttpContext.RequestAborted);
        }

        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("data/metadata")]
        //[PermissionAuthorize(Permission.Report)]
        public Metadata GetMetadata()
        {
            var sort = 1;
            return new Metadata() {
                new RptField(false,"AccoSubjectCode", "科目编码", RptDataType.String, sort++),
                new RptField(false,"AccoSubjectID","科目ID", RptDataType.String, sort++),
                new RptField("EnterpriseName", "单位", RptDataType.String, true, sort++),
                new RptField("AccoSubjectFullName", "往来科目", RptDataType.String, true, sort++),
                new RptField("CurrentTypeName", "往来类型", RptDataType.String, true, sort++),
                new RptField("CustomerName", "往来单位", RptDataType.String, true, sort++),
                new RptField("Show_BegDebit", "期初借方", RptDataType.Number, true, sort++),
                 new RptField("Show_BegCredit", "期初贷方", RptDataType.Number, true, sort++),
                new RptField("BegBalance", "期初余额", RptDataType.Number, true, sort++),
                new RptField("Debit", "借方", RptDataType.Number, true, sort++),
                new RptField("Credit", "贷方", RptDataType.Number, true, sort++),
                new RptField("Show_LastDebit", "期末借方", RptDataType.Number, true, sort++),
                new RptField("Show_LastCredit", "期末贷方", RptDataType.Number, true, sort++),
                new RptField("LastBalance", "期末余额", RptDataType.Number, true, sort++),
                new RptField(false,"CustomerID", "往来单位ID", RptDataType.String, sort++),
                new RptField(false,"CurrentType", "往来类型ID", RptDataType.String, sort++),
                new RptField(false,"Begindate", "开始日期", RptDataType.String, sort++),
                new RptField(false,"Enddate", "结束日期", RptDataType.String, sort++),
            };

        }
        /// <summary>
        /// 获取菜单权限单位
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenuPermissionEnteList")]
        //[AppCode(CommonField.MenuId)]
        //[PermissionAuthorize(Permission.Retrieve)]
        public async Task<RptResult> GetMenuPermissionEnteList()
        {
            var request = new PermissionEnter { Bo_ID = _identityService.UserId, EnterpriseID = _identityService.EnterpriseId, MenuID = "2302071543270000109" };
            var data= await _financeTradeUtil.GetPermissionMenuEnter(request);
            return RptResult.Success(data);
        }

        /// <summary>
        /// 获取组织单位（提供：fang）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("GetOrgEntityList")]
        public async Task<RptResult> GetOrgEntityList(OrgEnterRequest req)
        {
            try
            {
                if (req == null)
                {
                    return RptResult.Failed("参数不能为空");
                }
                if (string.IsNullOrEmpty(req.enterpriseId)) { req.enterpriseId = _identityService.EnterpriseId; }
                if (string.IsNullOrEmpty(req.boid)) { req.boid = _identityService.UserId; }
                var url = $"{_hostCongfiguration.NxinGatewayInnerUrl}api/nxin.permission.org.enter.power.tree.list/1.0?open_req_src=nxin_shuju&enterpriseid={req.enterpriseId}&checklist={req.checklist}&istree={req.istree}&&pid={req.pid}&name={req.name}&boid={req.boid}&isthree=0&isuse={req.isuse}";
                var requestResult = await _httpClientUtil1.GetJsonAsync<RestfulResult>(url);
                var list = new List<OrgEnterResult>();
                if (requestResult?.code == 0 && requestResult?.data != null)
                {
                    list = JsonConvert.DeserializeObject<List<OrgEnterResult>>(requestResult.data.ToString());
                    list = list?.Where(p => p.IsEnter != 2)?.ToList();
                }

                return RptResult.Success(list);
            }
            catch (Exception ex)
            {
                return RptResult.Failed("组织单位查询异常");
            }
        }

        /// <summary>
        /// 获取当前单位或集团科目最大层级
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSubjectMaxLevel")]
        public async Task<RptResult> GetSubjectMaxLevel(SubjectLevelRequest req)
        {
            try
            {
                if (req == null)
                {
                    return RptResult.Failed("参数不能为空");
                }
                if (string.IsNullOrEmpty(req.enterpriseId)) { req.enterpriseId = _identityService.EnterpriseId; }
                if (string.IsNullOrEmpty(req.groupId)) { req.groupId = _identityService.GroupId; }
                var url = $"{ _hostCongfiguration.ReportService }/api/SettleReceiptBalance/GetSubjectMaxLevel";
                var requestResult = await _httpClientUtil1.PostJsonAsync<dynamic>(url,req);
                return RptResult.Success(requestResult);
            }
            catch (Exception ex)
            {
                return RptResult.Failed("科目级次查询异常");
            }
        }
    }
}
