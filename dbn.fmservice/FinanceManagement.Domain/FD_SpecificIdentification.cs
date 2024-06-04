using Architecture.Seedwork.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace FinanceManagement.Domain
{
    public class FD_SpecificIdentification : EntityBase
    {
        public FD_SpecificIdentification()
        {
            DataDate = DateTime.Now;
            Lines = new List<FD_SpecificIdentificationDetail>();
        }

        public void AddDetail(FD_SpecificIdentificationDetail detail)
        {
            Lines.Add(detail);
        }


        public void Update(DateTime DataDate,string Number)
        {
            this.DataDate = DataDate;
            this.Number = Number;
            this.ModifiedDate = DateTime.Now;
        }
        [Key]
        public string NumericalOrder { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime DataDate { get; set; }
        public string Number { get; set; }
        public string OwnerID { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string AccoSubjectID1 { get; set; }

        public string AccoSubjectID2 { get; set; }

        public string BusinessType { get; set; }
        public string NumericalOrderSetting { get; set; }

        [ForeignKey(nameof(NumericalOrder))]
        public List<FD_SpecificIdentificationDetail> Lines { get; set; }

        //public List<FD_SpecificIdentificationDetail> Details { get; set; }
    }

    public class FD_SpecificIdentificationDetail : EntityBase
    {
        public FD_SpecificIdentificationDetail()
        {
            AgingList = new List<FD_SpecificIdentificationExt>();
        }

        public void Update(string ProvisionType, string CustomerID, decimal Amount,decimal AccoAmount,  string Remarks)
        {
            this.ProvisionType = ProvisionType;
            this.CustomerID = CustomerID;
            this.Amount = Amount;
            this.Remarks = Remarks;
            this.ModifiedDate = DateTime.Now;
            this.AccoAmount = AccoAmount;
        }

        [Key]
        public string NumericalOrderDetail { get; set; }
        public string NumericalOrder { get; set; }
        public string ProvisionType { get; set; }
        public string CustomerID { get; set; }

        public string AccoSubjectID { get; set; }

        public decimal Amount { get; set; }

        public decimal AccoAmount { get; set; }

        public string Remarks { get; set; }
        public DateTime ModifiedDate { get; set; }        

        public FD_SpecificIdentification FD_SpecificIdentification { get; set; }

        [ForeignKey(nameof(NumericalOrderDetail))]

        public List<FD_SpecificIdentificationExt> AgingList { get; set; }
    }

    public class FD_SpecificIdentificationExt : EntityBase
    {

        [Key]
        public int RecordID { get; set; }
        public string NumericalOrderDetail { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
    }
}
