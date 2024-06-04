using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmCostparamssetConfiguration : IEntityTypeConfiguration<FmCostparamsset>
    {
        public void Configure(EntityTypeBuilder<FmCostparamsset> entity)
        {

            entity.HasKey(e => e.NumericalOrder)
                    .HasName("PRIMARY");

            entity.ToTable("fm_costparamsset");

            entity.HasComment("成本计算设置主表");

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .ValueGeneratedNever()
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.EnterpriseId)
                .HasColumnType("bigint(20)")
                .HasColumnName("EnterpriseID")
                .HasComment("单位ID")
                .AddStringToLongConvert();

            entity.Property(e => e.GenerationMode)
                .HasColumnType("bigint(20)")
                .HasComment("卡片生成方式")
                .AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.OwnerId)
                .HasColumnType("bigint(20)")
                .HasColumnName("OwnerID")
                .HasComment("制单人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
                .HasMaxLength(1024)
                .HasDefaultValueSql("''")
                .HasComment("备注");

            entity.Property(e => e.ResidualValueCalMethod)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("残值计算方法")
                .AddStringToLongConvert();

            entity.Property(e => e.ResidualValueRate)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("残值率(%)");

            entity.Property(e => e.ResidualValue)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("固定残值");

            entity.Property(e => e.TotalDepreciationMonths)
                .HasColumnType("int(11)")
                .HasComment("总折旧月数");

        }
    }
}
