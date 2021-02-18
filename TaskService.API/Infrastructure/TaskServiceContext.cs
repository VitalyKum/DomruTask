using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TaskService.API.Models;

#nullable disable

namespace TaskService.API.Infrastructure
{
    public partial class TaskServiceContext : DbContext
    {
        public TaskServiceContext()
        {
        }

        public TaskServiceContext(DbContextOptions<TaskServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<OrderInfo> OrderInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("attachment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Hash)
                    .HasColumnType("character varying")
                    .HasColumnName("hash");
            });

            modelBuilder.Entity<OrderInfo>(entity =>
            {
                entity.ToTable("order_info");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AttachmentId).HasColumnName("attachment_id");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.Email)
                    .HasColumnType("character varying")
                    .HasColumnName("email");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnType("character varying")
                    .HasColumnName("phone_number");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Type)
                    .HasColumnType("character varying")
                    .HasColumnName("type");

                entity.Property(e => e.Value)
                    .HasColumnType("character varying")
                    .HasColumnName("value");

                entity.HasOne(d => d.Attachment)
                    .WithMany(p => p.OrderInfos)
                    .HasForeignKey(d => d.AttachmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("order_info_attachment_id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
