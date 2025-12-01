// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Data;
using IoTCenter.Data;
using IoTCenter.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    public class EquipLinkDbContext : GWDbContext
    {
        public EquipLinkDbContext(DbContextOptions<EquipLinkDbContext> options) : base(options) { }

        public virtual DbSet<ConditionAutoProc> ConditionAutoProcs { get; set; }
        public virtual DbSet<ConditionEquipExpr> ConditionEquipExprs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConditionAutoProc>(entity =>
            {
                entity.HasMany(e => e.ConditionExpressions)
                    .WithOne(e => e.ConditionAutoProc)
                    .HasForeignKey(e => e.ConditionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ConditionEquipExpr>(entity =>
            {
                entity.HasOne(e => e.ConditionAutoProc)
                    .WithMany(e => e.ConditionExpressions)
                    .HasForeignKey(e => e.ConditionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
