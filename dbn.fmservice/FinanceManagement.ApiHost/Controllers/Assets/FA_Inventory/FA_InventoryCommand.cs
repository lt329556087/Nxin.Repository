using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_Inventory
{
    public class FA_InventoryDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FA_InventoryAddCommand : FA_InventoryCommand, IRequest<Result>
    {
    }

    public class FA_InventoryModifyCommand : FA_InventoryCommand, IRequest<Result>
    {
    }
    /// <summary>
    /// 表头表体关联
    /// </summary>
    /// <typeparam name="TLineCommand"></typeparam>
    public class MutilLineCommand<TLineCommand> : CommonOperate where TLineCommand : CommonOperate
    {
        public List<TLineCommand> Lines { get; set; }
    }

    public class FA_InventoryCommand : MutilLineCommand<FA_InventoryDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>		
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 日期
        /// </summary>

        public DateTime DataDate { get; set; }
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
        /// 创建时间
        /// </summary>		
        public string CreatedDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string ModifiedDate { get; set; }
        ///// <summary>
        ///// 附件
        ///// </summary>
        //public string UploadInfo { get; set; }
    }

    public class FA_InventoryDetailCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
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
        /// <summary>
        /// 资产名称 
        /// </summary>		
        public string AssetsName { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public List<FMFileModel> FileModels { get; set; }

        //public string RowStatus { get; set; }
    }
    
}
