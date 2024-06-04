using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_AccoCheckODataEntity : OneWithManyQueryEntity<FM_AccoCheckDetailODataEntity>
    {
        public FM_AccoCheckODataEntity()
        {
            Lines = new List<FM_AccoCheckDetailODataEntity>();
        }
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 结账标识
        /// </summary>
        public bool CheckMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        /// <summary>
        /// 所属单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
    }
}
