using Architecture.Common.NumericalOrderCreator;
using Architecture.Common.Z.NumberCreator;
using Architecture.Seedwork.Core;
using Architecture.Seedwork.Security;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Architecture.Common.HttpClientUtil;
using FinanceManagement.Common;
using Architecture.Codeless.Report;
namespace FinanceManagement.ApiHost.Controllers.CurrentAccount
{
    /// <summary>
    /// 往来余额表
    /// </summary>
    public class CurrentAccountHandler : IRequestHandler<CurrentAccountQueryCommand, SettleReceiptBalance.GatewayResultModel>
    {
        IIdentityService _identityService;
        private readonly HttpClientUtil _httpClientUtil;
        private readonly HostConfiguration _hostConfiguration;

        public CurrentAccountHandler(IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostConfiguration)
        {
            _identityService = identityService;
            _httpClientUtil = httpClientUtil;
            _hostConfiguration = hostConfiguration;
        }

        public async Task<SettleReceiptBalance.GatewayResultModel> Handle(CurrentAccountQueryCommand request, CancellationToken cancellationToken)
        {
            var result = new SettleReceiptBalance.GatewayResultModel() { code=-1};
            try
            {
                // 校验参数信息
                var validationMsg = request.CheckModel();
                if (validationMsg.IsNotNullOrEmpty())
                {
                    result.msg = validationMsg;
                    return result;
                }
                if (request.CurrentUnitIDList?.Count > 0)
                {
                    request.CurrentUnitIDs = string.Join(',', request.CurrentUnitIDList);
                }
                if (string.IsNullOrEmpty(request.groupID))
                {
                    request.groupID = _identityService.GroupId??"0";
                }
                if (string.IsNullOrEmpty(request.boid))
                {
                    request.boid = _identityService.UserId ?? "0";
                }
                // 接口获取数据
                //if (_hostConfiguration.ReportService == "http://rptserviceqlw.nxin.com")
                //{
                //    _hostConfiguration.ReportService = "http://demorptserviceqlw.nxin.com";
                //}
                var url = $"{_hostConfiguration.ReportService}/api/RptCurrentAccount/GetCurrentAccountReport";
                return await _httpClientUtil.PostJsonAsync<object, SettleReceiptBalance.GatewayResultModel>(url, request);
            }
            catch (Exception ex)
            {
                result.msg = "请求异常";
                Serilog.Log.Error("CurrentAccountHandler查询：异常："+ex.ToString()+";param="+JsonConvert.SerializeObject(request));
                return result;
            }
        }
    }
}
