using Microsoft.Extensions.DependencyInjection;
using Architecture.Seedwork.Security;
using FinanceManagement.Infrastructure.Repositories;
using FinanceManagement.Infrastructure.Repositories.Interfaces;

namespace FinanceManagement.ApiHost.Extensions
{
    public static class RepositoryCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            //services.AddScoped<IBizCustomerRepository, BizCustomerRepository>();
            //services.AddScoped<IBsdatadictRepository, BsdatadictRepository>();
            //services.AddScoped<ISaCustomeraccountRepository, SaCustomeraccountRepository>();

            services.AddScoped<IBiz_Related, BIZ_RelatedRepositories>();
            services.AddScoped<IBiz_Related_FM, BIZ_Related_FMRepositories>();
            services.AddScoped<IBiz_RelatedDetailRepository, BIZ_RelatedDetailRepository>();
            services.AddScoped<IBiz_ReviewRepository, Biz_ReviewRepository>();

            services.AddScoped<IFD_AccountTransferDetailRepository, FD_AccountTransferDetailRepository>();
            services.AddScoped<IFD_AccountTransferRepository, FD_AccountTransferRepository>();

            services.AddScoped<IFD_AccountInventoryDetailRepository, FD_AccountInventoryDetailRepository>();
            services.AddScoped<IFD_AccountInventoryRepository, FD_AccountInventoryRepository>();

            services.AddScoped<IFD_AccountRepository, FD_AccountRepository>();

            services.AddScoped<IFD_CapitalBudgetRepository, FD_CapitalBudgetRepository>();
            services.AddScoped<IFD_CapitalBudgetDetailRepository, FD_CapitalBudgetDetailRepository>();

            services.AddScoped<IFD_BalanceadJustmentDetailRepository, FD_BalanceadJustmentDetailRepository>();
            services.AddScoped<IFD_BalanceadJustmentRepository, FD_BalanceadJustmentRepository>();

            services.AddScoped<IFM_CarryForwardVoucherDetailRepository, FM_CarryForwardVoucherDetailRepository>();
            services.AddScoped<IFM_CarryForwardVoucherRepository, FM_CarryForwardVoucherRepository>();
            services.AddScoped<IFM_CarryForwardVoucherExtendRepository, FM_CarryForwardVoucherExtendRepository>();
            services.AddScoped<IFM_CarryForwardVoucherFormulaRepository, FM_CarryForwardVoucherFormulaRepository>();

            services.AddScoped<IFD_VoucherAmortizationDetailRepository, FD_VoucherAmortizationDetailRepository>();
            services.AddScoped<IFD_VoucherAmortizationRepository, FD_VoucherAmortizationRepository>();
            services.AddScoped<IFD_VoucherAmortizationPeriodDetailRepository, FD_VoucherAmortizationPeriodDetailRepository>();
            services.AddScoped<IFM_CarryForwardVoucherRecordRepository, FM_CarryForwardVoucherRecordRepository>();

            services.AddScoped<IFD_BadDebtProvisionRepository, FD_BadDebtProvisionRepository>();
            services.AddScoped<IFD_BadDebtProvisionDetailRepository, FD_BadDebtProvisionDetailRepository>();
            services.AddScoped<IFD_BadDebtProvisionExtRepository, FD_BadDebtProvisionExtRepository>();

            services.AddScoped<IFD_BadDebtOccurRepository, FD_BadDebtOccurRepository>();
            services.AddScoped<IFD_BadDebtOccurDetailRepository, FD_BadDebtOccurDetailRepository>();

            services.AddScoped<IFD_BadDebtRecoverRepository, FD_BadDebtRecoverRepository>();
            services.AddScoped<IFD_BadDebtRecoverDetailRepository, FD_BadDebtRecoverDetailRepository>();

            services.AddScoped<IFD_BadDebtExecutionRepository, FD_BadDebtExecutionRepository>();

