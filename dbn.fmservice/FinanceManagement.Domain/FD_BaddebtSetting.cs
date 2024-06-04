using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FD_BaddebtSetting : EntityBase
    {
        public FD_BaddebtSetting()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            ProvisionMethod = "0";
            NumericalOrder = "0";
            OwnerID = EnterpriseID = "0";
            BadAccoSubjectOne = BadAccoSubjectTwo = OtherAccoSubjectOne = OtherAccoSubjectTwo = DebtReceAccoSubjectOne = DebtReceAccoSubjectTwo = "0";
            ProvisionReceiptAbstractID= OccurReceiptAbstractID= RecoverReceiptAbstractID = "0";
        }

        public void Update(DateTime DataDate, string ProvisionMethod, string BadAccoSubjectOne, string BadAccoSubjectTwo, string OtherAccoSubjectOne, string OtherAccoSubjectTwo, string DebtReceAccoSubjectOne, string DebtReceAccoSubjectTwo, string ReceAccoSubjectOne, string ReceAccoSubjectTwo, string ProvisionReceiptAbstractID,string OccurReceiptAbstractID,string RecoverReceiptAbstractID, string ReversalReceiptAbstractID, string BadReversalReceiptAbstractID, string EnterpriseID)//,string GroupNumericalOrder)
        {
            this.DataDate = DataDate;
            this.ProvisionMethod = ProvisionMethod;
            this.BadAccoSubjectOne = BadAccoSubjectOne;
            this.BadAccoSubjectTwo = BadAccoSubjectTwo;
            this.OtherAccoSubjectOne = OtherAccoSubjectOne;
            this.OtherAccoSubjectTwo = OtherAccoSubjectTwo;
            this.DebtReceAccoSubjectOne = DebtReceAccoSubjectOne;
            this.DebtReceAccoSubjectTwo = DebtReceAccoSubjectTwo;
            this.ReceAccoSubjectOne = ReceAccoSubjectOne;
            this.ReceAccoSubjectTwo = ReceAccoSubjectTwo;
            this.ProvisionReceiptAbstractID = ProvisionReceiptAbstractID;
            this.OccurReceiptAbstractID = OccurReceiptAbstractID;
            this.RecoverReceiptAbstractID = RecoverReceiptAbstractID;
            this.BadReversalReceiptAbstractID = BadReversalReceiptAbstractID;
            this.ReversalReceiptAbstractID = ReversalReceiptAbstractID;
            //this.GroupNumericalOrder = GroupNumericalOrder;
            //this.OwnerID = OwnerID;
            this.EnterpriseID = EnterpriseID;
            this.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 计提方法
        /// </summary>		
        public string ProvisionMethod { get; set; }

        /// <summary>
        /// 坏账准备科目--应收账款
        /// </summary>		
        public string BadAccoSubjectOne { get; set; }

        /// <summary>
        /// 坏账准备科目--其他应收账款
        /// </summary>	

        public string BadAccoSubjectTwo { get; set; }

        /// <summary>
        /// 减值损失科目-应收账款
        /// </summary>		
        public string OtherAccoSubjectOne { get; set; }
        /// <summary>
        /// 减值损失科目-其他应收账款
        /// </summary>		
        public string OtherAccoSubjectTwo { get; set; }
        /// <summary>
        /// 应收账款-应收账款
        /// </summary>		
        public string DebtReceAccoSubjectOne { get; set; }
        /// <summary>
        /// 应收账款-其他应收账款
        /// </summary>		
        public string DebtReceAccoSubjectTwo { get; set; }
        /// <summary>
        /// 收款科目--应收账款
        /// </summary>		
        public string ReceAccoSubjectOne { get; set; }
        /// <summary>
        /// 收款科目--其他应收账款
        /// </summary>		
        public string ReceAccoSubjectTwo { get; set; }

        /// <summary>
        /// 计提坏账摘要
        /// </summary>		
        public string ProvisionReceiptAbstractID { get; set; }
        /// <summary>
        /// 坏账发生摘要
        /// </summary>		
        public string OccurReceiptAbstractID { get; set; }
        /// <summary>
        /// 坏账收回摘要
        /// </summary>		
        public string RecoverReceiptAbstractID { get; set; }
        /// <summary>
        /// 计提冲销摘要
        /// </summary>		
        public string ReversalReceiptAbstractID { get; set; }
        /// <summary>
        /// 坏账冲销摘要
        /// </summary>		
        public string BadReversalReceiptAbstractID { get; set; }
        /// <summary>
        /// 集团流水号
        /// </summary>
        //public string GroupNumericalOrder { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }


        //public List<FD_BaddebtSettingDetail> Details { get; set; }

    }

    public class FD_BaddebtSettingDetail : EntityBase
    {

        public void Update(int AgingIntervalID, decimal ProvisionRatio,  string Remarks,int BusType)
        {
            this.AgingIntervalID = AgingIntervalID;
            this.ProvisionRatio = ProvisionRatio;
            this.Remarks = Remarks;
            this.BusType = BusType;
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
        /// 账龄区间设置RecordID
        /// </summary>		
        public int AgingIntervalID { get; set; }


        /// <summary>
        /// 计提比例
        /// </summary>		
        public decimal ProvisionRatio { get; set; }
        /// <summary>
        /// 类型（0：应收，1：其他应收）
        /// </summary>
        public int BusType { get; set; }

        /// <summary>
        /// Remarks 
        /// </summary>		
        public string Remarks { get; set; }

        public DateTime ModifiedDate { get; set; }


    }
}
