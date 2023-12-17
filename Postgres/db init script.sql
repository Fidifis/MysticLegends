-- Remove conflicting tables
DROP TABLE IF EXISTS accepted_quest CASCADE;
DROP TABLE IF EXISTS access_token CASCADE;
DROP TABLE IF EXISTS area CASCADE;
DROP TABLE IF EXISTS battle_stats CASCADE;
DROP TABLE IF EXISTS city CASCADE;
DROP TABLE IF EXISTS city_inventory CASCADE;
DROP TABLE IF EXISTS character CASCADE;
DROP TABLE IF EXISTS character_inventory CASCADE;
DROP TABLE IF EXISTS inventory_item CASCADE;
DROP TABLE IF EXISTS item CASCADE;
DROP TABLE IF EXISTS mob CASCADE;
DROP TABLE IF EXISTS mob_item_drop CASCADE;
DROP TABLE IF EXISTS npc CASCADE;
DROP TABLE IF EXISTS price CASCADE;
DROP TABLE IF EXISTS quest CASCADE;
DROP TABLE IF EXISTS quest_requirement CASCADE;
DROP TABLE IF EXISTS quest_reward CASCADE;
DROP TABLE IF EXISTS refresh_token CASCADE;
DROP TABLE IF EXISTS trade_market CASCADE;
DROP TABLE IF EXISTS travel CASCADE;
DROP TABLE IF EXISTS users CASCADE;
-- End of removing

CREATE TABLE accepted_quest (
    character_name VARCHAR(32) NOT NULL,
    quest_id INTEGER NOT NULL,
    quest_state INTEGER NOT NULL
);
ALTER TABLE accepted_quest ADD CONSTRAINT pk_accepted_quest PRIMARY KEY (character_name, quest_id);

CREATE TABLE access_token (
    username VARCHAR(32) NOT NULL,
    access_token VARCHAR(256) NOT NULL,
    expiration TIMESTAMP NOT NULL
);
ALTER TABLE access_token ADD CONSTRAINT pk_access_token PRIMARY KEY (username);
ALTER TABLE access_token ADD CONSTRAINT uc_access_token_access_token UNIQUE (access_token);

CREATE TABLE area (
    area_name VARCHAR(32) NOT NULL
);
ALTER TABLE area ADD CONSTRAINT pk_area PRIMARY KEY (area_name);

CREATE TABLE battle_stats (
    stat_id SERIAL NOT NULL,
    invitem_id INTEGER,
    mob_id INTEGER,
    stat_type INTEGER NOT NULL,
    method INTEGER NOT NULL,
    value DOUBLE PRECISION NOT NULL
);
ALTER TABLE battle_stats ADD CONSTRAINT pk_battle_stats PRIMARY KEY (stat_id);

CREATE TABLE city (
    city_name VARCHAR(32) NOT NULL
);
ALTER TABLE city ADD CONSTRAINT pk_city PRIMARY KEY (city_name);

CREATE TABLE city_inventory (
    city_name VARCHAR(32) NOT NULL,
    character_name VARCHAR(32) NOT NULL,
    capacity INTEGER NOT NULL
);
ALTER TABLE city_inventory ADD CONSTRAINT pk_city_inventory PRIMARY KEY (city_name, character_name);

CREATE TABLE character (
    character_name VARCHAR(32) NOT NULL,
    city_name VARCHAR(32) NOT NULL,
    username VARCHAR(32) NOT NULL,
    character_class INTEGER NOT NULL,
    level INTEGER NOT NULL,
    currency_gold INTEGER NOT NULL,
    xp INTEGER NOT NULL
);
ALTER TABLE character ADD CONSTRAINT pk_character PRIMARY KEY (character_name);

CREATE TABLE character_inventory (
    character_name VARCHAR(32) NOT NULL,
    capacity INTEGER NOT NULL
);
ALTER TABLE character_inventory ADD CONSTRAINT pk_character_inventory PRIMARY KEY (character_name);

CREATE TABLE inventory_item (
    invitem_id SERIAL NOT NULL,
    city_name VARCHAR(32),
    city_inventory_character_name VARCHAR(32),
    character_name VARCHAR(32),
    character_inventory_character_n VARCHAR(32),
    item_id INTEGER NOT NULL,
    npc_id INTEGER,
    stack_count INTEGER NOT NULL,
    position INTEGER NOT NULL,
    level INTEGER,
    durability INTEGER
);
ALTER TABLE inventory_item ADD CONSTRAINT pk_inventory_item PRIMARY KEY (invitem_id);
ALTER TABLE inventory_item ADD CONSTRAINT c_fk_inventory_item_city_invent CHECK ((city_name IS NOT NULL AND city_inventory_character_name IS NOT NULL) OR (city_name IS NULL AND city_inventory_character_name IS NULL));

CREATE TABLE item (
    item_id SERIAL NOT NULL,
    name VARCHAR(64) NOT NULL,
    icon VARCHAR(64) NOT NULL,
    item_type INTEGER NOT NULL,
    max_stack INTEGER NOT NULL,
    max_durability INTEGER
);
ALTER TABLE item ADD CONSTRAINT pk_item PRIMARY KEY (item_id);

