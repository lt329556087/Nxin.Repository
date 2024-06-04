using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediatR;
using FinanceManagement.ApiHost.Controllers.SettleReceiptBalance;

namespace FinanceManagement.ApiHost.Controllers.MallPayment
{
    public class FD_MallPaymentCommand : IRequest<GatewayResultModel>
    {
        public FD_MallPaymentCommand()
        {
            NumericalOrder = "0";
            CusRowState = 0;
            StateName = "生成付款单";
            OrginType = 1;//来源 1:货联运费 2:订单
        }
        public string extServiceId { get; set; }//商城订单号
        public string orderId { get; set; }//运单单号
        public string carriageSerialNo { get; set; }//运费单流水号
        public string payOrderId { get; set; }//支付单号
        public string draweeName { get; set; }//付款人 单位名称
        public string drawee { get; set; }//付款人
        public string payee { get; set; }//收款人 客户

        public string payeeName { get; set; }//收款人名称 客户名称
        public string payType { get; set; }//付款类型

        public string payTime { get; set; }//支付时间
        public decimal amount { get; set; }//金额
        public string payCode { get; set; }//结算方式
        public string paySource { get; set; }//付款来源
        public string payMethod { get; set; }//支付方式标识
        public int OrginType { get; set; }//来源 1:货联运费
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
        //public string CustomerID { get; set; }
        //public string CustomerName { get; set; }
        //public string Content { get; set; }
        //public string PaymentTypeID { get; set; }//付款方式--结算
        //public string PaymentTypeName { get; set; }
        public string NumericalOrder { get; set; }//支付流水号
        public int CusRowState { get; set; }
        public string StateName { get; set; }
        public string BusinessType { get; set; }
    }

    public class MallPaymentRequest : IRequest<GatewayResultModel>
    {
        public MallPaymentRequest()
        {
            PageIndex = 1;
            PageSize = 20;
           // CusRowState = 2;//0:全部 1：已处理 2：未处理
        }
        /// <summary>
        /// 付款单位
        /// </summary>
        public string EnterpriseID { get; set; }
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

        public string BeginDate { get; set; }
        public string EndDate { get; set; }        
        public string OrginType { get; set; }//1：货联运费 2:商城订单

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

    public class MallPayment
    {
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 商城订单号    结算方式   
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 付款单位
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 往来单位名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public string PayTime { get; set; }
       
        /// <summary>
        /// 结算方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 付款来源
        /// </summary>
        public string PaySource { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayMethod { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessType { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string BusinessTypeName { get; set; }
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
        /// 1：货联运费 2:商城订单
        /// </summary>
        public string OrginType { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string Content { get; set; }
    }
}
