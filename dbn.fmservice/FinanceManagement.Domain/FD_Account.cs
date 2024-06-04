using System;
using System.Collections.Generic;
using System.Text;
using Architecture.Seedwork.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Domain
{
    public class FD_Account : EntityBase
    {
        public FD_Account()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            OwnerID = EnterpriseID = "0";
            AccountID=AccountType = BankID = BankAreaID = AccoSubjectID = ExpenseAccoSubjectID = AccountUseType = ResponsiblePerson= MarketID = "0";
        }

        public void Update(string AccountName, string AccountType, string AccountFullName, string BankID, string BankAreaID, string DepositBank, string Address, string AccoSubjectID, string ExpenseAccoSubjectID, string AccountUseType, string ResponsiblePerson, 
            bool IsUse, string MarketID, string BankNumber, string Remarks, bool OpenBankEnterConnect,bool TubeAccountNumber,string  AccountNumber, DateTime CreatedDate)
        {
            this.AccountName = AccountName;
            this.AccountNumber = AccountNumber;
            this.AccountType = AccountType;
            this.AccountFullName = AccountFullName;
            this.BankID = BankID;
            this.BankAreaID = BankAreaID;
            this.DepositBank = DepositBank;
            this.Address = Address;
            this.AccoSubjectID = AccoSubjectID;
            this.ExpenseAccoSubjectID = ExpenseAccoSubjectID;
            this.AccountUseType = AccountUseType;
            this.ResponsiblePerson = ResponsiblePerson;
            this.IsUse = IsUse;
            this.MarketID = MarketID;
            this.BankNumber = BankNumber;
            this.Remarks = Remarks;            
            this.OpenBankEnterConnect = OpenBankEnterConnect;
            this.TubeAccountNumber = TubeAccountNumber;            
            this.ModifiedDate = DateTime.Now;
            this.CreatedDate = CreatedDate;
        }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string AccountID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[NotMapped]
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
        public bool IsUse { get; set; }
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


        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
        //public int? Sort { get; set; }

    }

}
