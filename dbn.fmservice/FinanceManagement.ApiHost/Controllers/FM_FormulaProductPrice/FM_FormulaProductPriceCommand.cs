using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.MS_FormulaProductPrice
{
    public class MS_FormulaProductPriceDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class MS_FormulaProductPriceAddCommand : MS_FormulaProductPriceBaseCommand
    {
    }

    public class MS_FormulaProductPriceModifyCommand : MS_FormulaProductPriceBaseCommand
    {
    }
    public class MS_FormulaProductPriceListCommand : IRequest<Result>
    {
        public List<Domain.MS_FormulaProductPrice> List = new List<Domain.MS_FormulaProductPrice>();
    }
    public class MS_FormulaProductPriceBaseCommand : MS_FormulaProductPriceCommand, IRequest<Result>
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

    public class MS_FormulaProductPriceCommand : MutilLineCommand<MS_FormulaProductPriceDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 集团ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 最后修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// 账务单位
        /// </summary>
        public List<string> EnterpriseList { get; set; } = new List<string>();

    }

    public class MS_FormulaProductPriceDetailCommand : CommonOperate
    {
        public string RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 标包
        /// </summary>
        public decimal? StandardPack { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnit { get; set; }

        /// <summary>
        /// 市场单价
        /// </summary>

        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 最后修改日期
        /// </summary>

        public DateTime ModifiedDate { get; set; }
    }


    public class MS_FormulaProductPriceExtCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 单位ID
        /// </summary>
        public string EnterpriseID { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class MS_FormulaProductPriceExportRequest
    {
        /// <summary>
        /// 集团ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string BeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 审核状态 -1：全部 0：未审 1:已审
        /// </summary>
        public int CheckState { get; set; }
        public string NumericalOrder { get; set; }
    }

    public class MS_FormulaProductPriceExcelModel
    {
         public string Number { get; set; }
        public string EnterpriseName { get; set; }
        public string DataDate { get; set; }
        public string ProductName { get; set; }
        public decimal? MarketPrice { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 账务单位
        /// </summary>
        public List<string> EnterpriseList { get; set; } = new List<string>();
        public string ProductID { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 标包
        /// </summary>
        public decimal? StandardPack { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string MeasureUnit { get; set; }
        public DateTime Date { get; set; }
    }
}
