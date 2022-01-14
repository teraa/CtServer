﻿// <auto-generated />
using System;
using CtServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CtServer.Data.Migrations
{
    [DbContext(typeof(CtDbContext))]
    [Migration("20220114153206_UniqueAttachmentFilePath")]
    partial class UniqueAttachmentFilePath
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CtServer.Data.Models.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_path");

                    b.Property<int>("PresentationId")
                        .HasColumnType("integer")
                        .HasColumnName("presentation_id");

                    b.HasKey("Id")
                        .HasName("pk_attachments");

                    b.HasIndex("FilePath")
                        .IsUnique()
                        .HasDatabaseName("ix_attachments_file_path");

                    b.HasIndex("PresentationId")
                        .IsUnique()
                        .HasDatabaseName("ix_attachments_presentation_id");

                    b.ToTable("attachments", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTimeOffset>("EndAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_at");

                    b.Property<DateTimeOffset>("StartAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_at");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_events");

                    b.ToTable("events", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EventId")
                        .HasColumnType("integer")
                        .HasColumnName("event_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_locations");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_locations_event_id");

                    b.ToTable("locations", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("data");

                    b.Property<int>("EventId")
                        .HasColumnType("integer")
                        .HasColumnName("event_id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_notifications");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_notifications_event_id");

                    b.ToTable("notifications", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.Presentation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AttachmentId")
                        .HasColumnType("integer")
                        .HasColumnName("attachment_id");

                    b.Property<string[]>("Authors")
                        .IsRequired()
                        .HasColumnType("text[]")
                        .HasColumnName("authors");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("duration");

                    b.Property<string>("MainAuthorPhoto")
                        .HasColumnType("text")
                        .HasColumnName("main_author_photo");

                    b.Property<int>("Position")
                        .HasColumnType("integer")
                        .HasColumnName("position");

                    b.Property<int>("SectionId")
                        .HasColumnType("integer")
                        .HasColumnName("section_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_presentations");

                    b.HasIndex("SectionId")
                        .HasDatabaseName("ix_presentations_section_id");

                    b.ToTable("presentations", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BackgroundColor")
                        .HasColumnType("integer")
                        .HasColumnName("background_color");

                    b.Property<string[]>("Chairs")
                        .IsRequired()
                        .HasColumnType("text[]")
                        .HasColumnName("chairs");

                    b.Property<DateTimeOffset>("EndAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_at");

                    b.Property<int>("EventId")
                        .HasColumnType("integer")
                        .HasColumnName("event_id");

                    b.Property<int>("LocationId")
                        .HasColumnType("integer")
                        .HasColumnName("location_id");

                    b.Property<DateTimeOffset>("StartAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_at");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_sections");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_sections_event_id");

                    b.HasIndex("LocationId")
                        .HasDatabaseName("ix_sections_location_id");

                    b.ToTable("sections", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Auth")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("auth");

                    b.Property<string>("Endpoint")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("endpoint");

                    b.Property<string>("P256dh")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("p256dh");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_subscriptions");

                    b.HasIndex("Endpoint")
                        .IsUnique()
                        .HasDatabaseName("ix_subscriptions_endpoint");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_subscriptions_user_id");

                    b.ToTable("subscriptions", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean")
                        .HasColumnName("is_admin");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("password_hash");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("password_salt");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.UserEvent", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("EventId")
                        .HasColumnType("integer")
                        .HasColumnName("event_id");

                    b.HasKey("UserId", "EventId")
                        .HasName("pk_user_events");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_user_events_event_id");

                    b.ToTable("user_events", (string)null);
                });

            modelBuilder.Entity("CtServer.Data.Models.Attachment", b =>
                {
                    b.HasOne("CtServer.Data.Models.Presentation", "Presentation")
                        .WithOne("Attachment")
                        .HasForeignKey("CtServer.Data.Models.Attachment", "PresentationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_attachments_presentations_presentation_id1");

                    b.Navigation("Presentation");
                });

            modelBuilder.Entity("CtServer.Data.Models.Location", b =>
                {
                    b.HasOne("CtServer.Data.Models.Event", "Event")
                        .WithMany("Locations")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_locations_events_event_id");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("CtServer.Data.Models.Notification", b =>
                {
                    b.HasOne("CtServer.Data.Models.Event", "Event")
                        .WithMany("Notifications")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_notifications_events_event_id");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("CtServer.Data.Models.Presentation", b =>
                {
                    b.HasOne("CtServer.Data.Models.Section", "Section")
                        .WithMany("Presentations")
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_presentations_sections_section_id");

                    b.Navigation("Section");
                });

            modelBuilder.Entity("CtServer.Data.Models.Section", b =>
                {
                    b.HasOne("CtServer.Data.Models.Event", "Event")
                        .WithMany("Sections")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_sections_events_event_id");

                    b.HasOne("CtServer.Data.Models.Location", "Location")
                        .WithMany("Sections")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_sections_locations_location_id");

                    b.Navigation("Event");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("CtServer.Data.Models.Subscription", b =>
                {
                    b.HasOne("CtServer.Data.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_subscriptions_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CtServer.Data.Models.UserEvent", b =>
                {
                    b.HasOne("CtServer.Data.Models.Event", "Event")
                        .WithMany("UserEvents")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_events_events_event_id");

                    b.HasOne("CtServer.Data.Models.User", "User")
                        .WithMany("UserEvents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_events_users_user_id");

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CtServer.Data.Models.Event", b =>
                {
                    b.Navigation("Locations");

                    b.Navigation("Notifications");

                    b.Navigation("Sections");

                    b.Navigation("UserEvents");
                });

            modelBuilder.Entity("CtServer.Data.Models.Location", b =>
                {
                    b.Navigation("Sections");
                });

            modelBuilder.Entity("CtServer.Data.Models.Presentation", b =>
                {
                    b.Navigation("Attachment");
                });

            modelBuilder.Entity("CtServer.Data.Models.Section", b =>
                {
                    b.Navigation("Presentations");
                });

            modelBuilder.Entity("CtServer.Data.Models.User", b =>
                {
                    b.Navigation("Subscriptions");

                    b.Navigation("UserEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
