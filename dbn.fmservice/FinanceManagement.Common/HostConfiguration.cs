using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Common
{
    public class HostConfiguration
    {
        public string QlwServiceHost { get; set; }
        public string ReportService { get; set; }
        public string YQTBalLink { get; set; }
        public string YQTAccessToken { get; set; }
        public string baseQlwServiceUrl { get; set; }
        public string NxinGatewayUrl { get; set; }
        public string OrderKey { get; set; }
        public string NxinGatewayInnerUrl { get; set; }
        public string CheckenAppId { get; set; }
        public string OAToBanking { get; set; }
        public string OAToBankingPass { get; set; }
        public string _wgUrl { get; set; }
        public string _rdUrl { get; set; }
        public string qlwMobileUrl { get; set; }
        public string fsfMobileUrl { get; set; }
        public string NoticeAccountID { get; set; }
        public string DBN_HrServiceHost { get; set; }
        public string DBN_ZLWServiceHost { get; set; }
        public string EnterpriseUnionKey { get; set; }
        public string QlwBase { get; set; }
        /// <summary>
        /// 财务执行器 xxl-job
        /// </summary>
        public string FinanceJobGroup { get; set; }
        public string FmXxlJobUrl { get; set; }
        /// <summary>
        /// 资金归集安全认证地址 账号加密key 
        /// </summary>
        public string CashsweepAesKey { get; set; }
        public string RedisServer { get; set; }

        // 是否启用新工作流
        public string IsEnableNewWorkflow { get; set; }
        // 新工作流地址
        public string NewWorkflowHost { get; set; }
        public string dbnfmUrl { get; set; }
        //商城
        public string ScUrl { get; set; }
    }
}