CREATE TABLE mob (
    mob_id SERIAL NOT NULL,
    area_name VARCHAR(32) NOT NULL,
    mob_name VARCHAR(32) NOT NULL,
    type INTEGER NOT NULL,
    level INTEGER NOT NULL,
    group_size INTEGER NOT NULL
);
ALTER TABLE mob ADD CONSTRAINT pk_mob PRIMARY KEY (mob_id);

CREATE TABLE mob_item_drop (
    item_id INTEGER NOT NULL,
    mob_id INTEGER NOT NULL,
    drop_rate DOUBLE PRECISION NOT NULL,
    max_amount INTEGER NOT NULL
);
ALTER TABLE mob_item_drop ADD CONSTRAINT pk_mob_item_drop PRIMARY KEY (item_id, mob_id);

CREATE TABLE npc (
    npc_id SERIAL NOT NULL,
    city_name VARCHAR(32) NOT NULL,
    npc_type INTEGER NOT NULL,
    currency_gold INTEGER
);
ALTER TABLE npc ADD CONSTRAINT pk_npc PRIMARY KEY (npc_id);

CREATE TABLE price (
    invitem_id INTEGER NOT NULL,
    price_gold INTEGER NOT NULL,
    bid_gold INTEGER,
    quantity_per_purchase INTEGER
);
ALTER TABLE price ADD CONSTRAINT pk_price PRIMARY KEY (invitem_id);

CREATE TABLE quest (
    quest_id SERIAL NOT NULL,
    npc_id INTEGER NOT NULL,
    name VARCHAR(64) NOT NULL,
    description VARCHAR(1024) NOT NULL,
    is_repeable BOOLEAN NOT NULL,
    is_offered BOOLEAN NOT NULL,
    level INTEGER NOT NULL
);
ALTER TABLE quest ADD CONSTRAINT pk_quest PRIMARY KEY (quest_id);

CREATE TABLE quest_requirement (
    quest_id INTEGER NOT NULL,
    item_id INTEGER NOT NULL,
    amount INTEGER NOT NULL
);
ALTER TABLE quest_requirement ADD CONSTRAINT pk_quest_requirement PRIMARY KEY (quest_id, item_id);

CREATE TABLE quest_reward (
    quest_id INTEGER NOT NULL,
    currency_gold INTEGER NOT NULL,
    xp INTEGER NOT NULL
);
ALTER TABLE quest_reward ADD CONSTRAINT pk_quest_reward PRIMARY KEY (quest_id);

CREATE TABLE refresh_token (
    record_id SERIAL NOT NULL,
    username VARCHAR(32) NOT NULL,
    refresh_token VARCHAR(256) NOT NULL,
    expiration TIMESTAMP NOT NULL
);
ALTER TABLE refresh_token ADD CONSTRAINT pk_refresh_token PRIMARY KEY (record_id);
ALTER TABLE refresh_token ADD CONSTRAINT uc_refresh_token_refresh_token UNIQUE (refresh_token);

CREATE TABLE trade_market (
    invitem_id INTEGER NOT NULL,
    listed_since TIMESTAMP NOT NULL,
    bidding_ends TIMESTAMP NOT NULL
);
ALTER TABLE trade_market ADD CONSTRAINT pk_trade_market PRIMARY KEY (invitem_id);

CREATE TABLE travel (
    character_name VARCHAR(32) NOT NULL,
    area_name VARCHAR(32),
    arrival TIMESTAMP NOT NULL
);
ALTER TABLE travel ADD CONSTRAINT pk_travel PRIMARY KEY (character_name);

CREATE TABLE users (
    username VARCHAR(32) NOT NULL,
    password_hash VARCHAR(32) NOT NULL
);
ALTER TABLE users ADD CONSTRAINT pk_users PRIMARY KEY (username);

ALTER TABLE accepted_quest ADD CONSTRAINT fk_accepted_quest_character FOREIGN KEY (character_name) REFERENCES character (character_name) ON DELETE CASCADE;
ALTER TABLE accepted_quest ADD CONSTRAINT fk_accepted_quest_quest FOREIGN KEY (quest_id) REFERENCES quest (quest_id) ON DELETE CASCADE;

ALTER TABLE access_token ADD CONSTRAINT fk_access_token_users FOREIGN KEY (username) REFERENCES users (username) ON DELETE CASCADE;

ALTER TABLE battle_stats ADD CONSTRAINT fk_battle_stats_inventory_item FOREIGN KEY (invitem_id) REFERENCES inventory_item (invitem_id) ON DELETE CASCADE;
ALTER TABLE battle_stats ADD CONSTRAINT fk_battle_stats_mob FOREIGN KEY (mob_id) REFERENCES mob (mob_id) ON DELETE CASCADE;

ALTER TABLE city_inventory ADD CONSTRAINT fk_city_inventory_city FOREIGN KEY (city_name) REFERENCES city (city_name) ON DELETE CASCADE;
ALTER TABLE city_inventory ADD CONSTRAINT fk_city_inventory_character FOREIGN KEY (character_name) REFERENCES character (character_name) ON DELETE CASCADE;

