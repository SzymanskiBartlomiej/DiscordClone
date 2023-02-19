using DiscordClone.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserServer> UserServers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = System.IO.File.ReadAllText("connectionString.txt");
            optionsBuilder.UseNpgsql(connectionString);
        } 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("Messages_pk");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("nextval('\"Messages_UserID_seq\"'::regclass)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Server).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ServerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Messages_Servers_null_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Messages_Users_null_fk");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.ServerId).HasName("Servers_pk");

            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.InviteCode)
                .HasColumnType("character varying")
                .HasColumnName("invite_code");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("Users_pk");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("character varying")
                .HasColumnName("password_hash");
            entity.Property(e => e.UserName)
                .HasColumnType("character varying")
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<UserServer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserServers_id_uindex");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Role)
                .HasColumnType("character varying")
                .HasColumnName("role");
            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Server).WithMany(p => p.UserServers)
                .HasForeignKey(d => d.ServerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserServers_Servers_null_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserServers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserServers_Users_null_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}