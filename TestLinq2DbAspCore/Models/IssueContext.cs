using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TestLinq2DbAspCore.Models
{
    public partial class IssueContext : DbContext
    {
        public IssueContext()
        {
        }

        public IssueContext(DbContextOptions<IssueContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Devices> Devices { get; set; }
        public virtual DbSet<Devtypes> Devtypes { get; set; }

        public virtual DbSet<DAL_Gateway> MeshGateways { get; set; }
        public virtual DbSet<DAL_GatewaySettings> MeshGatewaySettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp")
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            //modelBuilder.Entity<DevReadingType>(entity =>
            //{
            //    entity.ToTable("DevReadingType", "billing");

            //    entity.Property(e => e.Name)
            //        .IsRequired()
            //        .HasColumnType("character varying");
            //});

            modelBuilder.Entity<Devices>(entity =>
            {
                entity.HasKey(e => e.Devid);
            
                entity.ToTable("devices", "billing");
            
                entity.Property(e => e.Devid)
                    .HasColumnName("devid")
                    .ValueGeneratedNever();
            
                entity.Property(e => e.Devtypeid).HasColumnName("devtypeid");
            
                entity.Property(e => e.Sernum)
                    .HasColumnName("sernum")
                    .HasColumnType("character varying");
            });
            
            modelBuilder.Entity<Devtypes>(entity =>
            {
                entity.HasKey(e => e.Devtypeid);
            
                entity.ToTable("devtypes", "billing");
            
                entity.Property(e => e.Devtypeid).HasColumnName("devtypeid");
            
                entity.Property(e => e.Typename)
                    .IsRequired()
                    .HasColumnName("typename")
                    .HasColumnType("character varying");
            });

            //modelBuilder.Entity<TempReading>(entity =>
            //{
            //    entity.ToTable("TempReading", "billing");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.DevSerNum)
            //        .IsRequired()
            //        .HasColumnType("character varying");

            //    entity.Property(e => e.Devid)
            //        .HasColumnName("devid")
            //        .HasColumnType("character varying");

            //    entity.Property(e => e.ReadingTypeName).HasColumnType("character varying");

            //    entity.Property(e => e.Tsdevice).HasColumnName("tsdevice");

            //    entity.Property(e => e.Value)
            //        .HasColumnName("value")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<TestTable>(entity =>
            //{
            //    entity.ToTable("TestTable", "billing");
            //
            //    entity.Property(e => e.Id)
            //        .HasColumnName("id")
            //        
            //    //    .ValueGeneratedNever()
            //    ;
            //
            //    entity.Property(e => e.Name)
            //        .IsRequired()
            //        .HasColumnType("character varying");
            //});

            modelBuilder.Entity<DAL_GatewaySettings>(entity =>
            {

                entity.HasIndex(e => e.GatewayId);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Gateway)
                    .WithMany(p => p.GatewaySets)
                    .HasForeignKey(d => d.GatewayId);
            });

            modelBuilder.Entity<DAL_Gateway>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
        }
    }
}