ALTER TABLE character ADD CONSTRAINT fk_character_city FOREIGN KEY (city_name) REFERENCES city (city_name) ON DELETE CASCADE;
ALTER TABLE character ADD CONSTRAINT fk_character_users FOREIGN KEY (username) REFERENCES users (username) ON DELETE CASCADE;

ALTER TABLE character_inventory ADD CONSTRAINT fk_character_inventory_characte FOREIGN KEY (character_name) REFERENCES character (character_name) ON DELETE CASCADE;

ALTER TABLE inventory_item ADD CONSTRAINT fk_inventory_item_city_inventor FOREIGN KEY (city_name, city_inventory_character_name) REFERENCES city_inventory (city_name, character_name) ON DELETE CASCADE;
ALTER TABLE inventory_item ADD CONSTRAINT fk_inventory_item_character FOREIGN KEY (character_name) REFERENCES character (character_name) ON DELETE CASCADE;
ALTER TABLE inventory_item ADD CONSTRAINT fk_inventory_item_character_inv FOREIGN KEY (character_inventory_character_n) REFERENCES character_inventory (character_name) ON DELETE CASCADE;
ALTER TABLE inventory_item ADD CONSTRAINT fk_inventory_item_item FOREIGN KEY (item_id) REFERENCES item (item_id) ON DELETE CASCADE;
ALTER TABLE inventory_item ADD CONSTRAINT fk_inventory_item_npc FOREIGN KEY (npc_id) REFERENCES npc (npc_id) ON DELETE CASCADE;

ALTER TABLE mob ADD CONSTRAINT fk_mob_area FOREIGN KEY (area_name) REFERENCES area (area_name) ON DELETE CASCADE;

ALTER TABLE mob_item_drop ADD CONSTRAINT fk_mob_item_drop_item FOREIGN KEY (item_id) REFERENCES item (item_id) ON DELETE CASCADE;
ALTER TABLE mob_item_drop ADD CONSTRAINT fk_mob_item_drop_mob FOREIGN KEY (mob_id) REFERENCES mob (mob_id) ON DELETE CASCADE;

ALTER TABLE npc ADD CONSTRAINT fk_npc_city FOREIGN KEY (city_name) REFERENCES city (city_name) ON DELETE CASCADE;

ALTER TABLE price ADD CONSTRAINT fk_price_inventory_item FOREIGN KEY (invitem_id) REFERENCES inventory_item (invitem_id) ON DELETE CASCADE;

ALTER TABLE quest ADD CONSTRAINT fk_quest_npc FOREIGN KEY (npc_id) REFERENCES npc (npc_id) ON DELETE CASCADE;

ALTER TABLE quest_requirement ADD CONSTRAINT fk_quest_requirement_quest FOREIGN KEY (quest_id) REFERENCES quest (quest_id) ON DELETE CASCADE;
ALTER TABLE quest_requirement ADD CONSTRAINT fk_quest_requirement_item FOREIGN KEY (item_id) REFERENCES item (item_id) ON DELETE CASCADE;

ALTER TABLE quest_reward ADD CONSTRAINT fk_quest_reward_quest FOREIGN KEY (quest_id) REFERENCES quest (quest_id) ON DELETE CASCADE;

ALTER TABLE refresh_token ADD CONSTRAINT fk_refresh_token_users FOREIGN KEY (username) REFERENCES users (username) ON DELETE CASCADE;

ALTER TABLE trade_market ADD CONSTRAINT fk_trade_market_inventory_item FOREIGN KEY (invitem_id) REFERENCES inventory_item (invitem_id) ON DELETE CASCADE;

ALTER TABLE travel ADD CONSTRAINT fk_travel_character FOREIGN KEY (character_name) REFERENCES character (character_name) ON DELETE CASCADE;
ALTER TABLE travel ADD CONSTRAINT fk_travel_area FOREIGN KEY (area_name) REFERENCES area (area_name) ON DELETE CASCADE;

ALTER TABLE battle_stats ADD CONSTRAINT xc_battle_stats_invitem_id_mob_ CHECK ((invitem_id IS NOT NULL AND mob_id IS NULL) OR (invitem_id IS NULL AND mob_id IS NOT NULL));

ALTER TABLE inventory_item ADD CONSTRAINT xc_inventory_item_city_name_cha CHECK ((city_name IS NOT NULL AND character_name IS NULL AND character_inventory_character_n IS NULL AND npc_id IS NULL) OR (city_name IS NULL AND character_name IS NOT NULL AND character_inventory_character_n IS NULL AND npc_id IS NULL) OR (city_name IS NULL AND character_name IS NULL AND character_inventory_character_n IS NOT NULL AND npc_id IS NULL) OR (city_name IS NULL AND character_name IS NULL AND character_inventory_character_n IS NULL AND npc_id IS NOT NULL));


-- INDEXES --

-- Drop existing indexes --
DROP INDEX IF EXISTS ix_access_token;
DROP INDEX IF EXISTS ix_refresh_token;
DROP INDEX IF EXISTS ix_inventory_item_inv_char_name;
DROP INDEX IF EXISTS ix_inventory_item_char_name;
DROP INDEX IF EXISTS ix_inventory_item_city_inv_name;
DROP INDEX IF EXISTS ix_quest_npc_id;

