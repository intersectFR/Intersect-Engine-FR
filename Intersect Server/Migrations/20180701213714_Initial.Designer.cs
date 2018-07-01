﻿// <auto-generated />
using Intersect.Server.Classes.Database.PlayerData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Intersect.Server.Migrations
{
    [DbContext(typeof(PlayerContext))]
    [Migration("20180701213714_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Ban", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("Ip");

                    b.Property<Guid>("PlayerId");

                    b.Property<string>("Reason");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Bag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SlotCount");

                    b.HasKey("Id");

                    b.ToTable("Bags");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.BagSlot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BagId");

                    b.Property<Guid>("ParentBagId");

                    b.Property<int>("Quantity");

                    b.Property<int>("Slot");

                    b.Property<string>("StatBoostJson")
                        .HasColumnName("StatBoost");

                    b.HasKey("Id");

                    b.HasIndex("BagId");

                    b.HasIndex("ParentBagId");

                    b.ToTable("Bag_Items");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.BankSlot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BagId");

                    b.Property<Guid>("CharacterId");

                    b.Property<int>("Quantity");

                    b.Property<int>("Slot");

                    b.Property<string>("StatBoostJson")
                        .HasColumnName("StatBoost");

                    b.HasKey("Id");

                    b.HasIndex("BagId");

                    b.HasIndex("CharacterId");

                    b.ToTable("Character_Bank");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Friend", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("OwnerId");

                    b.Property<Guid?>("TargetId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("TargetId");

                    b.ToTable("Character_Friends");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.HotbarSlot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CharacterId");

                    b.Property<int>("ItemSlot");

                    b.Property<int>("Slot");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("Character_Hotbar");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.InventorySlot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BagId");

                    b.Property<Guid>("CharacterId");

                    b.Property<int>("Quantity");

                    b.Property<int>("Slot");

                    b.Property<string>("StatBoostJson")
                        .HasColumnName("StatBoost");

                    b.HasKey("Id");

                    b.HasIndex("BagId");

                    b.HasIndex("CharacterId");

                    b.ToTable("Character_Items");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Quest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CharacterId");

                    b.Property<int>("Completed");

                    b.Property<Guid>("QuestId");

                    b.Property<int>("TaskId");

                    b.Property<int>("TaskProgress");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("QuestId", "CharacterId")
                        .IsUnique();

                    b.ToTable("Character_Quests");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.SpellSlot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CharacterId");

                    b.Property<int>("Slot");

                    b.Property<long>("SpellCd");

                    b.Property<Guid>("SpellId");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("Character_Spells");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Switch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CharacterId");

                    b.Property<int>("SwitchId");

                    b.Property<bool>("Value");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("SwitchId", "CharacterId")
                        .IsUnique();

                    b.ToTable("Character_Switches");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Variable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CharacterId");

                    b.Property<int>("Value");

                    b.Property<int>("VariableId");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("VariableId", "CharacterId")
                        .IsUnique();

                    b.ToTable("Character_Variables");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Mute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("Ip");

                    b.Property<string>("Muter");

                    b.Property<Guid>("PlayerId");

                    b.Property<string>("Reason");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Mutes");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Access");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("PasswordResetCode");

                    b.Property<DateTime?>("PasswordResetTime");

                    b.Property<string>("Salt");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Entities.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClassId");

                    b.Property<int>("Dir");

                    b.Property<string>("EquipmentJson")
                        .HasColumnName("Equipment");

                    b.Property<long>("Exp");

                    b.Property<string>("Face");

                    b.Property<int>("Gender");

                    b.Property<DateTime?>("LastOnline");

                    b.Property<int>("Level");

                    b.Property<Guid>("MapId");

                    b.Property<string>("MaxVitalsJson")
                        .HasColumnName("MaxVitals");

                    b.Property<string>("Name");

                    b.Property<string>("Sprite");

                    b.Property<int>("StatPoints");

                    b.Property<string>("StatsJson")
                        .HasColumnName("Stats");

                    b.Property<string>("VitalsJson")
                        .HasColumnName("Vitals");

                    b.Property<int>("X");

                    b.Property<int>("Y");

                    b.Property<int>("Z");

                    b.HasKey("Id");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Ban", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Database.PlayerData.User", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.BagSlot", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Database.PlayerData.Characters.Bag", "Bag")
                        .WithMany()
                        .HasForeignKey("BagId");

                    b.HasOne("Intersect.Server.Classes.Database.PlayerData.Characters.Bag", "ParentBag")
                        .WithMany("Slots")
                        .HasForeignKey("ParentBagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.BankSlot", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Database.PlayerData.Characters.Bag", "Bag")
                        .WithMany()
                        .HasForeignKey("BagId");

                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Character")
                        .WithMany("Bank")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Friend", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Owner")
                        .WithMany("Friends")
                        .HasForeignKey("OwnerId");

                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Target")
                        .WithMany()
                        .HasForeignKey("TargetId");
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.HotbarSlot", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Character")
                        .WithMany("Hotbar")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.InventorySlot", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Database.PlayerData.Characters.Bag", "Bag")
                        .WithMany()
                        .HasForeignKey("BagId");

                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Character")
                        .WithMany("Items")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Quest", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Character")
                        .WithMany("Quests")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.SpellSlot", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Character")
                        .WithMany("Spells")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Switch", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Character")
                        .WithMany("Switches")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Characters.Variable", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Entities.Player", "Character")
                        .WithMany("Variables")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Database.PlayerData.Mute", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Database.PlayerData.User", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Intersect.Server.Classes.Entities.Player", b =>
                {
                    b.HasOne("Intersect.Server.Classes.Database.PlayerData.User", "Account")
                        .WithMany("Characters")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
