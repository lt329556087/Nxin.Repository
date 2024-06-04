using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmPigoriginalassetsdetailConfiguration : IEntityTypeConfiguration<FmPigoriginalassetsdetail>
    {
        public void Configure(EntityTypeBuilder<FmPigoriginalassetsdetail> entity)
        {
            entity.HasKey(e => e.EarNumber)
                    .HasName("PRIMARY");

            entity.Property(e => e.EarNumber)
                .ValueGeneratedNever()
                .HasComment("耳号").AddStringToLongConvert();

            entity.Property(e => e.AccruedMonth)
                .HasDefaultValueSql("'0'")
                .HasComment("已计提月份");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DepreciationAccumulated)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("累计折旧");

            entity.Property(e => e.DepreciationUseMonth)
                .HasDefaultValueSql("'0'")
                .HasComment("折旧年限（月）");

            entity.Property(e => e.ModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.NumericalOrder).HasComment("流水号").AddStringToLongConvert();

            entity.Property(e => e.OriginalValue)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("原值");

            entity.Property(e => e.PigType).HasComment("猪只类型").AddStringToLongConvert();

            entity.Property(e => e.ResidualValue)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("净残值");

            entity.Property(e => e.ResidualValueRate)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("净残值率（%）");

            entity.Property(e => e.StartDate).HasComment("开始使用日期");
        }
    }
}
