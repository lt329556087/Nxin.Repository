using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class AssistRecordResultEntity 
    {
        //注：以下列顺序不可变 用于导出，如要加字段请加在这些字段后面。账务单位 日期  凭证类别 凭证字号    记账号 摘要  内容 会计科目    客商 部门  员工 项目  归属单位 借方金额    贷方金额 备注
        /// <summary>
        /// 单位
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string DataDate { get; set; }
        /// <summary>
        /// 凭证类别
        /// </summary>
        public string SettleReceipTypeName { get; set; }      
        /// <summary>
        /// 单据字
        /// </summary>
        public string TicketedPointName { get; set; }
        /// <summary>
        /// 凭证号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 凭证字号
        /// </summary>
        //[NotMapped]
        //public string ANumber { get; set; }
        /// <summary>
        /// 记账号
        /// </summary>
        public string AccountNo { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string SettleSummaryName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 会计科目
        /// </summary>
        public string AccoSubjectFullName { get; set; }
        //客商 部门  员工 项目  归属单位 借方金额    贷方金额 备注
        /// <summary>
        /// 客户/供应商
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string MarketFullName { get; set; }
        /// <summary>
        /// 员工
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 项目
        /// </summary>

        public string ProjectName { get; set; }       
        /// <summary>
        /// 所属单位
        /// </summary>
        public string BelongEnterpriseName { get; set; }
        /// <summary>
        /// 业务单元
        /// </summary>
        public string OrgMarketName { get; set; }
        /// <summary>
        /// 借方金额
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// 贷方金额
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string AccoSubjectCode { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// key 无意义 
        /// </summary>
        [Key]
        public string UID { get; set; }

        #region 自定义辅助项
        public string Auxiliary1 { get; set; }
        public string Auxiliary2 { get; set; }
        public string Auxiliary3 { get; set; }
        public string Auxiliary4 { get; set; }
        public string Auxiliary5 { get; set; }
        public string Auxiliary6 { get; set; }
        public string Auxiliary7 { get; set; }
        public string Auxiliary8 { get; set; }
        public string Auxiliary9 { get; set; }
        public string Auxiliary10 { get; set; }
        public string AuxiliaryName1 { get; set; }
        public string AuxiliaryName2 { get; set; }
        public string AuxiliaryName3 { get; set; }
        public string AuxiliaryName4 { get; set; }
        public string AuxiliaryName5 { get; set; }
        public string AuxiliaryName6 { get; set; }
        public string AuxiliaryName7 { get; set; }
        public string AuxiliaryName8 { get; set; }
        public string AuxiliaryName9 { get; set; }
        public string AuxiliaryName10 { get; set; }

        #endregion
        //public string EnterpriseID { get; set; }
        //public string TicketedPointID { get; set; }
        //public string MarketID { get; set; }
        //public string CustomerID { get; set; }
        //public string PersonID { get; set; }
        //public string ProjectID { get; set; }
        //public string AccoSubjectID { get; set; }
    }

    public class AssistRecordSummaryResultEntity: RptDataResult
    {
        //Debit/Credit 字段顺序不可修改
        /// <summary>
        /// key 无意义 
        /// </summary>
        [Key]
        public string UID { get; set; }
        /// <summary>
        /// 借方金额
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// 贷方金额
        /// </summary>
        public decimal Credit { get; set; }
    }
}
