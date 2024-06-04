using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace FinanceManagement.Common.MonthEndCheckout
{
    /// <summary>
    /// 月末结账
    /// </summary>
    public class FMSAccoCheckResultModel : FMSBusResultModel
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public int RecordID { get; set; }
        /// <summary>
        /// 明细流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 结账类型
        /// </summary>
        public string AccoCheckType { get; set; }
        /// <summary>
        /// 菜单
        /// </summary>
        public int MenuId { get; set; }

        private string _optionOpen { get; set; }
        public string OptionOpen
        {
            get
            {
                if (string.IsNullOrEmpty(_optionOpen))
                {
                    return "0";
                }
                return _optionOpen;
            }
            set
            {
                _optionOpen = value;
            }
        }
        /// <summary>
        /// 启用物品结账
        /// </summary>
        private string _optionOpenEndMonth { get; set; }
        public string OptionOpenEndMonth
        {
            get
            {
                if (string.IsNullOrEmpty(_optionOpenEndMonth))
                {
                    return "0";
                }
                return _optionOpenEndMonth;
            }
            set
            {
                _optionOpenEndMonth = value;
            }
        }
        public string IsStepMonth { get; set; }
        public string PigFarmIds { get; set; }
        public AuthenticationHeaderValue _token { get; set; }
    }
    /// <summary>
    /// 财务模块 接受/返回 信息类
    /// </summary>
    public class FMSBusResultModel
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public long AppID { get; set; }

        /// <summary>
        /// 集团ID
        /// </summary>
        public long GroupID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public long EnterpriseID { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public long OwnerID { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }

        /// <summary>
        /// DataDate 转换后的日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
    }
    /// <summary>
    /// 财务模块 接受/返回 信息类
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class FMSAccoCheckResultModel<T> : FMSAccoCheckResultModel
    {
        /// <summary>
        /// 泛型数据
        /// </summary>
        public T data { get; set; }
    }
    public class FMSResultModel<T> : FMSResultModel
    {
        /// <summary>
        /// 泛型数据
        /// </summary>
        public T data { get; set; }

    }
    public class FMSResultModel
    {
        #region api 接口调用结果

        private FMErrorEnum _errcode = FMErrorEnum.OK;
        /// <summary>
        /// 错误码 -- 枚举
        /// </summary>
        public FMErrorEnum errcode
        {
            get { return _errcode; }
            set { _errcode = value; }
        }

        /// <summary>
        /// 错误码 -- 值
        /// </summary>
        public long errcode_value
        {
            get
            {
                return (long)_errcode;
            }
        }

        private string _errmsg = string.Empty;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg
        {
            get
            {
                return _errmsg;
            }
            set
            {
                _errmsg = value;
            }
        }

        #endregion

        #region api 接口调用成功后 业务结果

        private FMBusinessResultEnum _result_code = FMBusinessResultEnum.FAIL;
        /// <summary>
        /// 业务查询结果 -- 枚举
        /// </summary>
        public FMBusinessResultEnum result_code
        {
            get { return _result_code; }
            set { _result_code = value; }
        }
        /// <summary>
        /// 业务查询结果 -- 值
        /// </summary>
        public long result_code_value
        {
            get
            {
                return (long)_result_code;
            }
        }

        private string _result_msg = string.Empty;
        /// <summary>
        /// 业务信息
        /// </summary>
        public string result_msg
        {
            get
            {
                return _result_msg;
            }
            set
            {
                _result_msg = value;
            }
        }

        #endregion

        #region ToString

        /// <summary>
        /// 返回表示当前对象的字符串
        /// 格式：{ 错误码：errcode_value 描述：errmsg }
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("错误码：{0} \r\n描述：{1}", this.errcode_value, this.errmsg);
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// 格式：{ 事件：eventName 错误码：errcode_value 描述：errmsg }
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <returns></returns>
        public string ToString(string eventName)
        {
            return string.Format("事件：{0} \r\n错误码：{1} \r\n描述：{2}", eventName, this.errcode_value, this.errmsg);
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// 格式：{ 事件：eventName 错误码：errcode_value 描述：errmsg  arg0 arg1 ...}
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="args">需要拼接的内容项</param>
        /// <returns></returns>
        public string ToString(string title, params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in args)
            {
                sb.Append("  ");
                sb.Append(item);
            }
            return string.Format("事件：{0} \r\n错误码：{1} \r\n描述：{2}{3}", title, this.errcode_value, this.errmsg, sb);
        }

        #endregion


        #region ToResult

        /// <summary>
        /// 返回一个只包含errcode，errmsg 属性的 object 对象
        /// </summary>
        /// <returns></returns>
        public virtual object ToResult()
        {
            return new
            {
                this.errcode,
                this.errmsg
            };
        }

        /// <summary>
        /// 返回一个包含errcode，errmsg 属性的 object 对象
        /// errcode == FMErrorEnum.OK 时，包含 result_code，result_msg 属性
        /// </summary>
        /// <returns></returns>
        public virtual object ToResultBus()
        {
            if (this.errcode == FMErrorEnum.OK)
            {
                return new
                {
                    this.errcode,
                    this.errmsg,
                    this.result_code,
                    this.result_msg
                };
            }
            return new
            {
                this.errcode,
                this.errmsg
            };
        }


        #endregion
    }
    public class StockResult: EntitySubClass
    {
        public decimal fsQuantity { get; set; }
        public decimal fsCost { get; set; }
        public decimal fsPackages { get; set; }
        public decimal BeginningQuantity { get; set; }
        public decimal BeginningCost { get; set; }
        public decimal BeginningPackages { get; set; }
        public decimal OutboundDeliveryQuantity { get; set; }
        public decimal OutboundDeliveryCost { get; set; }
        public decimal OutboundDeliveryPackages { get; set; }
        public decimal InboundDeliveryQuantity { get; set; }
        public decimal InboundDeliveryCost { get; set; }
        public decimal InboundDeliveryPackages { get; set; }
        public decimal EndingQuantity { get; set; }
        public decimal SafeQuantity { get; set; }
        public decimal EndingCost { get; set; }
        public decimal EndingPackages { get; set; }
        public decimal EndingEnteCost { get; set; }
        public decimal Pandian { get; set; }
        public decimal PandianPackages { get; set; }
        public decimal Chayi { get; set; }
        public decimal ChayiPackages { get; set; }
        public decimal StandardPack { get; set; }
       
        public string UnitName { get; set; }
        public string Specification { get; set; }

        public decimal xishu { get; set; }
        public string batchidd { get; set; }
        public decimal BeginAmount { get; set; }
        public decimal InQTAmount { get; set; }
        public decimal OutQTAmount { get; set; }
        public decimal EndAmount { get; set; }
        public string SummaryType1 { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
    }
    public class WarehouseModel
    {
        public string PersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EnterpriseID { get; set; }
    }
    public class AbstractModel
    {
        public string cDictCode { get; set; }
        public string cDictName { get; set; }
        public int iRank { get; set; }
        public string DictID { get; set; }
    }
    public class CheckResultModel
    {
        public string Number { get; set; }
        public string NumericalOrder { get; set; }
        public string cText { get; set; }
        public string MenuId { get; set; }
    }
    public class ApproveModel
    {
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        public string DataDate { get; set; }
    }
    #region 审核表

    /// <summary>
    /// 信息关联
    /// </summary>
    public partial class BIZ_Reviwe
    {
        /// <summary>
        /// 审核人
        /// </summary>
        public long CheckedByID { get; set; }

        /// <summary>
        /// 级次
        /// </summary>
        public int Level { get; set; }
    }

    #endregion
    public class SubjectBalance: EntitySubClass
    {
        public string AccoSubjectName { get; set; }
        public string AccoSubjectFullName { get; set; }
        public string AccoSubjectID { get; set; }
        public bool IsLorR { get; set; }
        public bool IsCus { get; set; }
        public bool IsSup { get; set; }
        public bool IsPerson { get; set; }
        public decimal qcDebit { get; set; }
        public decimal qcCredit { get; set; }
        /// <summary>
        /// 本期借方
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// 本期贷方
        /// </summary>
        public decimal Credit { get; set; }
        public decimal DebitYear { get; set; }
        public decimal CreditYear { get; set; }
        public decimal fsDebit { get; set; }
        public decimal fsCredit { get; set; }
        public string SummaryType1 { get; set; }
        public string SummaryType1FieldName { get; set; }
        #region 期初余额

        public decimal Show_BegDebit
        {
            get
            {
                decimal num = Calc_BegDebit;
                if (num <= 0)
                {
                    num = Calc_BegCredit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }

        public decimal Show_BegCredit
        {
            get
            {
                decimal num = Calc_BegCredit;
                if (num <= 0)
                {
                    num = Calc_BegDebit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }

        public decimal Calc_BegDebit
        {
            get
            {
                return !IsLorR ? qcDebit + fsDebit - fsCredit - qcCredit : 0;
            }
        }

        public decimal Calc_BegCredit
        {
            get
            {
                return IsLorR ? qcCredit + fsCredit - fsDebit - qcDebit : 0;
            }
        }

        public decimal BegBalance
        {
            get
            {
                return IsLorR ? Show_BegCredit - Show_BegDebit : Show_BegDebit - Show_BegCredit;
            }
        }

        #endregion



        #region 期末余额

        public decimal Show_LastDebit
        {
            get
            {
                decimal num = Calc_LastDebit;
                if (num <= 0)
                {
                    num = Calc_LastCredit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }
        public decimal Show_LastCredit
        {
            get
            {
                decimal num = Calc_LastCredit;
                if (num <= 0)
                {
                    num = Calc_LastDebit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }

        public decimal Calc_LastDebit
        {
            get
            {
                return !IsLorR ? Calc_BegDebit + Debit - Credit : 0;
            }
        }
        public decimal Calc_LastCredit
        {
            get
            {
                return IsLorR ? Calc_BegCredit + Credit - Debit : 0;
            }
        }

        public decimal LastBalance
        {
            get
            {
                return IsLorR ? Show_LastCredit - Show_LastDebit : Show_LastDebit - Show_LastCredit;
            }
        }

        #endregion
    }
    public interface IR2PRequest
    {
    }
    //R:report 报表端
    //F:front 网站端
    //P:plantform 服务端
    /// <summary>
    /// 报表到服务端的请求
    /// </summary>
    public class R2PRequest : IR2PRequest
    {
        /// <summary>
        ///  //是否查询结账数据
        ///  0为实时
        ///  1为结账
        /// </summary>
        public int DataSource { get; set; }
        /// <summary>
        /// 汇总方式1，2，3
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1Name { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public List<string> OwnEntes { get; set; }
        public List<string> GetZBOwnEntes()
        {
            List<string> strLst = new List<string>() {
            "634086739144000981","634086739144000461","634086739144001161"};
            return strLst;
        }
        public List<string> CanWatchEntes { get; set; }
        /// <summary>
        /// 用于产业汇总用
        /// </summary>
        public string SummaryT1Rank { get; set; }
        public string SummaryT2Rank { get; set; }
        public string SummaryT3Rank { get; set; }
        /// <summary>
        /// 判断是否用的是产业/事业部/核算单元的方式汇总
        /// </summary>
        public bool IsGroupByEnteCate { get; set; }
        /// <summary>
        /// 专门针对 2019年3月25-28日提出来的 核算单元 创业单位 organizationID 进行判断的
        /// </summary>
        public bool[] IsGroupByChuangYeDanYuan { get; set; }
        public string EnteCateSummary
        {
            get
            {
                var sum = string.Empty;
                if (!string.IsNullOrEmpty(SummaryType1) && this.SummaryType1.Contains("EnteCate_"))
                {
                    var split = SummaryType1.Split('_')[2];
                    sum += split + "_";
                }
                if (!string.IsNullOrEmpty(SummaryType2) && SummaryType2.Contains("EnteCate_"))
                {
                    var split = SummaryType2.Split('_')[2];
                    sum += split + "_";
                }
                if (!string.IsNullOrEmpty(SummaryType3) && SummaryType3.Contains("EnteCate_"))
                {
                    var split = SummaryType3.Split('_')[2];
                    sum += split + "_";
                }
                return sum;
            }
        }
        public long GroupID { get; set; }
        public long EnteID { get; set; }
        public string Boid { get; set; }
        /// <summary>
        /// 0为业务系统进入用enteid，1为OA菜单进入用groupid
        /// </summary>
        public string MenuParttern { get; set; }
        //public string CustomerField { get; set; }

        #region 权限使用
        /// <summary>
        /// 某一用户下负责的部门。 第一类人
        /// </summary>
        public string MarketsUnderUser { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public string Orgs { get; set; }

        /// <summary>
        /// 是否是公司负责人 第二类人
        /// </summary>
        public bool IsCompanyManager { get; set; }
        /// <summary>
        /// 是否有报表权限， 第三类人
        /// </summary>
        public bool HasReportAuth { get; set; }
        #endregion

    }
    public class CostSummaryDataModel: EntitySubClass
    {

        public string SummaryType1 { get; set; }


        public string SummaryType1FieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2FieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3FieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qcQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qcAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal rkQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cgQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal scQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal tzQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cmQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cgcmQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal dbcmQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal hscmQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal rkAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cgAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal scAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal tzAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cmAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cgcmAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal dbcmAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal hscmAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal syAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ckQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal xsQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal lyQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal kyQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal kyAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal kyscQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal kyscAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qtlyQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal hsQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ckAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal xsAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal tzCost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal lyAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qtlyAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal hsAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ckAmountSum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qmQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qmAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qmUnitAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IsSubTotal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SortId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal yhlrAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal yhlrzcAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal dbzrQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal dbzrAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal dbzcAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal dbzcQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal dqAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qtlzAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal qtlzQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal wzlyAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal wzlyQuantity { get; set; }

    }
    public class PM_PurchaseCarriage: EntitySubClass
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 运费金额
        /// </summary>
        public decimal Amount { get; set; }
    }
    public class CostSummary: EntitySubClass
    {
    /// <summary>
    /// 
    /// </summary>
    public string pigFarmID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string productID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string pigID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string earNumber { get; set; }
    /// <summary>
    /// 乳猪
    /// </summary>
    public string productName { get; set; }
    /// <summary>
    /// 养殖部种猪养殖场
    /// </summary>
    public string pigFarmName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qcTsQuantity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qcOrginalValue { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qcAccumulatedDepreciation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qcPrepaidExpenses { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal cgTsQuantity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal cgZcAmount { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zrTsQuantity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zrOrginalValue { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zrAccumulatedDepreciation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zrPrepaidExpenses { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal cgZcCost { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zcTsQuantity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zcOrginalValue { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zcAccumulatedDepreciation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal zcPrepaidExpenses { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qmTsQuantity { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qmOrginalValue { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qmAccumulatedDepreciation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qmPrepaidExpenses { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public decimal qmUnitExpenses { get; set; }
        public decimal pigCostAmount { get; set; }
    }
    public class SubjectBalanceRequset : R2PRequest
    {
        public string Begindate { get; set; }
        public string Enddate { get; set; }
        /// <summary>
        /// 会计科目 、往来科目
        /// </summary>
        public string AccountingSubjects { get; set; }
        public string AccountingSubjectsRadio { get; set; }

        /// <summary>
        /// 会计科目 、往来科目
        /// </summary>
        public string AccountingSubjects2 { get; set; }
        public string AccountingSubjectsRadio2 { get; set; }

        /// <summary>
        /// 科目类型
        /// </summary>
        public string AccountingType { get; set; }

        /// <summary>
        /// 是否合并
        /// </summary>
        public bool OnlyCombineEnte { get; set; }
        public string EnterpriseList { get; set; }
        public string EnterpriseList_id { get; set; }

        public int SubjectLevel { get; set; }
        public int SubjectLevel2 { get; set; }
    }
    /// <summary>
    /// 资金汇总表
    /// 结果集
    /// </summary>
    public class FundsSummaryResult : EntitySubClass
    {
        /// <summary>
        /// SummaryType
        /// </summary>
        public string SummaryType { get; set; }

        /// <summary>
        /// SummaryTypeName
        /// </summary>
        public string SummaryTypeName { get; set; }
        /// <summary>
        /// 汇总方式1
        /// </summary>
        public string SummaryType1 { get; set; }


        /// <summary>
        /// 期初金额
        /// </summary>
        public decimal BeginningAmount { get; set; }

        /// <summary>
        /// 收款
        /// </summary>
        public decimal Receipt { get; set; }

        /// <summary>
        /// 付款
        /// </summary>
        public decimal Payment { get; set; }

        /// <summary>
        /// 期末金额
        /// </summary>
        public decimal EnddingAmount { get; set; }

        /// <summary>
        /// 农富宝余额
        /// </summary>
        public decimal Financing_NongFuBaoBalance { get; set; }

        /// <summary>
        /// 其他理财余额
        /// </summary>
        public decimal Financing_OtherFinancingBalance { get; set; }

        /// <summary>
        /// 经营性资金——流动资金
        /// </summary>
        public decimal OperatingFunds_FloatingCapital { get; set; }

        /// <summary>
        /// 定期存款
        /// </summary>
        public decimal OperatingFunds_TimeDeposit { get; set; }

        /// <summary>
        /// 不可用资金
        /// </summary>
        public decimal OperatingFunds_UnavailableFunds { get; set; }

        /// <summary>
        /// 未达资金
        /// </summary>
        public decimal OperatingFunds_NotReachFunds { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public decimal OperatingFunds_XiaoJi { get; set; }

        /// <summary>
        /// 募投资金——流动资金
        /// </summary>
        public decimal RaiseInvestmentFunds_FloatingCapital { get; set; }

        /// <summary>
        /// 募投资金——定期存款
        /// </summary>
        public decimal RaiseInvestmentFunds_TimeDeposit { get; set; }

        /// <summary>
        /// 募投资金小计
        /// </summary>
        public decimal RaiseInvestmentFunds_XiaoJi { get; set; }

        /// <summary>
        /// 盘点合计
        /// </summary>
        public decimal PanDianHeJi { get; set; }

        /// <summary>
        /// 差异
        /// </summary>
        public decimal ChaYi { get; set; }
    }
    public class FundsSummaryRequest : RequestModel
    {
        //public string EnteIdForOtherReport { get; set; }
        /// <summary>
        /// 判断是否选得是组织
        /// </summary>
        public bool IsEnterCate { get; set; }
        public string AccountTypeList { get; set; }
        /// <summary>
        public string EnterpriseList { get; set; }

    }
    public class RequestModel
    {
        /// <summary>
        /// 汇总方式 SummaryType 集合的形式
        /// </summary>
        private List<string> summaryTypeList;

        /// <summary>
        /// 汇总方式用"="分割
        /// </summary>
        public string SummaryType { get; set; }

       public List<string> SummaryTypeList { get; set; }

        /// <summary>
        /// 集团ID
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public long EnterpriseId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long BoId { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public int MenuParttern { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }

        

        /// <summary>
        /// 是否合并单位
        /// </summary>
        public bool OnlyCombineEnte { get; set; }

        /// <summary>
        /// 逗号分割的单位ID
        /// </summary>
        public string EnterpriseList { get; set; }

        /// <summary>
        /// 业务单元/部门组织
        /// </summary>
        public string MarketOrgs { get; set; }

        /// <summary>
        /// 逗号分割的部门ID
        /// </summary>
        public string Markets { get; set; }

        /// <summary>
        /// 员工
        /// </summary>
        public string Personnels { get; set; }

        /// <summary>
        /// 已拥用的权限单位
        /// </summary>
        public List<string> OwnEntes { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        public long APPID { get; set; }

        /// <summary>
        ///  是否查询结账数据 0为实时 1为结账
        /// </summary>
        public int DataSource { get; set; }

        /// <summary>
        ///  企店 0不是 1是
        /// </summary>
        public int QiDian { get; set; }

        /// <summary>
        /// 会计期间开始日期
        /// </summary>
        public DateTime APBeginDate { get; set; }

        /// <summary>
        /// 会计期间结束日期
        /// </summary>
        public DateTime APEndDate { get; set; }

        #region 辅助方法


        /// <summary>
        /// 生成缓存key
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns>字符串</returns>
        public string GetCacheKey(string prefix)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}", prefix, this.GroupId, this.EnterpriseId, this.MenuParttern, this.QiDian);
        }

       

       
        #endregion
    }

    public class SettleReceiptBalance:EntitySubClass
    {

        #region 期初余额

        public decimal Show_BegDebit
        {
            get
            {
                decimal num = Calc_BegDebit;
                if (num <= 0)
                {
                    num = Calc_BegCredit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }

        public decimal Show_BegCredit
        {
            get
            {
                decimal num = Calc_BegCredit;
                if (num <= 0)
                {
                    num = Calc_BegDebit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }

        public decimal Calc_BegDebit
        {
            get
            {
                return !IsLorR ? qcDebit + fsDebit - fsCredit - qcCredit : 0;
            }
        }

        public decimal Calc_BegCredit
        {
            get
            {
                return IsLorR ? qcCredit + fsCredit - fsDebit - qcDebit : 0;
            }
        }

        public decimal BegBalance
        {
            get
            {
                return IsLorR ? Show_BegCredit - Show_BegDebit : Show_BegDebit - Show_BegCredit;
            }
        }

        #endregion



        #region 期末余额

        public decimal Show_LastDebit
        {
            get
            {
                decimal num = Calc_LastDebit;
                if (num <= 0)
                {
                    num = Calc_LastCredit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }
        public decimal Show_LastCredit
        {
            get
            {
                decimal num = Calc_LastCredit;
                if (num <= 0)
                {
                    num = Calc_LastDebit;
                    if (num < 0)
                    {
                        num = Math.Abs(num);
                    }
                    else
                    {
                        num = 0;
                    }
                }
                return num < 0 ? 0 : num;
            }
        }

        public decimal Calc_LastDebit
        {
            get
            {
                return !IsLorR ? Calc_BegDebit + Debit - Credit : 0;
            }
        }
        public decimal Calc_LastCredit
        {
            get
            {
                return IsLorR ? Calc_BegCredit + Credit - Debit : 0;
            }
        }

        public decimal LastBalance
        {
            get
            {
                return IsLorR ? Show_LastCredit - Show_LastDebit : Show_LastDebit - Show_LastCredit;
            }
        }

        #endregion
        public int Rank { get; set; }
        public string AccoSubjectSystemID { get; set; }
        public string CustomerName { get; set; }
        public string EnterpriseName { get; set; }
        public string AccoSubjectName { get; set; }
        public string AccoSubjectFullName { get; set; }
        public string AccoSubjectID { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        public bool IsLorR { get; set; }
        public decimal qcDebit { get; set; }
        public decimal qcCredit { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal fsDebit { get; set; }
        public decimal fsCredit { get; set; }
        
        public decimal DebitYear { get; set; }
        public decimal CreditYear { get; set; }

    }
    public class SettleReceiptRequest : R2PRequest
    {
        public int SubjectLevel { get; set; }
        public string EnteIDForOtherReport { get; set; }
        public DateTime Begindate { get; set; }
        public DateTime Enddate { get; set; }

        /// <summary>
        /// 会计科目 、往来科目
        /// </summary>
        public string AccountingSubjects { get; set; }

        /// <summary>
        /// 合并单位
        /// </summary>
        public bool OnlyCombineEnte { get; set; }

        public string EnterpriseList { get; set; }
        public string EnterpriseList_id { get; set; }
        /// <summary>
        /// 往来类型
        /// </summary>
        public string AccountingType { get; set; }
        public bool IsEnterCate { get; set; }
        public string IsAccountingSubjectsTreeCate { get; set; }
        public string AccountingSubjectsRadio { get; set; }
        /// <summary>
        /// 对方单位ID
        /// </summary>
        public string CustomerAndSupplierName_id { get; set; }

        /// <summary>
        /// 对方单位Name
        /// </summary>
        public string CustomerAndSupplierName { get; set; }

        /// <summary>
        /// 员工Name
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string Employee_id { get; set; }
    }
    public class ReceivableSummaryRequest : R2PRequest
    {
        /// <summary>
        /// 是否为Pc段访问，默认为是
        /// </summary>
        public bool IsPc { get; set; }

        /// <summary>
        /// 是否需要查询中间表 默认为是
        /// </summary>
        public bool UseMidTable { get; set; }

        public string MidBeginDate { get; set; }
        public string MidEndDate { get; set; }

        public string RealBeginDate { get; set; }
        public string RealEndDate { get; set; }

        public string Begindate { get; set; }
        public string Enddate { get; set; }
        public bool OnlyCombineEnte { get; set; }
        /// <summary>
        /// 判断是否选得是组织
        /// </summary>
        public bool IsEnterCate { get; set; }
        public string EnterpriseList { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CustomerCategory { get; set; }
        public string CustomerCategory_id { get; set; }
        //public string GroupID { get; set; }

        public string IsPayment { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CustomerType { get; set; }
        /// <summary>
        /// 是否是是赠料 0 ：不是赠料 1：赠料 -1： 全部
        /// </summary>
        public string isGift { get; set; }
        /// <summary>
        /// 金融接口=== 需要选取所有公司，不进行单位筛选。
        /// </summary>
        public bool isJinRong { get; set; }

        /// <summary>
        /// -1 没有启用应收额度
        /// 1、应收帐款/应收额度（默认）
        ///2、应收净额/应收额度
        ///3、（应收帐款+应收其他款）/应收额度
        /// </summary>
        public int eDuZhanBiGongShi { get; set; }
        /// <summary>
        /// 折扣数据来源
        /// </summary>
        public int DiscountSource { get; set; }
        public string onlyCurrentFS { get; set; }
        /// <summary>
        /// 批量计提使用业务员，区域维度
        /// </summary>
        public bool IsPLJTUsePersonMarketDimension { get; set; }
        /// <summary>
        /// 给新企店使用的
        /// </summary>
        public bool ForNewQiDian { get; set; }

        /// <summary>
        /// 应收余额开始
        /// </summary>
        public decimal ScopeStart { get; set; }

        /// <summary>
        /// 应收余额结束
        /// </summary>
        public decimal ScopeEnd { get; set; }

        public int ScopeControl { get; set; }
        public string TimeState { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime _StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime _EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
        /// <summary>
        /// 显示负数
        /// </summary>
        public string ShowMinus { get; set; }
    }
    public class ReceivableSummaryDataResult : DataResult
    {
        //#region 小计专用字段
        //public int iOrder { get; set; }
        //public bool IsSubTotal { get; set; }
        //#endregion

        //public string SummaryType1 { get; set; }
        //public string SummaryType1Name { get; set; }
        //public string SummaryType1FieldName { get; set; }
        //public string SummaryType2 { get; set; }
        //public string SummaryType2Name { get; set; }
        //public string SummaryType2FieldName { get; set; }
        //public string SummaryType3 { get; set; }
        //public string SummaryType3Name { get; set; }
        //public string SummaryType3FieldName { get; set; }
        /// <summary>
        /// 抹零
        /// </summary>
        //public decimal DerateAmount { get; set; }
        private decimal derateAmount;

        public decimal DerateAmount
        {
            get { return derateAmount; }
            set { derateAmount = value; }
        }

        /// <summary>
        /// 抹零期初
        /// </summary>
        //  public decimal DerateAmount_qc { get; set; }
        private decimal derateAmount_qc;

        public decimal DerateAmount_qc
        {
            get { return derateAmount_qc; }
            set { derateAmount_qc = value; }
        }

        /// <summary>
        /// 期初金额
        /// </summary>
        public decimal BeginningAmount { get; set; }
        /// <summary>
        /// 增加额--销售额
        /// </summary>
        //public decimal Increase_SalesAmount { get; set; }
        private decimal _increase_SalesAmount;

        public decimal Increase_SalesAmount
        {
            get { return _increase_SalesAmount; }
            set { _increase_SalesAmount = value; }
        }

        /// <summary>
        /// 增加额--现场折扣
        /// </summary>
        //public decimal Increase_PresentDiscount { get; set; }
        private decimal _increase_PresentDiscount;

        public decimal Increase_PresentDiscount
        {
            get { return _increase_PresentDiscount; }
            set { _increase_PresentDiscount = value; }
        }
        public decimal yuefenhoujieguo { get; set; }
        /// <summary>
        /// 增加额--月度折扣
        /// </summary>
        //public decimal Increase_MonthlyDiscount { get; set; }
        private decimal _increase_MonthlyDiscount;

        public decimal Increase_MonthlyDiscount
        {
            get { return _increase_MonthlyDiscount; }
            set { _increase_MonthlyDiscount = value; }
        }


        //public decimal Increase_XSFS { get; set; }
        private decimal _increase_XSFS;
        /// <summary>
        /// 销售发生---纯粹为了算期初
        /// </summary>
        public decimal Increase_XSFS
        {
            get { return _increase_XSFS; }
            set { _increase_XSFS = value; }
        }


        //public decimal Increase_ZCQC { get; set; }
        private decimal increase_ZCQC;
        /// <summary>
        /// 也是为了纯粹算期初
        /// </summary>
        public decimal Increase_ZCQC
        {
            get { return increase_ZCQC; }
            set { increase_ZCQC = value; }
        }

        /// <summary>
        /// 也是为了算期初。。。叫啥我就不管了。。
        /// </summary>
        public decimal Decrease_SKFS { get; set; }
        /// <summary>
        /// 月折期初
        /// </summary>
        public decimal MonthDiscount_qc { get; set; }
        /// <summary>
        /// 增加额--小计
        /// </summary>
        public decimal Increase_SubTotal { get; set; }

        /// <summary>
        /// 减少额--实收货款
        /// </summary>
        //public decimal Decrease_Payment { get; set; }
        private decimal _decrease_Payment;

        public decimal Decrease_Payment
        {
            get { return _decrease_Payment; }
            set { _decrease_Payment = value; }
        }


        /// <summary>
        /// 减少额--往来冲抵
        /// </summary>
        //public decimal Decrease_OffsetBargain { get; set; }
        private decimal _decrease_OffsetBargain;

        public decimal Decrease_OffsetBargain
        {
            get { return _decrease_OffsetBargain; }
            set { _decrease_OffsetBargain = value; }
        }


        /// <summary>
        /// 减少额--小计
        /// </summary>
        public decimal Decrease_SubTotal { get; set; }

        /// <summary>
        /// 应收账款--应收账款
        /// </summary>
        //public decimal Receivable_Account { get; set; }
        private decimal _receivable_Account;
        /// <summary>
        /// 应收账款
        /// </summary>
        public decimal Receivable_Account
        {
            get { return _receivable_Account; }
            set { _receivable_Account = value; }
        }


        /// <summary>
        /// 应收账款--应收其他款
        /// </summary>
        //public decimal Receivable_OtherAccount { get; set; }
        private decimal _receivable_OtherAccount;

        public decimal Receivable_OtherAccount
        {
            get { return _receivable_OtherAccount; }
            set { _receivable_OtherAccount = value; }
        }


        /// <summary>
        /// 应收账款--小计
        /// </summary>
        public decimal Receivable_SubTotal { get; set; }

        /// <summary>
        /// 预收金额
        /// </summary>
        //public decimal PrePaidAccount { get; set; }
        private decimal _prePaidAccount;

        public decimal PrePaidAccount
        {
            get { return _prePaidAccount; }
            set { _prePaidAccount = value; }
        }

        private decimal _yingshouedu;

        public decimal Yingshouedu
        {
            get { return _yingshouedu; }
            set { _yingshouedu = value; }
        }

        public decimal EduZhanBi
        {
            get; set;
        }

        //public decimal nianzhe { get; set; }
        private decimal _nianzhe;

        public decimal nianzhe
        {
            get { return _nianzhe; }
            set { _nianzhe = value; }
        }


        private decimal _yszk;

        public decimal yszk
        {
            get { return _yszk; }
            set { _yszk = value; }
        }

        //public decimal yszk { get { return 11; }  }

        /// <summary>
        /// 应收净额
        /// </summary>
        //public decimal NetReceivableAccount { get; set; }
        private decimal _netReceivableAccount;

        public decimal NetReceivableAccount
        {
            get { return _netReceivableAccount; }
            set { _netReceivableAccount = value; }
        }

        /// <summary>
        /// 给单位口径用
        /// </summary>
        //public string SortId { get; set; }


    }
    public class PayableSummaryDataResult: EntitySubClass
    {
        #region 小计专用字段
        public int iOrder { get; set; }
        public bool IsSubTotal { get; set; }
        /// <summary>
        /// 汇总方式1
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
        #endregion
        /// <summary>
        /// 期初金额 QCYE= zcqc+t1.AmountTotal_qc-t2.fk_qc
        /// </summary>
        public decimal QCYE { get; set; }
        /// <summary>
        /// 采购数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 结算数量
        /// </summary>
        public decimal SettlementQuantity { get; set; }
        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 调整金额
        /// </summary>
        public decimal AmountAdjust { get; set; }
        /// <summary>
        /// 增加额小计
        /// </summary>
        public decimal AmountTotal { get; set; }
        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal FK { get; set; }
        /// <summary>
        /// 往来冲抵
        /// </summary>
        public decimal WL { get; set; }
        /// <summary>
        /// 减少额小计
        /// </summary>
        public decimal JSTotal { get; set; }
        /// <summary>
        /// 应付账款
        /// </summary>
        public decimal ReceiptAmount { get; set; }

        /// <summary>
        /// 计算用
        /// </summary>
        public decimal ZCQC { get; set; }
        public decimal AmountTotal_qc { get; set; }
        public decimal FK_qc { get; set; }
        public string SortId { get; internal set; }
    }
    public class DataResult:EntitySubClass
    {
        #region 小计专用字段
        public int iOrder { get; set; }
        public bool IsSubTotal { get; set; }
        #endregion
        #region 汇总方式
        /// <summary>
        /// 汇总方式1
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public string SummaryType3FieldName { get; set; }
        #endregion

        public string SortId { get; set; }
    }
    public class SuppliesModelForDataResult: EntitySubClass
    {
        #region 小计 及 汇总 保留字段
        #region 小计专用字段
        public double iOrder { get; set; }
        public bool IsSubTotal { get; set; }
        /// 给单位口径用
        /// </summary>
        public string SortId { get; set; }
        public string iRank { get; set; }
        #endregion
        /// <summary>
        /// 汇总方式1
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public string SummaryType3FieldName { get; set; }
        #endregion 小计 及 汇总 保留字段
        public decimal DifferencePrice { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        public string BatchID { get; set; }
        public string BatchNumber { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string dDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string iNum { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string iNumericalOrder { get; set; }
        /// <summary>
        /// 出入库方式
        /// </summary>
        public string cAbstract { get; set; }
        /// <summary>
        /// 物资仓库
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string cSupplies { get; set; }
        /// <summary>
        /// 物品编号
        /// </summary>
        public string cSuppliesCode { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string cSpecification { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnit { get; set; }
        /// <summary>
        /// 出入库数量
        /// </summary>
        public decimal InOutQuantity { get; set; }
        /// <summary>
        /// 出入库单价
        /// </summary>
        public decimal InOutPrice { get; set; }
        /// <summary>
        /// 出入库金额
        /// </summary>
        public decimal InOutAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string cRemarks { get; set; }
        /// <summary>
        /// 表头备注
        /// </summary>
        public string aRemarks { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal qcQuantity { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal qcAmount { get; set; }

        /// <summary>
        /// 调整金额
        /// </summary>
        public decimal AdjustAmount { get; set; }

        /// <summary>
        /// 出入库类型
        /// </summary>
        public string cDictCode { get; set; }
        /// <summary>
        /// pid
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 单据类型
        /// </summary>
        public string iType { get; set; }



        /// <summary>
        /// 入库数量  汇总使用
        /// </summary>
        public decimal InQuantity { get; set; }
        /// <summary>
        /// 入库金额  汇总使用
        /// </summary>
        public decimal InAmount { get; set; }
        /// <summary>
        /// 出库数量  汇总使用
        /// </summary>
        public decimal OutQuantity { get; set; }
        /// <summary>
        /// 出库金额  汇总使用
        /// </summary>
        public decimal OutAmount { get; set; }

        /// <summary>
        /// 期末数量  汇总使用
        /// </summary>
        public decimal LastQuantity { get; set; }
        /// <summary>
        /// 期末金额  汇总使用
        /// </summary>
        public decimal LastAmount { get; set; }

        /// <summary>
        /// 供应商id
        /// </summary>
        public string SupplierID { get; set; }
        /// <summary>
        /// 供应商Name
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 领用部门ID
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 领用部门名称
        /// </summary>
        public string MarketName { get; set; }
        /// <summary>
        /// 第一级 物资分类名称
        /// </summary>
        public string esxoneSortName { get; set; }

        /// <summary>
        /// 第二级 物资分类名称
        /// </summary>
        public string esxtwoSortName { get; set; }

        /// <summary>
        /// 第三级 物资分类名称
        /// </summary>
        public string esxthreeSortName { get; set; }

        /// <summary>
        /// 二级项目名称
        /// </summary>
        public string ProjectNameTwo { get; set; }
        /// <summary>
        /// 一级项目名称
        /// </summary>
        public string ProjectNameOne { get; set; }
        /// <summary>
        /// 明细表流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 关联流水号
        /// </summary>
        public string NumericalOrderInput { get; set; }

        /// <summary>
        /// 出入库方式
        /// </summary>
        public string OutInWarehouseType { get; set; }

        /// <summary>
        /// 出入库方式名称
        /// </summary>
        public string OutInWarehouseTypeName { get; set; }

        /// <summary>
        /// 猪场
        /// </summary>
        public string PigAddressID { get; set; }

        /// <summary>
        /// 猪场
        /// </summary>
        public string PigAddressName { get; set; }

        /// <summary>
        /// 养户
        /// </summary>
        public string BreedManID { get; set; }

        /// <summary>
        /// 养户
        /// </summary>
        public string BreedManName { get; set; }
        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 物品标识ID
        /// </summary>
        public string SuppliesID { get; set; }
        /// <summary>
        /// 判断是否是期初数据
        /// </summary>
        public bool isQcDataBool { get; set; }




        /// <summary>
        /// 临时id
        /// </summary>
        public int DetailId { get; set; }
        /// <summary>
        /// 临时bool
        /// </summary>
        public bool DetailBool { get; set; }
        /// <summary>
        /// 临时 出入库 1 入库 2 出库 3 出入库
        /// </summary>
        public int DetailInOrOut { get; set; }

        public string iAbstract { get; set; }


        public string SummaryType { get { return $"{SummaryType1}_{SummaryType1Name}_{SummaryType1FieldName}|{SummaryType2}_{SummaryType2Name}_{SummaryType2FieldName}|{SummaryType3}_{SummaryType3Name}_{SummaryType3FieldName}|{cSpecification}|{MeasureUnit}"; } }



        #region 程序中筛选使用
        public string PPMProjectTwoPID { get; set; }
        public string ProjectID { get; set; }
        public string TicketedPointID { get; set; }
        public string WarehouseID { get; set; }
        /// <summary>
        /// 单位分类的轴
        /// </summary>
        public string SuppliesSortcAxis { get; set; }
        /// <summary>
        /// 集团轴分类
        /// </summary>
        public string GroupSuppliesSortcAxis { get; set; }
        #endregion

        #region Extend add by jingkun Xu 20190626
        /// <summary>
        /// 明细主键
        /// </summary>
        public int RecordID { get; set; }

        /// <summary>
        /// 加权单位成本=（期初金额+物品入库金额）/（期初数量+物品入库数量）
        /// </summary>
        public decimal UnitPrice1 { get; set; }

        /// <summary>
        /// 调拨后加权单位成本=（期初金额+物品入库金额+调拨入库金额-调拨转出金额）/（期初数量+物品入库数量+调拨入库数量-调拨转出数量）
        /// </summary>
        public decimal UnitPrice2 { get; set; }

        /// <summary>
        /// 部门关系轴
        /// </summary>
        public string MarketAxis { get; set; }
        #endregion

        /// <summary>
        /// 制单人ID
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 制单人名称
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 是否财务审核
        /// </summary>
        public int IsCwAuditID { get; set; }

        /// <summary>
        /// 是否审批通过-出库
        /// </summary>
        public long IsFaccAudit { get; set; }
        public string StoreType { get; set; }
        public string StoreTypeName { get; set; }
    }
    public class AssetsCardDepreciationSummary: EntitySubClass
    {
        #region 小计 及 汇总 保留字段
        #region 小计专用字段
        public double iOrder { get; set; }
        public bool IsSubTotal { get; set; }
        #endregion
        /// <summary>
        /// 汇总方式1
        /// </summary>
        public string SummaryType1 { get; set; }
        public string SummaryType1FieldName { get; set; }
        public string SummaryType2 { get; set; }
        public string SummaryType2Name { get; set; }
        public string SummaryType2FieldName { get; set; }
        public string SummaryType3 { get; set; }
        public string SummaryType3Name { get; set; }
        public string SummaryType3FieldName { get; set; }
        #endregion 小计 及 汇总 保留字段
        public string CostAccoSubjectName { get; set; }
        public string DepreAssetsAccoSubjectName { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal DepreciationMonthAmount { get; set; }
        public decimal DepreciationAccumulated { get; set; }
        public decimal NetValue { get; set; }
        public decimal DepreciationYearAmount { get; set; }
        public string MarketName1 { get; set; }
        public string MarketName2 { get; set; }
        public string MarketName3 { get; set; }
    }
    public class AssetsCardInfo: EntitySubClass
    {
        public string AssetsName { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal DepreciationAccumulated { get; set; }
    }
    public class Freight
    {
        /// <summary>
        /// 送货上门
        /// </summary>
        public string SummaryType1 { get; set; }
        /// <summary>
        /// 送货上门
        /// </summary>
        public string SummaryType1Name { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string SummaryType1FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType2FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Packages { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal UnitPriceTax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AmountAdjust { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AmountTotal { get; set; }
    }
    public class FreightRequest
    {
       
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string EnterPriseIdList { get; set; }
        public bool OnlyCombineEnte { get; set; }
        public string CarriageAbstract { get; set; }
        public string SupplierNameId { get; set; }
        public string DataSource { get; set; }
        public string SummaryType1 { get; set; }
        public string SummaryType2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryType3 { get; set; }
        public bool IsGroupByEnteCate { get; set; }
        public string GroupID { get; set; }
        public string EnteID { get; set; }
        public string MenuParttern { get; set; }
        public string Boid { get; set; }
        public int SubjectLevel { get; set; }
        public string ProductLst { get; set; }
    }
    public class SuppliesModelForRequeset : R2PRequest
    {
        public SuppliesModelForRequeset()
        {
            this.WarehouseIdAddressList = new List<string>();
        }
        /// <summary>
        /// 是否财务审核
        /// </summary>
        public bool IsCwAudit { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        public string Begindate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string Enddate { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnteNameLst { get; set; }
        /// <summary>
        /// 是否合并单位，此实体中用不到该属性
        /// </summary>
        public bool OnlyCombineEnte { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductTreeLst { get; set; }


        public bool IsCompanyManager { get; set; }
        public string MarketsUnderUser { get; set; }
        public bool HasReportAuth { get; set; }

        /// <summary>
        /// 判断是否选得是组织
        /// </summary>
        public bool IsEnterCate { get; set; }
        /// <summary>
        /// 单位集合
        /// </summary>
        public string EnterpriseList { get; set; }

        /// <summary>
        /// 商品分类
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// 商品 集合
        /// </summary>
        public string ProductLst { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTreeName { get; set; }

        /// <summary>
        /// 存货分类
        /// </summary>
        public string CunHuoFenLei { get; set; }
        /// <summary>
        /// 客户类别 CustomerCategory
        /// </summary>
        public string CustomerCategory { get; set; }
        public string CustomerCategory_id { get; set; }

        /// <summary>
        /// 审核 
        /// 已审核1，
        /// 未审核2，
        /// 全部3
        /// </summary>
        public int IsAudit { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public string SummarySortField { get; set; }
        /// <summary>
        /// 排序字段是否降序
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// 日期规则
        /// </summary>
        public string DateRules { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        public string warehouse_id { get; set; }
        public string warehouseName_id { get; set; }
        /// <summary>
        /// 业务人员
        /// </summary>
        public string BussinessMan_Name { get; set; }
        public string BussinessMan_id { get; set; }


        /// <summary>
        /// 领用部门
        /// </summary>
        public string MarketName { get; set; }
        /// <summary>
        /// 领用部门
        /// </summary>
        public string MarketName_id { get; set; }


        public string IsDepartTreeCate { get; set; }
        public string DepartList { get; set; }
        public string DepartTreeInputName { get; set; }


        public string StoreType { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string GiveCustomer { get; set; }

        /// <summary>
        /// 物资地址
        /// </summary>
        public string WarehouseIdAddress { get; set; }
        public List<string> WarehouseIdAddressList { get; set; }
        /// <summary>
        /// 出库方式
        /// </summary>
        public string OutInWarehouseType { get; set; }
        /// <summary>
        /// 是否去详细列表页
        /// </summary>
        public bool IsToDetail { get; set; }


        /// <summary>
        /// 供应商idList
        /// </summary>
        public string SupplierName_id { get; set; }


        /// <summary>
        /// 一级项目
        /// </summary>
        public string OutInprojectSelectListType { get; set; }

        /// <summary>
        /// 二级项目
        /// </summary>
        public string OutInprojectSelectListTypeTwo { get; set; }
        /// <summary>
        /// 是否开启审批
        /// </summary>
        public bool IsOpenFavor { get; set; }


        /// <summary>
        /// 猪场
        /// </summary>
        public string pigAddressTreeName { get; set; }
        /// <summary>
        /// 猪场
        /// </summary>
        public string pigAddressLst { get; set; }


        /// <summary>
        /// 养户
        /// </summary>
        public string breedManTreeName { get; set; }
        /// <summary>
        /// 养户
        /// </summary>
        public string breedManLst { get; set; }

        public string batchLst { get; set; }
        public string batchTreeName { get; set; }


        /// <summary>
        /// 核算单元
        /// </summary>
        public string TicketedPointSelectList { get; set; }

        /// <summary>
        /// 查询新方法 是否启用结账数据
        /// </summary>
        public bool IsUseNewMethod { get; set; }
        public bool IsNotUseNewMethodAndQc { get; set; }
        /// <summary>
        /// 查询新方法  会计期间起始时间
        /// </summary>
        public string SelectNewMethodUseStartDate { get; set; }
        /// <summary>
        /// 查询新方法 会计期间结束时间
        /// </summary>
        public string SelectNewMethodUseEndDate { get; set; }
        /// <summary>
        /// 查询新方法 请求的开始时间临时 
        /// </summary>
        public DateTime SelectNewMethodUseTempBeginDate { get; set; }
        /// <summary>
        /// 查询新方法  是否跨月
        /// </summary>
        public bool SelectNewMethodIsStepMonth { get; set; }
        /// <summary>
        /// 查询新方法  跨月起始时间
        /// </summary>
        public string SelectNewMethodStepMonthUseStartDate { get; set; }
        /// <summary>
        /// 查询新方法 跨月结束时间
        /// </summary>
        public string SelectNewMethodStepMonthUseEndDate { get; set; }


        /// <summary>
        /// 系统选项：启用物品出库金额用“全月加权平均法”核算       20190319132324
        /// </summary>
        private string _IsOpenAddWeight { get; set; }
        public string IsOpenAddWeight
        {
            get
            {
                if (string.IsNullOrEmpty(_IsOpenAddWeight))
                {
                    return "0";
                }
                return _IsOpenAddWeight;
            }
            set
            {
                _IsOpenAddWeight = value;
            }
        }

        /// <summary>
        /// 系统选项：启用物品结账    ID：20181213092354
        /// </summary>
        private string _IsOpenEndMoneyOption { get; set; }
        public string IsOpenEndMoneyOption
        {
            get
            {
                if (string.IsNullOrEmpty(_IsOpenEndMoneyOption))
                {
                    return "0";
                }
                return _IsOpenEndMoneyOption;
            }
            set
            {
                _IsOpenEndMoneyOption = value;
            }
        }


        public string groupByStr { get; set; }
        /// <summary>
        /// 是否结账数操作
        /// </summary>
        public bool IsMonthFinish { get; set; }

        /// <summary>
        /// 期初查询的是否结账数据
        /// </summary>
        public bool IsQueryMonthFinishData { get; set; }

        /// <summary>
        /// 制单人集合
        /// </summary>
        public string Owners { get; set; }
    }

    public class MySettleReceiptRequest: R2PRequest
    {
        
        public string Begindate { get; set; }
        public string Enddate { get; set; }
        /// <summary>
        /// 凭证字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string EnteNameLst { get; set; }
        public string IsEnterCate { get; set; }
        public string EnterpriseList { get; set; }
        public string BelongTypeID { get; set; }
        public string BelongType { get; set; }
        public string onlyCombineEnte { get; set; }
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 所属员工
        /// </summary>
        public string PersonID { get; set; }
        public string SupplierCategory { get; set; }
        /// <summary>
        /// 所属类别
        /// </summary>
        public string SupplierCategoryID { get; set; }
        /// <summary>
        /// 所属单位
        /// </summary>
        public string BelongEnterPriseNameLst { get; set; }
        public string BelongEnterPriseIDLst { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectNameLst { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectIDLst { get; set; }
        /// <summary>
        /// 凭证类型
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 收付款方式
        /// </summary>
        public string PaymentTypeID { get; set; }

        /// <summary>
        /// 凭证内容
        /// </summary>
        public string TicketedPointContent { get; set; }
        /// <summary>
        /// 一级项目
        /// </summary>
        public string FirstProject { get; set; }
        /// <summary>
        /// 二级项目
        /// </summary>
        public string SecondProject { get; set; }
        /// <summary>
        /// 报表类型
        /// </summary>
        public string reportType { get; set; }


        public bool OnlyCombineEnte { get; set; }
        /// <summary>
        /// 凭证状态
        /// </summary>
        public string pzstatus { get; set; }
        public string CashFlowProjectID { get; set; }
        public string CustomerName { get; set; }
        public string Amount { get; set; }
    }


    #region 一致性检测详情
    /// <summary>
    /// 存货汇总和成本汇总
    /// </summary>
    public class StockCostResult
    {
        public string SummaryType1 { get; set; }
        /// <summary>
        /// 商品代号
        /// </summary>
        public string SummaryType1Name { get; set; }
        /// <summary>
        /// 汇总方式
        /// </summary>
        public string SummaryType1FieldName { get; set; }       
        /// <summary>
        /// 存货汇总表-期末数量
        /// </summary>
        public decimal StockEndQuantity { get; set; }
        /// <summary>
        /// 成本汇总表-期末数量
        /// </summary>
        public decimal CostEndQuantity { get; set; }
        /// <summary>
        /// 差异 存货-成本期末数量
        /// </summary>
        public decimal DiffQuantity { get; set; }
    }
    /// <summary>
    /// 应付账款汇总表和往来余额表
    /// </summary>
    public class PaySummaryAndSettleBalanceResult:SettleBalanceResult
    {      
        /// <summary>
        /// 期初金额 --应付账款
        /// </summary>
        public decimal QCYE { get; set; }
        /// <summary>
        /// 增加额小计 --应付账款
        /// </summary>
        public decimal AmountTotal { get; set; }
        /// <summary>
        /// 减少额小计--应付账款
        /// </summary>
        public decimal JSTotal { get; set; }
        /// <summary>
        /// 应付账款--应付账款
        /// </summary>
        public decimal ReceiptAmount { get; set; }
        /// <summary>
        /// 期初差异
        /// </summary>
        public decimal BeginDiffAmount { get; set; }
        /// <summary>
        /// 本期借方差异
        /// </summary>
        public decimal DebitDiffAmount { get; set; }
        /// <summary>
        /// 本期贷方差异
        /// </summary>
        public decimal CreditDiffAmount { get; set; }
        /// <summary>
        /// 期末余额差异
        /// </summary>
        public decimal EndDiffAmount { get; set; }

        
    }

    /// <summary>
    /// 应收账款汇总表和往来余额表
    /// </summary>
    public class ReceivableSummaryAndSettleBalanceResult : SettleBalanceResult
    {
        /// <summary>
        /// 期初金额 --应收账款
        /// </summary>
        public decimal YSBeginningAmount { get; set; }
        /// <summary>
        /// 增加额小计--应收账款
        /// </summary>
        public decimal YSIncrease_SubTotal { get; set; }

        /// <summary>
        /// 减少额小计--应收账款
        /// </summary>
        public decimal YSDecrease_SubTotal { get; set; }
        /// <summary>
        /// 应收账款--应收账款
        /// </summary>
        public decimal YSReceivable_SubTotal { get; set; }
        /// <summary>
        /// 期初差异
        /// </summary>
        public decimal BeginDiffAmount { get; set; }
        /// <summary>
        /// 本期借方差异
        /// </summary>
        public decimal DebitDiffAmount { get; set; }
        /// <summary>
        /// 本期贷方差异
        /// </summary>
        public decimal CreditDiffAmount { get; set; }
        /// <summary>
        /// 期末余额差异
        /// </summary>
        public decimal EndDiffAmount { get; set; }

    }

    /// <summary>
    /// 往来余额
    /// </summary>
    public class SettleBalanceResult
    {
        public string SummaryType1 { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SummaryType1Name { get; set; }
        public string SummaryType1FieldName { get; set; }
        /// <summary>
        /// 期初余额-往来余额
        /// </summary>
        public decimal BeginBalance { get; set; }
        /// <summary>
        /// 本期借方--往来余额
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// 本期贷方--往来余额
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// 期末余额--往来余额
        /// </summary>
        public decimal LastBalance { get; set; }

        //=============跳转往来明细=============
        public string AccoSubjectCode { get; set; }
        public string CustomerID { get; set; }
        public string AccoSubjectFullName { get; set; }
        //public string AccoSubjectID { get; set; }
        public string CustomerName { get; set; }
    }
    /// <summary>
    /// 收付款单和会计凭证
    /// </summary>  
    public class PaymentreceivablesAndSettlereceipt
    {
        /// <summary>
        /// 流水号--收付款
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期--收付款
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 单据字--收付款
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号--收付款
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 单据类型--收付款
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 资金账户--收付款
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 单据类型名称--收付款
        /// </summary>
        public string SettleReceipTypeName { get; set; }
        /// <summary>
        /// 资金账户名称--收付款
        /// </summary>
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 收方金额
        /// </summary>
        public decimal RAmount { get; set; }
        /// <summary>
        /// 付方金额
        /// </summary>
        public decimal PAmount { get; set; }
        /// <summary>
        /// 是否集中支付生成
        /// </summary>
        public bool IsGroupPay { get; set; }

        /// <summary>
        /// 流水号--凭证
        /// </summary>
        public string SNumericalOrder { get; set; }
        /// <summary>
        /// 日期--凭证
        /// </summary>
        public string SDataDate { get; set; }
        /// <summary>
        /// 单据字--凭证
        /// </summary>
        public string STicketedPointName { get; set; }
        /// <summary>
        /// 单据号--凭证
        /// </summary>
        public string SNumber { get; set; }
        /// <summary>
        /// 单据类型--凭证
        /// </summary>
        public string SSettleReceipType { get; set; }
        /// <summary>
        /// 资金账户--凭证
        /// </summary>
        public string SAccountID { get; set; }
        /// <summary>
        /// 单据类型名称--凭证
        /// </summary>
        public string SSettleReceipTypeName { get; set; }
        /// <summary>
        /// 资金账户名称--凭证
        /// </summary>
        public string SAccountName { get; set; }

        /// <summary>
        /// 贷方金额--凭证
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// 借方金额--凭证
        /// </summary>
        public decimal Debit { get; set; }
    }
    #endregion
    public class PayableSummaryRequest:ReceivableSummaryRequest
    {
        public string SupplierName_id { get; set; }
        public string SupplierName { get; set; }
    }
    public class PayableSummaryModel:FMSBusResultModel
    {
        public string SupplierName_id { get; set; }
        public string SupplierName { get; set; }
        public long EnteID { get; set; }
    }
    
    public class EntitySubClass
    {
        public string AccoSubjectCode { get; set; }
        public string AccoSubjectType { get; set; }
        public string SummaryType1Name { get; set; }


    }
    public class DataSourceEntity
    {
        public DataSourceEntity(string DataSource, List<EntitySubClass> EntityList)
        {
            this.DataSource = DataSource;
            this.EntityList = EntityList;
        }
        public string DataSource { get; set; }
        public List<EntitySubClass> EntityList { get; set; }
    }
}
