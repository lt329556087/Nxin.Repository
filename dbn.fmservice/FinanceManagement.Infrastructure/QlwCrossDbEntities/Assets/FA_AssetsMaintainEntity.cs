using Architecture.Common.Application.Query;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FA_AssetsMaintainODataEntity : OneWithManyQueryEntity<FA_AssetsMaintainDetailODataEntity>
    {
        public FA_AssetsMaintainODataEntity()
        {
            Lines = new List<FA_AssetsMaintainDetailODataEntity>();
        }

        /// <summary>
        /// 流水号
        /// </summary>
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
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

    public class FA_AssetsMaintainDetailODataEntity
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        /// <summary>
        /// 资产ID
        /// </summary>
        public string CardID { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>
        public string AssetsCode { get; set; }
        /// <summary>
        /// 保养方式
        /// </summary>
        public string MaintainID { get; set; }
        public string MaintainName { get; set; }
        /// <summary>
        /// 保养时间
        /// </summary>
        public string MaintainDate { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 存放地点ID
        /// </summary>
        public string DepositID { get; set; }
        public string DepositName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        [NotMapped]
        public List<FileModel> FileModels { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string PersonID { get; set; }
        public string PersonName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
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
    public class FileModel
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
    }

    public class FA_AssetsMaintainListODataEntity 
    {
        /// <summary>
        /// 流水号
        /// </summary>	
        [Key]
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产编码
        /// </summary>
        public string AssetsCode { get; set; }
        /// <summary>
        /// 保养方式
        /// </summary>
        public string MaintainID { get; set; }
        public string MaintainName { get; set; }
        /// <summary>
        /// 保养时间
        /// </summary>
        public string MaintainDate { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 存放地点ID
        /// </summary>
        public string DepositID { get; set; }
        public string DepositName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string PersonID { get; set; }
        public string PersonName { get; set; }
    }
}
