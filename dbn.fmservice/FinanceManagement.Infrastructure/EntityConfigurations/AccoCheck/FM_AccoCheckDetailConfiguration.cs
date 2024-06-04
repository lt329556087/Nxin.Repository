using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_AccoCheckDetailConfiguration : IEntityTypeConfiguration<FM_AccoCheckDetail>
    {
        public void Configure(EntityTypeBuilder<FM_AccoCheckDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("明细流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoCheckType)
            .HasComment("结账类型（字典表）")
            .AddStringToLongConvert();

            entity.Property(e => e.CheckMark)
            .HasComment("是否结账（0：未结账，1：已结账）");

            entity.Property(e => e.IsNew)
            .HasComment("是否一键结账");

            entity.Property(e => e.OwnerID)
            .HasComment("操作员ID")
            .AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");

        }
    }
}
