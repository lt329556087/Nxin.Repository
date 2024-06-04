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
using System.Text;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.Extensions.Logging;
using FinanceManagement.ApiHost.Controllers.SettleReceiptBalance;

namespace FinanceManagement.ApiHost.Controllers.MallPayment
{
    /// <summary>
    /// 商城付款
    /// </summary>
    public class FD_MallPaymentHandler : IRequestHandler<MallPaymentRequest, GatewayResultModel>
    {
        IIdentityService _identityService;
        private readonly HttpClientUtil _httpClientUtil;
        private readonly HostConfiguration _hostConfiguration;
        private readonly FinanceTradeUtil _financeTradeUtil;
        private FD_MallPaymentODataProvider _mallPaymentProvider;
        private FMBaseCommon _fmbaseCommon;
        private readonly ILogger<FD_MallPaymentHandler> _logger;
        private string AppID = "2307131627050000150"; //商城付款列表
        private readonly string SupplierBusinessType = "201611160104402104";
        private readonly string CustomerBusinessType = "201611160104402101";
        private string FormRelatedType = "201610210104402122";//表单关联值
        private string PayAppID = "1612011058280000101";//付款单
        private string PaySummaryAppID = "1612060935280000101";//付款汇总
        private string MallAppID = "201612070104402204";
        string msg = "";
        //List<string> cusidList = new List<string>();//商城客户id
        public FD_MallPaymentHandler(IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostConfiguration, FinanceTradeUtil financeTradeUtil
           , FD_MallPaymentODataProvider mallPaymentProvider, FMBaseCommon fmbaseCommon
           , ILogger<FD_MallPaymentHandler> logger)
        {
            _identityService = identityService;
            _httpClientUtil = httpClientUtil;
            _hostConfiguration = hostConfiguration;
            _financeTradeUtil = financeTradeUtil;
            _logger = logger;
            _mallPaymentProvider = mallPaymentProvider;
            _fmbaseCommon = fmbaseCommon;
        }
        public async Task<GatewayResultModel> Handle(MallPaymentRequest request, CancellationToken cancellationToken)
        {
            var result = new GatewayResultModel() { code=-1};
            try
            {
                //参数验证
                if (request == null) { result.msg = "参数空"; return result; }
                if (string.IsNullOrEmpty(request.BeginDate)) { request.BeginDate = DateTime.Now.ToString("yyyy-MM-01 00:00:00"); }
                else{request.BeginDate=DateTime.Parse(request.BeginDate).ToString("yyyy-MM-01 00:00:00");}
                if (string.IsNullOrEmpty(request.EndDate)) { request.EndDate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59"); }
                else { request.EndDate = DateTime.Parse(request.EndDate).ToString("yyyy-MM-dd 23:59:59"); }
                //菜单权限单位
                AppID = _identityService.AppId.ToString();
                var perParam = new PermissionEnter { Bo_ID = _identityService.UserId, EnterpriseID = _identityService.EnterpriseId, MenuID = AppID };
                var perdata = await _financeTradeUtil.GetPermissionMenuEnter(perParam);
                if (perdata == null || perdata.Count == 0) return result;
                var perEnteridList = perdata.Select(p => p.EnterpriseID).ToList();
                var enteridList = new List<string>();
                if (string.IsNullOrEmpty(request.EnterpriseID))
                {
                    enteridList = perEnteridList;
                }
                else
                {
                    var enList = request.EnterpriseID.Split(',');
                    enteridList = perEnteridList.Intersect(enList).ToList();
                }
                if (enteridList==null||enteridList.Count==0)return result;
                //    //测试单位
                //    enteridList = new List<string>() { "1274179", "1895697", "1802847" };
                // 接口获取数据
                var url = "";
                msg = "";
                //cusidList = new List<string>();
                var list = new List<MallPayment>();
                if (request.OrginType == "1") //1：货联运费
                {
                    url = $"{_hostConfiguration.NxinGatewayInnerUrl}api/nxin.hl.paymment.bills/1.0";
                    list = GetData(url,request, enteridList);
                    list?.ForEach(x => x.OrginType = "1");
                }
                else if (request.OrginType == "2")
                {
                    url = $"{_hostConfiguration.NxinGatewayUrl}api/nxin.sc.push.payment.order/1.0";
                    list =GetData(url, request, enteridList);
                    list?.ForEach(x => x.OrginType = "2");
                }
                else 
                {
                    url = $"{_hostConfiguration.NxinGatewayInnerUrl}api/nxin.hl.paymment.bills/1.0";
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        list = GetData(url, request, enteridList);
                    });
                    var list2 = new List<MallPayment>();
                    var urlorder = $"{_hostConfiguration.NxinGatewayUrl}api/nxin.sc.push.payment.order/1.0";
                    var task2 = Task.Factory.StartNew(() =>
                    {
                        list2 = GetData(urlorder, request, enteridList);
                    });
                    Task.WaitAll(task1, task2);
                    list?.ForEach(x=>x.OrginType = "1");
                    list2?.ForEach(x => x.OrginType = "2");
                    list = list.Concat(list2).ToList();
                }
                result.totalcount = list.Count;
                if(list.Count> request.StartIndex&&request.StartIndex>=0)
                {
                    result.data = list.Skip(request.StartIndex).Take(request.PageSize).ToList();
                }
                else
                {
                    result.data =new List<MallPayment>();
                }
                result.code = ErrorCode.Success.GetIntValue();
                result.msg = msg;
                return result;
            }
            catch (Exception ex)
            {
                result.msg = "请求异常";
                Serilog.Log.Error("FD_MallPayment查询：异常：" + ex.ToString()+";param="+JsonConvert.SerializeObject(request));
                return result;
            }
        }
        private List<MallPayment> GetData(string url, MallPaymentRequest request, List<string> enteridList)
        {
            var rlist = new List<MallPayment>();
            ////拆分多个
            //var pageSize = 50;
            //decimal avg = Convert.ToDecimal(enteridList.Count) / pageSize;
            //int Size = Convert.ToInt32(Math.Ceiling(avg));
            var size = enteridList.Count;
            Parallel.For(0, size, n => {
                //var enterList = enteridList.Skip(n * pageSize).Take(pageSize).ToList();
                var enterid = enteridList[n];
                var list =GetDataList(url, request, enterid)?.Result;
                if (list?.Count > 0)
                {
                    rlist.AddRange(list);
                }                
            });
            

            return rlist;
        }
        private async Task<List<MallPayment>> GetDataList(string url, MallPaymentRequest request, string enterid)// List<string> enteridList)
        {
            var rlist = new List<MallPayment>();
            var list = new List<FD_MallPaymentCommand>();
            RestfulResult response = null;
            var param = GetRequestData(request, enterid);// enteridList);
            url = url + "?" + param;
            try
            {
                response = await _httpClientUtil.GetJsonAsync<RestfulResult>(url, null);
                _logger.LogInformation($"付款GetDataList;result={JsonConvert.SerializeObject(response)},url={url}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"付款GetDataList;result={JsonConvert.SerializeObject(response)},+url={url}\n ex={ex}");
                msg = "商城接口异常" + (string.IsNullOrWhiteSpace(response?.msg) ? "" : response?.msg);
            }           
           
            if (response == null || response.code != 0)
            {
                //1：货联运费 2:商城订单               
                 if (url.Contains("api/nxin.hl.paymment.bills"))
                {
                    msg += "货联运费:";
                }
                else if(url.Contains("api/nxin.sc.push.payment.order/1.0"))
                {
                    msg += "商城订单:";
                }
                msg += response?.msg;
                return rlist;
            }
            if (response?.code == 0)
            {
                list = JsonConvert.DeserializeObject<List<FD_MallPaymentCommand>>(response.data.ToString());
            }
            if (list?.Count > 0)
            {
                if (!string.IsNullOrEmpty(request.NumericalOrder))
                {
                    list = list?.Where(p => !string.IsNullOrEmpty(p.payOrderId) && p.payOrderId.Contains(request.NumericalOrder))?.ToList();
                }
                if (!string.IsNullOrEmpty(request.OrderNo))
                {
                    list = list?.Where(p => !string.IsNullOrEmpty(p.payOrderId) && p.extServiceId.Contains(request.OrderNo))?.ToList();
                }
                //查找是否已生成
                for (var i = 0; i < list.Count; i++)
                {
                    var relate = new Domain.BIZ_Related() { RelatedType = FormRelatedType, ParentType = PayAppID + "," + PaySummaryAppID, ChildType = MallAppID, ChildValue = list[i].payOrderId };
                    var relateList = _fmbaseCommon.GetScHasRelated(relate);//_mallPaymentProvider.GetRelatedListByTpyes(relate)?.Result;
                    if (relateList?.Count > 0) { list.Remove(list[i]); i--; }
                }
                if (list?.Count > 0)
                {
                    rlist = SetResultList(list);
                }
            }
            //筛选客户
            if (!string.IsNullOrEmpty(request.CustomerName))
            {
                rlist = rlist?.Where(p =>!string.IsNullOrEmpty(p.CustomerName)&& p.CustomerName.Contains(request.CustomerName))?.ToList();
            }
            //if (!string.IsNullOrEmpty(request.CustomerID) && request.CustomerID != "0")
            //{
            //    rlist = rlist?.Where(p => request.CustomerID.Contains(p.CustomerID))?.ToList();
            //}
            return rlist;
        }
        private List<MallPayment> SetResultList(List<FD_MallPaymentCommand> list)
        {
            var resultList = new List<MallPayment>();
            foreach(var group in list.GroupBy(p => p.drawee))
            {
                if (group == null || group.Count() == 0) continue;
                //var cusids = string.Join(',', group.Where(p=>!string.IsNullOrEmpty(p.payee)).Select(p => p.payee));
                //var customerList = new List<DropODataEntity>();
                //if (!string.IsNullOrEmpty(cusids))
                //{
                //    //customerList = _fmbaseCommon.GetCustomerByRelated(new Domain.CustomerSearch { EnterpriseID = group.Key, CustomerID = cusids });
                //}
                list.ForEach(p =>
                {
                    var row = new MallPayment();
                    row.NumericalOrder = p.payOrderId;
                    row.OrderNo = p.extServiceId;
                    row.EnterpriseID = p.drawee;
                    row.EnterpriseName = p.draweeName;
                    row.PayType = p.payType;
                    row.PayTime = p.payTime;
                    row.PayMethod = p.payMethod;
                    row.PaySource = p.paySource;
                    row.Amount = p.amount;
                    row.ContractId= p.contractId;
                    row.ContractUrl= p.contractUrl;
                    row.OrderDetailUrl= p.orderDetailUrl;
                    row.HlOrderDetailUrl = p.hlOrderDetailUrl;
                    if (!string.IsNullOrEmpty(p.payee) && p.payee != "0")
                    {

                        //var filtercustomer = customerList.Where(c => c.curtype == p.payee);
                        //if (filtercustomer?.Count() > 0)
                        //{
                        //    var customer = filtercustomer.FirstOrDefault();
                        //    row.CustomerName = customer.name;
                        //    row.CustomerID = customer.id;
                        //}
                        var customerList = _fmbaseCommon.GetQlwCustomerBySc(new Domain.Biz_CustomerDrop { EnterpriseID = row.EnterpriseID, ChildValue = p.payee });
                        if (customerList?.Count > 0)
                        {
                            var customer = customerList.FirstOrDefault();
                            row.CustomerID = customer.CustomerID;
                            row.CustomerName = customer.CustomerName;
                        }
                        
                        ////内容
                        //if (request.OrginType == 1)//运费
                        //{
                        //    row.Content = "代付" + p.CustomerName + "运费";
                        //}
                        //else if (request.OrginType == 2)//订单
                        //{
                        //    row.Content = "支付" + p.CustomerName + "公司货款";
                        //}
                    }

                    row.PaymentTypeID = "201612070104402306";
                    row.PaymentTypeName = "余额结算";
                    //row.OrginType = request.OrginType;
                    row.BusinessType = CustomerBusinessType;
                    row.BusinessTypeName = "客户";
                    resultList.Add(row);
                });
            }
            
            return resultList;
        }
        private string GetRequestData(MallPaymentRequest model,string enterid)//List<string> enteridList)
        {

            var sb = new StringBuilder();
            //var enterid = string.Join(',', enteridList);
            sb.AppendFormat("drawee={0}", enterid);
            //if (!string.IsNullOrEmpty(model.NumericalOrder) && model.NumericalOrder != "0")
            //{
            //    sb.AppendFormat(@"&pay_order_id={0}", model.NumericalOrder);//支付ID
            //}
            //if (!string.IsNullOrEmpty(model.OrderNo) && model.OrderNo != "0")
            //{
            //    sb.AppendFormat(@"&ext_service_id={0}", model.OrderNo);//订单ID
            //}

            if (!string.IsNullOrEmpty(model.BeginDate))
            {
                sb.AppendFormat(@"&start_time={0}", model.BeginDate);
            }
            if (!string.IsNullOrEmpty(model.EndDate))
            {
                sb.AppendFormat(@"&end_time={0}", model.EndDate);
            }
            //if (!string.IsNullOrEmpty(model.CustomerID) && model.CustomerID != "0")
            //{
            //    var customerList = new List<BIZ_RelatedODataEntity>();
            //    if (cusidList.Count == 0)
            //    {
            //        customerList = _mallPaymentProvider.GetRelatedListByValues(new BIZ_RelatedODataEntity() { RelatedType = "201610210104402102", ParentType = "201610200104402109", ChildType = "201610200104402102", ParentValue = model.CustomerID });
            //    }
            //    if (customerList.Count > 0)
            //    {
            //        cusidList = customerList.Select(p => p.ChildValue).ToList();
            //        if (cusidList.Count == 1)
            //        {
            //            var cusids = string.Join(',', cusidList);
            //            sb.AppendFormat(@"&payee={0}", cusids);
            //        }                    
            //    }                
            //}
            sb.Append("&open_req_src=qlw-fm");
            return sb.ToString();
        }        
    }
}
