using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FinanceManagement.Common.MonthEndCheckout
{
    public enum AccoCheckTypeEnum : long
    {
        销售结账 = 201708010104402101,
        采购结账 = 201708010104402102,
        折扣分摊 = 201708010104402103,
        存货结账 = 201708010104402104,
        会计结账 = 201708010104402105,
        出厂成本 = 201708010104402106,
        固定资产 = 201708010104402107,
        物品结账 = 201708010104402108,
        猪场结账 = 201708010104402109,
        羊场结账 = 201708010104402110,
        应收结账 = 201708010104402111,
        应付结账 = 201708010104402112,
        成本管理 = 201708010104402113,
        放养管理 = 201708010104402114,
    }
    public enum AccocheckDataSource : long
    {
        会计凭证 = 2016033105263350476,
        往来余额表 = 1612121633410000101,
        科目余额表 = 1612121627090000101,
        销售汇总表 = 1612062114270000101,
        折扣汇总表 = 201512289551945035,
        采购汇总表 = 1612121053430000101,
        运费汇总表 = 1707311529570000105,
        存货汇总表 = 1707261523090000101,
        物品明细表 = 1712061720030000101,
        成本汇总表 = 1706152008290000101,
        折旧汇总表 = 1801111409580000101,
        固定资产卡片报表 = 1712251756400000101,
        资金汇总表 = 1612121622160000101,
        应收账款汇总表 = 1612121330480000101,
        应付账款汇总表 = 1612121126590000101,
        成本变动汇总表 = 2010301143010000101,
        猪场成本明细表 = 2211231846540000109,
        养户成本汇总表 = 1709191611440000105,
        销售单 = 1610311318270000101,
        猪只销售单 = 2109021808150000109,
        猪只销售 = 2007092157330000101,
        精液销售单 = 2104141012380000101,
        其他销售单 = 2001070948380000102,
        直营销售单 = 1907161719230000102,
        折扣计提 = 201512292046349819,
        采购单 = 1611051506540000101,
        采购单集团版 = 2108111356400000109,
        猪只采购 = 2007092156450000101,
        猪只采购单 = 2207211859410000109,
        精液采购单 = 2104141011350000101,
    }
    public enum AccocheckState : long
    {
        已审核 = 2306301629250000121,
        已计提 = 2306301629250000122,
        无余额 = 2306301629250000123,
        一致 = 2306301629250000124,
    }
    public enum AppIDEnum : long
    {
        利润表 = 1612131339390000101,
        资产负债表 = 1612131339250000101,
        现金流量表 = 1612131340200000101,
        损益概算表 = 1709050914370000101,
        损益调整 = 1612131343580000101,
        业务分摊系数 = 201512288505836822,
        资产负债期初表 = 1612131339250000101,

        利润公式表 = 1612131341240000101,
        资产负债公式表 = 1705241417480000101,
        现金流量公式表 = 201512291400036746,
        损益概算公式表 = 1710271339130000101,

        出厂成本汇总表 = 1612131334520000102,

        月末结账 = 1612121625120000101,
        费用汇总 = 201512292115872959,

        销售单 = 1610311318270000101,
        采购单 = 1611051506540000101,
        会计凭证 = 1611091727140000101,

        资金成本 = 1803051124560000101,
        收款单 = 1611231950150000101,
        收款汇总单 = 1612101120530000101,
        付款单 = 1612011058280000101,
        付款汇总单 = 1612060935280000101,

        收款单_大北农 = 2108021641470000109,
        付款单_大北农 = 2108021638320000109,
        蛋品销售 = 2204020855040000109,
    }
    public enum SystemOptionEnum : long
    {
        /// <summary>
        /// 利润表启用业务类型 -- 集团级
        /// </summary>
        ProfitEnableBus_Group = 20171109164840,
        /// <summary>
        /// 利润表启用附表 -- 集团级
        /// </summary>
        ProfitEnableAttached_Group = 20171113152139,
        /// <summary>
        /// 费用填报启用业务类型 -- 集团级
        /// </summary>
        CostEnableBus_Group = 20171109164935,
        /// <summary>
        /// 销售结账启用：有未审核的单据允许结账 -- 集团级
        /// </summary>
        SAEnableReview_Group = 20180119170341,
        /// <summary>
        /// 采购结账启用：有未审核的单据允许结账 -- 集团级
        /// </summary>
        PMEnableReview_Group = 20180119171931,
        /// <summary>
        /// 会计结账启用：有未审核的单据允许结账 -- 集团级
        /// </summary>
        FMEnableReview_Group = 20180119172021,
        /// <summary>
        /// 启用物品结账
        /// </summary>
        EnableSupplies = 20181213092354,
        /// <summary>
        /// 启用出厂成本模块
        /// </summary>
        EnableFactoryCostMonthEnd = 20190704102132,
        /// <summary>
        /// 启用资金类科目
        /// </summary>
        EnableFunding = 20190805132724,
        /// <summary>
        /// 启用固定资产结账
        /// </summary>
        EnableFAAssets = 20191214111210
    }
    public enum FMErrorEnum
    {
        /// <summary>
        /// 系统错误
        /// </summary>
        [Description("system error")]
        SystemError = -1,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("ok")]
        OK = 0,
        /// <summary>
        /// 无效的请求参数
        /// </summary>
        [Description("invalid request data")]
        InvalidRequestData = 10000,
        /// <summary>
        /// 不合法的参数
        /// </summary>
        [Description("illegal parameters")]
        IllegalParameters = 10001,
        /// <summary>
        /// 提交保存的数据为空
        /// </summary>
        [Description("the saved data is empty")]
        SavedDataIsEmpty = 10002,
        /// <summary>
        /// 远程请求数据
        /// </summary>
        [Description("remote request data")]
        RemoteRequestData = 10003,
        /// <summary>
        /// 远程未知错误
        /// </summary>
        [Description("remote unknown error")]
        RemoteUnknownError = 20001,
        /// <summary>
        /// 没有财务报表模板
        /// </summary>
        [Description("no financial statements template")]
        NoFSTemplate = 30001,
        /// <summary>
        /// 业务请求结果
        /// </summary>
        [Description("business result")]
        BusinessResult = 30002,
        /// <summary>
        /// 没有设置业务分摊系数
        /// </summary>
        [Description("no business sharing factor is set")]
        NoBusinessSharingFactor = 30003,
        /// <summary>
        /// 没有费用汇总数据
        /// </summary>
        [Description("no cost summary data")]
        NoCostSummaryData = 30004,
        /// <summary>
        /// 没有业务类型
        /// </summary>
        [Description("no business type")]
        NoBusinessType = 30005,
        /// <summary>
        /// 结账失败
        /// </summary>
        [Description("checkout fail")]
        CheckoutFail = 30006,
        /// <summary>
        /// 未结账
        /// </summary>
        [Description("no checkout")]
        NoCheckout = 30007,
        /// <summary>
        /// 未审核单据
        /// </summary>
        [Description("no check form")]
        NoCheckForm = 30008,
        /// <summary>
        /// 未设置财务公式
        /// </summary>
        [Description("not set financial formula")]
        NotSetFinancialFormula = 30009,
        /// <summary>
        /// 保存失败
        /// </summary>
        [Description("save failure")]
        SaveFailure = 30010,
        /// <summary>
        /// 远程调用API未找到
        /// </summary>
        [Description("remote call api not found")]
        RemoteCallAPINotFound = 40004,
        /// <summary>
        /// 无权限
        /// </summary>
        [Description("no permission")]
        NoPermission = 50001
    }
    /// <summary>
    /// 财务模块业务查询结果
    /// </summary>
    public enum FMBusinessResultEnum
    {
        /// <summary>
        /// 失败
        /// </summary>
        [Description("fail")]
        FAIL = 0,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("success")]
        SUCCESS = 1,
        /// <summary>
        /// 不存在的数据
        /// </summary>
        [Description("not exist data")]
        NotExistData = 2
    }
}