-- tokens
CREATE INDEX ix_access_token ON access_token USING HASH (access_token);
CREATE INDEX ix_refresh_token ON refresh_token USING HASH (refresh_token);

-- inventory item
CREATE INDEX ix_inventory_item_inv_char_name ON inventory_item USING HASH (character_inventory_character_n);
CREATE INDEX ix_inventory_item_char_name ON inventory_item USING HASH (character_name);
CREATE INDEX ix_inventory_item_city_inv_name ON inventory_item USING btree (city_name, city_inventory_character_name);

-- quest
CREATE INDEX ix_quest_npc_id ON quest USING btree (npc_id);


-- BEGINING OF ISERTIONS --

INSERT INTO "users" (username, password_hash)
VALUES ('demo', 'fe01ce2a7fbac8fafaed7c982a04e229'),
       ('nemo', 'e587f6146ebfbdefdc028c591643f220');

INSERT INTO "refresh_token" (record_id, username, refresh_token, expiration)
VALUES (1, 'demo', 'f427f6146ebfbdefdc028c591643e453', '12/01/2023 12:20:50');

INSERT INTO city (city_name)
VALUES ('Ayreim'),
       ('Tisling'),
       ('Dagos'),
       ('Soria');


INSERT INTO character (character_name, username, character_class, level, currency_gold, city_name, xp)
VALUES ('burger', 'demo', 1, 30, 1000, 'Ayreim', 0),
       ('hellmanz', 'nemo', 2, 20, 1200, 'Ayreim', 50),
       ('qwertz', 'nemo', 0, 5, 300, 'Ayreim', 30);


INSERT INTO city_inventory (city_name, character_name, capacity)
VALUES ('Ayreim', 'burger', 100),
       ('Ayreim', 'hellmanz', 100),
       ('Ayreim', 'qwertz', 100),
       ('Tisling', 'burger', 100),
       ('Tisling', 'hellmanz', 100),
       ('Tisling', 'qwertz', 100),
       ('Dagos', 'burger', 100),
       ('Dagos', 'hellmanz', 100),
       ('Dagos', 'qwertz', 100),
       ('Soria', 'burger', 100),
       ('Soria', 'hellmanz', 100),
       ('Soria', 'qwertz', 100);

INSERT INTO area (area_name)
VALUES ('Vergarni Hills'),
       ('Dagos Valley'),
       ('Mellow Woodland'),
       ('Croyware Wood'),
       ('Draysor Thicket'),
       ('Starlight Oasis'),
       ('Frostfeather Mountains');


INSERT INTO mob (mob_id, mob_name, area_name, type, level, group_size)
VALUES (1, 'Wolves', 'Vergarni Hills', 0, 1, 100),
       (2, 'Wolves', 'Vergarni Hills', 0, 10, 500),
       (3, 'Goat', 'Vergarni Hills', 1, 1, 600),
       (4, 'Driftshade Serpent', 'Vergarni Hills', 2, 30, 150),
       (5, 'Twilightcoil Serpent', 'Vergarni Hills', 2, 20, 110),

       (6, 'Wolves', 'Dagos Valley', 0, 6, 70),
       (7, 'Wolves', 'Dagos Valley', 0, 14, 30),
       (8, 'Hare', 'Dagos Valley', 3, 1, 754),
       (9, 'Sigil', 'Dagos Valley', 4, 32, 202),

       (10, 'Frostbite Falcon', 'Vergarni Hills', 3, 15, 60),
       (11, 'Shadow Lynx', 'Vergarni Hills', 1, 25, 40),
       (12, 'Ethereal Stag', 'Vergarni Hills', 5, 22, 80),
       (13, 'Crystal Golem', 'Vergarni Hills', 6, 35, 20),

       (14, 'Flameback Scorpion', 'Dagos Valley', 7, 18, 150),
       (15, 'Venomous Viper', 'Dagos Valley', 2, 12, 90),
       (16, 'Dagos Shaman', 'Dagos Valley', 8, 40, 5),
       (17, 'Rockhide Boar', 'Dagos Valley', 1, 10, 200),

       (18, 'Whispering Willow Wisp', 'Mellow Woodland', 5, 8, 300),
       (19, 'Meadow Fairy', 'Mellow Woodland', 9, 20, 50),
       (20, 'Barkskin Bear', 'Mellow Woodland', 1, 15, 75),
       (21, 'Moonlight Moth', 'Mellow Woodland', 3, 5, 500),

       (22, 'Croyware Centaur', 'Croyware Wood', 0, 28, 25),
       (23, 'Mystic Fox', 'Croyware Wood', 1, 12, 100),
       (24, 'Vine Entangler', 'Croyware Wood', 6, 35, 30),
       (25, 'Spirit Owl', 'Croyware Wood', 3, 18, 85),

       (26, 'Thicket Troll', 'Draysor Thicket', 0, 33, 10),
       (27, 'Draysor Dryad', 'Draysor Thicket', 9, 27, 40),
       (28, 'Thorned Serpent', 'Draysor Thicket', 2, 21, 60),
       (29, 'Razorwing Butterfly', 'Draysor Thicket', 3, 14, 200),

       (30, 'Oasis Djinn', 'Starlight Oasis', 8, 38, 15),
       (31, 'Mirage Manticore', 'Starlight Oasis', 0, 30, 20),
       (32, 'Sand Whisperer', 'Starlight Oasis', 5, 24, 90),
       (33, 'Solar Scorpion', 'Starlight Oasis', 7, 20, 140),

       (34, 'Icefang Yeti', 'Frostfeather Mountains', 0, 40, 5),
       (35, 'Glacial Griffin', 'Frostfeather Mountains', 3, 35, 10),
       (36, 'Frost Elemental', 'Frostfeather Mountains', 6, 28, 30),
       (37, 'Snowdrift Owl', 'Frostfeather Mountains', 3, 17, 80);



