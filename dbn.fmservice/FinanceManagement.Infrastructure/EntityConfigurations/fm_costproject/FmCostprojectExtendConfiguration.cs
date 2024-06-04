using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmCostprojectExtendConfiguration : IEntityTypeConfiguration<FmCostprojectExtend>
    {
        public void Configure(EntityTypeBuilder<FmCostprojectExtend> entity)
        {
            entity.HasKey(e => e.RecordId)
                    .HasName("PRIMARY");

            entity.ToTable("fm_costprojectextend");

            entity.Property(e => e.RecordId)
                .HasColumnType("int(11)")
                .HasColumnName("RecordID")
                .HasComment("主键");

            entity.Property(e => e.DetailID)
                .HasColumnType("int(11)")
                .HasColumnName("DetailID")
                .HasComment("明细ID");

            entity.Property(e => e.RelatedId)
                .HasColumnType("bigint(20)")
                .HasColumnName("RelatedID")
                .HasComment("关联ID")
                .AddStringToLongConvert();

            entity.Property(e => e.RelatedType)
                .HasColumnType("bigint(20)")
                .HasComment("关联类型")
                .AddStringToLongConvert();
        }
    }
}
