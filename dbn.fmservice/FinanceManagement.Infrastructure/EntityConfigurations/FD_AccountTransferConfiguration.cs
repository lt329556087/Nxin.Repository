using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_AccountTransferConfiguration : IEntityTypeConfiguration<FD_AccountTransfer>
    {
        public void Configure(EntityTypeBuilder<FD_AccountTransfer> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
            .HasComment("日期");

            entity.Property(e=>e.AccountTransferType)
            .HasComment("结转类型")
            .AddStringToLongConvert();

            entity.Property(e => e.AccountTransferAbstract)
            .HasComment("结转摘要")
            .AddStringToLongConvert();

            entity.Property(e => e.OwnerID)
            .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            entity.Property(e => e.UploadUrl)
             .HasComment("上传地址");
            
            entity.HasMany(o => o.Details)
            .WithOne(o => o.FD_AccountTransfer)
             .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();
        }
    }
}
