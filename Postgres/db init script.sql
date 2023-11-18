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
    stat_type INTEGER NOT NULL,
    method INTEGER NOT NULL,
    invitem_id INTEGER NOT NULL,
    value DOUBLE PRECISION NOT NULL
);
ALTER TABLE battle_stats ADD CONSTRAINT pk_battle_stats PRIMARY KEY (stat_type, method, invitem_id);

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
    drop_rate DOUBLE PRECISION NOT NULL
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

CREATE TABLE users (
    username VARCHAR(32) NOT NULL,
    password_hash VARCHAR(32) NOT NULL
);
ALTER TABLE users ADD CONSTRAINT pk_users PRIMARY KEY (username);

ALTER TABLE accepted_quest ADD CONSTRAINT fk_accepted_quest_character FOREIGN KEY (character_name) REFERENCES character (character_name) ON DELETE CASCADE;
ALTER TABLE accepted_quest ADD CONSTRAINT fk_accepted_quest_quest FOREIGN KEY (quest_id) REFERENCES quest (quest_id) ON DELETE CASCADE;

ALTER TABLE access_token ADD CONSTRAINT fk_access_token_users FOREIGN KEY (username) REFERENCES users (username) ON DELETE CASCADE;

ALTER TABLE battle_stats ADD CONSTRAINT fk_battle_stats_inventory_item FOREIGN KEY (invitem_id) REFERENCES inventory_item (invitem_id) ON DELETE CASCADE;

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

ALTER TABLE inventory_item ADD CONSTRAINT xc_inventory_item_city_name_cha CHECK ((city_name IS NOT NULL AND character_name IS NULL AND character_inventory_character_n IS NULL AND npc_id IS NULL) OR (city_name IS NULL AND character_name IS NOT NULL AND character_inventory_character_n IS NULL AND npc_id IS NULL) OR (city_name IS NULL AND character_name IS NULL AND character_inventory_character_n IS NOT NULL AND npc_id IS NULL) OR (city_name IS NULL AND character_name IS NULL AND character_inventory_character_n IS NULL AND npc_id IS NOT NULL));


-- BEGINING OF ISERTIONS --


-- Insert data into the "users" table
INSERT INTO "users" (username, password_hash) VALUES ('demo', 'fe01ce2a7fbac8fafaed7c982a04e229');

-- Insert data into the "city" table
INSERT INTO city (city_name) VALUES ('Ayreim');

-- Insert data into the "character" table
INSERT INTO character (character_name, username, character_class, level, currency_gold, city_name, xp)
VALUES ('zmrdus', 'demo', 1, 1, 1000, 'Ayreim', 0);

-- Insert data into the "city_inventory" table
INSERT INTO city_inventory (city_name, character_name, capacity)
VALUES ('Ayreim', 'zmrdus', 100);

-- Insert data into the "area" table
INSERT INTO area (area_name) VALUES ('Area1');

-- Insert data into the "mob" table
INSERT INTO mob (area_name, mob_name, type, level, group_size)
VALUES ('Area1', 'Mob1', 1, 1, 5);

-- Insert data into the "item" table
INSERT INTO item (item_id, name, icon, item_type, max_stack, max_durability)
VALUES (1, 'Armor of Ayreim warriors', 'bodyArmor/ayreimWarrior', 10, 1, 100),
       (2, 'Helmet of Ayreim warriors', 'helmet/ayreimWarrior', 11, 1, 100),
       (3, 'Small Health Potion', 'potion/smallHealth', 20, 50, NULL);


INSERT INTO character_inventory (character_name, capacity)
VALUES ('zmrdus', 10);

-- Insert data into the "npc" table
INSERT INTO npc (npc_id, city_name, npc_type, currency_gold)
VALUES (1, 'Ayreim', 1, 500);

-- Insert data into the "inventory_item" table
INSERT INTO inventory_item (invitem_id, character_inventory_character_n, character_name, npc_id, item_id, level, stack_count, position)
VALUES (1, 'zmrdus', NULL, NULL, 1, 1, 1, 0),
       (2, 'zmrdus', NULL, NULL, 2, 1, 1, 1),
       (3, 'zmrdus', NULL, NULL, 1, 1, 1, 2),
       (4, NULL, NULL, 1, 1, 1, 1, 3),
       (5, NULL, NULL, 1, 2, 1, 1, 4),
       (6, NULL, NULL, 1, 3, NULL, 10, 5),
       (7, NULL, NULL, 1, 3, NULL, 200, 0),
       (8, NULL, NULL, 1, 3, NULL, 150, 0);

INSERT INTO price (invitem_id, price_gold, quantity_per_purchase)
VALUES (4, 200, NULL),
	   (5, 100, NULL),
	   (6, 50, NULL),
       (7, 52, 6),
       (8, 50, 5);

-- Insert data into the "quest" table
INSERT INTO quest (quest_id, npc_id, name, description, is_repeable, is_offered, level)
VALUES (1, 1, 'Out of stock',
'Master Alaric, a highly esteemed potion crafter, faces an unprecedented crisis. The demand for his renowned potions has surged, leaving his once-teeming shelves barren. Desperate to uphold his reputation, Master Alaric urgently seeks the assistance of intrepid adventurers. He seeks help with refilling his dwindling potion supplies',
TRUE, TRUE, 1),
(2, 1, 'The Cursed Fang',
'Master Alarics alchemical prowess faces a dire challenge. The cure for a potent ailment lies within the venomous fang of the elusive Shadow Serpent, a creature rumored to haunt the forbidden Mistwood Forest. The demand for this rare potion is urgent, and Master Alaric implores the bravest of adventurers to embark on a perilous quest.',
FALSE, TRUE, 2);

-- Insert data into the "quest_requirement" table
INSERT INTO quest_requirement (quest_id, item_id, amount)
VALUES (1, 3, 10),
       (2, 3, 1);

-- Insert data into the "quest_reward" table
INSERT INTO quest_reward (quest_id, currency_gold, xp)
VALUES (1, 100, 20),
       (2, 500, 1000);

INSERT INTO accepted_quest (character_name, quest_id, quest_state)
VALUES ('zmrdus', 1, 1);

INSERT INTO battle_stats (stat_type, method, invitem_id, value)
VALUES (1, 0, 1, 50),  -- Example stat 1
       (1, 1, 2, 3),  -- Example stat 2
       (1, 0, 3, 60);  -- Example stat 3

SELECT setval('inventory_item_invitem_id_seq', (SELECT max(invitem_id) FROM inventory_item));
SELECT setval('item_item_id_seq', (SELECT max(item_id) FROM item));
SELECT setval('mob_mob_id_seq', (SELECT max(mob_id) FROM mob));
SELECT setval('npc_npc_id_seq', (SELECT max(npc_id) FROM npc));
SELECT setval('quest_quest_id_seq', (SELECT max(quest_id) FROM quest));
SELECT setval('refresh_token_record_id_seq', (SELECT max(record_id) FROM refresh_token));
