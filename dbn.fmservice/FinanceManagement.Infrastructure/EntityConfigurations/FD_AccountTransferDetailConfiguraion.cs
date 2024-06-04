using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_AccountTransferDetailConfiguraion : IEntityTypeConfiguration<FD_AccountTransferDetail>
    {
        public void Configure(EntityTypeBuilder<FD_AccountTransferDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccountID)
            .HasComment("账户ID")
            .AddStringToLongConvert();

            entity.Property(e => e.Amount)
            .HasComment("金额");

            entity.Property(e => e.Guid)
            .HasComment("Guid");

            entity.Property(e => e.PaymentTypeID)
            .HasComment("账户ID")
            .AddStringToLongConvert();

            entity.Property(e => e.IsIn)
            .HasComment("出入");

            entity.Property(e => e.DataDateTime)
            .HasComment("出入");

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.ModifiedDate)
            .HasComment("更新时间");
            
        }
    }
}