INSERT INTO item (item_id, name, icon, item_type, max_stack, max_durability)
VALUES
        -- Armor / weapons
        (0, 'Ares Blade', 'weapon/aresBlade', 5, 1, 100),
        (1, 'Armor of Ayreim warriors', 'bodyArmor/ayreimWarrior', 10, 1, 100),
        (2, 'Helmet of Ayreim warriors', 'helmet/ayreimWarrior', 11, 1, 100),
        (3, 'Boots of Ayreim warriors', 'boots/ayreimWarrior', 13, 1, 100),
        (4, 'Gloves of Ayreim warriors', 'gloves/ayreimWarrior', 12, 1, 100),

        -- Potions
        (100, 'Small Health Potion', 'potion/smallHealth', 20, 50, NULL),
        (101, 'Medium Health Potion', 'potion/mediumHealth', 20, 50, NULL),
        (102, 'Small Strength Potion', 'potion/smallStrength', 20, 50, NULL),
        (103, 'Small Dexterity Potion', 'potion/smallDexterity', 20, 50, NULL),

        -- Armor materials
        (1000, 'Copper Ore', 'regular/copperOre', 17, 50, NULL),
        (1001, 'Iron Ore', 'regular/ironOre', 17, 50, NULL),
        (1002, 'Gold Ore', 'regular/goldOre', 17, 50, NULL),
        (1003, 'Platinum Ore', 'regular/platinumOre', 17, 50, NULL),
        (1004, 'Mithril Ore', 'regular/mithrilOre', 17, 50, NULL),
        (1005, 'Orichalcum Ore', 'regular/orichalcumOre', 17, 50, NULL),

        -- Regular items
        (1010, 'Jute', 'regular/juteScrap', 0, 50, NULL),
        (1011, 'Linen', 'regular/linenScrap', 0, 50, NULL),
        (1012, 'Wool', 'regular/woolScrap', 0, 50, NULL),
        (1013, 'Cotton', 'regular/cottonScrap', 0, 50, NULL),
        (1014, 'Silk', 'regular/silkScrap', 0, 50, NULL),

        (1020, 'Goat Horn', 'regular/goatHorn', 0, 50, NULL),
        (1021, 'Wolve Fang', 'regular/wolveFang', 0, 50, NULL),
        (1022, 'Spike', 'regular/spike', 0, 50, NULL),

        -- Potion materials
        (1023, 'Cherry', 'regular/cherry', 25, 50, NULL),
        (1024, 'Asparagus', 'regular/asparagus', 25, 50, NULL),
        (1025, 'Blackberry', 'regular/blackberry', 25, 50, NULL),
        (1026, 'Carrot', 'regular/carrot', 25, 50, NULL),
        (1027, 'Mushroom', 'regular/mushroom', 25, 50, NULL),
        (1028, 'Fungus', 'regular/fungus', 25, 50, NULL),
        (1029, 'Mint', 'regular/mint', 25, 50, NULL),
        (1030, 'Strawberry', 'regular/strawberry', 25, 50, NULL),
        (1031, 'Mr.Bean', 'regular/bean', 25, 50, NULL),

        -- Magic items
        (2000, 'Eclipse Dust', 'magic/eclipseDust', 1, 50, NULL),
        (2001, 'Sunset Dust', 'magic/sunsetDust', 1, 50, NULL),
        (2002, 'Star Dust', 'magic/starDust', 1, 50, NULL),
        (2003, 'Sprout Dust', 'magic/sproutDust', 1, 50, NULL),
        (2004, 'Burn Dust', 'magic/burnDust', 1, 50, NULL),
        (2005, 'Life Dust', 'magic/lifeDust', 1, 50, NULL),
        (2006, 'Akasha Dust', 'magic/akashicDust', 1, 50, NULL),

        (2010, 'Water Crystal', 'magic/waterCrystal', 1, 50, NULL),
        (2011, 'Earth Crystal', 'magic/earthCrystal', 1, 50, NULL),
        (2012, 'Air Crystal', 'magic/windCrystal', 1, 50, NULL),
        (2013, 'Fire Crystal', 'magic/fireCrystal', 1, 50, NULL),
        (2014, 'Akasha Crystal', 'magic/akashCrystal', 1, 50, NULL),

        (2020, 'Vial of Transcendence', 'magic/vialOfTranscendence', 1, 1, NULL);


