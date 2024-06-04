using Architecture.Seedwork.Domain;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;


namespace FinanceManagement.Infrastructure.EntityConfigurations
{

    public class Biz_RelatedEntityConfigurations : IEntityTypeConfiguration<BIZ_Related>
    {
        public void Configure(EntityTypeBuilder<BIZ_Related> entity)
        {

            entity.HasKey(e => e.RelatedID)
                .HasName("PRIMARY");

            entity.Property(e => e.RelatedID)
            .HasComment("自增ID");


            entity.Property(e => e.RelatedType)
            .HasComment("关联类型")
            .AddStringToLongConvert();

            entity.Property(e => e.ParentType)
            .HasComment("父级类型")
             .AddStringToLongConvert();


            entity.Property(e => e.ChildType)
            .HasComment("子级类型")
            .AddStringToLongConvert();

            entity.Property(e => e.ParentValue)
            .HasComment("父级流水号")
              .AddStringToLongConvert();

            entity.Property(e => e.ChildValue)
            .HasComment("子级流水号")
              .AddStringToLongConvert();

            entity.Property(e => e.ParentValueDetail)
            .HasComment("数量")
            .AddStringToLongConvert();


            entity.Property(e => e.ChildValueDetail)
            .HasComment("调整数量")
            .AddStringToLongConvert();


            entity.Property(e => e.Remarks)
            .HasComment("备注");

        }
    }

    public class Biz_Related_FMEntityConfigurations : IEntityTypeConfiguration<BIZ_Related_FM>
    {
        public void Configure(EntityTypeBuilder<BIZ_Related_FM> entity)
        {

            entity.HasKey(e => e.RelatedID)
                .HasName("PRIMARY");

            entity.Property(e => e.RelatedID)
            .HasComment("自增ID");


            entity.Property(e => e.RelatedType)
            .HasComment("关联类型")
            .AddStringToLongConvert();

            entity.Property(e => e.ParentType)
            .HasComment("父级类型")
             .AddStringToLongConvert();


            entity.Property(e => e.ChildType)
            .HasComment("子级类型")
            .AddStringToLongConvert();

            entity.Property(e => e.ParentValue)
            .HasComment("父级流水号")
              .AddStringToLongConvert();

            entity.Property(e => e.ChildValue)
            .HasComment("子级流水号")
              .AddStringToLongConvert();

            entity.Property(e => e.ParentValueDetail)
            .HasComment("数量")
            .AddStringToLongConvert();


            entity.Property(e => e.ChildValueDetail)
            .HasComment("调整数量")
            .AddStringToLongConvert();


            entity.Property(e => e.Remarks)
            .HasComment("备注");

        }
    }
}
