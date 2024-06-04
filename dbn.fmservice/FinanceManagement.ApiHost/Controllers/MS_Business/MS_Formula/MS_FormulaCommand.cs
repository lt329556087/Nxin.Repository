using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers
{

    public class MS_FormulaAddCommand : MS_FormulaCommand, IRequest<Result>
    {

    }

    public class MS_FormulaDeleteCommand : MS_FormulaCommand, IRequest<Result>
    {

    }

    public class MS_FormulaModifyCommand : MS_FormulaCommand, IRequest<Result>
    {
    }
    public class MS_FormulaListModifyCommand : MS_FormulaCommand, IRequest<Result>
    {
    }
    public class MS_FormulaCommand : MutilLineCommand<MS_FormulaDetailCommand, MS_FormulaExtendCommand>
    {
        public MS_FormulaCommand()
        {
            this.Lines = new List<MS_FormulaDetailCommand>();
            this.Extends = new List<MS_FormulaExtendCommand>();
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 字号（开票点）
        /// </summary>
        public string TicketedPointID { get; set; }
        public string Number { get; set; }
        public string FormulaName { get; set; }
        public bool IsUse { get; set; }
        public decimal BaseQuantity { get; set; }
        public string Remarks { get; set; }
        public string PackageRemarks { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseID { get; set; }
        public decimal EarlyWarning { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
        public string UseEnterprise { get; set; }
        public List<DictionaryData> UseEnterpriseList { get; set; }
        public string UseProduct { get; set; }
        public List<DictionaryData> UseProductList { get; set; }
        public int IsGroup { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? EffectiveBeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EffectiveEndDate { get; set; }
    }

    public class MS_FormulaDetailCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string StockType { get; set; }
        public string StockTypeName { get; set; }
        public string FormulaTypeID { get; set; }
        public string FormulaTypeName { get; set; }
        public decimal ProportionQuantity { get; set; }
        public decimal Quantity { get; set; }
        public int RowNum { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Cost { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class MS_FormulaExtendCommand : CommonOperate
    {
        /// <summary>
        /// RecordID
        /// </summary>		
        public int RecordID { get; set; }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        public Guid Guid { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string PackingID { get; set; }
        public string PackingName { get; set; }
        public decimal Quantity { get; set; }
        public bool IsUse { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int RowNum { get; set; }

    }

    public class MutilLineCommand<TLineCommand1, TLineCommand2> : CommonOperate where TLineCommand1 : CommonOperate
                                                                                where TLineCommand2 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
        public List<TLineCommand2> Extends { get; set; }
    }
    public class MS_FormulaImport
    {
        public string RowNum { get; set; }
        public string FormulaName { get; set; }
        public DateTime DataDate { get; set; }
        public string UseEnterprise { get; set; }
        public List<DictionaryData> UseEnterpriseList { get; set; }
        public string UseProduct { get; set; }
        public List<DictionaryData> UseProductList { get; set; }
        public decimal BaseQuantity { get; set; }
        public string EffectiveDate { get; set; }
        public DateTime? EffectiveBeginDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public decimal ProportionQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string ProductExtendName { get; set; }
        public string ProductExtendID { get; set; }
        public string PackingName { get; set; }
        public string PackingID { get; set; }
        public decimal Quantity { get; set; }
        public bool IsUse { get; set; }
    }

    public class MS_FormulaSearch
    {
        public string GroupID { get; set; }
        public string UseEnterpriseIds { get; set; }
        public string UseProductIds { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string Number { get; set; }
        public string OwnerID { get; set; }
        public string CheckType { get; set; }
        public string NumericalOrder { get; set; }

    }



}
