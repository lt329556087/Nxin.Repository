using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class Nxin_DigitalIntelligenceMapConfiguration : IEntityTypeConfiguration<Nxin_DigitalIntelligenceMap>
    {
        public void Configure(EntityTypeBuilder<Nxin_DigitalIntelligenceMap> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();
         
            entity.Property(e => e.GroupID)
            .HasComment("集团标识")
            .AddStringToLongConvert();

            entity.Property(e => e.MapType)
            .HasComment("地图类型")
            .AddStringToLongConvert();

            entity.Property(e => e.BackgroundValue)
             .HasComment("背景色");

            entity.Property(e => e.BlockList)
            .HasComment("区块配置信息");

            entity.Property(e => e.OwnerID)
             .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");
         
        }
    }

   
}

