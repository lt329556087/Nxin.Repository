using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_MarketSubjectConfiguration : IEntityTypeConfiguration<FA_MarketSubject>
    {
        public void Configure(EntityTypeBuilder<FA_MarketSubject> entity)
        {

            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
           .HasComment("日期");

            entity.Property(e => e.Remarks)
           .HasComment("备注");

            entity.Property(e => e.OwnerID)
            .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
          .HasComment("修改日期");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.FA_MarketSubject)
             .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();
        }
    }

    public class FA_MarketSubjectDetailConfiguration : IEntityTypeConfiguration<FA_MarketSubjectDetail>
    {
        public void Configure(EntityTypeBuilder<FA_MarketSubjectDetail> entity)
        {

            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.MarketID)
            .HasComment("部门ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID)
            .HasComment("科目ID")
            .AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
             .HasComment("修改日期");
        }
    }
}
