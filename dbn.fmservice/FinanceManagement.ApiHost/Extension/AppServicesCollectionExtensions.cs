using FinanceManagement.ApiHost.Applications.Queries;
using FinanceManagement.ApiHost.Controllers;
using FinanceManagement.ApiHost.Controllers.FD_AccountInventory;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtExecution;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtOccur;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtProvision;
using FinanceManagement.ApiHost.Controllers.FD_BadDebtRecover;
using FinanceManagement.ApiHost.Controllers.FD_MarketingProductCostSettingManagement;
using FinanceManagement.ApiHost.Controllers.FD_Payment;
using FinanceManagement.ApiHost.Controllers.FM_CostParamsSet;
using FinanceManagement.ApiHost.Controllers.FM_CostProject;
using FinanceManagement.ApiHost.Controllers.FM_Expensereport;
using FinanceManagement.Common;
using FinanceManagement.Common.MonthEndCheckout;
using FinanceManagement.Util;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Extensions
{
    public static class AppServicesCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<FD_SpecificIdentificationODataProvider>();
            services.AddScoped<FD_AccountTransferODataProvider>();
            services.AddScoped<FD_AccountInventoryODataProvider>();
            services.AddScoped<FD_BaddebtSettingODataProvider>();
            services.AddScoped<FD_BaddebtGroupSettingODataProvider>();
            services.AddScoped<FD_CapitalBudgetODataProvider>();
            services.AddScoped<FD_BalanceadJustmentODataProvider>();
            services.AddScoped<FM_CarryForwardVoucherODataProvider>();
            services.AddScoped<FM_NewCarryForwardVoucherODataProvider>();
            services.AddScoped<FD_VoucherAmortizationODataProvider>();
            services.AddScoped<BIZ_DataDictODataProvider>();
            services.AddScoped<TreeModelODataProvider>();
            services.AddScoped<FD_CashSweepODataProvider>();
            services.AddScoped<FD_BadDebtProvisionODataProvider>();
            services.AddScoped<FD_BadDebtOccurODataProvider>();
            services.AddScoped<FD_BadDebtRecoverODataProvider>();
            services.AddScoped<FD_BadDebtExecutionODataProvider>();
            services.AddScoped<FD_PaymentReceivablesODataProvider>();
            services.AddScoped<AuditUtil>();
            services.AddScoped<BankAccountBalanceUtil>();
            services.AddScoped<ApplySubjectUtil>();
            services.AddScoped<FMBaseCommon>();
            services.AddScoped<FMAPIService>();
            services.AddScoped<AccocheckFormulaProperty>();
            services.AddScoped<VoucherAmortizationUtil>();
            services.AddScoped<AgingDataUtil>();
            services.AddScoped<SettleReceiptUtil>();
            services.AddScoped<ReceiptExecutionUtil>();
            services.AddScoped<AccountUtil>();
            services.AddScoped<SubjectBalanceUtil>();
            services.AddScoped<FundSummaryUtil>();
            services.AddScoped<RptAgingReclassificationUtil>();
            services.AddScoped<EnterprisePeriodUtil>();
            services.AddScoped<FM_CostParamsSetODataProvider>();
            services.AddScoped<FM_CostProjectODataProvider>();
            services.AddScoped<FD_AccountODataProvider>();
            services.AddScoped<BIZ_RelatedODataProvider>();
            services.AddScoped<FD_IndividualIdentificationODataProvider>();
            services.AddScoped<FD_BaddebtAccrualODataProvider>();
            services.AddScoped<FD_SettleReceiptODataProvider>();
            services.AddScoped<FM_AccoCheckODataProvider>();
            services.AddScoped<PerformanceIncomeODataProvider>();
            services.AddScoped<FinanceTradeUtil>();
            services.AddScoped<FD_SettleReceiptInterfaceODataProvider>();
            services.AddScoped<fd_settletypesetODataProvider>();
            services.AddScoped<FD_MallPaymentODataProvider>();
            services.AddScoped<SalesSummaryODataProvider>();
            //波尔莱特定制化 销售汇总表
            services.AddScoped<SalesSummaryBorODataProvider>();
            //波尔莱特定制化 利润表
            services.AddScoped<ProfitBorODataProvider>();
            services.AddScoped<FD_MarketingProductCostSettingODataProvider>();
            services.AddScoped<RptAssistRecordODataProvider>();
            services.AddScoped<MS_StockCostODataProvider>();
            services.AddScoped<MS_FormulaODataProvider>();
            services.AddScoped<CostAnalysisODataProvider>();
            services.AddScoped<MS_FormulaProductPriceODataProvider>();

            services.AddScoped<FormulaCostODataProvider>();
            services.AddScoped<ViewModel>();
            services.AddScoped<BusinessOutputProvider>();
            services.AddScoped<ProductionCostODataProvider>();

            services.AddScoped<ExportCommon>();
            #region 坏账
            services.AddScoped<FD_BaddebtWriteOffODataProvider>();
            services.AddScoped<FD_BaddebtRecoveryODataProvider>();
            #endregion

            #region 固定资产
            services.AddScoped<FA_MarketSubjectODataProvider>();           
            services.AddScoped<FA_UseStateTransferODataProvider>();
            services.AddScoped<FA_PurchaseSettingsODataProvider>();
            services.AddScoped<FA_AssetsApplyODataProvider>();
            services.AddScoped<FA_AssetsContractODataProvider>();
            services.AddScoped<FA_AssetsMaintainODataProvider>();
            services.AddScoped<FA_AssetsInspectODataProvider>();
            services.AddScoped<FA_PigAssetsResetODataProvider>();
            services.AddScoped<FA_PigAssetsAccrualODataProvider>();
            services.AddScoped<AssetsCommonUtil>();
            services.AddScoped<FA_InventoryODataProvider>();
            #endregion

            #region 费用填报
            services.AddScoped<FM_ExpensereportODataProvider>();
            services.AddScoped<FM_ExpenseCalFactory>();
            #endregion

            #region 种猪资产原值
            services.AddScoped<FM_PigOriginalAssetsODataProvider>();
            #endregion

            #region 集中支付
            services.AddScoped<fd_schedulesetODataProvider>();
            services.AddScoped<fd_scheduleplanODataProvider>();
            services.AddScoped<FD_ExpenseODataProvider>();
            services.AddScoped<FD_PaymentReceivablesTODataProvider>();
            services.AddScoped<FD_SettleReceiptNewODataProvider>();
            #endregion
            #region IAP大屏自定义配置
            services.AddScoped<Nxin_DigitalIntelligenceMapODataProvider>();
            #endregion

            #region 资金归集
            services.AddScoped<FM_CashSweepODataProvider>();
            services.AddScoped<FM_CashSweepSettingODataProvider>();
            #endregion
            services.AddScoped<FD_AuxiliaryODataProvider>();

            return services;
        }

        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration Configuration)
        {
            var host = new HostConfiguration();
            host.QlwServiceHost = Configuration.GetSection("AppSettings:_rdUrl").Value;
            host.ReportService = Configuration.GetSection("AppSettings:_rptUrl").Value;
            host.YQTBalLink = Configuration.GetSection("AppSettings:_yqtLink").Value;
            host.YQTAccessToken = Configuration.GetSection("AppSettings:_yqtToken").Value;
            host.OrderKey = Configuration.GetSection("AppSettings:OrderKey").Value;
            host.baseQlwServiceUrl = Configuration.GetSection("AppSettings:_wgUrl").Value;
            host.NxinGatewayUrl = Configuration.GetSection("AppSettings:NxinGatewayUrl").Value;
            host.CheckenAppId = Configuration.GetSection("AppSettings:CheckenAppId").Value;
            host.NxinGatewayInnerUrl = Configuration.GetSection("AppSettings:NxinGatewayInnerUrl").Value;
            host.OAToBanking = Configuration.GetSection("AppSettings:OAToBanking").Value;
            host.OAToBankingPass = Configuration.GetSection("AppSettings:OAToBankingPass").Value;
            host._wgUrl = Configuration.GetSection("AppSettings:_wgUrl").Value;
            host._rdUrl = Configuration.GetSection("AppSettings:_rdUrl").Value;
            host.qlwMobileUrl = Configuration.GetSection("AppSettings:qlwMobileUrl").Value;
            host.fsfMobileUrl = Configuration.GetSection("AppSettings:fsfMobileUrl").Value;
            host.DBN_HrServiceHost = Configuration.GetSection("AppSettings:DBN_HrServiceHost").Value;
            host.DBN_ZLWServiceHost = Configuration.GetSection("AppSettings:DBN_ZLWServiceHost").Value;
            host.NoticeAccountID = Configuration.GetSection("AppSettings:NoticeAccountID").Value;
            host.EnterpriseUnionKey = Configuration.GetSection("AppSettings:EnterpriseUnionKey").Value;
            host.QlwBase = Configuration.GetSection("AppSettings:qlwbase").Value;
            host.FinanceJobGroup = Configuration.GetSection("AppSettings:FinanceJobGroup").Value;
            host.CashsweepAesKey = Configuration.GetSection("AppSettings:CashsweepAesKey").Value;
            host.FmXxlJobUrl = Configuration.GetSection("AppSettings:FmXxlJobUrl").Value;
            host.RedisServer = Configuration.GetSection("AppSettings:RedisServer").Value;
            host.IsEnableNewWorkflow = Configuration.GetSection("AppSettings:IsEnableNewWorkflow").Value;
            host.NewWorkflowHost = Configuration.GetSection("AppSettings:NewWorkflowHost").Value;
            host.dbnfmUrl = Configuration.GetSection("AppSettings:_dbnfmUrl").Value;
            host.ScUrl = Configuration.GetSection("AppSettings:ScUrl").Value;
            services.AddSingleton(host);
            return services;
        }
    }






}
