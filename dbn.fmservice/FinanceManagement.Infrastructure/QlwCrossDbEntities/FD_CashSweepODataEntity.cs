using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_CashSweepODataEntity
    {

        [Key]
        public string NumericalOrder { get; set; }

        public string    EnterpriseId { get; set; }


        /// <summary>
        /// 单位
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 归集账号
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 归集状态
        /// </summary>
        public string ExecuteStatusResult { get; set; }

        /// <summary>
        /// 归集执行日期
        /// </summary>
        public string ExcuteDate { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal AccountBalance { get; set; }

        /// <summary>
        /// 其他账户余额
        /// </summary>
        public decimal OtherAccountBalance { get; set; }

        /// <summary>
        /// 归集总额
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// 归集总额
        /// </summary>
        public decimal SweepBalance { get; set; }

        /// <summary>
        /// 自动归集金额
        /// </summary>
        public decimal AutoSweepBalance { get; set; }

        /// <summary>
        /// 其他账户归集金额
        /// </summary>
        public decimal ManualSweepBalance { get; set; }
    }
}
