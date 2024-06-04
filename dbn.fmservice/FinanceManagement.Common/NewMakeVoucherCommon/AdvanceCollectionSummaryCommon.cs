using Architecture.Common.HttpClientUtil;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FinanceManagement.Common.MonthEndCheckout;

namespace FinanceManagement.Common.NewMakeVoucherCommon
{
    /// <summary>
    /// 预收款核销
    /// </summary>
    public class AdvanceCollectionSummaryCommon : MakeVoucherBase
    {
        HostConfiguration _hostCongfiguration;
        HttpClientUtil _httpClientUtil;
        public AdvanceCollectionSummaryCommon(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration) : base(httpClientUtil, hostCongfiguration)
        {
            _hostCongfiguration = hostCongfiguration;
            _httpClientUtil = httpClientUtil;
        }
        public override FD_SettleReceipt getVoucherList(FM_NewCarryForwardVoucherODataEntity model, FM_CarryForwardVoucherSearchCommand request, AuthenticationHeaderValue token)
        {
           
            FD_SettleReceipt domain = new FD_SettleReceipt()
            {
                SettleReceipType = request.SettleReceipType,
                TicketedPointID = model.TicketedPointID,
                TransBeginDate = request.Begindate,
                TransEndDate = request.Enddate
            };
            List<FD_SettleReceiptDetailCommand> Lines = new List<FD_SettleReceiptDetailCommand>();
            Dictionary<string, string> dictList = GetDict();
            foreach (var item in model.Lines)
            {
                item.DebitSecFormula = item.DebitSecFormula ?? "0";
                item.CreditSecFormula = item.CreditSecFormula ?? "0";
                if (item.DebitSecFormula.Contains("2208081518000000151") || item.CreditSecFormula.Contains("2208081518000000152")) continue;
                bool isDebit = false;//借贷方标识  false为借，true为贷
                string formular = string.Empty;
                if (!string.IsNullOrEmpty(item.DebitSecFormula) && item.DebitSecFormula != "0") { isDebit = false; formular = item.DebitSecFormula; }
                if (!string.IsNullOrEmpty(item.CreditSecFormula) && item.CreditSecFormula != "0") { isDebit = true; formular = item.CreditSecFormula; }
                List<AdvanceCollectionODataEnetity> AdvanceCollectionSummarys = GetAdvanceCollectionSummaryDataList(request, item, token);//预收款核销数据
                AdvanceCollectionSummarys?.ForEach(summary =>
                {
                    FD_SettleReceiptDetailCommand detail = base.NewObject<AdvanceCollectionODataEnetity>(summary, item, isDebit, formular, dictList);
                    detail.EnterpriseID = request.EnterpriseID;
                    Lines.Add(detail);
                });
            }
            #region 解析借方合计金额和贷方合计金额
            var debit = model.Lines.LastOrDefault(s => s.DebitSecFormula.Contains("2208081518000000151"));
            var credit = model.Lines.LastOrDefault(s => s.CreditSecFormula.Contains("2208081518000000152"));
            if (debit != null)
            {
                Lines.Add(new FD_SettleReceiptDetailCommand()
                {
                    AccoSubjectCode = debit.AccoSubjectCode,
                    AccoSubjectID = debit.AccoSubjectID,
                    ReceiptAbstractID = debit.ReceiptAbstractID,
                    ReceiptAbstractName = debit.ReceiptAbstractName,
                    Debit = Lines.Sum(s => s.Credit),
                    LorR = false,
                });
                domain.Lines = Lines;
                return domain;
            }
            if (credit != null)
            {
                Lines.Add(new FD_SettleReceiptDetailCommand()
                {
                    AccoSubjectCode = credit.AccoSubjectCode,
                    AccoSubjectID = credit.AccoSubjectID,
                    ReceiptAbstractID = credit.ReceiptAbstractID,
                    ReceiptAbstractName = credit.ReceiptAbstractName,
                    Credit = Lines.Sum(s => s.Debit),
                    LorR = true,
                });
                domain.Lines = Lines;
                return domain;
            }
            #endregion
            domain.Lines = Lines;
            return domain;
        }

        private List<AdvanceCollectionODataEnetity> GetAdvanceCollectionSummaryDataList(FM_CarryForwardVoucherSearchCommand request, FM_NewCarryForwardVoucherDetailODataEntity item, AuthenticationHeaderValue token)
        {
            try
            {
                var result = _httpClientUtil.PostJsonAsync<List<AdvanceCollectionODataEnetity>>(_hostCongfiguration._wgUrl + "/dbn/fm/api/FM_NewCarryForwardVoucher/AdvanceCollectionDataList", request, (a) => { a.Authorization = token; }).Result;
                if (result == null)
                {
                    return new List<AdvanceCollectionODataEnetity>();
                }
                List<AdvanceCollectionODataEnetity> list = new List<AdvanceCollectionODataEnetity>()
                    {
                         new AdvanceCollectionODataEnetity()
                         {
                            CancellationAmount = result.Sum(n => n.CancellationAmount),
                         }
                    };
                return list;
            }
            catch (Exception ex)
            {
                return new List<AdvanceCollectionODataEnetity>();
            }
        }
        public Dictionary<string, string> GetDict()
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("1905141325260000101", "CancellationAmount");//核销金额
            return value;
        }
    }
}
