using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_SpecificIdentificationConfiguration : IEntityTypeConfiguration<FD_SpecificIdentification>
    {

        public void Configure(EntityTypeBuilder<FD_SpecificIdentification> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
           .HasComment("单据日期");

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.OwnerID)
            .HasComment("制单人ID")
            .AddStringToLongConvert();

            entity.Property(e => e.BusinessType)
            .HasComment("往来类型")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID2)
            .HasComment("科目2")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID1)
            .HasComment("科目1")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderSetting)
            .HasComment("科目1")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("更新时间");

            entity.HasMany(o => o.Lines)
                .WithOne(o => o.FD_SpecificIdentification)
                .HasForeignKey(o => o.NumericalOrder)
                .IsRequired();
        }
    }

    public class FD_SpecificIdentificationDetailConfiguration : IEntityTypeConfiguration<FD_SpecificIdentificationDetail>
    {
        public void Configure(EntityTypeBuilder<FD_SpecificIdentificationDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("自增ID")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.ProvisionType)
            .HasComment("计提类型")
            .AddStringToLongConvert();

            entity.Property(e => e.CustomerID)
            .HasComment("客户")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID)
            .HasComment("科目ID")
            .AddStringToLongConvert();

        entity.Property(e => e.Amount)
           .HasComment("金额");

            entity.Property(e => e.AccoAmount)
       .HasComment("金额");

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.ModifiedDate)
           .HasComment("更新时间");
        }
    }

    public class FD_SpecificIdentificationExtConfiguration : IEntityTypeConfiguration<FD_SpecificIdentificationExt>
    {
        public void Configure(EntityTypeBuilder<FD_SpecificIdentificationExt> entity)
        {
            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Amount)
           .HasComment("金额");

            entity.Property(e => e.Name)
           .HasComment("账龄区间名称");

        }
    }
}
