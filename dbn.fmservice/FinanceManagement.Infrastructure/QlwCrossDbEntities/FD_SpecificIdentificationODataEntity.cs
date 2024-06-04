using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FD_SpecificIdentificationODataEntity : OneWithManyQueryEntity<FD_SpecificIdentificationDetailODataEntity>
    {

        [Key]
        public string Guid { get; set; }
        public string NumericalOrder { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>		
        public string DataDate { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        public string OwnerID { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }
        public string CheckedByID { get; set; }
        public string CheckedByName { get; set; }
        public string ProvisionTypeName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public bool IsProvision { get; set; }
        public string ProvisionType { get; set; }
        public string CustomerID { get; set; }

        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID1 { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectName1 { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectID2 { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public string AccoSubjectName2 { get; set; }

        public string BusinessType { get; set; }
        public string AccoSubjectCode { get; set; }

        public string AccoSubjectName { get; set; }

        [NotMapped]
        public List<FD_SpecificIdentificationDetailODataEntity> Lines1 { get; set; }

        [NotMapped]
        public List<FD_SpecificIdentificationDetailODataEntity> Lines2 { get; set; }
    }
    public class FD_SpecificIdentificationDetailODataEntity
    {
        [Key]
        /// <summary>
        /// RecordID
        /// </summary>		
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 计提类型
        /// </summary>		
        public string ProvisionType { get; set; }
        public string ProvisionTypeName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户
        /// </summary>		
        public string CustomerID { get; set; }

        public string AccoSubjectID { get; set; }

        /// <summary>
        /// 待计提金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal AccoAmount { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }
        public string ModifiedDate { get; set; }
        public bool IsProvision { get; set; }

        public List<FD_SpecificIdentificationExtODataEntity> AgingList { get; set; }
    }

    public class FD_SpecificIdentificationRangeODataEntity
    {
        [Key]
        /// <summary>
        /// 计提表体ID
        /// </summary>
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>		
        public string DataDate { get; set; }

        /// <summary>
        /// Number
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 待计提金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 计提类型
        /// </summary>		
        public string ProvisionType { get; set; }

        public string ProvisionTypeName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户
        /// </summary>		
        public string CustomerID { get; set; }

        public string AccoSubjectID { get; set; }

        public string BusinessType { get; set; }

    }


    /// <summary>
    /// 扩展表
    /// </summary>
    public class FD_SpecificIdentificationExtODataEntity
    {
        [Key]
        public int RecordID { get; set; }
        /// <summary>
        /// 计提表体ID
        /// </summary>
        public string NumericalOrderDetail { get; set; }

        /// <summary>
        /// 账龄名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class Biz_CustomerODataEntity
    {
        [Key]
        public string CustomerID { get; set; }

        public string CustomerName { get; set; }
        public bool IsUse { get; set; }
    }

    public enum ProvisionTypeEnum : long
    {
        FullNOProvision = 2110251443470000109,
        /** 全额计提 */
        FullProvision = 2110251444240000109,
        /** 全额部分计提 */
        PartProvision = 2110251444360000109,
        /*按账龄计提*/
        AgingProvision = 2110251444380000109
    }
}
