using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceiptNew
{
    public class VoucherCommand : FD_SettleReceiptNewCommand, IRequest<Result>
    {
        /// <summary>
        /// 是否科目合并  true=合并 false = 不合并
        /// </summary>
        public bool IsSummaryAccosubject { get; set; }
    }
    public class FD_SettleReceiptNewFundAddCommand : FD_SettleReceiptNewCommand, IRequest<Result>
    {

    }
    public class FD_SettleReceiptNewOutSideAddCommand : FD_SettleReceiptNewCommand, IRequest<Result>
    {
        /// <summary>
        /// 金融流水号存储
        /// </summary>
        public string JRNumericalOrder { get; set; }
    }
    public class FD_SettleReceiptNewAddCommand : FD_SettleReceiptNewCommand, IRequest<Result>
    {
        /// <summary>
        /// 模板名称(会计凭证/收款单 设置 nxin_qlw_business.fd_receivablesset)
        /// </summary>
        public string TemplateName { get; set; }
    }

    public class FD_SettleReceiptNewDeleteCommand : FD_SettleReceiptNewCommand, IRequest<Result>
    {

    }

    public class FD_SettleReceiptNewModifyCommand : FD_SettleReceiptNewCommand, IRequest<Result>
    {
    }
    
    public class FD_SettleReceiptNewCommand : MutilLineCommand<FD_SettleReceiptDetailInterfacesCommand>
    {
        public FD_SettleReceiptNewCommand()
        {
            this.Lines = new List<FD_SettleReceiptDetailInterfacesCommand>();
        }
        public string UploadInfo { get; set; }

        public string NumericalOrder { get; set; }
        public string Guid { get; set; }
        //public Guid? Guid { get; set; }
        /// <summary>
        /// 凭证类型
        /// </summary>
        public string SettleReceipType { get; set; }
        public string DataDate { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Number { get; set; }
        /// <summary>
        /// 记账号
        /// </summary>
        public string AccountNo { get; set; }
        /// <summary>
        /// 附件数
        /// </summary>
        public string AttachmentNum { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class FD_SettleReceiptDetailInterfacesCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public string Guid { get; set; }
        /// <summary>
        /// 所属单位
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 科目编码
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 客商
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 借贷方（0：借方，1贷方）
        /// </summary>
        public bool LorR { get; set; }
        /// <summary>
        /// 借方金额
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// 贷方金额
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 账龄
        /// </summary>
        public DateTime? AgingDate { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 业务单元
        /// </summary>
        public string OrganizationSortID { get; set; }
        /// <summary>
        /// 是否费用
        /// </summary>
        public bool IsCharges { get; set; }
        /// <summary>
        /// 商品名称Id
        /// </summary>
        public string ProductGroupID { get; set; }
        /// <summary>
        /// 商品分类Id
        /// </summary>
        public string ClassificationID { get; set; }
        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        #endregion
    }
    public class MutilLineCommand<TLineCommand1> : CommonOperate where TLineCommand1 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
    }
}
