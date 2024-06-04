using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BadDebtExecutionConfiguratioin : IEntityTypeConfiguration<FD_BadDebtExecution>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtExecution> entity)
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

            entity.Property(e => e.NumericalOrderReceipt)
            .HasComment("凭证流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AppID)
            .HasComment("菜单ID")
            .AddStringToLongConvert();

            entity.Property(e => e.State)
            .HasComment("生成凭证情况");

            entity.Property(e => e.CreateDate)
            .HasComment("单据日期");
        }
    }
}
