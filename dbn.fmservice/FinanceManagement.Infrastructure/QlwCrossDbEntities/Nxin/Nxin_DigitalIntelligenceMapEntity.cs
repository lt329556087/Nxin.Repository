using Architecture.Common.Application.Query;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class Nxin_DigitalIntelligenceMapODataEntity 
    {

        /// <summary>
        /// 流水号
        /// </summary>	
        [Key]
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        /// <summary>
        /// 地图类型
        /// </summary>
        public string MapType { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string BackgroundValue { get; set; }
        /// <summary>
        /// 区块配置信息
        /// </summary>
        public string BlockList { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }

    }
}
