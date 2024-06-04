using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_InventoryConfiguration : IEntityTypeConfiguration<FA_Inventory>
    {
        public void Configure(EntityTypeBuilder<FA_Inventory> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                   .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder).HasComment("流水号").AddStringToLongConvert(); 

            entity.Property(e => e.EnterpriseID).HasComment("单位ID").AddStringToLongConvert();

            entity.Property(e => e.DataDate).HasComment("单据日期");

            entity.Property(e => e.Number).HasComment("单据号").AddStringToLongConvert();

            entity.Property(e => e.FAPlaceID)
                .HasComment("存放地点");

            entity.Property(e => e.UseStateID)
               .HasComment("使用状态");

            entity.Property(e => e.Remarks)
                .HasComment("备注");

            entity.Property(e => e.OwnerID)
                .HasComment("制单人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
           .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasMany(o => o.Lines)
               .WithOne(o => o.FA_Inventory)
               .HasForeignKey(o => o.NumericalOrder)
               .IsRequired();

    }
    }

    public class FA_InventoryDetailConfiguration : IEntityTypeConfiguration<FA_InventoryDetail>
    {
        public void Configure(EntityTypeBuilder<FA_InventoryDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
                     .HasName("PRIMARY");

            entity.Property(e => e.RecordID).HasComment("主键");

            entity.Property(e => e.NumericalOrder)
           .HasComment("流水号")
           .AddStringToLongConvert();

            entity.Property(e => e.CardID)
            .HasComment("卡片ID")
            .AddStringToLongConvert();


            entity.Property(e => e.Quantity)
            .HasComment("贮存数量");

            entity.Property(e => e.InventoryQuantity)
            .HasComment("盘点数量");           

            entity.Property(e => e.Remarks)
             .HasComment("备注");

            entity.Property(e => e.FileName)
            .HasComment("文件名");

            entity.Property(e => e.PathUrl)
            .HasComment("文件路径");

            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期")
                .ValueGeneratedOnAddOrUpdate();

           
           
        }
    }


}
