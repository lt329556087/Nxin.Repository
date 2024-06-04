using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Common
{
    public class AuditUtil
    {
        HttpClientUtil _httpClientUtil1;

        HostConfiguration _hostCongfiguration;

        public AuditUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<Result<List<AuditModel>>> GetAuditList(List<string> listNum)
        {
            var res = await _httpClientUtil1.PostJsonAsync<Result<List<AuditModel>>>($"{_hostCongfiguration.QlwServiceHost}/api/FAAuditRecord/GetBatchAuditResult", listNum);
            return res;
        }
    }

    public enum AuditState
    {
        审批中 = 4,
        拒绝 = 3,
        驳回 = 2,
        通过 = 1,
        待审批 = 0,
    }

    public class AuditModel
    {
        public string NumericalOrder { get; set; }
        public int AuditStatus { get; set; }
    }



    public class AccountUtil
    {

        HttpClientUtil _httpClientUtil1;

        HostConfiguration _hostCongfiguration;

        public AccountUtil(HttpClientUtil httpClientUtil1, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil1 = httpClientUtil1;
            _hostCongfiguration = hostCongfiguration;
        }

        public async Task<List<FD_Account>> GetAccountInfo(string accountId)
        {
            var res = await _httpClientUtil1.PostJsonAsync<List<FD_Account>>($"{_hostCongfiguration.QlwServiceHost}/api/FDAccount/GetAccountInfo", new { accountId });
            return res;
        }
    }

    public class FD_Account
    {
        public FD_Account()
        { }
        #region Model
        private long _accountid;
        private string _guid;
        private string _accountname;
        private string _accountnumber;
        private long _accounttype;
        private string _accountfullname;
        private long _bankid;
        private string _depositbank;
        private string _address;
        private long _accountusetype;
        private long _responsibleperson;
        private string _responsiblepersonname;
        private string _remarks;
        private long _ownerid;
        private DateTime _createddate;
        private DateTime _modifieddate;
        public int IsUse { get; set; }
        //银行名称
        public string BankName { get; set; }
        public string AccountTypeName { get; set; }
        public string AccountUseTypeName { get; set; }
        public int Sort { get; set; }
        public long EnterpriseID { get; set; }
        /// <summary>
        /// auto_increment
        /// </summary>
        public long AccountID
        {
            set { _accountid = value; }
            get { return _accountid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Guid
        {
            set { _guid = value; }
            get { return _guid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string AccountName
        {
            set { _accountname = value; }
            get { return _accountname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string AccountNumber
        {
            set { _accountnumber = value; }
            get { return _accountnumber; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long AccountType
        {
            set { _accounttype = value; }
            get { return _accounttype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string AccountFullName
        {
            set { _accountfullname = value; }
            get { return _accountfullname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long BankID
        {
            set { _bankid = value; }
            get { return _bankid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long BankAreaID
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string DepositBank
        {
            set { _depositbank = value; }
            get { return _depositbank; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Address
        {
            set { _address = value; }
            get { return _address; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long AccoSubjectID
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public long AccountUseType
        {
            set { _accountusetype = value; }
            get { return _accountusetype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long ResponsiblePerson
        {
            set { _responsibleperson = value; }
            get { return _responsibleperson; }
        }
        public string ResponsiblePersonName
        {
            set { _responsiblepersonname = value; }
            get { return _responsiblepersonname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Remarks
        {
            set { _remarks = value; }
            get { return _remarks; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long OwnerID
        {
            set { _ownerid = value; }
            get { return _ownerid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate
        {
            set { _createddate = value; }
            get { return _createddate; }
        }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>
        public DateTime ModifiedDate
        {
            set { _modifieddate = value; }
            get { return _modifieddate; }
        }
        public string BankNumber { set; get; }
        #endregion Model

        /// <summary>
        /// 开通银企连接
        /// </summary>
        public bool OpenBankEnterConnect { get; set; }
        /// <summary>
        /// 现管账号
        /// </summary>
        public bool TubeAccountNumber { get; set; }

        public bool? SearchForTubeAccountNumber { get; set; }
        public bool? SearchForOpenBankEnterConnect { get; set; }
        public string AccountAreaId { get; set; }
        public string AccountAreaName { get; set; }
        public List<string> OptionsSettingValue { get; set; }
        public string SearchEnterpriseID { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string PaymentTypeID
        {
            get;
            set;
        }
        public string PaymentTypeName
        {
            get;
            set;
        }
        public long ExpenseAccoSubjectID { get; set; }
        public string ExpenseAccoSubjectFullName { get; set; }
        public string MarketID { get; set; }
        public string MarketName { get; set; }
    }

    public class ReceiptExecutionUtil
    {
        IBiz_ReviewRepository _biz_ReviewRepository;
        IFD_BadDebtExecutionRepository _exeRepository;
        SettleReceiptUtil _settleReceiptUtil;

        public ReceiptExecutionUtil(IBiz_ReviewRepository biz_ReviewRepository, IFD_BadDebtExecutionRepository exeRepository, SettleReceiptUtil settleReceiptUtil)
        {
            _biz_ReviewRepository = biz_ReviewRepository;
            _exeRepository = exeRepository;
            _settleReceiptUtil = settleReceiptUtil;
        }

        public async Task<RpcResult<ReceiptResult>> AfterSaveReceipt(ReceiptData recetiptData, string numericalOrder, string appId, string enterpriseId,string userId)
        {
            var resultReceipt = _settleReceiptUtil.Save(recetiptData);

            if (resultReceipt.code == 1)
            {
                //生成制单
                Biz_Review review = new Biz_Review(resultReceipt.data.NumericalOrder, "1611091727140000101", userId).SetMaking();
                await _biz_ReviewRepository.AddAsync(review);
                //添加执行情况:成功
                await _exeRepository.AddAsync(new Domain.FD_BadDebtExecution()
                {
                    AppID = appId,
                    EnterpriseID = enterpriseId,
                    NumericalOrder = numericalOrder,
                    NumericalOrderReceipt = resultReceipt.data.NumericalOrder,
                    State = true,//生成成功
                    CreateDate = DateTime.Now,
                    Remarks = "凭证生成成功"
                }).ConfigureAwait(false);
                await _exeRepository.UnitOfWork.SaveChangesAsync();
            }
            else
            {
                var domain = new Domain.FD_BadDebtExecution()
                {
                    AppID = appId,
                    EnterpriseID = enterpriseId,
                    NumericalOrder = numericalOrder,
                    NumericalOrderReceipt = "0",
                    //NumericalReceipt = result.data.NumericalOrder,
                    State = false,//生成失败
                    CreateDate = DateTime.Now,
                    Remarks = resultReceipt.msg ?? "凭证生成异常"
                };
                //添加执行情况：失败
                await _exeRepository.AddAsync(domain);
                await _exeRepository.UnitOfWork.SaveChangesAsync();
            }

            return resultReceipt;
        }
    }











}
