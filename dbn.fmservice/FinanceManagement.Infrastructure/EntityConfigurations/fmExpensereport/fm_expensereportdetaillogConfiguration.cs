using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class fm_expensereportdetaillogConfiguration : IEntityTypeConfiguration<fm_expensereportdetaillog>
    {
        public void Configure(EntityTypeBuilder<fm_expensereportdetaillog> entity)
        {
            entity.HasKey(e => e.RecordID)
                    .HasName("PRIMARY");

            entity.Property(e => e.RecordID).HasComment("主键");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DetailID).HasComment("关联明细ID");

            entity.Property(e => e.ErrorCode)
                .HasDefaultValueSql("''")
                .HasComment("错误编码");

            entity.Property(e => e.ErrorMsg)
                .HasDefaultValueSql("''")
                .HasComment("错误信息");

            entity.Property(e => e.ModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.OccuredAmount)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("金额");

            entity.Property(e => e.SubsidiaryOption)
                .HasDefaultValueSql("'-'")
                .HasComment("辅助项");
        }
    }
}
