using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_BaddebtGroupSettingODataEntity : OneWithManyQueryEntity<FD_BaddebtGroupSettingDetailODataEntity>
    {
        public FD_BaddebtGroupSettingODataEntity()
        {
            Extends = new List<FD_BaddebtGroupSettingExtendODataEntity>();
        }
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 日期
        /// </summary>		
        //public string Number { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>	

        public string StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>		
        public string EndDate { get; set; }

        /// <summary>
        /// 备注
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
        private DateTime _CreatedDate;
        public string CreatedDate
        {
            get { return _CreatedDate.ToString(); }
            set { _CreatedDate = Convert.ToDateTime(value); }
        }


        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }

        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }
        //public string DataStatus { get; set; }
        [NotMapped]
        public List<FD_BaddebtGroupSettingExtendODataEntity> Extends { get; set; }
        [NotMapped]
        public List<FD_IdentificationTypeAndSubjectODataEntity> TypeAndSubjects { get; set; }
        /// <summary>
        /// 应用单位
        /// </summary>
        //[NotMapped]
        public string EnterpriseIDs { get; set; }
        //[NotMapped]
        public string EnterpriseNames { get; set; }
        [NotMapped]
        public bool IsFiltersubject { get; set; }

    }    
    public class FD_BaddebtGroupSettingDetailODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }


        /// <summary>
        /// 类型（0：应收，1：其他应收）
        /// </summary>
        public int? BusType { get; set; }

        public string IntervalType { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 包含天数
        /// </summary>
        public int? DayNum { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int? Serial { get; set; }
        /// <summary>
        /// 计提比例
        /// </summary>		
        public decimal ProvisionRatio { get; set; }

        /// <summary>
        /// CreatedDate 
        /// </summary>		
        public string CreatedDate { get; set; }

        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }
        public string IntervalTypeName { get; set; }
        //public string RowStatus { get; set; }
    }


    public class FD_BaddebtGroupSettingExtendODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 应用单位
        /// </summary>
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 前端显示用
        /// </summary>
        public string ShowID { get; set; }

        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }

        //public string RowStatus { get; set; }
    }
    public class FD_AgingDetaiODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        public string IntervalType { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 包含天数
        /// </summary>
        public int? DayNum { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int? Serial { get; set; }
        public string IntervalTypeName { get; set; }
    }

    public class IdentificationTypeSearchModel
    {
        public string DataDate { get; set; }
        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 计提方式（0：个别认定 1：账龄计提）
        /// </summary>
        public int AccrualType { get; set; }
        /// <summary>
        /// 业务类型（0：应收账款 1：其他应收款）
        /// </summary>
        public int BusiType { get; set; }
    }

    public class DropSubject
    {
        [Key]
        public string AccoSubjectID { get; set; }
        //实体类和接口的科目编号
        public string cAccoSubjectCode { get; set; }
        public string cAccoSubjectName { get; set; }
        public string cAccoSubjectFullName { get; set; }
        //public bool bTorF { get; set; }
        public bool bLorR { get; set; }
        //public bool bEnd { get; set; }
    }
}
