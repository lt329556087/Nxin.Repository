using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_BadDebtOccurODataEntity : OneWithManyQueryEntity<FD_BadDebtOccurDetailODataEntity>
    {
        public FD_BadDebtOccurODataEntity()
        {
            Lines1 = new List<FD_BadDebtOccurDetailODataEntity>();
            Lines2 = new List<FD_BadDebtOccurDetailODataEntity>();
            BusinessType = "201611160104402101";
        }

        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        public string DataDate { get; set; }
        public string TicketedPointID { get; set; }
        public string EnterpriseID { get; set; }
        public string CustomerID { get; set; }
        public string PersonID { get; set; }
        public string ModifiedDate { get; set; }

        //贷方科目
        public string CAccoSubjectID { get; set; }


        public string CAccoSubjectName { get; set; }


        /// <summary>
        /// 坏账科目1
        /// </summary>
        public string AccoSubjectID1 { get; set; }

        public string AccoSubjectName1 { get; set; }

        /// <summary>
        /// 坏账科目2
        /// </summary>
        public string AccoSubjectID2 { get; set; }

        public string AccoSubjectName2 { get; set; }

        public string NumericalOrderSetting { get; set; }

        public string BusinessType { get; set; }


        [NotMapped]
        /// <summary>
        /// 坏账科目1列表
        /// </summary>
        public List<FD_BadDebtOccurDetailODataEntity> Lines1 { get; set; }

        [NotMapped]
        /// <summary>
        /// 坏账科目2列表
        /// </summary>
        public List<FD_BadDebtOccurDetailODataEntity> Lines2 { get; set; }
    }


    public class FD_BadDebtOccurListOnly
    {
        [Key]
        public string NumericalOrder { get; set; }
        public string Number { get; set; }
        public string CreateDate { get; set; }

        public string CustomerID { get; set; }

        public string CustomerName { get; set; }

        public decimal CurrentOccurAmount { get; set; }

        public string OwnerName { get; set; }

        public string OwnerID { get; set; }

        public string ReviewName { get; set; }

        public string ReviewID { get; set; }
    }

    public class FD_BadDebtOccurDetailODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID { get; set; }
        public string MarketID { get; set; }

        public string MarketName { get; set; }

        public string PersonID { get; set; }

        public string PersonName { get; set; }

        /// <summary>
        /// 本次坏账发生金额
        /// </summary>
        public decimal CurrentOccurAmount { get; set; }

        /// <summary>
        /// 期末余额
        /// </summary>
        public decimal Amount { get; set; }

    }
}
