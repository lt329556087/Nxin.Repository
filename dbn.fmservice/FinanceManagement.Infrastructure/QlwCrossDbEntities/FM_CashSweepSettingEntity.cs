using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    //主表一条，detail表向上、向下归集共两条，extend表向上、向下分别有收款和付款共四条
    public class FM_CashSweepSettingODataEntity : OneWithManyQueryEntity<FM_CashSweepSettingDetailODataEntity>
    {
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
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
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }

    }    
    public class FM_CashSweepSettingDetailODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
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

        public string ModifiedDate { get; set; }
        /// <summary>
        /// 调拨类别名称
        /// </summary>
        public string AccountTransferAbstractName { get; set; }
        /// <summary>
        /// 制单人名称
        /// </summary>
        public string OwnerName { get; set; }
        [NotMapped]
        public List<FM_CashSweepSettingExtODataEntity> Extends { get; set; }
    }


    public class FM_CashSweepSettingExtODataEntity
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
        public string ModifiedDate { get; set; }

        public string TicketedPointName { get; set; }
        /// <summary>
        /// 摘要名称
        /// </summary>
        public string ReceiptAbstractName { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>
        public string AccoSubjectName { get; set; }
        /// <summary>
        /// 科目全称
        /// </summary>
        public string AccoSubjectFullName { get; set; }  
        /// <summary>
        /// 科目编码
        /// </summary>
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 业务单元名称
        /// </summary>
        public string OrganizationSortName { get; set; }

        public string OwnerName { get; set; }
    }
   
}
