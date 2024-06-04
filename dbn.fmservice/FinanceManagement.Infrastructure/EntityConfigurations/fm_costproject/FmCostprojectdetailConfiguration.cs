using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmCostprojectdetailConfiguration : IEntityTypeConfiguration<FmCostprojectdetail>
    {
        public void Configure(EntityTypeBuilder<FmCostprojectdetail> entity)
        {
            entity.HasKey(e => e.RecordId)
                    .HasName("PRIMARY");

            entity.ToTable("fm_costprojectdetail");

            entity.Property(e => e.RecordId)
                .HasColumnType("int(11)")
                .HasColumnName("RecordID")
                .HasComment("主键");

            entity.Property(e => e.CostProjectId)
                .HasColumnType("bigint(20)")
                .HasColumnName("CostProjectID")
                .HasComment("费用项目ID")
                .AddStringToLongConvert();

            entity.Property(e => e.DataFormula)
                .HasColumnType("bigint(20)")
                .HasComment("取数公式")
                .AddStringToLongConvert();

            entity.Property(e => e.RelatedId)
                .HasColumnType("bigint(20)")
                .HasColumnName("RelatedID")
                .HasComment("关联ID")
                .AddStringToLongConvert();

            entity.Property(e => e.RelatedType)
                .HasColumnType("bigint(20)")
                .HasComment("关联类型")
                .AddStringToLongConvert();

            entity.Property(e => e.SubsidiaryAccounting)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("辅助核算");

            #region 排除
            entity.Ignore(e => e.ExtendDetails);
            #endregion
        }
    }
}