            services.AddScoped<IFD_BaddebtSettingRepository, FD_BaddebtSettingRepository>();
            services.AddScoped<IFD_BaddebtSettingDetailRepository, FD_BaddebtSettingDetailRepository>();

            services.AddScoped<IFD_SpecificIdentificationRepository, FD_SpecificIdentificationRepository>();
            services.AddScoped<IFD_SpecificIdentificationDetailRepository, FD_SpecificIdentificationDetailRepository>();
            services.AddScoped<IFD_SpecificIdentificationExtRepository, FD_SpecificIdentificationExtRepository>();

            services.AddScoped<IFD_IndividualIdentificationRepository, FD_IndividualIdentificationRepository>();
            services.AddScoped<IFD_IndividualIdentificationDetailRepository, FD_IndividualIdentificationDetailRepository>();
            services.AddScoped<IFD_IndividualIdentificationExtRepository, FD_IndividualIdentificationExtRepository>();

            #region 坏账
            services.AddScoped<IFD_BaddebtGroupSettingRepository, FD_BaddebtGroupSettingRepository>();
            services.AddScoped<IFD_BaddebtGroupSettingDetailRepository, FD_BaddebtGroupSettingDetailRepository>();
            services.AddScoped<IFD_BaddebtGroupSettingExtendRepository, FD_BaddebtGroupSettingExtendRepository>();
            services.AddScoped<IFD_IdentificationTypeRepository, FD_IdentificationTypeRepository>();
            services.AddScoped<IFD_IdentificationTypeSubjectRepository, FD_IdentificationTypeSubjectRepository>();

            services.AddScoped<IFD_BaddebtAccrualRepository, FD_BaddebtAccrualRepository>();
            services.AddScoped<IFD_BaddebtAccrualDetailRepository, FD_BaddebtAccrualDetailRepository>();
            services.AddScoped<IFD_BaddebtAccrualExtRepository, FD_BaddebtAccrualExtRepository>();

            services.AddScoped<IFD_BaddebtWriteOffRepository, FD_BaddebtWriteOffRepository>();
            services.AddScoped<IFD_BaddebtWriteOffDetailRepository, FD_BaddebtWriteOffDetailRepository>();

            services.AddScoped<IFD_BaddebtRecoveryRepository, FD_BaddebtRecoveryRepository>();
            services.AddScoped<IFD_BaddebtRecoveryDetailRepository, FD_BaddebtRecoveryDetailRepository>();
            #endregion
            //成本计算设置
            services.AddScoped<IFM_CostParamsSetRepository, FM_CostParamsSetRepository>();
            services.AddScoped<IFM_CostParamsSetDetailRepository, FM_CostParamsSetDetailRepository>();
            services.AddScoped<IFM_CostParamsSetExtendRepository, FM_CostParamsSetExtendRepository>();

            //费用项目设置
            services.AddScoped<IFmCostprojectRepository, FmCostprojectRepository>();
            services.AddScoped<IFmCostprojectDetailRepository, FmCostprojectDetailRepository>();
            services.AddScoped<IFmCostprojectExtendRepository, FmCostprojectExtendRepository>();

            //结账管理
            services.AddScoped<IFM_AccoCheckDetailRepository, FM_AccoCheckDetailRepository>();
            services.AddScoped<IFM_AccoCheckRepository, FM_AccoCheckRepository>();
            services.AddScoped<IFM_AccoCheckRuleRepository, FM_AccoCheckRuleRepository>();
            services.AddScoped<IFM_AccoCheckExtendRepository, FM_AccoCheckExtendRepository>();

            #region 费用填报
            services.AddScoped<Ifm_expensereportRepository, fm_expensereportRepository>();
            services.AddScoped<Ifm_expensereportdetailRepository, fm_expensereportdetailRepository>();
            services.AddScoped<Ifm_expensereportdetaillogRepository, fm_expensereportdetaillogRepository>();
            services.AddScoped<Ifm_expensereportextendRepository, fm_expensereportextendRepository>();
            services.AddScoped<Ifm_expensereportextendlistRepository, fm_expensereportextendlistRepository>();
            #endregion

