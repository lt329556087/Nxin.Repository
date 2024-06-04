using FinanceManagement.Domain;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Extensions
{
    public static class ODataServiceExtensions
    {
        /// <summary>
        /// odata服务
        /// </summary>
        /// <param name="app"></param>
        public static void UseODataService(this IApplicationBuilder app)
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder(app.ApplicationServices);
            //实体关联控制器

            builder.EntitySet<FD_AccountTransferODataEntity>("FD_AccountTransferOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<FD_AccountInventoryODataEntity>("FD_AccountInventoryOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<FD_CapitalBudgetODataEntity>("FD_CapitalBudgetOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<FD_BalanceadJustmentODataEntity>("FD_BalanceadJustmentOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FM_CarryForwardVoucherODataEntity>("FM_CarryForwardVoucherOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FM_CarryForwardVoucherRecordODataEntity>("FM_CarryForwardVoucherRecordOData").EntityType.HasKey(p => p.RecordID);
            builder.EntitySet<FM_NewCarryForwardVoucherODataEntity>("FM_NewCarryForwardVoucherOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FM_NewCarryForwardVoucherRecordODataEntity>("FM_NewCarryForwardVoucherRecordOData").EntityType.HasKey(p => p.RecordID);
            builder.EntitySet<FD_VoucherAmortizationODataEntity>("FD_VoucherAmortizationOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_VoucherAmortizationRecordODataEntity>("FD_VoucherAmortizationRecordOData").EntityType.HasKey(p => p.RecordID);
            builder.EntitySet<FM_AccoCheckODataEntity>("FM_AccoCheckOData").EntityType.HasKey(p => p.RecordID);
            builder.EntitySet<FD_CashSweepODataEntity>("FD_CashSweepOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<VoucherHandleInfoEntity>("VoucherHandleOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<AuditODataEntity>("FD_AuditOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<FD_BadDebtProvisionListOnlyODataEntity>("FD_BadDebtProvisionOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<FD_BadDebtOccurListOnly>("FD_BadDebtOccurOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<FD_BadDebtRecoverListOnly>("FD_BadDebtRecoverOData").EntityType.HasKey(p => p.NumericalOrder);

            builder.EntitySet<FD_PaymentReceivablesODataEntity>("FD_PaymentReceivablesOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_AccountAODataEntity>("FD_AccountOData").EntityType.HasKey(p => p.AccountID);

            builder.EntitySet<FD_BadDebtExecutionODataEntity>("FD_BadDebtExecutionOData").EntityType.HasKey(p => p.NumericalOrder);

            //个别认定
            builder.EntitySet<FD_SpecificIdentificationODataEntity>("FD_SpecificIdentificationOData").EntityType.HasKey(p => p.NumericalOrder);


            builder.EntitySet<Biz_CustomerODataEntity>("Biz_CustomerOData").EntityType.HasKey(p => p.CustomerID);
            //  var saCustomerAccountConf = builder.EntitySet<SaCustomeraccount>("CustomerBankAccountOData").EntityType.HasKey(p => p.NumericalOrder);
            //  saCustomerAccountConf.Ignore(o => o.DomainEvents);

            builder.EntitySet<FM_CostParamsSetEntity>("FM_CostParamsSetOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FM_CostProjectEntity>("FM_CostProjectOData").EntityType.HasKey(p => p.CostProjectId);


            builder.EntitySet<BIZ_RelatedODataEntity>("BIZ_RelatedOData").EntityType.HasKey(p => p.RelatedID);
            builder.EntitySet<FD_SettleReceiptODataEntity>("FD_SettleReceiptOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<DropODataEntity>("DropOData").EntityType.HasKey(p => p.id);
            #region 坏账
            //坏账参数设置
            builder.EntitySet<FD_BaddebtSettingODataEntity>("FD_BaddebtSettingOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_BaddebtGroupSettingODataEntity>("FD_BaddebtGroupSettingOData").EntityType.HasKey(p => p.NumericalOrder);
            //个别认定
            builder.EntitySet<FD_IndividualIdentificationODataEntity>("FD_IndividualIdentificationOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_BaddebtAccrualODataEntity>("FD_BaddebtAccrualOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_BaddebtWriteOffODataEntity>("FD_BaddebtWriteOffOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_BaddebtRecoveryODataEntity>("FD_BaddebtRecoveryOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion
            #region 固定资产
            //部门费用科目设置
            builder.EntitySet<FA_MarketSubjectODataEntity>("FA_MarketSubjectOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FA_PurchaseSettingsODataEntity>("FA_PurchaseSettingsOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FA_AssetsApplyListODataEntity>("FA_AssetsApplyOData").EntityType.HasKey(p => p.NumericalOrderDetail);
            builder.EntitySet<FA_AssetsInspectListODataEntity>("FA_AssetsInspectOData").EntityType.HasKey(p => p.NumericalOrderDetail);
            builder.EntitySet<FA_AssetsMaintainListODataEntity>("FA_AssetsMaintainOData").EntityType.HasKey(p => p.NumericalOrderDetail);
            builder.EntitySet<FA_AssetsApplyMobileListODataEntity>("FA_AssetsApplyMobileOData").EntityType.HasKey(p => p.NumericalOrderDetail);
            builder.EntitySet<FA_AssetsApplyMobileByInspectListODataEntity>("FA_AssetsApplyMobileByInspectOData").EntityType.HasKey(p => p.NumericalOrderDetail);
            builder.EntitySet<FA_AssetsContractMobileListODataEntity>("FA_AssetsContractMobileByInspectOData").EntityType.HasKey(p => p.NumericalOrderDetail);
            builder.EntitySet<FA_AssetsContractListODataEntity>("FA_AssetsContractOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FA_UseStateTransferODataEntity>("FA_UseStateTransferOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FA_PigAssetsResetListODataEntity>("FA_PigAssetsResetOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FA_AssetsInspectMobileODataEntity>("FA_AssetsInspectMobileOData").EntityType.HasKey(p => p.NumericalOrderDetail);
            builder.EntitySet<FA_AssetsCardMobileODataEntity>("FA_AssetsCardMobileOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FA_PigAssetsAccrualListODataEntity>("FA_PigAssetsAccrualOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FA_InventoryODataEntity>("FA_InventoryOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion

            #region 费用填报
            builder.EntitySet<FM_ExpensereportEntity>("FM_ExpensereportOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion

            #region 种猪资产原值
            builder.EntitySet<FM_PigOriginalAssetsEntity>("FM_PigOriginalAssetsOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion

            #region 集中支付
            builder.EntitySet<FD_ExpenseODataEntity>("FD_ExpenseOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_PaymentReceivablesEntity>("FD_PaymentReceivablesTOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_PaymentReceivablesEntity>("FD_PaymentReceivablesSummaryOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_PaymentReceivablesHeadEntity>("FD_PaymentReceivablesMergeOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_PaymentReceivablesEntity>("FD_ReceivablesMergeOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<RecheckPaymentList>("FD_PaymentReceivablesRecheckOData").EntityType.HasKey(p => p.RecordId);
            builder.EntitySet<fd_scheduleset>("fd_schedulesetOData").EntityType.HasKey(p => p.RecordId);
            builder.EntitySet<fd_scheduleplanEntity>("fd_scheduleplanOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<BankReceivablesEntity>("FD_BankReceivablesOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FD_SettleReceiptEntity>("FD_SettleReceiptNewOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion
            #region IAP大屏
            //builder.EntitySet<Nxin_DigitalIntelligenceMapODataEntity>("Nxin_DigitalIntelligenceMapOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion

            #region 资金归集
            builder.EntitySet<FM_CashSweepODataEntity>("FM_CashSweepOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<FM_CashSweepSettingODataEntity>("FM_CashSweepSettingOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion

            #region 营销商品成本设定

            builder.EntitySet<FD_MarketingProductCostSettingListQueryModel>("FD_MarketingProductCostSettingOData").EntityType.HasKey(p => p.NumericalOrder);

            #endregion

            #region 总账报表
            builder.EntitySet<AssistRecordResultEntity>("AssistRecordOData").EntityType.HasKey(p => p.UID);
            builder.EntitySet<AssistRecordSummaryResultEntity>("AssistRecordSummaryOData").EntityType.HasKey(p => p.UID);
            #endregion
            #region 进销存
            builder.EntitySet<MS_FormulaListODataEntity>("MS_FormulaOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion

            #region 配方商品价格设定
            builder.EntitySet<MS_FormulaProductPriceODataEntity>("MS_FormulaProductPriceOData").EntityType.HasKey(p => p.NumericalOrder);
            #endregion

            builder.EntitySet<FD_PayextendODataEntity>("FD_PayextendOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<fd_auxiliaryproject>("FD_AuxiliaryOData").EntityType.HasKey(p => p.ProjectId);
            builder.EntitySet<fd_auxiliarytype>("FD_AuxiliaryTypeOData").EntityType.HasKey(p => p.NumericalOrder);
            builder.EntitySet<fd_auxiliarytype>("FD_AuxiliaryUnAuthorizeOData").EntityType.HasKey(p => p.NumericalOrder);

            var model = builder.GetEdmModel();
            //设置路由
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.MapODataServiceRoute("ODataRoute", "oq", model);
                //参数构造
                routeBuilder
                        .MaxTop(5000)
                        .Filter()
                        .Count()
                        .Expand()
                        .OrderBy()
                        .Select()
                        .Expand();
                routeBuilder.EnableDependencyInjection();

            });
        }
    }
}
