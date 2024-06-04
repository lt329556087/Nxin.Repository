using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_AccountTransfer
{
    public class FD_AccountTransferDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FD_AccountTransferAddCommand : FD_AccountTransferCommand, IRequest<Result>
    {
    }

    public class FD_AccountTransferModifyCommand : FD_AccountTransferCommand, IRequest<Result>
    {
    }

    /// <summary>
    /// 表头表体关联
    /// </summary>
    /// <typeparam name="TLineCommand"></typeparam>
    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }

    public class FD_AccountTransferCommand : MutilLineCommand<FD_AccountTransferDetailCommand>
    {
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// AccountTransferType 调拨类型
        /// </summary>		
        public string AccountTransferType { get; set; }
        public string AccountTransferTypeName { get; set; }

        /// <summary>
        /// AccountTransferAbstract 调拨类别
        /// </summary>		
        public string AccountTransferAbstract { get; set; }
        public string AccountTransferAbstractName { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>	
        private DateTime _DateDate;

        public string DataDate
        {
            get { return _DateDate.ToString("yyyy-MM-dd"); }
            set { _DateDate = Convert.ToDateTime(value); }
        }

        /// <summary>
        /// Remarks 备注
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }

        public string ModifiedDate { get; set; }
   
        public decimal Amount { get; set; }
        public decimal RAmount { get; set; }
        public string OwnerName { get; set; }

        public string Results { get; set; }
        public string ResultsName { get; set; }
        //调出单位
        public string OutEnterpriseID { get; set; }
        public string OutEnterpriseName { get; set; }
        public string UploadUrl { get; set; }

    }

    public class FD_AccountTransferDetailCommand : CommonOperate
    {
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// EnterpriseID 调入/出单位
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// Amount
        /// </summary>		
        public decimal Amount { get; set; }
        public string AmountUpper { get; set; }
        public string PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
        /// <summary>
        /// IsIn
        /// </summary>		
        public bool IsIn { get; set; }

        /// <summary>
        /// DataDateTime 归还/调出时间
        /// </summary>		
        public string DataDateTime { get; set; }
   
        /// <summary>
        /// Remarks 事由
        /// </summary>		
        public string Remarks { get; set; }

        public string ModifiedDate { get; set; }

        public string AccountName { get; set; }
        public string DepositBank { get; set; }
        public string AccountNumber { get; set; }
    }



}
