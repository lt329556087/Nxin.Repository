using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class FM_AccoCheckExtend : EntityBase
    {
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 扩展流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int MenuID { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public bool CheckMark { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
