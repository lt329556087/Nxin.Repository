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
namespace FinanceManagement.ApiHost.Controllers.BaddebtAccrualDraft
{
    /// <summary>
    /// 往来余额表
    /// </summary>
    public class BaddebtAccrualDraftHandler : IRequestHandler<BaddebtAccrualDraftQueryCommand, RptResult>
    {
        IIdentityService _identityService;
        private readonly HttpClientUtil _httpClientUtil;
        private readonly HostConfiguration _hostConfiguration;

        public BaddebtAccrualDraftHandler(IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostConfiguration)
        {
            _identityService = identityService;
            _httpClientUtil = httpClientUtil;
            _hostConfiguration = hostConfiguration;
        }

        public async Task<RptResult> Handle(BaddebtAccrualDraftQueryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 校验参数信息
                var validationMsg = request.CheckModel();
               
                if (validationMsg.IsNotNullOrEmpty())
                {
                    return RptResult.Failed(validationMsg!);
                }
                
                // 接口获取数据
                var url = $"{_hostConfiguration.ReportService}/api/BaddebtAccrualDraft/GetBalanceReport";
                return await _httpClientUtil.PostJsonAsync<object, RptResult>(url, request, resStr =>
                {
                    var resData = JObject.Parse(resStr);

                // 判断返回状态
                var code = Convert.ToInt32(resData["code"]);
                    if (code != 0)
                    {
                        return RptResult.Failed($"接口返回数据异常：{resData["msg"]}");
                    }

                    // 获取数据节点
                    var resultData = ((JArray)resData.GetValue("data")!);//(JArray)(((JObject)resData.GetValue("data")!)?.GetValue("changeSummaryLines"))!;
                    return RptResult.Success(resultData);

                });
            }
            catch (Exception ex)
            {
                return RptResult.Failed($"请求异常：{ex.Message}");
            }
        }
    }
}
