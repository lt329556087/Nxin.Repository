using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_AuxiliaryODataEntity
    {        
        [Key]
        /// <summary>
        /// 资金账户ID
        /// </summary>		
        public string AccountID { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// 资金账户名称
        /// </summary>		
        public string AccountName { get; set; }
        /// <summary>
        /// 资金账号（银行账号）
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// 账户类型ID（字典表）
        /// </summary>		
        public string AccountType { get; set; }

        /// <summary>
        /// 账户全称
        /// </summary>	

        public string AccountFullName { get; set; }

        /// <summary>
        /// 开户银行ID（字典表）
        /// </summary>		
        public string BankID { get; set; }
        /// <summary>
        /// 银行地理区域ID
        /// </summary>		
        public string BankAreaID { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>		
        public string DepositBank { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>		
        public string Address { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>		
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 费用科目
        /// </summary>		
        public string ExpenseAccoSubjectID { get; set; }

        /// <summary>
        /// 账户功能ID（字典表）
        /// </summary>		
        public string AccountUseType { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>		
        public string ResponsiblePerson { get; set; }
        /// <summary>
        /// 状态（1：使用，0：停用）
        /// </summary>		
        public int IsUse { get; set; }
        /// <summary>
        /// 部门
        /// </summary>		
        public string MarketID { get; set; }
        /// <summary>
        /// 银行企业编号
        /// </summary>		
        public string BankNumber { get; set; }
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
        /// '开通银企连接',
        /// </summary>
        public bool OpenBankEnterConnect { get; set; }
        /// <summary>
        /// 现管账号
        /// </summary>
        public bool TubeAccountNumber { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }


        public string ModifiedDate{ get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>		
        public string AccountTypeName { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>		
        public string BankName { get; set; }

        /// <summary>
        /// 银行地理区域
        /// </summary>	

        public string BankAreaName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>		
        public string AccoSubjectName { get; set; }
        /// <summary>
        /// 费用科目
        /// </summary>		
        public string ExpenseAccoSubjectName { get; set; }
        /// <summary>
        /// 账户功能
        /// </summary>		
        public string AccountUseTypeName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>		
        public string ResponsiblePersonName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>		
        public string MarketName { get; set; }

        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }
        //public string DataStatus { get; set; }
        [NotMapped]
        public List<string> OptionsSettingValue { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
        [NotMapped]
        //账户用途
        public List<string> PurposeList { get; set; }
        public string AccoSubjectCode { get; set; }
        [NotMapped]
        public string BankType { get; set; }
    }
}
