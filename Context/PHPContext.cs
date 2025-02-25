using System;
using System.Collections.Generic;
using DotNetEnv;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PersonnelHolidayPlanner.Models;

namespace PersonnelHolidayPlanner.DBContext;

public partial class PHPContext : DbContext
{
    public string ConnStr { get; private set; }

    public PHPContext()
    {
        Env.Load();
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.DataSource = Environment.GetEnvironmentVariable("mssql_host");
        builder.InitialCatalog = Environment.GetEnvironmentVariable("mssql_initialCatalog");
        builder.UserID = Environment.GetEnvironmentVariable("mssql_username");
        builder.Password = Environment.GetEnvironmentVariable("mssql_password");
        builder.IntegratedSecurity = false; // Explicitly set to false
        builder.MultipleActiveResultSets = true;
        builder.TrustServerCertificate = true;

        ConnStr = builder.ConnectionString;
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Leave> Leaves { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(ConnStr);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11E00E8AEB");

            entity.ToTable("Employee");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);

            entity
                .HasMany(d => d.Projects)
                .WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeProject",
                    r =>
                        r.HasOne<Project>()
                            .WithMany()
                            .HasForeignKey("ProjectId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__EmployeeP__Proje__3F466844"),
                    l =>
                        l.HasOne<Employee>()
                            .WithMany()
                            .HasForeignKey("EmployeeId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__EmployeeP__Emplo__3E52440B"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "ProjectId")
                            .HasName("PK__Employee__6DB1E4FE38367151");
                        j.ToTable("EmployeeProject");
                    }
                );
        });

        modelBuilder.Entity<Leave>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__Leave__F7CD60157C6E773F");

            entity.ToTable("Leave");

            entity.Property(e => e.LeaveType).HasMaxLength(50);

            entity
                .HasOne(d => d.Employee)
                .WithMany(p => p.Leaves)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Leave__Employ__3B75D760");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Project__761ABEF0C3FB5F6E");

            entity.ToTable("Project");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