            #region 种猪资产原值
            services.AddScoped<IFmPigoriginalassetsRepository, FmPigoriginalassetsRepository>();
            services.AddScoped<IFmPigoriginalassetsdetailRepository, FmPigoriginalassetsdetailRepository>();
            services.AddScoped<IFmPigoriginalassetsdetaillistRepository, FmPigoriginalassetsdetaillistRepository>();
            #endregion

            #region 固定资产
            //部门费用科目设置
            services.AddScoped<IFA_MarketSubjectRepository, FA_MarketSubjectRepository>();
            services.AddScoped<IFA_MarketSubjectDetailRepository, FA_MarketSubjectDetailRepository>();
            services.AddScoped<IFA_PurchaseSettingsRepository, FA_PurchaseSettingsRepository>();
            services.AddScoped<IFA_PurchaseSettingsDetailRepository, FA_PurchaseSettingsDetailRepository>();
            services.AddScoped<IFA_UseStateTransferRepository, FA_UseStateTransferRepository>();
            services.AddScoped<IFA_UseStateTransferDetailRepository, FA_UseStateTransferDetailRepository>();
            services.AddScoped<IFA_AssetsApplyRepository, FA_AssetsApplyRepository>();
            services.AddScoped<IFA_AssetsApplyDetailRepository, FA_AssetsApplyDetailRepository>();
            services.AddScoped<IFA_AssetsMaintainRepository, FA_AssetsMaintainRepository>();
            services.AddScoped<IFA_AssetsMaintainDetailRepository, FA_AssetsMaintainDetailRepository>();
            services.AddScoped<IFA_AssetsContractRepository, FA_AssetsContractRepository>();
            services.AddScoped<IFA_AssetsContractDetailRepository, FA_AssetsContractDetailRepository>();
            services.AddScoped<IFA_AssetsInspectRepository, FA_AssetsInspectRepository>();
            services.AddScoped<IFA_AssetsInspectDetailRepository, FA_AssetsInspectDetailRepository>();
            services.AddScoped<IFA_PigAssetsResetRepository, FA_PigAssetsResetRepository>();
            services.AddScoped<IFA_PigAssetsResetDetailRepository, FA_PigAssetsResetDetailRepository>();
            services.AddScoped<IFA_PigAssetsAccrualRepository, FA_PigAssetsAccrualRepository>();
            services.AddScoped<IFA_PigAssetsAccrualDetailRepository, FA_PigAssetsAccrualDetailRepository>();
            services.AddScoped<IFA_InventoryRepository, FA_InventoryRepository>();
            services.AddScoped<IFA_InventoryDetailRepository, FA_InventoryDetailRepository>();
            #endregion

            #region 申请
            services.AddScoped<IFD_ExpenseRepository, FD_ExpenseRepository>();
            services.AddScoped<IFD_ExpenseDetailRepository, FD_ExpenseDetailRepository>();
            services.AddScoped<IFD_ExpenseExtRepository, FD_ExpenseExtRepository>();
            #endregion

