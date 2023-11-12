using MysticLegendsShared.Models;
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MysticLegendsServer.Models;

public partial class Xdigf001Context : DbContext
{
    public Xdigf001Context(DbContextOptions<Xdigf001Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AcceptedQuest> AcceptedQuests { get; set; }

    public virtual DbSet<AccessToken> AccessTokens { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<BattleStat> BattleStats { get; set; }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<CharacterInventory> CharacterInventories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<CityInventory> CityInventories { get; set; }

    public virtual DbSet<InventoryItem> InventoryItems { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Mob> Mobs { get; set; }

    public virtual DbSet<MobItemDrop> MobItemDrops { get; set; }

    public virtual DbSet<Npc> Npcs { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Quest> Quests { get; set; }

    public virtual DbSet<QuestRequirement> QuestRequirements { get; set; }

    public virtual DbSet<QuestReward> QuestRewards { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<TradeMarket> TradeMarkets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcceptedQuest>(entity =>
        {
            entity.HasKey(e => new { e.CharacterName, e.QuestId }).HasName("pk_accepted_quest");

            entity.ToTable("accepted_quest");

            entity.Property(e => e.CharacterName)
                .HasMaxLength(32)
                .HasColumnName("character_name");
            entity.Property(e => e.QuestId).HasColumnName("quest_id");
            entity.Property(e => e.QuestState).HasColumnName("quest_state");

            entity.HasOne(d => d.CharacterNameNavigation).WithMany(p => p.AcceptedQuests)
                .HasForeignKey(d => d.CharacterName)
                .HasConstraintName("fk_accepted_quest_character");

            entity.HasOne(d => d.Quest).WithMany(p => p.AcceptedQuests)
                .HasForeignKey(d => d.QuestId)
                .HasConstraintName("fk_accepted_quest_quest");
        });

        modelBuilder.Entity<AccessToken>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("pk_access_token");

            entity.ToTable("access_token");

            entity.HasIndex(e => e.AccessToken1, "uc_access_token_access_token").IsUnique();

            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasColumnName("username");
            entity.Property(e => e.AccessToken1)
                .HasMaxLength(256)
                .HasColumnName("access_token");
            entity.Property(e => e.Expiration)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expiration");

            entity.HasOne(d => d.UsernameNavigation).WithOne(p => p.AccessToken)
                .HasForeignKey<AccessToken>(d => d.Username)
                .HasConstraintName("fk_access_token_users");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaName).HasName("pk_area");

            entity.ToTable("area");

            entity.Property(e => e.AreaName)
                .HasMaxLength(32)
                .HasColumnName("area_name");
        });

        modelBuilder.Entity<BattleStat>(entity =>
        {
            entity.HasKey(e => new { e.StatType, e.Method, e.InvitemId }).HasName("pk_battle_stats");

            entity.ToTable("battle_stats");

            entity.Property(e => e.StatType).HasColumnName("stat_type");
            entity.Property(e => e.Method).HasColumnName("method");
            entity.Property(e => e.InvitemId).HasColumnName("invitem_id");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Invitem).WithMany(p => p.BattleStats)
                .HasForeignKey(d => d.InvitemId)
                .HasConstraintName("fk_battle_stats_inventory_item");
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.CharacterName).HasName("pk_character");

            entity.ToTable("character");

            entity.Property(e => e.CharacterName)
                .HasMaxLength(32)
                .HasColumnName("character_name");
            entity.Property(e => e.CharacterClass).HasColumnName("character_class");
            entity.Property(e => e.CityName)
                .HasMaxLength(32)
                .HasColumnName("city_name");
            entity.Property(e => e.CurrencyGold).HasColumnName("currency_gold");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasColumnName("username");

            entity.HasOne(d => d.CityNameNavigation).WithMany(p => p.Characters)
                .HasForeignKey(d => d.CityName)
                .HasConstraintName("fk_character_city");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Characters)
                .HasForeignKey(d => d.Username)
                .HasConstraintName("fk_character_users");
        });

        modelBuilder.Entity<CharacterInventory>(entity =>
        {
            entity.HasKey(e => e.CharacterName).HasName("pk_character_inventory");

            entity.ToTable("character_inventory");

            entity.Property(e => e.CharacterName)
                .HasMaxLength(32)
                .HasColumnName("character_name");
            entity.Property(e => e.Capacity).HasColumnName("capacity");

            entity.HasOne(d => d.CharacterNameNavigation).WithOne(p => p.CharacterInventory)
                .HasForeignKey<CharacterInventory>(d => d.CharacterName)
                .HasConstraintName("fk_character_inventory_characte");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityName).HasName("pk_city");

            entity.ToTable("city");

            entity.Property(e => e.CityName)
                .HasMaxLength(32)
                .HasColumnName("city_name");
        });

        modelBuilder.Entity<CityInventory>(entity =>
        {
            entity.HasKey(e => new { e.CityName, e.CharacterName }).HasName("pk_city_inventory");

            entity.ToTable("city_inventory");

            entity.Property(e => e.CityName)
                .HasMaxLength(32)
                .HasColumnName("city_name");
            entity.Property(e => e.CharacterName)
                .HasMaxLength(32)
                .HasColumnName("character_name");
            entity.Property(e => e.Capacity).HasColumnName("capacity");

            entity.HasOne(d => d.CharacterNameNavigation).WithMany(p => p.CityInventories)
                .HasForeignKey(d => d.CharacterName)
                .HasConstraintName("fk_city_inventory_character");

            entity.HasOne(d => d.CityNameNavigation).WithMany(p => p.CityInventories)
                .HasForeignKey(d => d.CityName)
                .HasConstraintName("fk_city_inventory_city");
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.InvitemId).HasName("pk_inventory_item");

            entity.ToTable("inventory_item");

            entity.Property(e => e.InvitemId).HasColumnName("invitem_id");
            entity.Property(e => e.CharacterInventoryCharacterN)
                .HasMaxLength(32)
                .HasColumnName("character_inventory_character_n");
            entity.Property(e => e.CharacterName)
                .HasMaxLength(32)
                .HasColumnName("character_name");
            entity.Property(e => e.CityInventoryCharacterName)
                .HasMaxLength(32)
                .HasColumnName("city_inventory_character_name");
            entity.Property(e => e.CityName)
                .HasMaxLength(32)
                .HasColumnName("city_name");
            entity.Property(e => e.Durability).HasColumnName("durability");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.NpcId).HasColumnName("npc_id");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.StackCount).HasColumnName("stack_count");

            entity.HasOne(d => d.CharacterInventoryCharacterNNavigation).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.CharacterInventoryCharacterN)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_inventory_item_character_inv");

            entity.HasOne(d => d.CharacterNameNavigation).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.CharacterName)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_inventory_item_character");

            entity.HasOne(d => d.Item).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("fk_inventory_item_item");

            entity.HasOne(d => d.Npc).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.NpcId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_inventory_item_npc");

            entity.HasOne(d => d.City).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => new { d.CityName, d.CityInventoryCharacterName })
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_inventory_item_city_inventor");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("pk_item");

            entity.ToTable("item");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Icon)
                .HasMaxLength(64)
                .HasColumnName("icon");
            entity.Property(e => e.ItemType).HasColumnName("item_type");
            entity.Property(e => e.MaxDurability).HasColumnName("max_durability");
            entity.Property(e => e.MaxStack).HasColumnName("max_stack");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Mob>(entity =>
        {
            entity.HasKey(e => e.MobId).HasName("pk_mob");

            entity.ToTable("mob");

            entity.Property(e => e.MobId).HasColumnName("mob_id");
            entity.Property(e => e.AreaName)
                .HasMaxLength(32)
                .HasColumnName("area_name");
            entity.Property(e => e.GroupSize).HasColumnName("group_size");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.MobName)
                .HasMaxLength(32)
                .HasColumnName("mob_name");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.AreaNameNavigation).WithMany(p => p.Mobs)
                .HasForeignKey(d => d.AreaName)
                .HasConstraintName("fk_mob_area");
        });

        modelBuilder.Entity<MobItemDrop>(entity =>
        {
            entity.HasKey(e => new { e.ItemId, e.MobId }).HasName("pk_mob_item_drop");

            entity.ToTable("mob_item_drop");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.MobId).HasColumnName("mob_id");
            entity.Property(e => e.DropRate).HasColumnName("drop_rate");

            entity.HasOne(d => d.Item).WithMany(p => p.MobItemDrops)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("fk_mob_item_drop_item");

            entity.HasOne(d => d.Mob).WithMany(p => p.MobItemDrops)
                .HasForeignKey(d => d.MobId)
                .HasConstraintName("fk_mob_item_drop_mob");
        });

        modelBuilder.Entity<Npc>(entity =>
        {
            entity.HasKey(e => e.NpcId).HasName("pk_npc");

            entity.ToTable("npc");

            entity.Property(e => e.NpcId).HasColumnName("npc_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(32)
                .HasColumnName("city_name");
            entity.Property(e => e.CurrencyGold).HasColumnName("currency_gold");
            entity.Property(e => e.NpcType).HasColumnName("npc_type");

            entity.HasOne(d => d.CityNameNavigation).WithMany(p => p.Npcs)
                .HasForeignKey(d => d.CityName)
                .HasConstraintName("fk_npc_city");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.InvitemId).HasName("pk_price");

            entity.ToTable("price");

            entity.Property(e => e.InvitemId)
                .ValueGeneratedNever()
                .HasColumnName("invitem_id");
            entity.Property(e => e.BidGold).HasColumnName("bid_gold");
            entity.Property(e => e.PriceGold).HasColumnName("price_gold");
            entity.Property(e => e.QuantityPerPurchase).HasColumnName("quantity_per_purchase");

            entity.HasOne(d => d.Invitem).WithOne(p => p.Price)
                .HasForeignKey<Price>(d => d.InvitemId)
                .HasConstraintName("fk_price_inventory_item");
        });

        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.QuestId).HasName("pk_quest");

            entity.ToTable("quest");

            entity.Property(e => e.QuestId).HasColumnName("quest_id");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("description");
            entity.Property(e => e.IsOffered).HasColumnName("is_offered");
            entity.Property(e => e.IsRepeable).HasColumnName("is_repeable");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NpcId).HasColumnName("npc_id");

            entity.HasOne(d => d.Npc).WithMany(p => p.Quests)
                .HasForeignKey(d => d.NpcId)
                .HasConstraintName("fk_quest_npc");
        });

        modelBuilder.Entity<QuestRequirement>(entity =>
        {
            entity.HasKey(e => e.QuestId).HasName("pk_quest_requirement");

            entity.ToTable("quest_requirement");

            entity.Property(e => e.QuestId)
                .ValueGeneratedNever()
                .HasColumnName("quest_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.MobType).HasColumnName("mob_type");

            entity.HasOne(d => d.Item).WithMany(p => p.QuestRequirements)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quest_requirement_item");

            entity.HasOne(d => d.Quest).WithOne(p => p.QuestRequirement)
                .HasForeignKey<QuestRequirement>(d => d.QuestId)
                .HasConstraintName("fk_quest_requirement_quest");
        });

        modelBuilder.Entity<QuestReward>(entity =>
        {
            entity.HasKey(e => e.QuestId).HasName("pk_quest_reward");

            entity.ToTable("quest_reward");

            entity.Property(e => e.QuestId)
                .ValueGeneratedNever()
                .HasColumnName("quest_id");
            entity.Property(e => e.CurrencyGold).HasColumnName("currency_gold");
            entity.Property(e => e.ItemCount).HasColumnName("item_count");
            entity.Property(e => e.ItemId).HasColumnName("item_id");

            entity.HasOne(d => d.Item).WithMany(p => p.QuestRewards)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quest_reward_item");

            entity.HasOne(d => d.Quest).WithOne(p => p.QuestReward)
                .HasForeignKey<QuestReward>(d => d.QuestId)
                .HasConstraintName("fk_quest_reward_quest");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("pk_refresh_token");

            entity.ToTable("refresh_token");

            entity.HasIndex(e => e.RefreshToken1, "uc_refresh_token_refresh_token").IsUnique();

            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.Expiration)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expiration");
            entity.Property(e => e.RefreshToken1)
                .HasMaxLength(256)
                .HasColumnName("refresh_token");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasColumnName("username");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.Username)
                .HasConstraintName("fk_refresh_token_users");
        });

        modelBuilder.Entity<TradeMarket>(entity =>
        {
            entity.HasKey(e => e.InvitemId).HasName("pk_trade_market");

            entity.ToTable("trade_market");

            entity.Property(e => e.InvitemId)
                .ValueGeneratedNever()
                .HasColumnName("invitem_id");
            entity.Property(e => e.BiddingEnds)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("bidding_ends");
            entity.Property(e => e.ListedSince)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("listed_since");

            entity.HasOne(d => d.Invitem).WithOne(p => p.TradeMarket)
                .HasForeignKey<TradeMarket>(d => d.InvitemId)
                .HasConstraintName("fk_trade_market_inventory_item");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("pk_users");

            entity.ToTable("users");

            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasColumnName("username");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(32)
                .HasColumnName("password_hash");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
