using Architecture.Common.Application.Query;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FA_PurchaseSettingsODataEntity : OneWithManyQueryEntity<FA_PurchaseSettingsDetailODataEntity>
    {
        public FA_PurchaseSettingsODataEntity()
        {
            Lines = new List<FA_PurchaseSettingsDetailODataEntity>();
        }
        
        [Key]
        public string NumericalOrder { get; set; }
        public string ModifyFieldID { get; set; }
        [NotMapped]
        public List<BIZ_DataDictODataEntity> ModifyFieldList { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
       
    }

    public class FA_PurchaseSettingsDetailODataEntity
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 资产类别
        /// </summary>
        public string AssetsTypeID { get; set; }
        [NotMapped]
        public List<AssetsTypeModel> AssetsTypeList { get; set; }
        /// <summary>
        /// 开始适用范围
        /// </summary>
        public decimal BeginRange { get; set; }
        /// <summary>
        /// 结束适用范围
        /// </summary>
        public decimal EndRange { get; set; }
        /// <summary>
        /// 浮动方向
        /// </summary>
        public string FloatingDirectionID { get; set; }
        public string FloatingDirectionName { get; set; }
        /// <summary>
        /// 浮动类型
        /// </summary>
        public string FloatingTypeID { get; set; }
        public string FloatingTypeName { get; set; }
        /// <summary>
        /// 最大浮动值
        /// </summary>
        public decimal MaxFloatingValue { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
    }

    public class FA_AssetsClassificationODataEntity
    {
        [Key]
        public string ClassificationID { get; set; }
        public string ClassificationName { get; set; }
        public string ClassificationCode { get; set; }
        public string PID { get; set; }
        public int Rank { get; set; }
        public decimal FixedYears { get; set; }
        public decimal ResetFixedYears { get; set; }
        public decimal ResidualValue { get; set; }
        public string DepreciationMethodID { get; set; }
        public string CodeRule { get; set; }
        public string AccruedRule { get; set; }
        public string AssetsAccoSubjectID { get; set; }
        public string DepreciationAccoSubjectID { get; set; }
        public string Remarks { get; set; }
        public string EnterpriseID { get; set; }
    }

}
