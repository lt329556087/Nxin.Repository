using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FD_SpecificIdentification
{
    public class FD_SpecificIdentificationDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FD_SpecificIdentificationAddCommand : FD_SpecificIdentificationCommand, IRequest<Result>
    {
    }

    public class FD_SpecificIdentificationModifyCommand : FD_SpecificIdentificationCommand, IRequest<Result>
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

    public class FD_SpecificIdentificationCommand : MutilLineCommand<FD_SpecificIdentificationDetailCommand>
    {
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        [Key]
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
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }
        public string OwnerName { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public string CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public string ModifiedDate { get; set; }

        public string EnterpriseName { get; set; }

        public string AccoSubjectID1 { get; set; }

        public string AccoSubjectName1 { get; set; }


        public string AccoSubjectID2 { get; set; }

        public string AccoSubjectName2 { get; set; }
        public string BusinessType { get; set; }
        public string NumericalOrderSetting { get; set; }

        [NotMapped]
        public List<FD_SpecificIdentificationDetailCommand> Lines1 { get; set; }

        [NotMapped]
        public List<FD_SpecificIdentificationDetailCommand> Lines2 { get; set; }

    }

    public class FD_SpecificIdentificationDetailCommand : CommonOperate
    {
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
        /// Amount
        /// </summary>		
        public decimal Amount { get; set; }
        public decimal AccoAmount { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>		
        public string Remarks { get; set; }
        public string ModifiedDate { get; set; }

        [NotMapped]
        public List<FD_SpecificIdentificationExtCommand> AgingList { get; set; }
    }


    public class FD_SpecificIdentificationExtCommand : CommonOperate
    {
        public int RecordID { get; set; }
        public string NumericalOrderDetail { get; set; }

        public decimal Amount { get; set; }
        public string Name { get; set; }
    }
}