INSERT INTO mob_item_drop (mob_id, item_id, drop_rate, max_amount)
VALUES
        -- By ChatGPT
        -- Regular item or potion/armor material drops for each mob
        (1, 1010, 0.8, 10), -- Jute from Wolves in Vergarni Hills
        (2, 1011, 0.85, 10), -- Linen from Wolves in Vergarni Hills
        (3, 1020, 0.9, 10), -- Goat Horn from Goat in Vergarni Hills
        (4, 1013, 0.7, 10), -- Cotton from Driftshade Serpent in Vergarni Hills
        (5, 1014, 0.75, 10), -- Silk from Twilightcoil Serpent in Vergarni Hills
        (6, 1000, 0.8, 10), -- Copper Ore from Wolves in Dagos Valley
        (7, 1001, 0.85, 10), -- Iron Ore from Wolves in Dagos Valley
        (8, 1002, 0.9, 10), -- Gold Ore from Hare in Dagos Valley
        (9, 1003, 0.7, 10), -- Platinum Ore from Sigil in Dagos Valley
        (10, 1004, 0.75, 10), -- Mithril Ore from Frostbite Falcon in Vergarni Hills
        (11, 1005, 0.8, 10), -- Orichalcum Ore from Shadow Lynx in Vergarni Hills
        (12, 1020, 0.85, 10), -- Goat Horn from Ethereal Stag in Vergarni Hills
        (13, 1021, 0.9, 10), -- Wolve Fang from Crystal Golem in Vergarni Hills
        (14, 1022, 0.7, 10), -- Spike from Flameback Scorpion in Dagos Valley
        (15, 1023, 0.75, 10), -- Cherry from Venomous Viper in Dagos Valley
        (16, 1024, 0.8, 10), -- Asparagus from Dagos Shaman in Dagos Valley
        (17, 1025, 0.85, 10), -- Blackberry from Rockhide Boar in Dagos Valley
        (18, 1026, 0.9, 10), -- Carrot from Whispering Willow Wisp in Mellow Woodland
        (19, 1027, 0.7, 10), -- Mushroom from Meadow Fairy in Mellow Woodland
        (20, 1028, 0.75, 10), -- Fungus from Barkskin Bear in Mellow Woodland
        (21, 1029, 0.8, 10), -- Mint from Moonlight Moth in Mellow Woodland
        (22, 1030, 0.85, 10), -- Strawberry from Croyware Centaur in Croyware Wood
        (23, 1031, 0.9, 10), -- Mr.Bean from Mystic Fox in Croyware Wood
        (24, 1010, 0.7, 10), -- Jute from Vine Entangler in Croyware Wood
        (25, 1011, 0.75, 10), -- Linen from Spirit Owl in Croyware Wood
        (26, 1012, 0.8, 10), -- Wool from Thicket Troll in Draysor Thicket
        (27, 1013, 0.85, 10), -- Cotton from Draysor Dryad in Draysor Thicket
        (28, 1014, 0.9, 10), -- Silk from Thorned Serpent in Draysor Thicket
        (29, 1000, 0.7, 10), -- Copper Ore from Razorwing Butterfly in Draysor Thicket
        (30, 1001, 0.75, 10), -- Iron Ore from Oasis Djinn in Starlight Oasis
        (31, 1002, 0.8, 10), -- Gold Ore from Mirage Manticore in Starlight Oasis
        (32, 1003, 0.85, 10), -- Platinum Ore from Sand Whisperer in Starlight Oasis
        (33, 1004, 0.9, 10), -- Mithril Ore from Solar Scorpion in Starlight Oasis
        (34, 1005, 0.7, 10), -- Orichalcum Ore from Icefang Yeti in Frostfeather Mountains
        (35, 1020, 0.75, 10), -- Goat Horn from Glacial Griffin in Frostfeather Mountains
        (36, 1021, 0.8, 10), -- Wolve Fang from Frost Elemental in Frostfeather Mountains
        (37, 1022, 0.85, 10), -- Spike from Snowdrift Owl in Frostfeather Mountains

        -- Optional magic item drops with significantly lower drop rates
        (1, 2000, 0.2, 1), -- Eclipse Dust from Wolves in Vergarni Hills
        (2, 2001, 0.15, 1), -- Sunset Dust from Wolves in Vergarni Hills
        (3, 2002, 0.1, 1), -- Star Dust from Goat in Vergarni Hills
        (4, 2003, 0.05, 1), -- Sprout Dust from Driftshade Serpent in Vergarni Hills
        (5, 2004, 0.08, 1), -- Burn Dust from Twilightcoil Serpent in Vergarni Hills
        (6, 2005, 0.1, 1), -- Life Dust from Wolves in Dagos Valley
        (7, 2006, 0.05, 1), -- Akasha Dust from Wolves in Dagos Valley
        (8, 2010, 0.08, 1), -- Water Crystal from Hare in Dagos Valley
        (9, 2011, 0.1, 1), -- Earth Crystal from Sigil in Dagos Valley
        (10, 2012, 0.05, 1), -- Air Crystal from Frostbite Falcon in Vergarni Hills
        (11, 2013, 0.08, 1), -- Fire Crystal from Shadow Lynx in Vergarni Hills
        (12, 2014, 0.01, 1), -- Akasha Crystal from Ethereal Stag in Vergarni Hills
        (13, 2000, 0.05, 1), -- Eclipse Dust from Crystal Golem in Vergarni Hills
        (14, 2001, 0.08, 1), -- Sunset Dust from Flameback Scorpion in Dagos Valley
        (15, 2002, 0.1, 1), -- Star Dust from Venomous Viper in Dagos Valley
        (16, 2003, 0.05, 1), -- Sprout Dust from Dagos Shaman in Dagos Valley
        (17, 2004, 0.08, 1), -- Burn Dust from Rockhide Boar in Dagos Valley
        (18, 2005, 0.1, 1), -- Life Dust from Whispering Willow Wisp in Mellow Woodland
        (19, 2006, 0.05, 1), -- Akasha Dust from Meadow Fairy in Mellow Woodland
        (20, 2010, 0.08, 1), -- Water Crystal from Barkskin Bear in Mellow Woodland
        (21, 2011, 0.1, 1), -- Earth Crystal from Moonlight Moth in Mellow Woodland
        (22, 2012, 0.05, 1), -- Air Crystal from Croyware Centaur in Croyware Wood
        (23, 2013, 0.08, 1), -- Fire Crystal from Mystic Fox in Croyware Wood
        (24, 2014, 0.01, 1), -- Akasha Crystal from Vine Entangler in Croyware Wood
        (25, 2000, 0.05, 1), -- Eclipse Dust from Spirit Owl in Croyware Wood
        (26, 2001, 0.08, 1), -- Sunset Dust from Thicket Troll in Draysor Thicket
        (27, 2002, 0.1, 1), -- Star Dust from Draysor Dryad in Draysor Thicket
        (28, 2003, 0.05, 1), -- Sprout Dust from Thorned Serpent in Draysor Thicket
        (29, 2004, 0.08, 1), -- Burn Dust from Razorwing Butterfly in Draysor Thicket
        (30, 2005, 0.1, 1), -- Life Dust from Oasis Djinn in Starlight Oasis
        (31, 2006, 0.05, 1), -- Akasha Dust from Mirage Manticore in Starlight Oasis
        (32, 2010, 0.08, 1), -- Water Crystal from Sand Whisperer in Starlight Oasis
        (33, 2011, 0.1, 1), -- Earth Crystal from Solar Scorpion in Starlight Oasis
        (34, 2012, 0.05, 1), -- Air Crystal from Icefang Yeti in Frostfeather Mountains
        (35, 2013, 0.08, 1), -- Fire Crystal from Glacial Griffin in Frostfeather Mountains
        (36, 2014, 0.01, 1), -- Akasha Crystal from Frost Elemental in Frostfeather Mountains
        (37, 2000, 0.05, 1); -- Eclipse Dust from Snowdrift Owl in Frostfeather


