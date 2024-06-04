using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_AccoCheckDetailODataEntity
    {
        public FM_AccoCheckDetailODataEntity()
        {
            Extends = new List<FM_AccoCheckExtendODataEntity>();
        }
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 结算摘要
        /// </summary>
        public string AccoCheckType { get; set; }
        
        public bool CheckMark { get; set; }
        public bool IsNew { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 扩展表
        /// </summary>
        [NotMapped]
        public List<FM_AccoCheckExtendODataEntity> Extends { get; set; }
    }
}
