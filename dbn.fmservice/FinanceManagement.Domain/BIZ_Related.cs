using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceManagement.Domain
{
    public partial class BIZ_Related : EntityBase
    {

        //表头修改入参
        public void Update(string parentValue, string childValue)
        {
            ParentValue = parentValue;
            ChildValue = childValue;
        }
        [Key]
        public long RelatedID { get; set; }
        public string RelatedType { get; set; }
        public string ParentType { get; set; }
        public string ChildType { get; set; }
        public string ParentValue { get; set; }
        public string ChildValue { get; set; }
        public string ParentValueDetail { get; set; }
        public string ChildValueDetail { get; set; }
        public string Remarks { get; set; }
    }
    public class BIZ_Related_FM : EntityBase
    {

        //表头修改入参
        public void Update(string parentValue, string childValue)
        {
            ParentValue = parentValue;
            ChildValue = childValue;
        }
        [Key]
        public long RelatedID { get; set; }
        public string RelatedType { get; set; }
        public string ParentType { get; set; }
        public string ChildType { get; set; }
        public string ParentValue { get; set; }
        public string ChildValue { get; set; }
        public string ParentValueDetail { get; set; }
        public string ChildValueDetail { get; set; }
        public string Remarks { get; set; }
    }
}
