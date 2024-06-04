using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_CarryForwardVoucherRecordConfiguration : IEntityTypeConfiguration<FM_CarryForwardVoucherRecord>
    {
        public void Configure(EntityTypeBuilder<FM_CarryForwardVoucherRecord> entity)
        {
                entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

                entity.Property(e => e.RecordID)
                .HasComment("记录标识");

                entity.Property(e => e.NumericalOrderCarry)
                .HasComment("模板流水号")
                .AddStringToLongConvert();

                entity.Property(e => e.NumericalOrderSettl)
                .HasComment("凭证流水号")
                .AddStringToLongConvert();

                entity.Property(e => e.TransferAccountsType)
                .HasComment("结转类别")
                .AddStringToLongConvert();

                entity.Property(e => e.CarryName)
                .HasComment("模板名称");

                entity.Property(e => e.TransferAccountsAbstract)
                .HasComment("业务摘要")
                .AddStringToLongConvert();

                entity.Property(e => e.TicketedPointID)
                .HasComment("单据字")
                .AddStringToLongConvert();

                entity.Property(e => e.DataSource)
                .HasComment("来源数据")
                .AddStringToLongConvert();

                entity.Property(e => e.OwnerID)
                .HasComment("制单人")
                .AddStringToLongConvert();

                entity.Property(e => e.ImplementResult)
                .HasComment("执行结果");

                entity.Property(e => e.ResultState)
                .HasComment("执行状态");

                entity.Property(e => e.EnterpriseID)
                .HasComment("所属单位ID")
                .AddStringToLongConvert();

                entity.Property(e => e.TransBeginDate)
                .HasComment("结转开始时间");

                entity.Property(e => e.TransEndDate)
                .HasComment("结转开始时间");

                entity.Property(e => e.TransSummary)
                .HasComment("汇总方式(code)");

                entity.Property(e => e.TransSummaryName)
                .HasComment("汇总方式名称");

                entity.Property(e => e.TransWhereList)
                .HasComment("查询条件");

                entity.Property(e => e.Remarks)
                .HasComment("备注");

                entity.Property(e => e.CreatedDate)
                .HasComment("创建时间");
                entity.Property(e => e.ModifiedDate)
                .HasComment("修改时间");
           
        }
    }

   
}