INSERT INTO character_inventory (character_name, capacity)
VALUES ('burger', 10),
       ('hellmanz', 10),
       ('qwertz', 10);

INSERT INTO npc (npc_id, city_name, npc_type, currency_gold)
VALUES (0, 'Ayreim', 0, 500),
       (1, 'Ayreim', 1, 500),
       (2, 'Ayreim', 2, 500),
       (3, 'Ayreim', 3, 500),
       (4, 'Ayreim', 4, 500),
       (5, 'Ayreim', 100, 1000000),
       (6, 'Tisling', 0, 500),
       (7, 'Tisling', 1, 500),
       (8, 'Tisling', 2, 500),
       (9, 'Tisling', 3, 500),
       (10, 'Tisling', 4, 500),
       (11, 'Soria', 0, 500),
       (12, 'Soria', 1, 500),
       (13, 'Soria', 2, 500),
       (14, 'Soria', 3, 500),
       (15, 'Dagos', 0, 500),
       (16, 'Dagos', 1, 500),
       (17, 'Dagos', 3, 500);


INSERT INTO inventory_item (invitem_id, character_inventory_character_n, character_name, npc_id, item_id, level, stack_count, position)
VALUES                     (1,          'burger',                        NULL,           NULL,   1,       1,     1,           0),
                           (2,           NULL,                          'burger',        NULL,   2,       1,     1,           1),
                           (3,          'burger',                        NULL,           NULL,   3,       1,     1,           2),
                           (4,           NULL,                          'hellmanz',      NULL,   1,       5,     1,           4),
                           (5,           NULL,                          'hellmanz',      NULL,   2,       5,     1,           1),
                           (6,           NULL,                          'hellmanz',      NULL,   3,       5,     1,           5),
                           (7,          'hellmanz',                      NULL,           NULL,   100,     NULL,  8,           0),
                           (8,           NULL,                           NULL,           0,      1,       3,     100,         0),
                           (9,           NULL,                           NULL,           6,      2,       5,     100,         0),
                           (10,          NULL,                           NULL,           11,     3,       3,     100,         0),
                           (11,          NULL,                           NULL,           15,     4,       9,     100,         0),
                           (12,          NULL,                           NULL,           1,      100,     NULL,  300,         0),
                           (13,          NULL,                           NULL,           1,      101,     NULL,  200,         0),
                           (14,          NULL,                           NULL,           1,      102,     NULL,  150,         0),
                           (15,          NULL,                           NULL,           1,      103,     NULL,  150,         0),
                           (16,          'hellmanz',                     NULL,           NULL,   1,       NULL,  1,           1),
                           (17,          'qwertz',                       NULL,           NULL,   1,       NULL,  1,           0),
                           (18,           NULL,                          NULL,           0,      0,       4,     100,         0);

