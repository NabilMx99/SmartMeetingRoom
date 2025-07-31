using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Data;

public partial class SmartMeetingRoomDBContext : DbContext
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

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomFeature> RoomFeatures { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=SmartMeetingRoomDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A20381A9B");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B61607947FE60").IsUnique();

            entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
            entity.Property(e => e.RoleDescription)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("No description provided");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C75F23F88");

            entity.ToTable("User");

            entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E38DD54C733").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D105346F70340B").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FkRoleId).HasColumnName("FK_RoleId");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.HasOne(d => d.FkRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__FK_RoleId__5165187F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
