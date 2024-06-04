using Architecture.Common.HttpClientUtil;
using FinanceManagement.Common;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nxin.Qlw.Common.MQ.RabbitMQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_PaymentReceivables
{
    /// <summary>
    /// 银行收款数据接收
    /// </summary>
    //public class BankReceivable : ConsumerBase
    //{
    //    public override string QueueName => "nx.fundmgr.qlwinfo.sync.exchange";

    //    HttpClientUtil _httpClientUtil1 = new HttpClientUtil(new IHttpClientFactory);
    //    HostConfiguration _hostCongfiguration;
    //    public BankReceivable()
    //    {
    //        _httpClientUtil1 = httpClientUtil;
    //        _hostCongfiguration = hostCongfiguration;
    //    }
    //    protected override void Execute(string receiveContent)
    //    {
    //        var oaresult = _httpClientUtil1.PostJsonAsync<dynamic, dynamic>($"{_hostCongfiguration._wgUrl}/dbn/fm/api/FD_PaymentCall/AutoWrite", receiveContent).Result;
    //        //处理逻辑
    //    }
    //}
    #region
    public class CustListItem
    {
        /// <summary>
        /// 客户id
        /// </summary>
        public string custId { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string custName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string marketId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string marketName { get; set; }
        /// <summary>
        /// 服务人员用户中心ID
        /// </summary>
        public string boId { get; set; }
        /// <summary>
        /// 服务人员姓名
        /// </summary>
        public string userName { get; set; }
    }

    public class DataItem
    {
        /// <summary>
        /// 交易索引
        /// </summary>
        public string transIndex { get; set; }
        /// <summary>
        /// 回单地址
        /// </summary>
        public string receiptUrl { get; set; }
        /// <summary>
        /// 交易流水
        /// </summary>
        public string bankSerial { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 到账时间
        /// </summary>
        public DateTime receiveDay { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string entId { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string entName { get; set; }
        /// <summary>
        /// 账户索引
        /// </summary>
        public string acctIndex { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public string acctNo { get; set; }
        /// <summary>
        /// 对方名称
        /// </summary>
        public string otherSideName { get; set; }
        /// <summary>
        /// 对方账户索引
        /// </summary>
        public string otherSideAcctIndex { get; set; }
        /// <summary>
        /// 对方账户
        /// </summary>
        public string otherSideAcct { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msgCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 来源：YQT-银企通；WX-微信；POS-pos机
        /// </summary>
        public string dataSource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CustListItem> custList { get; set; }
    }

    public class BankReceivableData
    {
        /// <summary>
        /// 
        /// </summary>
        public string sysnType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> data { get; set; }
    }
    public class OrgDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cAxis { get; set; }
        /// <summary>
        /// 创业单位/业务前台
        /// </summary>
        public string cFullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SortId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InheritanceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsEnd { get; set; }
        /// <summary>
        /// 业务前台
        /// </summary>
        public string SortName { get; set; }
    }

    public class OrgResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<OrgDataItem> data { get; set; }
        /// <summary>
        /// 获取数据成功
        /// </summary>
        public string msg { get; set; }
    }
    public class QueueMsg
    {
        public string message { get; set; }
    }
    #endregion
}
