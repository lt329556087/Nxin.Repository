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
using Aspose.Cells;
using System.Collections;
using Architecture.Common.Util;

namespace FinanceManagement.ApiHost.Controllers.MallReceive
{
    /// <summary>
    /// 商城付款
    /// </summary>
    public class FD_MallReceiveHandler : IRequestHandler<MallReceiveRequest, GatewayResultModel>
    {
        IIdentityService _identityService;
        private readonly HttpClientUtil _httpClientUtil;
        private readonly HostConfiguration _hostConfiguration;
        private readonly FinanceTradeUtil _financeTradeUtil;
        private FD_MallPaymentODataProvider _MallPaymentProvider;
        private readonly ILogger<FD_MallReceiveHandler> _logger;
        private FMBaseCommon _fmbaseCommon;
        private IBiz_Related _relatedRepo;
        private HostConfiguration _hostCongfiguration;
        private string AppID = "2307131623440000150";  //商城收款列表    
        private readonly string CustomerBusinessType = "201611160104402101";
        private string FormRelatedType = "201610210104402122";//表单关联值
        private string ReceAppID = "1611231950150000101";//收款单
        private string ReceSummaryAppID = "1612101120530000101";//收款汇总
        private string MallAppID = "201612070104402204";
        string msg = "";
        private int TotalPage = 1;//总页数
        public FD_MallReceiveHandler(IIdentityService identityService, HttpClientUtil httpClientUtil, HostConfiguration hostConfiguration, FinanceTradeUtil financeTradeUtil
           , FD_MallPaymentODataProvider MallPaymentProvider, FMBaseCommon fmbaseCommon
           , ILogger<FD_MallReceiveHandler> logger, IBiz_Related relatedRepo, HostConfiguration hostCongfiguration)
        {
            _identityService = identityService;
            _httpClientUtil = httpClientUtil;
            _hostConfiguration = hostConfiguration;
            _financeTradeUtil = financeTradeUtil;
            _MallPaymentProvider = MallPaymentProvider;
            _fmbaseCommon = fmbaseCommon;
            _relatedRepo = relatedRepo;
            _logger = logger;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<GatewayResultModel> Handle(MallReceiveRequest request, CancellationToken cancellationToken)
        {
            var result = new GatewayResultModel() { code = -1 };
            try
            {
                //参数验证
                if (request == null) { result.msg = "参数空"; return result; }
                if (string.IsNullOrEmpty(request.BeginDate)) { request.BeginDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"); }
                else { request.BeginDate = DateTime.Parse(request.BeginDate).ToString("yyyy-MM-dd"); }
                if (string.IsNullOrEmpty(request.EndDate)) { request.EndDate = DateTime.Now.ToString("yyyy-MM-dd"); }
                else { request.EndDate = DateTime.Parse(request.EndDate).ToString("yyyy-MM-dd"); }
                var cachReq = JsonConvert.DeserializeObject<MallReceiveRequestBase>(JsonConvert.SerializeObject(request));
                string rediskey = $"SCSK:{JsonConvert.SerializeObject(cachReq)}-GroupId:{_identityService.GroupId}-Userid={_identityService.UserId}";
                
                try
                {
                    var csredis = new CSRedis.CSRedisClient(_hostCongfiguration.RedisServer);
                    //初始化 RedisHelper
                    RedisHelper.Initialization(csredis);
                    string value = RedisHelper.Get(rediskey);
                    if (!string.IsNullOrEmpty(value))
                    {
                        var csr = JsonConvert.DeserializeObject<List<MallReceive>>(value);
                        return SetRetResultModel(csr, result, request);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"FD_MallReceiveHandler/Handle:获取Redis值异常：{ex.ToString()};\n param={JsonConvert.SerializeObject(request)}");
                }

               
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
                ////测试单位
                //enteridList =new List<string>() { "1802847","1802847", "1802847" };
                if (enteridList == null || enteridList.Count == 0) return result;
                // 接口获取数据
                var url = "";
                msg = "";
               
                var list = new List<MallReceive>();
                //if (request.OrginType == "1") //1：企订货
                //{
                //    url = $"{_hostConfiguration._rdUrl}/api/FDExpenseShouldPay/GetPayorderList";
                //    list = GetData(url, request, enteridList);
                //}
                if (request.OrginType == "2" || request.OrginType == "3")//商城订单或运费
                {
                    url = $"{_hostConfiguration.ScUrl}/nos/order/listV2";
                    list = GetData(url, request, enteridList);
                    list?.ForEach(x => x.OrginType = request.OrginType);
                }
                else
                {
                    //商城订单
                    request.OrginType = "2";
                    url = $"{_hostConfiguration.ScUrl}/nos/order/listV2";
                    var task = Task.Factory.StartNew(() =>
                    {
                        list = GetData(url, request, enteridList);
                    });
                    //运费
                    var list1 = new List<MallReceive>();
                    var newrequest = new MallReceiveRequest() {
                        EnterpriseID = request.EnterpriseID,
                        NumericalOrder = request.NumericalOrder,
                        OrderNo = request.OrderNo,
                        CustomerID = request.CustomerID,
                        BeginDate = request.BeginDate,
                        EndDate = request.EndDate,
                        OrginType = "3"
                    };
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        list1 = GetData(url, newrequest, enteridList);
                    });
                    Task.WaitAll(task, task1);
                    list?.ForEach(x => x.OrginType = "2");
                    list1?.ForEach(x => x.OrginType = "3");
                    list = list.Concat(list1).ToList();
                    //request.OrginType = orginType;
                    //var list2 = new List<MallReceive>();
                    //var urlorder = $"{_hostConfiguration._rdUrl}/api/FDExpenseShouldPay/GetPayorderList";
                    //var task2 = Task.Factory.StartNew(() =>
                    //{
                    //    list2 = GetData(urlorder, request, enteridList);
                    //});
                    //Task.WaitAll(task,task1, task2);
                    //list = list.Concat(list1).Concat(list2).ToList();
                }

                //商城全量数据 未过滤是否生成付款单，未过滤客户
                if (list?.Count > 0)
                {
                    RedisHelper.Set(rediskey, JsonConvert.SerializeObject(list), DateTimeOffset.Now.AddHours(6).Offset);
                    //RedisHelper.Set(rediskey, JsonConvert.SerializeObject(list), DateTimeOffset.Now.AddMinutes(3).Offset);
                }
                return SetRetResultModel(list, result, request);
            }
            catch (Exception ex)
            {
                result.msg = "请求异常";
                _logger.LogError("FD_MallReceive查询：异常：" + ex.ToString() + ";param=" + JsonConvert.SerializeObject(request));
                return result;
            }
        }
        private GatewayResultModel SetRetResultModel(List<MallReceive> rlist, GatewayResultModel result,MallReceiveRequest request)
        {
            var list = new List<MallReceive>();
            if (rlist?.Count > 0)
            {
                //是否已生成
                list= GetHadCreated(rlist, request);
            }
            result.totalcount = list.Count;
            if (list.Count > request.StartIndex && request.StartIndex >= 0)
            {
                result.data = list.Skip(request.StartIndex).Take(request.PageSize).ToList();
            }
            else
            {
                result.data = new List<MallReceive>();
            }
            result.code = ErrorCode.Success.GetIntValue();
            result.msg = msg;
            return result;
        }
        /// <summary>
        /// 商城
        /// </summary>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="enteridList"></param>
        /// <returns></returns>
        private List<MallReceive> GetData(string url, MallReceiveRequest request, List<string> enteridList)
        {
            var rlist = new List<MallReceive>();
            //拆分多个
            var pageSize = 30;
            decimal avg = Convert.ToDecimal(enteridList.Count) / pageSize;
            int Size = Convert.ToInt32(Math.Ceiling(avg));
            
            Parallel.For(0, Size, n =>
            {
                var enterList = enteridList.Skip(n * pageSize).Take(pageSize).ToList();
                var list = GetDataList(url, request, enterList);
                if (list?.Count > 0)
                {
                    rlist.AddRange(list);
                }
            });


            return rlist;
        }
        private List<MallReceive> GetDataList(string url, MallReceiveRequest request, List<string> enteridList)
        {
            var rlist = new List<MallReceive>();
            var enterid = string.Join(',', enteridList);
            rlist=GetRequestData(url, request, enterid);
            var index = 0;
            Parallel.For(1, TotalPage, n =>
            {
                var list = GetRequestData(url, request, enterid,n+1);
                if (list?.Count > 0)
                {
                    rlist.AddRange(list);
                    index += n;
                }                
            });
            return rlist;
        }

