using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FA_MarketSubjectODataEntity : OneWithManyQueryEntity<FA_MarketSubjectDetailODataEntity>
    {    
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        private DateTime _CreatedDate;
        public string CreatedDate
        {
            get { return _CreatedDate.ToString(); }
            set { _CreatedDate = Convert.ToDateTime(value); }
        }


        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }
        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }
    }

    public class FA_MarketSubjectDetailODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }


        /// <summary>
        /// 部门ID
        /// </summary>		
        public string MarketID { get; set; }


        /// <summary>
        /// 科目ID
        /// </summary>		
        public string AccoSubjectID { get; set; }

        private DateTime _ModifiedDate;
        public string ModifiedDate
        {
            get { return _ModifiedDate.ToString(); }
            set { _ModifiedDate = Convert.ToDateTime(value); }
        }
        /// <summary>
        /// 部门名称
        /// </summary>		
        public string MarketName { get; set; }


        /// <summary>
        /// 科目名称
        /// </summary>		
        public string AccoSubjectFullName { get; set; }
        /// <summary>
        /// 科目级次
        /// </summary>		
        public int Rank { get; set; }
    }

}
