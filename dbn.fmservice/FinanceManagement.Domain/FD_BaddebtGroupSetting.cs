using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FD_BaddebtGroupSetting : EntityBase
    {       
        public FD_BaddebtGroupSetting()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = EnterpriseID = "0";
            Details = new List<FD_BaddebtGroupSettingDetail>();
            Extends = new List<FD_BaddebtGroupSettingExtend>();
        }

        public void Update(DateTime DataDate, DateTime StartDate, DateTime? EndDate,  string Remarks)
        {
            this.DataDate = DataDate;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
        }

        public FD_BaddebtGroupSettingDetail AddDetail(FD_BaddebtGroupSettingDetail detail)
        {
            Details.Add(detail);
            return detail;
        }
        public FD_BaddebtGroupSettingExtend AddExtend(FD_BaddebtGroupSettingExtend extend)
        {
            Extends.Add(extend);
            return extend;
        }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>		
        //public string Number { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>	

        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>		
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>		
        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }


        public List<FD_BaddebtGroupSettingDetail> Details { get; set; }
        public List<FD_BaddebtGroupSettingExtend> Extends { get; set; }
        //public List<FD_IdentificationType> Types { get; set; }
        //public List<FD_IdentificationTypeSubject> TypeSubjects { get; set; }
    }

    public class FD_BaddebtGroupSettingDetail : EntityBase
    {     
        public void Update(int BusType,string IntervalType,string Name, int DayNum, int Serial,  decimal ProvisionRatio)
        {
            this.BusType = BusType;
            this.IntervalType = IntervalType;
            this.Name = Name;
            this.BusType = BusType;
            this.DayNum = DayNum;
            this.Serial = Serial;
            this.ProvisionRatio = ProvisionRatio;
            this.ModifiedDate = DateTime.Now;
        }
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
        public int BusType { get; set; }
      
        public string IntervalType { get; set; }

        public string Name { get; set; }
        public int DayNum { get; set; }
        public int Serial { get; set; }
        /// <summary>
        /// 计提比例
        /// </summary>		
        public decimal ProvisionRatio { get; set; }

        /// <summary>
        /// CreatedDate 
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }


        [ForeignKey("NumericalOrder")]
        public FD_BaddebtGroupSetting FD_BaddebtGroupSetting { get; set; }
    }

    public class FD_BaddebtGroupSettingExtend : EntityBase
    {
        public void Update( string EnterpriseID,string ShowID)
        {
            this.ShowID = ShowID;
            this.EnterpriseID = EnterpriseID;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        public string EnterpriseID { get; set; }
        /// <summary>
        /// 前端显示用
        /// </summary>
        public string ShowID { get; set; }
        public DateTime ModifiedDate { get; set; }


        [ForeignKey("NumericalOrder")]
        public FD_BaddebtGroupSetting FD_BaddebtGroupSetting { get; set; }
    }

    public class FD_IdentificationType : EntityBase
    {
        public void Update(int AccrualType)
        {
            this.AccrualType = AccrualType;
        }
        public void Update(string TypeName, int BusiType,int AccrualType)
        {
            this.TypeName = TypeName;
            this.BusiType = BusiType;
            this.AccrualType = AccrualType;
            //this.Remarks = Remarks;
        }
        [Key]
        public string TypeID { get; set; }
        public string TypeName { get; set; }
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public int BusiType { get; set; }
        public int AccrualType { get; set; }
        public string Remarks { get; set; }
        public string OwnerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        [ForeignKey("NumericalOrder")]
        public FD_BaddebtGroupSetting FD_BaddebtGroupSetting { get; set; }
    }

    public partial class FD_IdentificationTypeSubject : EntityBase
    {
        public void Update(string TypeID, string AccoSubjectID,  int? DataSourceType)//bool IsUse,
        {
            this.TypeID = TypeID;
            this.AccoSubjectID = AccoSubjectID;
            //this.IsUse = IsUse;
            this.DataSourceType = DataSourceType;
        }
        [Key]
        public int RecordID { get; set; }
        public string TypeID { get; set; }
        public string AccoSubjectID { get; set; }
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public bool IsUse { get; set; }
        public string OwnerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? DataSourceType { get; set; }
        [ForeignKey("NumericalOrder")]
        public FD_BaddebtGroupSetting FD_BaddebtGroupSetting { get; set; }
    }
}
