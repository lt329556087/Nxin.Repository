using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_AccoCheckRuleConfiguration : IEntityTypeConfiguration<FinanceManagement.Domain.FM_AccoCheckRule>
    {
        public void Configure(EntityTypeBuilder<FinanceManagement.Domain.FM_AccoCheckRule> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");


            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoCheckType)
            .HasComment("结账类型（字典表）")
            .AddStringToLongConvert();

            entity.Property(e => e.MasterDataSource)
            .HasComment("主数据源")
            .AddStringToLongConvert();

            entity.Property(e => e.MasterFormula)
            .HasComment("主公式");

            entity.Property(e => e.MasterSecFormula)
            .HasComment("主公式信息");

            entity.Property(e => e.FollowDataSource)
            .HasComment("从数据源")
            .AddStringToLongConvert();

            entity.Property(e => e.FollowFormula)
            .HasComment("从公式");

            entity.Property(e => e.FollowSecFormula)
            .HasComment("从公式信息");

            entity.Property(e => e.CheckValue)
            .HasComment("校验信息")
            .AddStringToLongConvert();

            entity.Property(e => e.OwnerID)
             .HasComment("操作员ID")
            .AddStringToLongConvert();
            
            entity.Property(e => e.IsUse)
           .HasComment("是否启用");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");
            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");

        }
    }
}

