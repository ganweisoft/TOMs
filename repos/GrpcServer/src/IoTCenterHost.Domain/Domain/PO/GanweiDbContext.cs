//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using IoTCenterHost.AppServices.Domain.DO.GWServiceToken;
using IoTCenterHost.Domain.Domain.DO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IoTCenterHost.AppServices.Domain.PO
{
    public class GanweiDbContext : DbContext
    {


        public virtual DbSet<DO.User.UserEntity> GWUserTable { get; set; }
        public virtual DbSet<GWRoleTableRow> GWRoleTable { get; set; }

        public virtual DbSet<YCP> YCP { get; set; }

        public virtual DbSet<YXP> YXP { get; set; }

        public virtual DbSet<GWMenu> GWMenu { get; set; }

        public virtual DbSet<GWServiceToken> GWServiceToken { get; set; }

        public virtual DbSet<Productproperty> ProductProperty { get; set; }

        public virtual DbSet<GWExProc> GWExProc { get; set; }

        public virtual DbSet<AlarmProc> AlarmProc { get; set; }

        public GanweiDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GWServiceToken>(entity =>
            {
                entity.ToTable("gwservicetoken");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).HasColumnName("token");
                entity.Property(e => e.Vaild).HasColumnName("vaild");
                entity.Property(e => e.Createdat).HasColumnName("createdat");
                entity.Property(e => e.Createdby).HasColumnName("createdby");
            });
            modelBuilder.Entity<DO.User.UserEntity>(entity =>
            {
                entity.ToTable("gwuser");

                entity.Property(e => e.ID)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<GWRoleTableRow>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.ToTable("gwrole");

                entity.Property(e => e.ControlEquips_Unit).HasColumnName("controlequips_unit");

                entity.Property(e => e.remark).HasColumnName("remark");
            });

            modelBuilder.Entity<YCP>(entity =>
            {
                entity.HasKey(e => new { e.equip_no, e.yc_no });

                entity.ToTable("YCP");

            });
            modelBuilder.Entity<YXP>(entity =>
            {
                entity.HasKey(e => new { e.equip_no, e.yx_no });

                entity.ToTable("YXP");

            });
            modelBuilder.Entity<GWMenu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("gwmenu");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasColumnName("code");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Menuname).HasColumnName("menuname");
                entity.Property(e => e.Parentid).HasColumnName("parentid");
                entity.Property(e => e.Route).HasColumnName("route");
                entity.Property(e => e.Path).HasColumnName("path");
                entity.Property(e => e.Icon).HasColumnName("icon");
                entity.Property(e => e.Nodetype).HasColumnName("nodetype");
                entity.Property(e => e.Order).HasColumnName("order");
                entity.Property(e => e.Menuowner).HasColumnName("menuowner");
                entity.Property(e => e.Enabled).HasColumnName("enabled");
                entity.Property(e => e.Packageid).HasColumnName("packageid");
            });

            modelBuilder.Entity<GWExProc>(entity =>
            {
                entity.ToTable("gwexproc");
                entity.HasKey(e => e.ProcCode);
                entity.Property(e => e.ProcCode).HasColumnName("proc_code");
                entity.Property(e => e.ProcModule).HasColumnName("proc_module");
                entity.Property(e => e.ProcName).HasColumnName("proc_name");
                entity.Property(e => e.ProcParm).HasColumnName("proc_parm");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.Reserve1).HasColumnName("reserve1");
                entity.Property(e => e.Reserve2).HasColumnName("reserve2");
                entity.Property(e => e.Reserve3).HasColumnName("reserve3");
            });
            modelBuilder.Entity<AlarmProc>(entity =>
            {
                entity.ToTable("alarmproc");
                entity.HasKey(e => e.ProcCode);
                entity.Property(e => e.ProcCode).HasColumnName("proc_code");
                entity.Property(e => e.ProcModule).HasColumnName("proc_module");
                entity.Property(e => e.ProcName).HasColumnName("proc_name");
                entity.Property(e => e.ProcParm).HasColumnName("proc_parm");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.Reserve1).HasColumnName("reserve1");
                entity.Property(e => e.Reserve2).HasColumnName("reserve2");
                entity.Property(e => e.Reserve3).HasColumnName("reserve3");
            });

            var entityTypes = modelBuilder.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                entityType.SetTableName(entityType.GetTableName().ToLower());

                foreach (var property in entityType.GetProperties())
                {
                    var storeObjectId = StoreObjectIdentifier.Table(entityType.GetTableName().ToLower());
                    property.SetColumnName(property.GetColumnName(storeObjectId).ToLower());
                }
            }


        }
    }
}
