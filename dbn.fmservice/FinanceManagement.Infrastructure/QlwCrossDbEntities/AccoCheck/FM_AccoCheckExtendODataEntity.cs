using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_AccoCheckExtendODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 模板表体流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 模板表体流水号
        /// </summary>
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string MenuID { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public bool CheckMark { get; set; }
        public string ModifiedDate { get; set; }

    }
}
