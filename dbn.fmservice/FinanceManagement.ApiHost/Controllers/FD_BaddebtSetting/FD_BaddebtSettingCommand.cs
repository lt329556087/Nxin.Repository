using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BaddebtSetting
{
    public class FD_BaddebtSettingDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FD_BaddebtSettingAddCommand : FD_BaddebtSettingCommand, IRequest<Result>
    {
    }

    public class FD_BaddebtSettingModifyCommand : FD_BaddebtSettingCommand, IRequest<Result>
    {
    }

    public class FD_BaddebtSettingCommand// : MutilLineCommand<FD_BaddebtSettingDetailCommand>
    {

        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }

        /// <summary>
        /// 计提方法
        /// </summary>		
        public string ProvisionMethod { get; set; }

        /// <summary>
        /// 坏账准备科目-应收账款
        /// </summary>		
        public string BadAccoSubjectOne { get; set; }

        /// <summary>
        /// 坏账准备科目-其他应收账款
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

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }

        /// <summary>
        /// 计提方法
        /// </summary>		
        public string ProvisionMethodName { get; set; }

        /// <summary>
        /// 坏账科目一
        /// </summary>		
        public string BadAccoSubjectOneName { get; set; }

        /// <summary>
        /// 坏账科目二
        /// </summary>	

        public string BadAccoSubjectTwoName { get; set; }

        /// <summary>
        /// 对方科目一
        /// </summary>		
        public string OtherAccoSubjectOneName { get; set; }
        /// <summary>
        /// 对方科目二
        /// </summary>		
        public string OtherAccoSubjectTwoName { get; set; }
        /// <summary>
        /// 应收科目一
        /// </summary>		
        public string DebtReceAccoSubjectOneName { get; set; }
        /// <summary>
        /// 应收科目二
        /// </summary>		
        public string DebtReceAccoSubjectTwoName { get; set; }

        /// <summary>
        /// 计提坏账摘要
        /// </summary>		
        public string ProvisionReceiptAbstractName { get; set; }
        /// <summary>
        /// 坏账发生摘要
        /// </summary>		
        public string OccurReceiptAbstractName { get; set; }
        /// <summary>
        /// 坏账收回摘要
        /// </summary>		
        public string RecoverReceiptAbstractName { get; set; }

        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }
    }
   
    public class FD_BaddebtSettingDetailCommand : CommonOperate
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
        /// 账龄区间设置RecordID
        /// </summary>		
        public int AgingIntervalID { get; set; }


        /// <summary>
        /// 计提比例
        /// </summary>		
        public decimal ProvisionRatio { get; set; }

        /// <summary>
        /// Remarks 事由
        /// </summary>		
        public string Remarks { get; set; }
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 类型（0：应收，1：其他应收）
        /// </summary>
        public int BusType { get; set; }
        #region 账龄区间设置
        public string IntervalType { get; set; }
        public string IntervalTypeName { get; set; }
        public string Name { get; set; }
        public int DayNum { get; set; }
        public int Serial { get; set; }
        #endregion
        public string RowStatus { get; set; }
    }



}
