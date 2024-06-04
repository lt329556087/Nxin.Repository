using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Architecture.Seedwork.Domain;

namespace FinanceManagement.Domain
{
    public class FM_CashSweepSetting : EntityBase
    {
        public FM_CashSweepSetting()
        {
            CreatedDate = ModifiedDate =  DateTime.Now;
            EnterpriseID = NumericalOrder = OwnerID = "0";
            Lines = new List<FM_CashSweepSettingDetail>();
        }

        public void AddDetail(FM_CashSweepSettingDetail detail)
        {
            Lines.Add(detail);
        }
     

        public void Update( string Remarks)
        {
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<FM_CashSweepSettingDetail> Lines { get; set; }
       
    }
    public class FM_CashSweepSettingDetail : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 详情流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 归集方向(0：向上归集；1：向下归集)
        /// </summary>
        public int? SweepDirection { get; set; }
        /// <summary>
        /// 调拨类别
        /// </summary>
        public string AccountTransferAbstract { get; set; }
        /// <summary>
        /// 调拨事由
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }

        public DateTime ModifiedDate { get; set; }
        [ForeignKey("NumericalOrder")]
        public FM_CashSweepSetting FM_CashSweepSetting { get; set; }
    }

    public class FM_CashSweepSettingExt : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 详情流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 业务类型(0:付款 1：收款)
        /// </summary>
        public int? BusiType { get; set; }
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointID { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string ReceiptAbstractID { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 业务单元
        /// </summary>
        public string OrganizationSortID { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}