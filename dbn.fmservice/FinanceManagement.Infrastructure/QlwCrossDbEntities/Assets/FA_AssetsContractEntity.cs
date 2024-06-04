using Architecture.Common.Application.Query;
using FinanceManagement.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class FA_AssetsContractODataEntity : OneWithManyQueryEntity<FA_AssetsContractDetailODataEntity>
    {
        public FA_AssetsContractODataEntity()
        {
            Lines = new List<FA_AssetsContractDetailODataEntity>();
        }

        /// <summary>
        /// 流水号
        /// </summary>	
        [Key]		
        public string NumericalOrder { get; set; }
        public string DataDate { get; set; }
        public string ContractName { get; set; }
        public string ContractNumber { get; set; }

        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        public string ContractTemplate { get; set; }
        public string ContractTemplateName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string Remarks { get; set; }
        public string UpDataInfo { get; set; }
        public string ContractClause { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }
        [NotMapped]
        public string ApplyForms { get; set; }
        public string ResultsName { get; set; }

    }

    public class FA_AssetsContractDetailODataEntity
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        [Key]
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrderInput { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产类别
        /// </summary>
        public string AssetsTypeID { get; set; }
        public string AssetsTypeName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnit { get; set; }
        public string MeasureUnitName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 含税单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }

    public class FA_AssetsContractListODataEntity 
    {
        /// <summary>
        /// 流水号
        /// </summary>	
        [Key]
        public string NumericalOrder { get; set; }
        public string DataDate { get; set; }
        public string ContractName { get; set; }
        public string ContractNumber { get; set; }

        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        public string ContractTemplate { get; set; }
        public string ContractTemplateName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string ResultsName { get; set; }
    }
    public class FA_AssetsContractMobileListODataEntity
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
        public string TicketedPointID { get; set; }
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        public string MeasureUnit { get; set; }
        public string MeasureUnitName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetsName { get; set; }
        /// <summary>
        /// 资产类别
        /// </summary>
        public string AssetsTypeID { get; set; }
        public string AssetsTypeName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        /// <summary>
        /// 已入合同数量和金额
        /// </summary>
        public decimal QuantityOut { get; set; }
        public decimal AmountOut { get; set; }
        /// <summary>
        /// 待入合同数量和金额
        /// </summary>
        public decimal QuantityWait { get; set; }
        public decimal AmountWait { get; set; }
    }


}
