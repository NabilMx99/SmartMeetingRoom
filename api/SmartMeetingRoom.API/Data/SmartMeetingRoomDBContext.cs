using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Data;

public partial class SmartMeetingRoomDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public SmartMeetingRoomDBContext()
    {
    }

    public SmartMeetingRoomDBContext(DbContextOptions<SmartMeetingRoomDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionItem> ActionItems { get; set; }

    public virtual DbSet<Attendee> Attendees { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Meeting> Meetings { get; set; }

    public virtual DbSet<MeetingMinute> MeetingMinutes { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomFeature> RoomFeatures { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=SmartMeetingRoomDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActionItem>(entity =>
        {
            entity.HasKey(e => e.ActionItemId).HasName("PK__ActionIt__56285AB24BCFCCD3");

            entity.ToTable("ActionItem");

            entity.Property(e => e.ActionItemDescription).IsUnicode(false);
            entity.Property(e => e.ActionItemDueDate).HasColumnType("datetime");
            entity.Property(e => e.ActionItemStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.FkMeetingMinutesId).HasColumnName("FK_MeetingMinutesId");
            entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");

            entity.HasOne(d => d.FkMeetingMinutes).WithMany(p => p.ActionItems)
                .HasForeignKey(d => d.FkMeetingMinutesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActionIte__FK_Me__7E37BEF6");

            entity.HasOne(d => d.FkUser).WithMany(p => p.ActionItems)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActionIte__FK_Us__7F2BE32F");
        });

        modelBuilder.Entity<Attendee>(entity =>
        {
            entity.HasKey(e => e.AttendeeId).HasName("PK__Attendee__1844010811F545B0");

            entity.ToTable("Attendee");

            entity.HasIndex(e => new { e.FkUserId, e.FkMeetingId }, "UQ_UserMeeting").IsUnique();

            entity.Property(e => e.AttendeeStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Invited");
            entity.Property(e => e.FkMeetingId).HasColumnName("FK_MeetingId");
            entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");

            entity.HasOne(d => d.FkMeeting).WithMany(p => p.Attendees)
                .HasForeignKey(d => d.FkMeetingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attendee__FK_Mee__73BA3083");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Attendees)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attendee__FK_Use__72C60C4A");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__Feature__82230BC9BF3A9108");

            entity.ToTable("Feature");

            entity.HasIndex(e => e.FeatureName, "UQ__Feature__55ABBB7113EB1BDE").IsUnique();

            entity.Property(e => e.FeatureId).ValueGeneratedOnAdd();
            entity.Property(e => e.FeatureDescription)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("No description provided");
            entity.Property(e => e.FeatureName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.HasKey(e => e.MeetingId).HasName("PK__Meeting__E9F9E94C34FD7662");

            entity.ToTable("Meeting");

            entity.Property(e => e.FkRoomId).HasColumnName("FK_RoomId");
            entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");
            entity.Property(e => e.MeetingAgenda).IsUnicode(false);
            entity.Property(e => e.MeetingEndTime).HasColumnType("datetime");
            entity.Property(e => e.MeetingStartTime).HasColumnType("datetime");
            entity.Property(e => e.MeetingStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Scheduled");
            entity.Property(e => e.MeetingTitle)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.FkRoom).WithMany(p => p.Meetings)
                .HasForeignKey(d => d.FkRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meeting__FK_Room__6C190EBB");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Meetings)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meeting__FK_User__6B24EA82");
        });

        modelBuilder.Entity<MeetingMinute>(entity =>
        {
            entity.HasKey(e => e.MeetingMinutesId).HasName("PK__MeetingM__58772D4754871672");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FkMeetingId).HasColumnName("FK_MeetingId");
            entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.MeetingSummary).IsUnicode(false);

            entity.HasOne(d => d.FkMeeting).WithMany(p => p.MeetingMinutes)
                .HasForeignKey(d => d.FkMeetingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MeetingMi__FK_Me__787EE5A0");

            entity.HasOne(d => d.FkUser).WithMany(p => p.MeetingMinutes)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MeetingMi__FK_Us__797309D9");
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("Role");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id).ValueGeneratedOnAdd();

            entity.Property(r => r.Name)
                  .HasColumnName("RoleName")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(r => r.NormalizedName)
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.HasIndex(r => r.NormalizedName).IsUnique();

            entity.Property(r => r.RoleDescription)
                  .HasMaxLength(255)
                  .IsUnicode(false)
                  .HasDefaultValue("No description provided.");

            entity.Property(r => r.ConcurrencyStamp)
                  .IsConcurrencyToken()
                  .IsUnicode(false);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__328639390ACAD311");

            entity.ToTable("Room");

            entity.HasIndex(e => e.RoomName, "UQ__Room__6B500B55D76ABCBD").IsUnique();

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.RoomLocation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RoomFeature>(entity =>
        {
            entity.HasKey(e => e.RoomFeatureId).HasName("PK__RoomFeat__E554235C70F7C736");

            entity.ToTable("RoomFeature");

            entity.Property(e => e.FkFeatureId).HasColumnName("FK_FeatureId");
            entity.Property(e => e.FkRoomId).HasColumnName("FK_RoomId");

            entity.HasOne(d => d.FkFeature).WithMany(p => p.RoomFeatures)
                .HasForeignKey(d => d.FkFeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomFeatu__FK_Fe__6754599E");

            entity.HasOne(d => d.FkRoom).WithMany(p => p.RoomFeatures)
                .HasForeignKey(d => d.FkRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomFeatu__FK_Ro__66603565");
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).ValueGeneratedOnAdd();

            entity.Property(u => u.UserName)
                  .HasMaxLength(256)
                  .IsUnicode(false);

            entity.Property(u => u.NormalizedUserName)
                  .HasMaxLength(256)
                  .IsUnicode(false);

            entity.Property(u => u.Email)
                  .HasMaxLength(255)
                  .IsUnicode(false);

            entity.Property(u => u.NormalizedEmail)
                  .HasMaxLength(255)
                  .IsUnicode(false);

            entity.Property(u => u.PasswordHash)
                  .IsUnicode(false);

            entity.Property(u => u.SecurityStamp)
                  .IsUnicode(false);

            entity.Property(u => u.ConcurrencyStamp)
                  .IsConcurrencyToken()
                  .IsUnicode(false);

            entity.Property(u => u.FirstName)
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .IsRequired();

            entity.Property(u => u.LastName)
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .IsRequired();

            entity.Property(u => u.IsOnline)
                  .HasDefaultValue(false);

            entity.Property(u => u.PhoneNumber)
                  .HasMaxLength(25)
                  .IsUnicode(false);

            entity.Property(u => u.FkRoleId)
                  .HasColumnName("FK_RoleId");

            entity.HasOne(u => u.FkRole)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.FkRoleId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_User_Role");

            entity.HasIndex(u => u.NormalizedUserName).IsUnique();
            entity.HasIndex(u => u.NormalizedEmail);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
