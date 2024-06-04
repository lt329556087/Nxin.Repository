using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_AccoCheckExtendConfiguration : IEntityTypeConfiguration<FM_AccoCheckExtend>
    {
        public void Configure(EntityTypeBuilder<FM_AccoCheckExtend> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("扩展流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.MenuID)
            .HasComment("菜单ID");

            entity.Property(e => e.CheckMark)
            .HasComment("标识");
         
            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");

        }
    }
}
