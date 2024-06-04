using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public class Nxin_DigitalIntelligenceMap : EntityBase
    {
        public Nxin_DigitalIntelligenceMap()
        {
            ModifiedDate = CreatedDate = DateTime.Now;
            NumericalOrder = "0";
            OwnerID = GroupID = "0";
        }
        public void Update(string MapType, string BackgroundValue, string BlockList,string Remarks)
        {
            this.MapType = MapType;
            this.BackgroundValue = BackgroundValue;
            this.BlockList = BlockList;
            this.Remarks = Remarks;
        }
        [Key]
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string GroupID { get; set; }
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
        public string Remarks { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
    }

}