INSERT INTO price (invitem_id, price_gold, quantity_per_purchase)
VALUES (8, 100, 1),
	   (9, 100, 1),
	   (10, 100, 1),
       (11, 100, 1),
       (12, 50, 5),
       (13, 90, 5),
       (14, 70, 5),
       (18, 200, 1);

INSERT INTO battle_stats (stat_type, method, invitem_id, value)
VALUES (11, 0, 1, 42),
       (11, 1, 2, 2),
       (21, 0, 3, 13),
       (32, 1, 4, 1.91),
       (12, 0, 5, 27),
       (12, 1, 6, 1.56),
       (22, 0, 7, 84),
       (35, 1, 8, 1.19),
       (12, 0, 9, 63),
       (11, 1, 10, 1.37),
       (22, 0, 11, 72),
       (12, 1, 1, 1.50),
       (10, 1, 2, 1.95),
       (20, 0, 3, 68),
       (34, 0, 4, 24),
       (11, 0, 5, 79),
       (10, 1, 6, 1.45),
       (20, 1, 7, 1.31),
       (30, 1, 8, 1.59),
       (10, 0, 9, 14),
       (10, 0, 10, 86),
       (10, 0, 11, 41),
       (10, 0, 18, 100),

       -- potions --
       (10, 1, 14, 2.5),
       (11, 1, 15, 2.5);

INSERT INTO battle_stats (stat_type, method, mob_id, value)
VALUES (11, 0, 1, 42),
       (11, 0, 2, 2),
       (21, 0, 3, 13),
       (32, 0, 4, 191),
       (12, 0, 5, 27),
       (12, 0, 6, 156),
       (22, 0, 7, 84),
       (35, 0, 8, 119),
       (12, 0, 9, 63),
       (11, 0, 10, 137),
       (22, 0, 11, 72),
       (12, 0, 12, 150),
       (10, 0, 13, 195),
       (20, 0, 14, 68),
       (34, 0, 15, 24),
       (11, 0, 16, 79),
       (10, 0, 17, 145),
       (20, 0, 18, 131),
       (30, 0, 19, 159),
       (10, 0, 20, 14),
       (10, 0, 21, 86),
       (10, 0, 22, 41),
       (10, 0, 23, 100);


INSERT INTO quest (quest_id, npc_id, name, description, is_repeable, is_offered, level)
VALUES (1, 1, 'Out of stock',
'Master Alaric, a highly esteemed potion crafter, faces an unprecedented crisis. The demand for his renowned potions has surged, leaving his once-teeming shelves barren. Desperate to uphold his reputation, Master Alaric urgently seeks the assistance of intrepid adventurers. He seeks help with refilling his dwindling potion supplies',
TRUE, TRUE, 1),
(2, 1, 'The Cursed Fang',
'Master Alarics alchemical prowess faces a dire challenge. The cure for a potent ailment lies within the venomous fang of the elusive Shadow Serpent, a creature rumored to haunt the forbidden Mistwood Forest. The demand for this rare potion is urgent, and Master Alaric implores the bravest of adventurers to embark on a perilous quest.',
FALSE, TRUE, 20),
(3, 5, 'Research',
'Our royal alchemists are researching new potion. They need a lot of resources for experimenting. Can you bring some?',
TRUE, TRUE, 7),
(4, 4, 'Medicine',
'Please help. My wife is ill. She is very week. I need a health potion for her.',
FALSE, TRUE, 1);

INSERT INTO quest_requirement (quest_id, item_id, amount)
VALUES (1, 101, 10),
       (2, 1021, 3),
       (3, 1020, 21),
       (3, 1021, 42),
       (4, 100, 1);

INSERT INTO quest_reward (quest_id, currency_gold, xp)
VALUES (1, 100, 20),
       (2, 500, 1020),
       (3, 30, 30),
       (4, 10, 15);

INSERT INTO accepted_quest (character_name, quest_id, quest_state)
VALUES ('burger', 1, 1),
       ('burger', 4, 1),
       ('hellmanz', 2, 1),
       ('hellmanz', 1, 2);

DO $$
BEGIN
PERFORM setval('inventory_item_invitem_id_seq', (SELECT max(invitem_id) FROM inventory_item));
PERFORM setval('item_item_id_seq', (SELECT max(item_id) FROM item));
PERFORM setval('mob_mob_id_seq', (SELECT max(mob_id) FROM mob));
PERFORM setval('npc_npc_id_seq', (SELECT max(npc_id) FROM npc));
PERFORM setval('quest_quest_id_seq', (SELECT max(quest_id) FROM quest));
PERFORM setval('refresh_token_record_id_seq', (SELECT max(record_id) FROM refresh_token));
PERFORM setval('battle_stats_stat_id_seq', (SELECT max(stat_id) FROM battle_stats));
END $$;
