-- Insert data into the "users" table
INSERT INTO "users" (username, password_hash) VALUES ('zmrdus', 'your_password_hash');

-- Insert data into the "city" table
INSERT INTO city (city_name) VALUES ('Ayreim');

-- Insert data into the "character" table
INSERT INTO character (character_name, username, character_class, level, currency_gold, city_name)
VALUES ('zmrdus', 'zmrdus', 1, 1, 1000, 'Ayreim');

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
       (6, NULL, NULL, 1, 3, NULL, 1, 5);
	   
INSERT INTO price (invitem_id, price_gold)
VALUES (4, 200),
	(5, 100),
	(6, 50);

-- Insert data into the "quest" table
INSERT INTO quest (quest_id, npc_id, name, description, is_repeable, is_offered)
VALUES (1, 1, 'Quest1', 'Description of Quest1', TRUE, TRUE);

-- Insert data into the "quest_requirement" table
INSERT INTO quest_requirement (quest_id, item_id, amount, mob_type)
VALUES (1, 1, 10, 1);

-- Insert data into the "quest_reward" table
INSERT INTO quest_reward (quest_id, item_id, currency_gold, item_count)
VALUES (1, 2, 100, 1);

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
