using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_AccountInventoryODataEntity : OneWithManyQueryEntity<FD_AccountInventoryDetailODataEntity>
    {
        public FD_AccountInventoryODataEntity()
        {
            Lines = new List<FD_AccountInventoryDetailODataEntity>();
        }

        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// Guid
        /// </summary>		
        public string Guid { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        private DateTime _dataDate;



        /// <summary>
        /// DataDate
        /// </summary>		
        public string DataDate
        {
            get { return _dataDate.ToString("yyyy-MM-dd"); }
            set { _dataDate = Convert.ToDateTime(value); }
        }

        /// <summary>
        /// TicketedPointID
        /// </summary>		
        public string TicketedPointID { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// 负责人ID
        /// </summary>		
        public string ResponsiblePerson { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string ResponsiblePersonName { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        //制单人
        public string OwnerName { get; set; }

        /// <summary>
        /// 不可用资金
        /// </summary>
        public decimal SumFrozeAmount { get; set; }


        /// <summary>
        /// 可用资金
        /// </summary>
        public decimal SumAvailableAmount { get; set; }

        public decimal SumAmount { get; set; }
        

        public string CheckedByName { get; set; }

        public string ReviewName { get; set; }
    }

    public class FD_AccountInventoryDetailODataEntity
    {
        [Key]
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
        public string Guid { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }

        public string AccountName { get; set; }

        /// <summary>
        /// AccoSubjectID
        /// </summary>		
        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 科目编码
        /// </summary>
        public string AccoSubjectCode { get; set; }
        public string cAccoSubjectFullName { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>		
        public string cAccoSubjectName { get; set; }
        /// <summary>
        /// FlowAmount
        /// </summary>		
        public decimal FlowAmount { get; set; }

        /// <summary>
        /// DepositAmount
        /// </summary>		
        public decimal DepositAmount { get; set; }

        /// <summary>
        /// FrozeAmount
        /// </summary>		
        public decimal FrozeAmount { get; set; }


        /// <summary>
        /// 期货保证金
        /// </summary>
        public decimal FuturesBond { get; set; }

        /// <summary>
        /// 其他保证金
        /// </summary>
        public decimal OtherBond { get; set; }

        /// <summary>
        /// 银行冻结
        /// </summary>
        public decimal BankFrozen { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public decimal OtherAmount { get; set; }

        /// <summary>
        /// 账面金额
        /// </summary>
        public decimal BookAmount { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }

        public bool bEnd { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public decimal SumAmount { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public decimal Subtotal { get; set; }

        public decimal Diff { get; set; }

    }

}
