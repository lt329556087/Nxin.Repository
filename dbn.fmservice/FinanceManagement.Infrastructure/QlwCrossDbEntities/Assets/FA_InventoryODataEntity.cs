using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    /*资产盘点单*/
    public class FA_InventoryODataEntity : OneWithManyQueryEntity<FA_InventoryDetailODataEntity>
    {
        [Key]
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 单位
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 日期
        /// </summary>	
        private DateTime _DateDate;

        public string DataDate
        {
            get { return _DateDate.ToString("yyyy-MM-dd"); }
            set { _DateDate = Convert.ToDateTime(value); }
        }
        /// <summary>
        /// 单据号
        /// </summary>		
        public string Number { get; set; }

        /// <summary>
        /// 存放地点
        /// </summary>		
        public string FAPlaceID { get; set; }


        /// <summary>
        /// 使用状态
        /// </summary>		
        public string UseStateID { get; set; }

       
        /// <summary>
        /// 备注
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>		
        public string OwnerID { get; set; }

       

        /// <summary>
        /// CreatedDate
        /// </summary>		
        private string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }

        public string OwnerName { get; set; }
        /// <summary>
        /// 存放地点
        /// </summary>		
        public string FAPlaceName { get; set; }


        /// <summary>
        /// 使用状态
        /// </summary>		
        public string UseStateName { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>		
        public string AuditID { get; set; }
        /// <summary>
        /// 审核人名称
        /// </summary>		
        public string AuditName { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 贮存数量
        /// </summary>		
        public decimal Quantity { get; set; }

        /// <summary>
        /// 盘点数量
        /// </summary>		
        public decimal InventoryQuantity { get; set; }

        /// <summary>
        /// 差异
        /// </summary>		
        //[NotMapped]
        public decimal QuantityDiff { get; set; }

    }

    public class FA_InventoryDetailODataEntity
    {
        [Key]
        /// <summary>
        /// auto_increment
        /// </summary>		
        public string RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 卡片ID
        /// </summary>		
        public string CardID { get; set; }

        /// <summary>
        /// 贮存数量
        /// </summary>		
        public decimal Quantity { get; set; }

        /// <summary>
        /// 盘点数量
        /// </summary>		
        public decimal InventoryQuantity { get; set; }
        
        /// <summary>
        /// Remarks 
        /// </summary>		
        public string Remarks { get; set; }
        ///// <summary>
        ///// 资产类别 
        ///// </summary>		
        //public string ClassificationID { get; set; }
        /// <summary>
        /// 资产类别 
        /// </summary>		
        public string ClassificationName { get; set; }
        /// <summary>
        /// 使用状态 
        /// </summary>		
        public string UseStateID { get; set; }
        /// <summary>
        /// 使用状态 
        /// </summary>		
        public string UseStateName { get; set; }
        /// <summary>
        /// 资产编码 
        /// </summary>		
        public string AssetsCode { get; set; }
        /// <summary>
        /// 卡片编码 
        /// </summary>		
        public string CardCode { get; set; }
        /// <summary>
        /// 资产名称 
        /// </summary>		
        public string AssetsName { get; set; }
        /// <summary>
        /// 规格型号 
        /// </summary>		
        public string Specification { get; set; }
        /// <summary>
        /// 存放地点 
        /// </summary>		
        public string FAPlaceID { get; set; }
        /// <summary>
        /// 存放地点 
        /// </summary>		
        public string FAPlaceName { get; set; }
        /// <summary>
        /// 记录单位 
        /// </summary>		
        public string MeasureUnitName { get; set; }
       
        /// <summary>
        /// 部门全称 
        /// </summary>		
        public string MarketFullName { get; set; }

        /// <summary>
        /// 差异
        /// </summary>		
        //[NotMapped]
        public decimal QuantityDiff { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string PathUrl { get; set; }
        [NotMapped]
        public List<FMFileModel> FileModels { get; set; }
    }

    public class AssetscardSearch
    {
        public string EnterpriseID { get; set; }

        /// <summary>
        /// DataDate
        /// </summary>	
        public string DateDate { get; set; }
        /// <summary>
        /// 存放地点
        /// </summary>
        public string FAPlaceID { get; set; }


        /// <summary>
        /// 使用状态
        /// </summary>		
        public string UseStateID { get; set; }
        public string NumericalOrder { get; set; }
    }
    public class FMFileModel
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string PathUrl { get; set; }
    }
}