        private List<MallReceive> GetRequestData(string url, MallReceiveRequest request, string enterid,int pagesize=1)
        {
            var rlist = new List<MallReceive>();
            var param = GetRequestParam(request, enterid, pagesize);
            RestfulResult response = null;
            try
            {
                response = _httpClientUtil.PostJsonAsync<RestfulResult>(url, param)?.Result;
                _logger.LogInformation($"收款GetDataList;result={JsonConvert.SerializeObject(response)},param={JsonConvert.SerializeObject(param)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"收款GetDataList;result={JsonConvert.SerializeObject(response)},param={JsonConvert.SerializeObject(param)}；+url={url}\n ex={ex}");
                msg = "商城接口异常" + (string.IsNullOrWhiteSpace(response?.msg) ? "" : response?.msg);
            }


            if (response == null || response.code != 0)
            {
                //1：企订货 2：商城订单 3：货联运费
                if (request.OrginType == "2")
                {
                    msg += "商城订单:";
                }
                if (request.OrginType == "3")
                {
                    msg += "货联运费:";
                }
                msg += response?.msg;
                return rlist;
            }
            var data = JsonConvert.DeserializeObject<MallReceiveResponse>(response.data.ToString());
            if (data?.count > 0 && data?.results?.Count > 0)
            {
                if (pagesize == 1)
                {
                    TotalPage = data.total;
                }
                foreach (var item in data.results)
                {
                    if (item.paymentRecordList == null || item.paymentRecordList.Count == 0) continue;
                    foreach (var pay in item.paymentRecordList)
                    {
                        var newItem = SetResultList(pay, item);
                        rlist.Add(newItem);
                    }
                }
            }
            return rlist;
        }
        private List<MallReceive> GetHadCreated(List<MallReceive> list, MallReceiveRequest request)
        {
            var rlist = new List<MallReceive>();
            if (!string.IsNullOrEmpty(request.NumericalOrder))
            {
                list = list?.Where(p => p.NumericalOrder.Contains(request.NumericalOrder))?.ToList();
            }
            if (!string.IsNullOrEmpty(request.OrderNo))
            {
                list = list?.Where(p => p.OrderNo.Contains(request.OrderNo))?.ToList();
            }
            foreach (var item in list)
            {
                //查找是否已生成
                var relate = new Domain.BIZ_Related() { RelatedType = FormRelatedType, ParentType = ReceAppID + "," + ReceSummaryAppID, ChildType = MallAppID, ChildValue =item.NumericalOrder };
                var relateList = _fmbaseCommon.GetScHasRelated(relate);
                if (relateList?.Count > 0) { continue; }
                SetQlwCustomer(item);
                rlist.Add(item);
            }
            ////筛选客户
            if (!string.IsNullOrEmpty(request.CustomerName))
            {
                rlist = rlist?.Where(p => p.CustomerName.Contains(request.CustomerName))?.ToList();
            }
            
            //if (!string.IsNullOrEmpty(request.CustomerID) && request.CustomerID != "0")
            //{
            //    rlist = rlist?.Where(p => request.CustomerID.Contains(p.CustomerID))?.ToList();
            //}
            return rlist;
        }
        private MallReceive SetResultList(FD_MallReceiveCommand pay, OrderModel scitem)//string enterid, string entername)
        {
            var newItem = new MallReceive();
            var enterid= scitem.sellerOrgId;
            newItem.NumericalOrder = pay.prId;
            newItem.OrderNo = pay.orderId;
            newItem.Amount = pay.pMoney;
            newItem.PaymentType = pay.paymentType;
            newItem.PaymentTime = pay.paymentTime;
            newItem.CollectionName = pay.CollectionName;
            newItem.MoneyOrigin = pay.payee;
            newItem.BusinessType = CustomerBusinessType;
            newItem.BusinessTypeName = "客户";
            newItem.EnterpriseName = scitem.sellerOrgName;
            newItem.EnterpriseID = scitem.sellerOrgId;
            newItem.ContractId = scitem.contractId;
            newItem.ContractUrl = scitem.contractUrl;
            newItem.OrderDetailUrl = scitem.orderDetailUrl;
            newItem.HlOrderDetailUrl = scitem.hlOrderDetailUrl;
            var orgId = pay.orgId;
            if (string.IsNullOrEmpty(orgId) || orgId == "0")
            {
                orgId = pay.boId;
            }
            newItem.ScCustomerID = orgId;
           
            var paycode = pay.paymentCode;
            //支付方式转换
            var strs = Payment2QLW(paycode);
            newItem.PaymentTypeID = strs[1];
            newItem.PaymentTypeName = strs[0];
            return newItem;
        }
        private MallReceive SetQlwCustomer(MallReceive newItem)
        {
            //客户转换
            if (!string.IsNullOrEmpty(newItem.ScCustomerID))
            {
                var customerList = _fmbaseCommon.GetQlwCustomerBySc(new Domain.Biz_CustomerDrop { EnterpriseID = newItem.EnterpriseID, ChildValue = newItem.ScCustomerID });
                if (customerList?.Count > 0)
                {
                    var customer = customerList.FirstOrDefault();
                    newItem.CustomerID = customer.CustomerID;
                    newItem.CustomerName = customer.CustomerName;
                }
            }
            var customerName = string.IsNullOrEmpty(newItem.CustomerName) ? "" : newItem.CustomerName;
            newItem.Content = "收" + customerName + "货款";
            return newItem;
        }
        /// <summary>
        /// 请求参数
        /// </summary>
        /// <param name="model"></param>
        /// <param name="enterid"></param>
        /// <returns></returns>
        private MallReceiveParam GetRequestParam(MallReceiveRequest model, string enterid, int pagesize)
        {
            var param = new MallReceiveParam();            
            string timeStamp = string.Empty;
            var identifier = EncryptUtils.GetNXMallIndentifierOA(out timeStamp);
            model.PageSize = 20;
            param.identifier = identifier;
            param.dtime = timeStamp;
            param.shopId = enterid;
            //param.prId = model.NumericalOrder;
            //param.orderId = model.OrderNo;
            param.pageNum = pagesize;
            param.size = model.PageSize;
            param.type = "2";
            param.paymentTimeStart = model.BeginDate;
            param.paymentTimeEnd = model.EndDate;
            if (model.OrginType == "3")
            {
                param.isFreight = 1;
            }
            return param;
        }
        #region 商城-企联网支付方式对应关系


        /// <summary>
        /// 商城支付状态转换为企联网对应的支付状态
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        private string[] Payment2QLW(string payment)
        {
            string[] strs = new string[2];
            switch (payment.Trim())
            {
                case "HK": strs[0] = "暂不结算"; strs[1] = "201612070104402301"; break;
                case "NFB": strs[0] = "农富宝"; strs[1] = "201612070104402302"; break;
                case "QB": strs[0] = "余额结算"; strs[1] = "201612070104402306"; break;
                case "HYLDS": strs[0] = "银联代收"; strs[1] = "201612070104402305"; break;
                default: strs[0] = "余额结算"; strs[1] = "201612070104402306"; break;
            }
            return strs;
        }
        #endregion
    }
}
