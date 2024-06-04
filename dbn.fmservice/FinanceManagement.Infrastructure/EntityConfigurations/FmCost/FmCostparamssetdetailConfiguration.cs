using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmCostparamssetdetailConfiguration : IEntityTypeConfiguration<FmCostparamssetdetail>
    {
        public void Configure(EntityTypeBuilder<FmCostparamssetdetail> entity)
        {

            entity.HasKey(e => e.RecordId)
                    .HasName("PRIMARY");

            entity.ToTable("fm_costparamssetdetail");

            entity.HasComment("成本计算设置明细表");

            entity.Property(e => e.RecordId)
                .HasColumnType("int(11)")
                .HasColumnName("RecordID")
                .HasComment("主键");

            entity.Property(e => e.BeginDate)
                .HasColumnType("date")
                .HasComment("猪场建账日期");

            entity.Property(e => e.BeginPeriod)
                .HasColumnType("varchar(20)")
                .HasComment("猪场建账期间");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.EnableDate)
                .HasColumnType("date")
                .HasComment("成本计算开始日期");

            entity.Property(e => e.EnablePeriod)
                .HasColumnType("varchar(20)")
                .HasComment("成本启用期间");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.PigFarmId)
                .HasColumnType("bigint(20)")
                .HasColumnName("PigFarmID")
                .HasComment("猪场ID")
                .AddStringToLongConvert();

        }
    }
}
