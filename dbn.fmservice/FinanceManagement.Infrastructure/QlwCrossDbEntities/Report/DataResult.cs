using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class RptDataResult : IComparable<RptDataResult>
    {
        #region 小计专用字段

        /// <summary>
        /// 汇总方式名称
        /// </summary>
        [NotMapped]
        public List<string> SummaryTypeNameList { get; set; } = new List<string>();
        /// <summary>
        /// 汇总方式字段
        /// </summary>
        [NotMapped]
        public List<string> SummaryTypeFieldNameList { get; set; } = new List<string>();
        /// <summary>
        /// 汇总方式
        /// </summary>
        [NotMapped]
        public List<string> SummaryTypeList { get; set; } = new List<string>();
        [NotMapped]
        public double iOrder { get; set; }
        [NotMapped]
        public bool IsSubTotal { get; set; }
        #endregion

        /// <summary>
        /// 汇总方式
        /// </summary>
        public string SummaryType { get; set; }
        /// <summary>
        /// 汇总方式名称
        /// </summary>
        public string SummaryTypeName { get; set; }
        /// <summary>
        /// 汇总方式字段
        /// </summary>
        public string SummaryTypeFieldName { get; set; }


        public int CompareTo(RptDataResult other)
        {
            if (this.iOrder > other.iOrder)
                return 1;
            else if (this.iOrder == other.iOrder)
                return 0;
            return -1;
        }
    }
}
