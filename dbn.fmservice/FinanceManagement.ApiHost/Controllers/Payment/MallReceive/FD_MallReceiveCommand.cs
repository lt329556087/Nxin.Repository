using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediatR;
using FinanceManagement.ApiHost.Controllers.SettleReceiptBalance;

namespace FinanceManagement.ApiHost.Controllers.MallReceive
{
    public class FD_MallReceiveCommand : IRequest<GatewayResultModel>
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 支付流水ID
        /// </summary>
        public string prId { get; set; } 
        /// <summary>
        /// 买家单位id
        /// </summary>
        public string orgId { get; set; }
        /// <summary>
        /// 买家boid
        /// </summary>
        public string boId { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        public string paymentType { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string paymentTime { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string paymentCode { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal pMoney { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string CollectionName { get; set; }
        /// <summary>
        /// 收款来源
        /// </summary>
        public string payee { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public string contractId { get; set; }
        /// <summary>
        /// 合同地址
        /// </summary>
        public string contractUrl { get; set; }
        /// <summary>
        /// 订单详情地址
        /// </summary>
        public string orderDetailUrl { get; set; }
        /// <summary>
        /// 货联运单详情地址
        /// </summary>
        public string hlOrderDetailUrl { get; set; }
    }

    public class MallReceiveResponse
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int pageNum { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int total { get; set; }
        public List<OrderModel> results { get; set; }
    }
    public class OrderModel
    {
        //public string sellerboId { get; set; }
        //public string sellerName { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public string contractId { get; set; }
        /// <summary>
        /// 合同地址
        /// </summary>
        public string contractUrl { get; set; }
        /// <summary>
        /// 订单详情地址
        /// </summary>
        public string orderDetailUrl { get; set; }
        /// <summary>
        /// 货联运单详情地址
        /// </summary>
        public string hlOrderDetailUrl { get; set; }
        public string sellerOrgId { get; set; }
        public string sellerOrgName { get; set; }
        public List<FD_MallReceiveCommand> paymentRecordList { get; set; }
    }

    public class MallReceiveRequest:MallReceiveRequestBase 
    {
        public MallReceiveRequest()
        {
            PageIndex = 1;
            PageSize = 20;
            // CusRowState = 2;//0:全部 1：已处理 2：未处理
        }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码 从1开始
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 开始位置
        /// </summary>
        public int StartIndex
        {
            get { return PageSize * (PageIndex - 1); }
        }

        //public int CusRowState { get; set; }

    }
    public class MallReceiveRequestBase : IRequest<GatewayResultModel>
    {
        /// <summary>
        /// 付款单位
        /// </summary>
        public string EnterpriseID { get; set; }
       

        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string OrginType { get; set; }//1：企订货 2：商城订单 3：货联运费
    }

    public class MallReceiveParam
    {
        public MallReceiveParam()
        {

        }
        public string identifier { get; set; }
        public string dtime { get; set; }
        public string shopId { get; set; }
        public int pageNum { get; set; }
        public int size { get; set; }
        public string prId { get; set; }
        public string orderId { get; set; }
        public string paymentTimeStart { get; set; }
        public string paymentTimeEnd { get; set; }
        public string orgId { get; set; }
        public int isFreight { get; set; }
        public string type { get; set; }
    }
    //public class GatewayResultModel
    //{
    //    /// <summary>
    //    /// 消息编号
    //    /// </summary>
    //    public int code { get; set; }
    //    /// <summary>
    //    /// 消息
    //    /// </summary>
    //    public string msg { get; set; }

    //    /// <summary>
    //    /// 返回数据
    //    /// </summary>
    //    public object data { get; set; }
    //    public int totalcount { get; set; }
    //}

    public class MallReceive
    {
        /// <summary>
        /// 收款流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 商城订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 收款单位
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }
       
    
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessTypeName { get; set; }
        /// <summary>
        /// 收款类型
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// 收款日期
        /// </summary>
        public string PaymentTime { get; set; }
    
       
        /// <summary>
        /// 收款来源
        /// </summary>
        public string MoneyOrigin { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        public string CollectionName { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string PaymentTypeName { get; set; }

        ///// <summary>
        ///// 内容
        ///// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractId { get; set; }
        /// <summary>
        /// 合同地址
        /// </summary>
        public string ContractUrl { get; set; }
        /// <summary>
        /// 订单详情地址
        /// </summary>
        public string OrderDetailUrl { get; set; }
        /// <summary>
        /// 货联运单详情地址
        /// </summary>
        public string HlOrderDetailUrl { get; set; }
        /// <summary>
        /// 1：企订货 2：商城订单 3：货联运费
        /// </summary>
        public string OrginType { get; set; }
        /// <summary>
        /// 商城客户id
        /// </summary>
        public string ScCustomerID { get; set; }
    }
}
