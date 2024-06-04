using Architecture.Common.HttpClientUtil;
using Architecture.Seedwork.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FinanceManagement.Common
{
    public class SettleReceiptUtil
    {
        HttpClientUtil _httpClientUtil1;

        HostConfiguration _hostCongfiguration;

        public SettleReceiptUtil(HttpClientUtil httpClientUtil, HostConfiguration hostCongfiguration)
        {
            _httpClientUtil1 = httpClientUtil;
            _hostCongfiguration = hostCongfiguration;
        }

        public RpcResult<ReceiptResult> Save(ReceiptData model)
        {
            var res = _httpClientUtil1.PostJsonAsync<RpcResult<ReceiptResult>>($"{_hostCongfiguration.QlwServiceHost}/api/FDSettleReceiptDetail/SaveByBadDebt", model).Result;
            return res;
        }
    }



    public class RpcResult<T>
    {
        public int code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }

    public class ReceiptResult
    {
        public string NumericalOrder { get; set; }

    }


    public enum SettleReceiptType : long
    {
        收款凭证 = 201610220104402201,
        付款凭证 = 201610220104402202,
        转账凭证 = 201610220104402203
    }

    public class ReceiptData
    {
        public ReceiptData()
        {
            DataA = new SettleReceipt();
            DataB = new List<SettleReceiptDetail>();
        }

        public SettleReceipt DataA { get; set; }

        public List<SettleReceiptDetail> DataB { get; set; }
    }

    public class SettleReceipt
    {
        #region Model
        private long _numericalorder;
        private long _settlereceiptype;
        private DateTime _datadate;
        private long _ticketedpointid;
        private long _number;
        private string _accountno;
        private string _attachmentnum;
        private string _remarks;
        private long _enterid;
        private long _ownerid;
        private DateTime _createddate;
        private DateTime _modifieddate;

        public SettleReceipt()
        {
            ModifiedDate = DateTime.Now;
            CreatedDate = DateTime.Now;
   
        }

   


        public string Guid { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        //类别名称
        public string cDictName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long NumericalOrder
        {
            set { _numericalorder = value; }
            get { return _numericalorder; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SettleReceipType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DataDate { get; set; }

        public DateTime? OldDataDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TicketedPointID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string AccountNo
        {
            set { _accountno = value; }
            get { return _accountno; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string AttachmentNum
        {
            set { _attachmentnum = value; }
            get { return _attachmentnum; }
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
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long OwnerID
        {
            set { _ownerid = value; }
            get { return _ownerid; }
        }
    
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
        public string EnterpriseName { get; set; }

        public string SettleReceipTypeName
        {
            get;
            set;
        }
        #endregion Model

        #region Exend
        public string Content { get; set; }
        public string PayReceType { get; set; }//收付款类型
        #endregion

    }

    public class SettleReceiptDetail
    {
        public SettleReceiptDetail()
        {
            OrganizationSortID = "0";
        }
        #region Model

        public string ReceiptAbstractName { get; set; }
        public string cAccoSubjectFullName { get; set; }

        public string OrganizationSortID { get; set; }

        public string Guid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReceiptAbstractID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectID
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string AccoSubjectCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PersonID { get; set; }


        public string MarketID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LorR { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Debit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Credit { get; set; }


        #endregion Model

        #region 科目相关
        //是否项目核算
        public bool bProject { get; set; }
        //是否客户核算
        public bool bCus { get; set; }
        //是否员工核算
        public bool bPerson { get; set; }
        //是否供应商核算
        public bool bSup { get; set; }
        //是否部门核算
        public bool bDept { get; set; }
        // 类型核算
        public bool bItem { get; set; }
        // 现金核算
        public bool bCash { get; set; }
        //银行核算
        public bool bBank { get; set; }
        //是否资金科目
        public bool bTorF { get; set; }
        public bool bEnd { get; set; }
        /// <summary>
        /// 科目类型
        /// </summary>
        public long AccoSubjectType { get; set; }
        #endregion

        #region Extend
        /// <summary>
        /// 现金流项目
        /// </summary>
        public string FinancialStatementName { get; set; }
        #endregion
        #region 部门是否末级
        public bool MarketIsEnd { get; set; }
        #endregion
    }
}
