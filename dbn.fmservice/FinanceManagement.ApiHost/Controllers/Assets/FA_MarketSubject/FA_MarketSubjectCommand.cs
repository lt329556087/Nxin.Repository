using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_MarketSubject
{
    public class FA_MarketSubjectDeleteCommand : IRequest<Result>
    {

        [Key]
        public string NumericalOrder { get; set; }
    }

    public class FA_MarketSubjectAddCommand : FA_MarketSubjectCommand, IRequest<Result>
    {
    }

    public class FA_MarketSubjectModifyCommand : FA_MarketSubjectCommand, IRequest<Result>
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

    public class FA_MarketSubjectCommand : MutilLineCommand<FA_MarketSubjectDetailCommand>
    {

        public string NumericalOrder { get; set; }
        public DateTime DataDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string Remarks { get; set; }
        /// <summary>
        /// OwnerID
        /// </summary>		
        public string OwnerID { get; set; }

        /// <summary>
        /// EnterpriseID
        /// </summary>		
        public string EnterpriseID { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

        public string OwnerName { get; set; }
        public string EnterpriseName { get; set; }       
    }

    public class FA_MarketSubjectDetailCommand : CommonOperate
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
        /// 部门ID
        /// </summary>		
        public string MarketID { get; set; }


        /// <summary>
        /// 科目ID
        /// </summary>		
        public string AccoSubjectID { get; set; }
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>		
        public string MarketName { get; set; }


        /// <summary>
        /// 科目名称
        /// </summary>		
        public string AccoSubjectFullName { get; set; }
        public string RowStatus { get; set; }
    }



}
