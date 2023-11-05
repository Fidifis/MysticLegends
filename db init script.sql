-- Insert data into the "user" table
INSERT INTO "user" (username, password_hash) VALUES ('zmrdus', 'your_password_hash');

-- Insert data into the "character" table
INSERT INTO character (character_name, username, character_class, level, currency_gold)
VALUES ('zmrdus', 'zmrdus', 1, 1, 1000);

-- Insert data into the "city" table
INSERT INTO city (city_name) VALUES ('City1');

-- Insert data into the "city_inventory" table
INSERT INTO city_inventory (city_name, character_name, capacity)
VALUES ('City1', 'zmrdus', 100);

-- Insert data into the "area" table
INSERT INTO area (area_name) VALUES ('Area1');

-- Insert data into the "mob" table
INSERT INTO mob (area_name, mob_name, type, level, group_size)
VALUES ('Area1', 'Mob1', 1, 1, 5);

-- Insert data into the "item" table
INSERT INTO item (item_id, name, icon, item_type, max_stack, max_durability)
VALUES (1, 'Item1', 'bodyArmor/ayreimWarrior', 10, 1, 100),
       (2, 'Item2', 'helmet/ayreimWarrior', 11, 1, 100);


INSERT INTO character_inventory (character_name, capacity)
VALUES ('zmrdus', 10);

-- Insert data into the "inventory_item" table
INSERT INTO inventory_item (invitem_id, character_inventory_character_n, character_name, item_id, level, stack_count, position)
VALUES (1, 'zmrdus', NULL, 1, 1, 1, 0),
       (2, 'zmrdus', NULL, 2, 1, 1, 1),
       (3, 'zmrdus', NULL, 1, 1, 1, 2),
       (4, NULL, NULL, 1, 1, 1, 3),
       (5, NULL, NULL, 2, 1, 1, 4);

-- Insert data into the "npc" table
INSERT INTO npc (npc_name, city_name, npc_type, currency_gold)
VALUES ('Npc1', 'City1', 1, 500);

-- Insert data into the "quest" table
INSERT INTO quest (quest_id, npc_name, name, description, is_repeable, is_offered)
VALUES (1, 'Npc1', 'Quest1', 'Description of Quest1', TRUE, TRUE);

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

INSERT INTO npc_item (npc_name, invitem_id, price_gold)
VALUES ('Npc1', 4, 100),
       ('Npc1', 5, 200);
