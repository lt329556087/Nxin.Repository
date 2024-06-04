using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Common.MakeVoucherCommon
{

    public class FD_SettleReceipt: FD_SettleReceiptCommand, IRequest<Result>
    {
        public FD_SettleReceipt()
        {
            this.Records = new List<FM_VoucherAmortizationRecord>();
            this.Relateds = new List<FM_VoucherAmortizationRelated>();
            this.Lines = new List<FD_SettleReceiptDetailCommand>();
        }
    }
    public class FD_SettleReceiptCommand : MutilLineCommand<FD_SettleReceiptDetailCommand>
    {
        
        public string CurrentEnterDate { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public Guid Guid { get; set; }

        /// <summary>
        /// SettleReceipType
        /// </summary>
        public string SettleReceipType { get; set; }
        /// <summary>
        /// DataDate
        /// </summary>		
        public DateTime DataDate { get; set; }

        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// Number
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// AccountNo
        /// </summary>
        public string AccountNo { get; set; }
        /// <summary>
        /// AttachmentNum
        /// </summary>
        public string AttachmentNum { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }

        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
        public string TransBeginDate { get; set; }
        public string TransEndDate { get; set; }
        public string SummaryType { get; set; }
        public string SummaryTypeName { get; set; }
        public string ProductLst { get; set; }
        public bool IsSettleCheckOut { get; set; }
        public string RptSearchText { get; set; }
        public FM_CarryForwardVoucherODataEntity CarryData { get; set; }
        public List< FM_VoucherAmortizationRelated> Relateds { get; set; }
        public List< FM_VoucherAmortizationRecord> Records { get; set; }
    }

    public class FD_SettleReceiptDetailCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// Guid
        /// </summary>		
        public Guid Guid { get; set; }
        /// <summary>
        /// EnterpriseID
        /// </summary>
        public string EnterpriseID { get; set; }
        public string ReceiptAbstractID { get; set; }
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 会计科目Code
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectID { get; set; }

        public string CustomerID { get; set; }

        public string PersonID { get; set; }

        public string MarketID { get; set; }

        public string ProjectID { get; set; }
        public string ProductID { get; set; }
        public string PaymentTypeID { get; set; }
        public string AccountID { get; set; }
        public bool LorR { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Content { get; set; }
        public DateTime AgingDate { get; set; }
        public int RowNum { get; set; }
        public string OrganizationSortID { get; set; }
    }
    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }
}
