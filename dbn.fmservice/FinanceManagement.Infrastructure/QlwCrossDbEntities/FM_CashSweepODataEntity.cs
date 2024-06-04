using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_CashSweepODataEntity : OneWithManyQueryEntity<FM_CashSweepDetailODataEntity>
    {
        #region 业务说明
        #endregion

        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

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
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }
        

        /// <summary>
        /// 归集方向
        /// </summary>		
        public string SweepDirectionID { get; set; }

        /// <summary>
        /// 归集类型
        /// </summary>
        public string SweepType { get; set; }


        /// <summary>
        /// Remarks 备注
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

       

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
        public string ExcuteDate { get; set; }
        /// <summary>
        /// 归集人
        /// </summary>
        public string ExcuterID { get; set; }
        public string AccountName { get; set; }
      
        public string OwnerName { get; set; }       

         /// <summary>
         /// 归集方案
         /// </summary>
        public string CollectionScheme { get; set; }
        /// <summary>
        /// 自动归集时是否启用
        /// </summary>
        public bool? IsUse { get; set; }

        /// <summary>
        /// 自动归集时间
        /// </summary>
        public string AutoTime { get; set; }
        /// <summary>
        /// 归集类型 对应coll_type
        /// </summary>
        public string SchemeType { get; set; }
        /// <summary>
        /// 方案金额 对应amount
        /// </summary>
        public decimal? SchemeAmount { get; set; }
        /// <summary>
        /// 方案比例 对应rate
        /// </summary>
        public decimal? Rate { get; set; }
        /// <summary>
        /// 资金计划类型 对应plan_type
        /// </summary>
        public string PlanType { get; set; }
        /// <summary>
        /// 归集公式
        /// </summary>
        public string SchemeFormula { get; set; }
        /// <summary>
        /// 是否新菜单生成 2210111522080000109
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// 归集方案类型名称
        /// </summary>
        public string SchemeTypeName { get; set; }
        public string EnterpriseName { get; set; }

        public string SweepDirectionName { get; set; }
        public string SweepTypeName { get; set; }
        public string ExcuterName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string AuditResult { get; set; }
        /// <summary>
        /// 审批状态名称
        /// </summary>
        public string AuditResultName { get; set; }
        /// <summary>
        /// 执行情况
        /// </summary>
        public string TradeResult { get; set; }
        public string BankNumber { get; set; }//资金银行客户号
        public string AccountFullName { get; set; }
        public string DepositBank { get; set; }
        public string BankID { get; set; }
        [NotMapped]
        public string BankCode { get; set; }//银行编码
        //[NotMapped]
        //public string AccountNumberEncrypt { get; set; }//加密的账号
        public string DetailAccounts { get; set; }
        //附件
        public string UploadInfo { get; set; }
    }

    public class FM_CashSweepDetailODataEntity
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
        /// NumericalOrderDetail
        /// </summary>		
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>		
        public decimal AccountBalance { get; set; }
        public string AutoSweepBalance_Show { get; set; }
        /// <summary>
        /// 其他账户余额
        /// </summary>

        public decimal OtherAccountBalance { get; set; }
        /// <summary>
        /// 理论额度
        /// </summary>
        public decimal TheoryBalance { get; set; }
        /// <summary>
        /// 调整金额
        /// </summary>
        public decimal TransformBalance { get; set; }
        /// <summary>
        /// 自动归集金额
        /// </summary>		
        public decimal AutoSweepBalance { get; set; }
        /// <summary>
        /// 手动归集金额
        /// </summary>
        public decimal ManualSweepBalance { get; set; }
        
        /// <summary>
        /// Remarks 事由
        /// </summary>		
        public string Remark { get; set; }
        /// <summary>
        /// 0：失败;1：成功;2：处理中 空：未归集
        /// </summary>
        public int? Status { get; set; }

        public string ExcuteMsg { get; set; }

        private string ModifiedDate { get; set; }

        public string AccountName { get; set; }
        //public string DepositBank { get; set; }
        public string AccountNumber { get; set; }
        public string EnterpriseName { get; set; }
        public string BankNumber { get; set; }//银行客户号
        public string AccountFullName { get; set; }
        public string DepositBank { get; set; }
        public string BankID { get; set; }
        [NotMapped]
        public string BankCode { get; set; }//银行编码
        [NotMapped]
        public string OrderNo { get; set; }
        [NotMapped]
        public string AccountNumberEncrypt { get; set; }//加密的账号
    }

    public class RevolvingFundDetailODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string AccountID { get; set; }
        /// <summary>
        /// 最低额度
        /// </summary>
        public decimal nMinimum { get; set; }
    }
    public class RevolvingFundDetailRequest
    {
        public string EnterpriseID { get; set; }
        public string dDate { get; set; }
        public string AccountID { get; set; }
    }
    public class FM_CashSweepRequest
    {
        //权限单位
        public string PermissionEnterpriseIDs { get; set; }
        public string EnterpriseID { get; set; }
        public string GroupID { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string DateStr { get; set; }
    }
}
