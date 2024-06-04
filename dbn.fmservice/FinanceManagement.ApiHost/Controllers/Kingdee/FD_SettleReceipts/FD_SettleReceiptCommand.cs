using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_SettleReceipt
{

    public class FD_SettleReceiptAddCommand : FD_SettleReceiptCommand, IRequest<Result>
    {

    }

    public class FD_SettleReceiptDeleteCommand : FD_SettleReceiptCommand, IRequest<Result>
    {

    }

    public class FD_SettleReceiptModifyCommand : FD_SettleReceiptCommand, IRequest<Result>
    {
    }
    public class FD_SettleReceiptListModifyCommand : FD_SettleReceiptCommand, IRequest<Result>
    {
    }
    public class FD_SettleReceiptCommand : MutilLineCommand<FD_SettleReceiptDetailCommand>
    {
        public FD_SettleReceiptCommand()
        {
            this.Lines = new List<FD_SettleReceiptDetailCommand>();
        }
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 凭证类型
        /// </summary>
        public string SettleReceipType { get; set; }
        public string SettleReceipTypeName { get; set; }
        public string DataDate { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
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
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class FD_SettleReceiptDetailCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 所属单位
        /// </summary>
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectID { get; set; }
        public string AccoSubjectCode { get; set; }
        public string AccoSubjectName { get; set; }
        /// <summary>
        /// 客商
        /// </summary>
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string PersonID { get; set; }
        public string PersonName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        /// <summary>
        /// 商品
        /// </summary>
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        public string AccountID { get; set; }
        public string AccountName { get; set; }
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
        public DateTime AgingDate { get; set; }
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

    

}