            services.AddScoped<INxin_DigitalIntelligenceMapRepository, Nxin_DigitalIntelligenceMapRepository>();
            services.AddScoped<Ifd_schedulesetRepository, fd_schedulesetRepository>();
            services.AddScoped<Ifd_scheduleplanRepository, fd_scheduleplanRepository>();
            services.AddScoped<IFD_PaymentReceivablesRepository, FD_PaymentReceivablesRepository>();
            services.AddScoped<IFD_PaymentReceivablesDetailRepository, FD_PaymentReceivablesDetailRepository>();
            services.AddScoped<IFD_SettleReceiptRepository, FD_SettleReceiptRepository>();
            services.AddScoped<IFD_SettleReceiptDetailRepository, FD_SettleReceiptDetailRepository>();
            services.AddScoped<IFD_PaymentExtendRepository, FD_PaymentExtendRepository>();
            services.AddScoped<IbsfileRepository, bsfileRepository>();
            services.AddScoped<IFD_ReceivablesRepository, FD_ReceivablesRepository>();
            services.AddScoped<IFD_ReceivablesDetailRepository, FD_ReceivablesDetailRepository>();
            services.AddScoped<IFD_ReceivablesExtendRepository, FD_ReceivablesExtendRepository>();
            services.AddScoped<Ifd_receivablessetRepository, fd_receivablessetRepository>();
            services.AddScoped<Ifd_bankreceivableRepository, fd_bankreceivableRepository>();
            services.AddScoped<Ifd_settletypesetRepository, fd_settletypesetRepository>();
            services.AddScoped<Ifd_paymentreceivablesvoucherRepository, fd_paymentreceivablesvoucherRepository>();
            services.AddScoped<Ifd_paymentreceivablesvoucherDetailRepository, fd_paymentreceivablesvoucherDetailRepository>();
            services.AddScoped<IFM_PerformanceIncomeRepository, FM_PerformanceIncomeRepository>();
            services.AddScoped<Ifm_voucheramortizationrelatedRepository, fm_voucheramortizationrelatedRepository>();
            services.AddScoped<Ifm_transferrealtimeRepository, fm_transferrealtimeRepository>();
            services.AddScoped<Ifm_transferrealtimedetailRepository, fm_transferrealtimedetailRepository>();
            services.AddScoped<IMS_FormulaProductPriceRepository, MS_FormulaProductPriceRepository>();
            services.AddScoped<IMS_FormulaProductPriceDetailRepository, MS_FormulaProductPriceDetailRepository>();
            services.AddScoped<IMS_FormulaProductPriceExtRepository, MS_FormulaProductPriceExtRepository>();

            #region 归集
            services.AddScoped<IFM_CashSweepRepository, FM_CashSweepRepository>();
            services.AddScoped<IFM_CashSweepDetailRepository, FM_CashSweepDetailRepository>();
            services.AddScoped<IFD_PayextendRepository, FD_PayextendRepository>();
            services.AddScoped<IFD_settlereceiptextendRepository, FD_settlereceiptextendRepository>();
            services.AddScoped<IFM_CashSweepLogRepository, FM_CashSweepLogRepository>(); 

            services.AddScoped<IFM_CashSweepSettingRepository, FM_CashSweepSettingRepository>();
            services.AddScoped<IFM_CashSweepSettingDetailRepository, FM_CashSweepSettingDetailRepository>();
            services.AddScoped<IFM_CashSweepSettingExtRepository, FM_CashSweepSettingExtRepository>();
            #endregion

            #region 营销商品成本设定

            services.AddScoped<IFD_MarketingProductCostSettingRepository, FD_MarketingProductCostSettingRepository>();
            services.AddScoped<IFD_MarketingProductCostSettingDetailRepository, FD_MarketingProductCostSettingDetailRepository>();

            #endregion
            #region 进销存
            services.AddScoped<IMS_FormulaDetailRepository, MS_FormulaDetailRepository>();
            services.AddScoped<IMS_FormulaRepository, MS_FormulaRepository>();
            services.AddScoped<IMS_FormulaExtendRepository, MS_FormulaExtendRepository>();
            #endregion
            //凭证类别
            services.AddScoped<Ifd_settletypeRepository, fd_settletypeRepository>();
            //自定义辅助项
            services.AddScoped<Ifd_auxiliaryprojectRepository, fd_auxiliaryprojectRepository>();
            //自定义辅助项类型
            services.AddScoped<Ifd_auxiliarytypeRepository, fd_auxiliarytypeRepository>();

            return services;
        }
    }
}
