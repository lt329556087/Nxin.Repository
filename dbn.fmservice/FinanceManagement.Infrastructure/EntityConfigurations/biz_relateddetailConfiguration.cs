﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure
{
    public class biz_relateddetailConfiguration : IEntityTypeConfiguration<biz_relateddetail>
    {
        public void Configure(EntityTypeBuilder<biz_relateddetail> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.HasComment("信息关联明细");

            entity.HasIndex(e => e.RelatedDetailID)
                .HasName("idx_RelatedDetailID");

            entity.HasIndex(e => e.RelatedDetailType)
                .HasName("idx_RelatedDetailType");

            entity.HasIndex(e => e.RelatedID)
                .HasName("idx_RelatedID");

            entity.Property(e => e.RecordID)
                .HasColumnType("int(11)")
                .HasComment("主键ID");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后修改日期")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.OwnerID)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("制单人");

            entity.Property(e => e.Paid)
                .HasColumnType("decimal(18,2)")
                .HasComment("已付金额");

            entity.Property(e => e.Payable)
                .HasColumnType("decimal(18,2)")
                .HasComment("应付金额");

            entity.Property(e => e.Payment)
                .HasColumnType("decimal(18,2)")
                .HasComment("本次支付金额");

            entity.Property(e => e.RelatedDetailID)
                .HasColumnType("bigint(20)")
                .HasComment("关联明细ID");

            entity.Property(e => e.RelatedDetailType)
                .HasColumnType("bigint(20)")
                .HasComment("关联明细类型ID");

            entity.Property(e => e.RelatedID)
                .HasColumnType("bigint(20)")
                .HasComment("信息关联ID");

            entity.Property(e => e.Remarks)
                .HasColumnType("varchar(1024)")
                .HasComment("备注")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");
        }
    }
}