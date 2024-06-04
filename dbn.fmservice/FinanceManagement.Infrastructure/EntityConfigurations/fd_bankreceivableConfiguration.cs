﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.EntityConfigurations
{
    public class fd_bankreceivableConfiguration : IEntityTypeConfiguration<fd_bankreceivable>
    {
        public void Configure(EntityTypeBuilder<fd_bankreceivable> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                .HasName("PRIMARY");

            entity.HasComment("银行流水记录表");

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.SourceNum)
                .HasColumnType("bigint(20)")
                .HasComment("自动生成的收款流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.CreateTime)
                .HasColumnType("datetime")
                .HasComment("创建日期");

            entity.Property(e => e.IsGenerate)
                .HasColumnType("int(4)")
                .HasDefaultValueSql("'0'")
                .HasComment("1:已生成,2:未生成,3:自动生成");

            entity.Property(e => e.Remarks)
                .HasColumnType("varchar(255)")
                .HasComment("备注")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.acctIndex)
                .HasColumnType("varchar(255)")
                .HasComment("账户索引")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.acctNo)
                .HasColumnType("varchar(255)")
                .HasComment("账户")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.amount)
                .HasColumnType("decimal(18,2)")
                .HasComment("金额");

            entity.Property(e => e.bankSerial)
                .HasColumnType("varchar(255)")
                .HasComment("交易流水")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.custList)
                .HasColumnType("text")
                .HasComment("客户信息")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.dataSource)
                .HasColumnType("varchar(255)")
                .HasComment("来源：YQT-银企通；WX-微信；POS-pos机")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.entId)
                .HasColumnType("varchar(255)")
                .HasComment("单位ID")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.entName)
                .HasColumnType("varchar(255)")
                .HasComment("单位名称")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.fee)
                .HasColumnType("decimal(18,2)")
                .HasComment("手续费");

            entity.Property(e => e.msg)
                .HasColumnType("varchar(255)")
                .HasComment("摘要")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.msgCode)
                .HasColumnType("varchar(255)")
                .HasComment("摘要编码")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.otherSideAcct)
                .HasColumnType("varchar(255)")
                .HasComment("对方账户")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.otherSideAcctIndex)
                .HasColumnType("varchar(255)")
                .HasComment("对方账户索引")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.otherSideName)
                .HasColumnType("varchar(255)")
                .HasComment("对方名称")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.receiveDay)
                .HasColumnType("date")
                .HasComment("到账日期");

            entity.Property(e => e.transIndex)
                .HasColumnType("varchar(255)")
                .HasComment("交易索引")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");
        }
    }
}