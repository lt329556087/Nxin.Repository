using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class fm_expensereportextendConfiguration : IEntityTypeConfiguration<fm_expensereportextend>
    {
        public void Configure(EntityTypeBuilder<fm_expensereportextend> entity)
        {
            entity.HasKey(e => e.RecordID)
                    .HasName("PRIMARY");

            entity.Property(e => e.RecordID).HasComment("主键");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DeptOrOthersID)
                .HasDefaultValueSql("'0'")
                .HasComment("部门ID")
                .AddStringToLongConvert();

            entity.Property(e => e.DetailID).HasComment("关联明细ID");

            entity.Property(e => e.ExpenseAmount)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("费用金额");

            entity.Property(e => e.ModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.PigFarmID)
                .HasDefaultValueSql("'0'")
                .HasComment("猪场ID")
                .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
                .HasDefaultValueSql("''")
                .HasComment("备注");
        }
    }
}
