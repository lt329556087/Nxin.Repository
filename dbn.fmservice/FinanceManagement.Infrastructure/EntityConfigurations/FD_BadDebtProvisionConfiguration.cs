using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BadDebtProvisionConfiguration : IEntityTypeConfiguration<FD_BadDebtProvision>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtProvision> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
           .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.TicketedPointID)
            .HasComment("核算单元ID")
            .AddStringToLongConvert();

            entity.Property(e => e.MarketID)
            .HasComment("部门ID")
            .AddStringToLongConvert();

            entity.Property(e => e.PersonID)
            .HasComment("员工ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID1)
             .HasComment("科目ID")
             .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID2)
             .HasComment("科目ID")
             .AddStringToLongConvert();

            entity.Property(e => e.HaveProvisionAmount1)
             .HasComment("已计提总额1");

            entity.Property(e => e.HaveProvisionAmount2)
            .HasComment("已计提总额2");

            entity.Property(e => e.DataDate)
            .HasComment("单据日期");

            entity.Property(e => e.NumericalOrderSetting)
            .HasComment("流水号")
            .AddStringToLongConvert();

        }
    }

    public class FD_BadDebtProvisionDetailConfiguration : IEntityTypeConfiguration<FD_BadDebtProvisionDetail>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtProvisionDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
           .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("表体流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.CustomerID)
            .HasComment("往来类型单位")
            .AddStringToLongConvert();


            entity.Property(e => e.AccoSubjectID)
            .HasComment("往来类型单位")
            .AddStringToLongConvert();

            entity.Property(e => e.NoReceiveAmount)
            .HasComment("未收回金额");

            entity.Property(e => e.CurrentDebtPrepareAmount)
             .HasComment("本期坏账计提准备金额");

            entity.Property(e => e.LastDebtPrepareAmount)
            .HasComment("上期坏账计提准备金额");

            entity.Property(e => e.TransferAmount)
            .HasComment("调整金额");

            entity.Property(e => e.ProvisionAmount)
            .HasComment("本期计提金额");

            entity.Property(e => e.NumericalOrderSpecific)
             .HasComment("个别认定流水号")
             .AddStringToLongConvert();


            entity.Property(e => e.ReclassAmount)
             .HasComment("重分类金额");

            entity.Property(e => e.EndAmount)
            .HasComment("期末金额");

            entity.Property(e => e.ProvisionType)
            .HasComment("计提类型")
            .AddStringToLongConvert();

            entity.Property(e => e.BusinessType)
            .HasComment("往来类型")
            .AddStringToLongConvert();

        }
    }

    public class FD_BadDebtProvisionExtConfiguration : IEntityTypeConfiguration<FD_BadDebtProvisionExt>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtProvisionExt> entity)
        {
            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("表体流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AgingID)
            .HasComment("账龄ID");

            entity.Property(e => e.Amount)
            .HasComment("本期计提金额");

            entity.Property(e => e.Ratio)
            .HasComment("比例");
        }
    }
}
