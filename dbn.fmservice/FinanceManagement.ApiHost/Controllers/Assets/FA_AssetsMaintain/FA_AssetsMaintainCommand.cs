using Architecture.Common.Application.Commands;
using Architecture.Seedwork.Core;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.ApiHost.Controllers.FA_AssetsMaintain
{

    public class FA_AssetsMaintainAddCommand : FA_AssetsMaintainCommand, IRequest<Result>
    {

    }

    public class FA_AssetsMaintainDeleteCommand : FA_AssetsMaintainCommand, IRequest<Result>
    {

    }

    public class FA_AssetsMaintainModifyCommand : FA_AssetsMaintainCommand, IRequest<Result>
    {
    }

    public class FA_AssetsMaintainCommand : MutilLineCommand<FA_AssetsMaintainDetailCommand>
    {
        public FA_AssetsMaintainCommand()
        {
            this.Lines = new List<FA_AssetsMaintainDetailCommand>();
        }
        /// <summary>
        /// 流水号
        /// </summary>		
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public DateTime DataDate { get; set; }
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
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }

    }

    public class FA_AssetsMaintainDetailCommand : CommonOperate
    {
        public FA_AssetsMaintainDetailCommand()
        {
            this.FileModels = new List<FileModel>();
        }
        /// <summary>
        /// NumericalOrder
        /// </summary>		
        public string NumericalOrder { get; set; }
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
        public DateTime MaintainDate { get; set; }
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
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// on update CURRENT_TIMESTAMP
        /// </summary>		
        public DateTime ModifiedDate { get; set; }
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
    public class MutilLineCommand<TLineCommand1> : CommonOperate where TLineCommand1 : CommonOperate
    {
        public List<TLineCommand1> Lines { get; set; }
    }

}
