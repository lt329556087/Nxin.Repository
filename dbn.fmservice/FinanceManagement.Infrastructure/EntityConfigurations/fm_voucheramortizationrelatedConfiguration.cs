﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class fm_voucheramortizationrelatedConfiguration : IEntityTypeConfiguration<fm_voucheramortizationrelated>
    {
        public void Configure(EntityTypeBuilder<fm_voucheramortizationrelated> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.HasComment("凭证摊销记录表");

            entity.Property(e => e.RecordID)
                .HasColumnType("int(11)")
                .HasComment("记录标识");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后修改日期")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.NumericalOrderInto)
                .HasColumnType("bigint(20)")
                .HasComment("期间流水号");

            entity.Property(e => e.NumericalOrderSettl)
                .HasColumnType("bigint(20)")
                .HasComment("凭证流水号");

            entity.Property(e => e.NumericalOrderStay)
                .HasColumnType("bigint(20)")
                .HasComment("待摊流水号");

            entity.Property(e => e.NumericalOrderVoucher)
                .HasColumnType("bigint(20)")
                .HasComment("摊销流水号");

            entity.Property(e => e.VoucherAmount)
                .HasColumnType("decimal(18,2)")
                .HasComment("摊销金额");
        }
    }
}
