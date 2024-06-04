using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Architecture.Seedwork.Domain;

namespace FinanceManagement.Domain.MarketingProductCostSettingManagement
{

    /// <summary>
    /// 营销商品成本设定
    /// </summary>
    public class FD_MarketingProductCostSetting: EntityBase
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        public DateTime DataDate { get; set; }

        ///// <summary>
        ///// 会计区间开始日期
        ///// </summary>
        //public DateTime PeriodBeginDate { get; set; }

        ///// <summary>
        ///// 会计区间结束日期
        ///// </summary>
        //public DateTime PeriodEndDate { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public long Number { get; set; }

        /// <summary>
        /// 账务单位
        /// </summary>		
        public string AccountingEnterpriseID { get; set; }

        
        /// <summary>
        /// 制单人
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
