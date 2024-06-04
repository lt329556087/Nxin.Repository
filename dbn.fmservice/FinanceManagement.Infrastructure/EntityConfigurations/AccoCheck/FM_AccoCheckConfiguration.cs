using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_AccoCheckConfiguration : IEntityTypeConfiguration<FinanceManagement.Domain.FM_AccoCheck>
    {
        public void Configure(EntityTypeBuilder<FinanceManagement.Domain.FM_AccoCheck> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
           .HasComment("日期");

            entity.Property(e => e.StartDate)
            .HasComment("开始日期");

            entity.Property(e => e.EndDate)
            .HasComment("结束日期");

            entity.Property(e => e.CheckMark)
            .HasComment("结账标识");

            entity.Property(e => e.Remarks)
                 .HasComment("备注");

            entity.Property(e => e.OwnerID)
             .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");
            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");


        }
    }
}

