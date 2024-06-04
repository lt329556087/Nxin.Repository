using Architecture.Seedwork.Domain;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    /// <summary>
    /// 审核表。需要手动设置审核操作
    /// </summary>
    public partial class Biz_Review : EntityBase
    {
        /// <summary>
        /// 审核表。需要手动设置审核操作。Level默认0，CheckMark默认-1
        /// </summary>
        /// <param name="numericalOrder"></param>
        /// <param name="reviweType"></param>
        /// <param name="checkedByID"></param>
        /// <param name="appID"></param>
        public Biz_Review(string numericalOrder, string reviweType, string checkedByID)
        {
            NumericalOrder = numericalOrder ?? throw new ArgumentNullException(nameof(numericalOrder));
            Guid = Guid.NewGuid();
            ReviweType = reviweType ?? throw new ArgumentNullException(nameof(reviweType));
            Level = 0;
            CheckMark = -1;
            CheckedByID = checkedByID ?? throw new ArgumentNullException(nameof(checkedByID));
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }
        public Biz_Review SetLevel(int level)
        {
            this.Level = level;
            return this;
        }
        public Biz_Review SetCheckMark(ReviewCode checkMark)
        {
            this.CheckMark = checkMark.GetIntValue();
            return this;
        }
        public Biz_Review SetMaking(int level = 1)
        {
            this.Level = level;
            this.CheckMark = ReviewCode.制单.GetIntValue();
            return this;
        }
        public Biz_Review SetAudit(int level = 1)
        {
            this.Level = level;
            this.CheckMark = ReviewCode.审核.GetIntValue();
            return this;
        }
        public Biz_Review SetFinanceAudit(int level = 1)
        {
            this.Level = level;
            this.CheckMark = ReviewCode.财务审核.GetIntValue();
            return this;
        }
        public Biz_Review SetWarehouseAudit(int level = 1)
        {
            this.Level = level;
            this.CheckMark = ReviewCode.仓库审核.GetIntValue();
            return this;
        }
        public bool IsSetCheckMark => CheckMark == -1;
        public bool IsSetLevel => Level == -1;
        public bool IsDefaultValue => IsSetCheckMark && IsSetLevel;


        [Key]
        /// <summary>
        /// 主键ID
        /// </summary>
        public int RecordID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string NumericalOrder { get; set; }
        /// <summary>
        /// 全球唯一关键字
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// 类型（菜单ID)
        /// </summary>
        public string ReviweType { get; set; }
        /// <summary>
        /// 级别/级次
        /// </summary>
        public long Level { get; set; }
        /// <summary>
        /// 审核标识值
        /// </summary>
        public int CheckMark { get; set; }
        /// <summary>
        /// 审核人ID
        /// </summary>
        public string CheckedByID { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public enum ReviewCode
    {
        财务审核 = 2048,
        仓库审核 = 4096,
        审核 = 16,
        制单 = 65536,
    }



}
