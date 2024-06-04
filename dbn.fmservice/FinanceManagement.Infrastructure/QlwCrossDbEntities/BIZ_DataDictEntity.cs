using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class BIZ_DataDictODataEntity
    {
        [Key]
        /// <summary>
        /// DataDictID
        /// </summary>		
        public string DataDictID { get; set; }
        /// <summary>
        /// DataDictName
        /// </summary>
        public string DataDictName { get; set; }
        /// <summary>
        /// DataDictType
        /// </summary>
        public string DataDictType { get; set; }
        /// <summary>
        /// PID
        /// </summary>
        public string PID { get; set; }
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

        public string cPrivCode { get; set; }
        /// <summary>
        /// 接口专用
        /// https://apiqlw.t.nxin.com/q/reportbreed/PoultryCostProject
        /// </summary>
        [NotMapped]
        public string DataDictField { get; set; }
    }
    public class TreeModelODataEntity
    {
        [Key]
        public string  Id { get; set; }
        public string cName { get; set; }
        public string Pid { get; set; }
        [NotMapped]
        public int Rank { get; set; }
        [NotMapped]
        public string ExtendId { get; set; }
    }
    public class TreeModelExtityODataEntity
    {
        [Key]
        public string Id { get; set; }
        public string cName { get; set; }
        public string Pid { get; set; }
        public int Rank { get; set; }
        [NotMapped]
        public string ExtendId { get; set; }
    }
    public class DropODataEntity
    {
        [Key]
        public string id { get; set; }
        public string name { get; set; }
        public string curtype { get; set; }
    }
    /// <summary>
    /// 资产负债表填报 查询是否审核，一键结账专用 -- 姚
    /// </summary>
    public class BalanceSheet
    {
        [Key]
        public string EnterpriseId { get; set; }
        /// <summary>
        /// 级次
        /// 1 == 审核
        /// </summary>
        public int Level { get; set; }
    }
}
