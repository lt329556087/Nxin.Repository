using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FM_AccoCheckRuleODataEntity 
    {
        public FM_AccoCheckRuleODataEntity()
        {
           
        }
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public long RecordID { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string AccoCheckType { get; set; }
        public string MasterDataSource { get; set; }
        public string MasterFormula { get; set; }
        public string MasterSecFormula { get; set; }
        public string FollowDataSource { get; set; }
        public string FollowFormula { get; set; }
        public string FollowSecFormula { get; set; }
        public string CheckValue { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public bool IsUse { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
        [NotMapped]

        public List<RuleEntity> MasterRules { get; set; } = new List<RuleEntity>();
        [NotMapped]

        public List<RuleEntity> FollowRules { get; set; } = new List<RuleEntity>();
    }
    public class RuleEntity
    {
        /// <summary>
        /// 公式值
        /// </summary>
        public string CellFormular { get; set; }
        public string CellFormularName { get; set; }
        public string AccoSubjectCode { get; set; }
        public string AccoSubjectType { get; set; }
        public string AccoSubjectID { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public string PropertyID { get; set; }
        public string PropertyValue { get; set; }
        public decimal OutValue { get; set; }

    }
}
