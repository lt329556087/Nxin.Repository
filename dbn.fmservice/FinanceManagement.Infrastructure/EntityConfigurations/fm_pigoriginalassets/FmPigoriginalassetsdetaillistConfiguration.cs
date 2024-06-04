using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmPigoriginalassetsdetaillistConfiguration : IEntityTypeConfiguration<FmPigoriginalassetsdetaillist>
    {
        public void Configure(EntityTypeBuilder<FmPigoriginalassetsdetaillist> entity)
        {
            entity.HasKey(e => e.RecordId)
                    .HasName("PRIMARY");

            entity.Property(e => e.RecordId).HasComment("主键");

            entity.Property(e => e.AccruedMonth)
                .HasDefaultValueSql("'0'")
                .HasComment("已计提月份");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DataDate).HasComment("日期").HasConversion(new ValueConverter<string, DateTime>((string model) => Convert.ToDateTime(model), (DateTime store) => store.ToString("yyyy-MM-dd")));

            entity.Property(e => e.DepreciationAccumulated)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("累计折旧");

            entity.Property(e => e.DepreciationMonthAmount)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("月折旧额");

            entity.Property(e => e.DepreciationMonthRate)
                .HasPrecision(18, 6)
                .HasDefaultValueSql("'0.000000'")
                .HasComment("月折旧率");

            entity.Property(e => e.DepreciationUseMonth)
                .HasDefaultValueSql("'0'")
                .HasComment("折旧年限（月）");

            entity.Property(e => e.EarNumber).HasComment("耳号").AddStringToLongConvert();
            entity.Property(e => e.PigType).HasComment("猪只类型").AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.NetValue)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("净值");

            entity.Property(e => e.OriginalValue)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("原值");

            entity.Property(e => e.PigFarmId).HasComment("猪场ID").AddStringToLongConvert();
            entity.Property(e => e.EnterpriseId).HasComment("单位ID").AddStringToLongConvert();

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
