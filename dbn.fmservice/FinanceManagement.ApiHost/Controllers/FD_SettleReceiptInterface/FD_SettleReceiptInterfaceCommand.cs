using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceiptInterface
{

    public class FD_SettleReceiptBatchReviewCommand : FD_SettleReceiptInterfaceCommand, IRequest<Result>
    {
    }
    public class FD_SettleReceiptBatchCancelReviewCommand : FD_SettleReceiptInterfaceCommand, IRequest<Result>
    {
    }
    public class FD_SettleReceiptRemakeCommand : FD_SettleReceiptInterfaceCommand, IRequest<Result>
    {
        public string beginDate { get; set; }
        public string endDate { get; set; }
        /// <summary>
        /// true = 按凭证号重新顺次排序  false = 按凭证日期重新顺次排序
        /// </summary>
        public bool IsOrder { get; set; }
        /// <summary>
        /// 起始编码
        /// </summary>
        public long? StartNumber { get; set; } = 1;
    }
    public class FD_SettleReceiptInterfaceAddCommand : FD_SettleReceiptInterfaceCommand, IRequest<Result>
    {

    }

    public class FD_SettleReceiptInterfaceDeleteCommand : FD_SettleReceiptInterfaceCommand, IRequest<Result>
    {

    }

    public class FD_SettleReceiptInterfaceModifyCommand : FD_SettleReceiptInterfaceCommand, IRequest<Result>
    {
    }
    
    public class FD_SettleReceiptInterfaceCommand : MutilLineCommand<FD_SettleReceiptDetailInterfaceCommand>
    {
        public FD_SettleReceiptInterfaceCommand()
        {
            this.Lines = new List<FD_SettleReceiptDetailInterfaceCommand>();
        }
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
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
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class FD_SettleReceiptDetailInterfaceCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
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
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
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
    }
    public class MutilLineCommand<TLineCommand1> : CommonOperate where TLineCommand1 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
    }
    /// <summary>
    /// 会计凭证导出参数
    /// </summary>
    public class VoucherParam
    {
        /// <summary>
        /// 单位ID  多单位  逗号分隔  1,2,3,4,5
        /// </summary>
        public string EnterpriseIds { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketPoint { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 凭证类别
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerId { get; set; }
        public string NumericalOrder { get; set; }
    }
}
