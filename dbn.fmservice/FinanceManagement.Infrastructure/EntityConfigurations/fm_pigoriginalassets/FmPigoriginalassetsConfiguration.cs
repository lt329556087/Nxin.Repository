using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmPigoriginalassetsConfiguration : IEntityTypeConfiguration<FmPigoriginalassets>
    {
        public void Configure(EntityTypeBuilder<FmPigoriginalassets> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                    .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
                .ValueGeneratedNever()
                .HasComment("流水号").AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DataDate).HasComment("日期").HasConversion(new ValueConverter<string, DateTime>((string model) => Convert.ToDateTime(model), (DateTime store) => store.ToString("yyyy-MM-dd"))); //.HasConversion<StringToDateTimeConverter>();//.HasConversion<DateTimeOffset>();

            entity.Property(e => e.EnterpriseId).HasComment("单位ID").AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.ModifiedOwnerId)
                .HasDefaultValueSql("'0'")
                .HasComment("修改人").AddStringToLongConvert();

            entity.Property(e => e.Number).HasComment("单据号").AddStringToLongConvert();

            entity.Property(e => e.OwnerId).HasComment("制单人ID").AddStringToLongConvert();

            entity.Property(e => e.PigFarmId).HasComment("猪场ID").AddStringToLongConvert();

            entity.Property(e => e.Remarks)
                .HasDefaultValueSql("''")
                .HasComment("备注");

            entity.Property(e => e.SourceType).HasComment("来源类型").AddStringToLongConvert();
        }
    }
}
