﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebAPI.Model;

#nullable disable

namespace REST.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Properties.ActivityOccurenceProperty", b =>
                {
                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.Property<int>("ActivityOccuranceId")
                        .HasColumnType("int");

                    b.Property<int>("DiscussionOrPollId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OcurredAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TeamId", "ActivityOccuranceId");

                    b.ToTable("Activity_Occurrence");
                });

            modelBuilder.Entity("Properties.CustomDiscussionProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.Property<string>("TopicText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Custom_Discussion");
                });

            modelBuilder.Entity("Properties.CustomPollProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PollOptions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Custom_Poll");
                });

            modelBuilder.Entity("Properties.PollVoteProperty", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(0);

                    b.Property<int>("ActivityOccuranceId")
                        .HasMaxLength(50)
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("VoteOptionNumber")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ActivityOccuranceId");

                    b.ToTable("Poll_Vote");
                });

            modelBuilder.Entity("Properties.SocioliteTeamProperty", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TeamId"));

                    b.Property<string>("MSTeamsChannelId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MSTeamsTeamId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Recurring")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.HasKey("TeamId");

                    b.ToTable("Sociolite_Team");
                });

            modelBuilder.Entity("Properties.Team.SocioliteTeamMembershipProperty", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(0);

                    b.Property<int>("TeamId")
                        .HasMaxLength(50)
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<string>("TeamSpecificRole")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId", "TeamId");

                    b.ToTable("Sociolite_Team_Membership");
                });

            modelBuilder.Entity("Properties.UserProperty", b =>
                {
                    b.Property<string>("MSTeamsId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MSTeamsId");

                    b.ToTable("Sociolite_User");
                });
#pragma warning restore 612, 618
        }
    }
}
