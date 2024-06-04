using Architecture.Common.Application.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Infrastructure.QlwCrossDbEntities
{
    public class BIZ_RelatedODataEntity
    {        
        [Key]
        public int RelatedID { get; set; }
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
