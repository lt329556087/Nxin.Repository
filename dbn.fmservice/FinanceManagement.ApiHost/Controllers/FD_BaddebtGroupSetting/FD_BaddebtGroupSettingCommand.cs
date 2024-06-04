using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_BaddebtGroupSetting
{
    public class FD_BaddebtGroupSettingDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FD_BaddebtGroupSettingAddCommand : FD_BaddebtGroupSettingCommand, IRequest<Result>
    {
    }

    public class FD_BaddebtGroupSettingModifyCommand : FD_BaddebtGroupSettingCommand, IRequest<Result>
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

    public class FD_BaddebtGroupSettingCommand : MutilLineCommand<FD_BaddebtGroupSettingDetailCommand>
    {

        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        ///// <summary>
        ///// 单据号
        ///// </summary>		
        //public string Number { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>	

        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>		
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string Remarks { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }

        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }
        public List<FD_IdentificationTypeAndSubjectCommand> TypeAndSubjects { get; set; }
        public List<FD_BaddebtGroupSettingExtendCommand> Extends { get; set; }
    }

    public class FD_BaddebtGroupSettingDetailCommand : CommonOperate
    {
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 类型（0：应收，1：其他应收）
        /// </summary>
        public int BusType { get; set; }
        public string IntervalType { get; set; }
        public string IntervalTypeName { get; set; }
        public string Name { get; set; }
        public int DayNum { get; set; }
        public int Serial { get; set; }
        /// <summary>
        /// 计提比例
        /// </summary>		
        public decimal ProvisionRatio { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
      
        public string RowStatus { get; set; }
    }

    public class FD_BaddebtGroupSettingExtendCommand : CommonOperate
    {
        /// <summary>
        /// auto_increment
        /// </summary>		
        public int RecordID { get; set; }

        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }

        public string EnterpriseID { get; set; }
        /// <summary>
        /// 前端显示用
        /// </summary>
        public string ShowID { get; set; }
        public string ModifiedDate { get; set; }
        public string RowStatus { get; set; }
    }

    public class FD_IdentificationTypeAndSubjectCommand : CommonOperate
    {
        [Key]
        public int RecordID { get; set; }
        public string TypeID { get; set; }
        public string OldTypeID { get; set; }
        public string TypeName { get; set; }
        public string NumericalOrder { get; set; }
        public int BusiType { get; set; }
        public int AccrualType { get; set; }
        public string AccoSubjectID { get; set; }
        //public string AccoSubjectCode { get; set; }
        public string AccoSubjectFullName { get; set; }
        public int? DataSourceType { get; set; }
        public string DataSourceTypeName { get; set; }
        public string RowStatus { get; set; }
    }
}
