# Semestrální projekt - Databáze

Autor: Filip Digrín

## Popis

Jedná se o databázový systém k online RPG hře Mystic Legends (klient i server jsou také součástí tohoto repozitáře).

Ve hře se můžete registrovat nebo přihlásit do systému.
Ověření uživatele je provedeno dvěma tokeny. Po poskytnutí username a hesla klient obdrží refresh token který si uloží.
Tento token expiruje v řádu měsíců a slouží k vygenerování Access tokenu.
Každý uživatel může mít více refresh tokenů (co zařízení to refresh token).
Access token je vydán po ověření platného refresh tokenu a slouží jako ověření identity uživatele.
Pro jednoho uživatele může existovat jen jeden Access token v jeden moment.
Toto chování zamezuje přístupu z několika zařízení současně.
Expirace Access token je v řádu minut/hodin.

Každý uživatel má jednu nebo několik postav za které může hrát.
Každá postava postava se nachází v nějakém městě a může mezi nimi cestovat.
Postava má "classu", peníze, level a zkušenosti (xp).
Klíčovou mechanikou hry jsou předměty, které můžou být uloženy v inventáři postavy, nasazeny na postavě (pouze typ brnění a zbraň) uloženy v městském skladě nebo je nemusí vlastnit hráč ale NPC (non-playable character).
Předměty pro boj mají své statistiky, díky nimž může být každý předmět unikátní. Tyto hodnoty se aplikují při boji.

Předměty mohou být kýmkoli nabídnuty k prodeji na trhu (trade market). Mohou být prodány za fixní cenu nebo jako aukce s příhozy.

Každé město má své obchodníky, kteří prodávají předměty, které vyrobili.
Zadavatelé (NPC které nic neprodává), a obchodníci můžou zadávat hráčům úkoly.
Na splnění úkolu jsou dány požadavky, přinést nějaké předměty.
Jako odměna jsou zkušenosti (xp) pro levelování a peníze.
Hráč pro získání nějakého předmětu, který nejde (nebo nechce) koupit, může vyrazit na cestu a pobít pár monster.
Z nich může získat nejrůznější předměty od běžných po velmi vzácné.

## Konceptuální schéma

![schéma](Conceptual%20schema.jpg)

## Integritní omezení

- U žádného atributu nejsou dovoleny záporné hodnoty, s vyjímkou peněz-*currency_gold* obchodníka (*npc*).
- Pokud je předmět (*inventory_item*) na prodej na trhu (existuje v tabulce *trade_market*), musí zároveň mít cenu (*price*).
- Pokud předmět (*inventory_item*) není na prodej na trhu (*trade_market*) ani ho neprodává obchodník (*npc*), nesmí existovat cena (*price*).
- Postava (*character*) může manipulovat s předměty (*inventory_item*) ve skladě (*city_inventory*), pouze v případě že sklad a postava se nacházejí ve stejném městě (*city*)
- Postava (*character*) může příjmat úkoly (*quest*) od zadavatele/obchodníka(*npc*) pouze pokud jsou v danou chvíli ve stejném městě (*city*)
- Atribut *quantity_per_purchase* musí být menší nebo roven atributu *stack_count* nebo být NULL.
- Atribut *listed_since* na trhu (*trade_market*) musí být menší roven aktuálnímu datu a času.

## Diskuse smyček

Ve schématu se nachází smyčky (hodně).

Vzhledem k velkému množstvý smyček jsem se je rozhodl okomentovat 3 nejsložitější. Celkově jsem se pokusil všechny smyčky zakreslit do obrázku, jelikož vypsání všech možných kombinací není v mých silách.

Pod obrázkem vizualizace smyček jsem vytvořil jiný náhled na schéma (*dependency tree*), které pomůže určit jestli je schéma "realizovatelné".

### 3 Okomentované

**character - travel - area - mob - mob_item_drop - item - inventory_item - character**
- Zde se jedná o Postavu (*Character*), která cestuje (*travel*) do oblasti (*area*) kde se nachází jedna či více příšer/zvířat (*mob*). Příšera může *"dropnout"* předmět nějakého typu (entita *item*).
- Tady je potřeba si vysvětlit jaký je rozdíl mezi entitou *item* a *inventory_item*. *Itemů* je konečné možství a jsou přidávány s aktualizací hry. Dalo by se přirovnat k abstraktní třídě. *inventory_item* už je konkrétní reprezentace *itemu* a může být umístěn do inventáře. *Inventory_itemů* je teoreticky nekonečno, protože vznikají (i zanikají) při hraní hry. Toto by se dalo přirovnat k instanci třídy.
- V této smyčce tedy není problém aby z *moba* "dropnul" *item* (abstraktní typ), který hráč už má, jelikož jde o jinou instanci.

**character - city_inventory - city - character**
- Jedná se o smyčku, díky které může nastat že postava (*character*) je ve městě (*city*) ve kterém je zároveň sklad (*city_inventory*), který patří postavě (*character*).
- Tato situace je naprosto v pořádku a v určitém případě i vyžadována, viz **integritní omezení**.

**character - city - quest - accepted_quest - character**
- Jedná se o smyčku kde může být postava (*character*) ve městě (*city*) ve kterém je obchodník/zadavatel(*npc*), který zadává úkol (*quest*) a postava (*character*) může mít tento úkol příjmutý/odmítnutý (*accepted_quest*).
- Tato situace je naprosto v pořádku a v určitém případě i vyžadována, viz **integritní omezení**.

### Smyčky vizuálně

![schéma_se_smyčkai](Loops.jpg)

### Dependency tree

Schéma jsem se pokusil přetransformovat do jiné podoby a udělat jakoby dependency tree. S tím že ořežu entity které nejsou ve smyčce (třeba user je mimo smyčku) a nechám jen ty které ve smyčce jsou.

Na vrcholu jsou entity které na ničem nezávisí. Každý řádek tvoří jednu úroveň. Entity na jedné úrovni mohou být vytvořeny jen v případě že existují entity z vyší úrovně. Na sousedech a nižší úrovni nezáleží. Vazba *required* vždy směřuje nahoru. Cardinalita *Many* je vždy dole.

**Z hlediska vytvoření databázového schématu a naplnění daty nevidím problém, jelikož je možné tento graf sestavit a tedy neobsahuje cyklickou závislost.**

**Z hlediska konzistence dat, také nevidím problém.**

![dependency_tree](dependency.jpg)

## Relační schéma

Omlovám se že to vypadá jako to vypadá, ovládání ERD toolu v PGAdminu pro mě není intuitivní.

![ERD](ERD.png)

## Dotazy

1. postavy které mají level větší roven 5
    ```
    character(level>=5)[character_name, level]
    ```
    ```sql
    SELECT DISTINCT character_name, level
    FROM character
    WHERE (level>=5)
    ```
2. item na prodej u obchodníka, které stojí více jak 50
    ```
    {inventory_item[invitem_id]} * {price(price_gold> 50)[price_gold]}
    ```
    ```sql
    select it.invitem_id, p.price_gold
    from inventory_item as it
    join price as p on it.invitem_id=p.invitem_id
    where p.price_gold > 50
    ```
3. itemy u obchodníka, které nejsou na prodej
    ```sql
    select it.invitem_id, it.npc_id
    from inventory_item as it
    left join price as p on it.invitem_id = p.invitem_id
    where p.invitem_id is null and it.npc_id is not null
    ```
4. uživatel který nemá refresh token
    ```
    users!*>refresh_token
    ```
    ```sql
    select distinct * from users u
    where not exists (
      select * from refresh_token r
      where u.username=r.username
    )
    ```
5. úkol společně s jeho requirements a reward
    ```sql
    select q.*, qr.item_id, qr.amount, qrd.currency_gold, qrd.xp
    from quest q
    left join quest_requirement qr on q.quest_id = qr.quest_id
    left join quest_reward qrd on q.quest_id = qrd.quest_id
    ```
6. celkové množství peněz všech npcs v jednotlivých městech, vetší rovno 2000, seřazeno sestupně
    ```sql
    select f.city_name as "city name", sum(f.currency_gold) as total_gold
    from (
      select npc.city_name, npc.currency_gold
      from npc
      join city on npc.city_name = city.city_name
    ) as f
    group by f.city_name
    having sum(f.currency_gold) >= 2000
    order by total_gold desc
    ```
7. postava, která nemá žádný úkol
    ```
    character[character_name] !*> accepted_quest
    ```
    ```sql
    select ch.character_name from character ch
    except (
        select ch2.character_name from character ch2
        inner join accepted_quest aq on ch2.character_name = aq.character_name
    )
    ```
8. postava, která je ve městě kde má úkol
    ```sql
    select distinct ch.character_name, npc.city_name from character ch
    join accepted_quest aq on ch.character_name = aq.character_name
    join quest q on q.quest_id = aq.quest_id
    join npc on npc.npc_id = q.npc_id
    where npc.city_name = ch.city_name
    ```
9. itemy které hráč již má a zároveň je potřebuje ke splnění úkolu
    ```sql
    with
    invit as (select * from inventory_item),
    reqit as (select it.item_id, it.name from quest_requirement qr join item it on it.item_id = qr.item_id)
    select reqit.item_id, reqit.name from reqit join invit on invit.item_id = reqit.item_id
    ```
10. u koho se nacházejí itemy, které přidávanjí dexterity nebo strength
    ```sql
    select distinct inv.npc_id, inv.character_name, inv.character_inventory_character_n, inv.invitem_id, bs.stat_type, bs.value
    from inventory_item inv
    natural join battle_stats bs
    where bs.stat_type = 0 or bs.stat_type = 1
    ```
11. expirované tokeny
    ```
    refresh_token(expiration < CURRENT_TIMESTAMP)[expiration->refresh_expiration] * users[username] *> access_token[expiration->current_access_expiration]
    ```
    ```sql
    SELECT DISTINCT u.username,
    rt.expiration as refresh_expiration,
    at.expiration as current_access_expiration
    FROM refresh_token rt
    inner join users u on rt.username = u.username
    left join access_token at on u.username = at.username
    WHERE (rt.expiration < CURRENT_TIMESTAMP)
    ```
12. Počet předmětů v invetáři u každé postavy
    ```sql
    select ch.character_name, count(inv)
    from character ch
    left join inventory_item inv on inv.character_inventory_character_n = ch.character_name
    group by ch.character_name
    ```
13. Úkol(y), který má přijmutý jen postava se jménem 'hellmanz' a nikdo jiný
    ```
    {character(character_name='hellmanz')*accepted_quest*quest}
    \
    {character(character_name!='hellmanz')*accepted_quest*quest}
    ```
    ```sql
    select distinct q.* from character ch natural join accepted_quest aq natural join quest q
    where ch.character_name='hellmanz'
    except
    select distinct q.* from character ch natural join accepted_quest aq natural join quest q
    where ch.character_name!='hellmanz'
    ```
14. Item který má každá postava
    ```
    {inventory_item[item_id, character_inventory_character_n]÷character[character_name]}*item
    ```
    ```sql
    with
    postavy as (select character_name from character),
    mozne_predmety as (select item_id, character_name from item cross join character),
    existuji_predmety as (select item_id, character_inventory_character_n from inventory_item),
    neexistuji_predmety as (select * from mozne_predmety except select * from existuji_predmety),
    item_co_nema_kazdy as (select item_id from neexistuji_predmety),
    item_co_ma_kazdy as (select item_id from item except select item_id from neexistuji_predmety)
    select * from item_co_ma_kazdy natural join item
    ```
    ```sql
    select * from item it where not exists(
      select * from character ch where not exists(
        select * from inventory_item inv
        where inv.item_id=it.item_id and inv.character_inventory_character_n=ch.character_name
      )
    )
    ```
    ```sql
    select * from item it where
    (select count(distinct character_inventory_character_n) from inventory_item inv where inv.item_id=it.item_id)
    =
    (select count(character_name) from character)
    ```
15. Item, který nikdo nemá
    ```
    item \ inventory_item
    ```
    ```sql
    select * from (
        select item_id from item
        except (
            select distinct item_id from inventory_item
        )
    ) as t
    natural join item
    ```
16. Kontrola dotazu z kategorie D1 (dotaz 14)
    ```sql
    -- žádný výstup znamená správnost
    select * from character
    except
    select * from character where character_name in(
    select character_inventory_character_n from inventory_item where inventory_item.character_inventory_character_n=character.character_name and item_id in(
    select item_id from (
    --overovana cast
        select * from item it where
        (select count(distinct character_inventory_character_n) from inventory_item inv where inv.item_id=it.item_id)
        =
        (select count(character_name) from character)
    ) it));
    --end of overovana cast
    ```
17. Předměty spojené s NPCs ukazující přehled které předměty jsou u NPC a které ne
    ```sql
    select inv.invitem_id, npc.npc_id, npc.city_name
    from inventory_item inv
    full outer join npc
    on inv.npc_id = npc.npc_id
    order by inv.invitem_id
    ```
18. Všechny tokeny (access + refresh) a jejich expirace, řazená podle expirace vzestupně
    ```sql
    select at.access_token as token, at.expiration from access_token as at
    union(
        select refresh_token, expiration from refresh_token
    )
    order by expiration asc
    ```
19. Počty mobů v jednotlivích oblastech
    ```sql
    select area.area_name,
    (
        select sum(mob.group_size)
        from mob
        where area.area_name = mob.area_name
    ) as total_mobs
    from area
    ```
20. Itemy, které někdo má
    ```
    item ∩ inventory_item
    ```
    ```sql
    select * from (
        select distinct item_id
        from item
        intersect (
            select distinct item_id from inventory_item
        )
    ) as t
    natural join item
    ```
21. Ověření dotazu 20 a 15
    ```
    item \ {{item ∩ inventory_item} ∪ {item \ inventory_item}}
    ```
    ```sql
    -- žádná odpověď znamená správnost
    with
    d15 as (
        select item_id from item
        except (
            select distinct item_id from inventory_item
        )
    ),
    d20 as (
        select distinct item_id
        from item
        intersect (
            select distinct item_id from inventory_item
        )
    )
    select distinct item_id from item except (
        select distinct * from d15
        union (select distinct * from d20)
    )
    ```
22. View s počtem postav každého hráče a nejvyší dosažený level
    ```sql
    create or replace view players_overview as
    select username, count(character), max(level) as max_level from users
    join character using (username)
    group by username
    order by max_level desc;

    select * from players_overview;
    ```
23. průměr nejvyšších levelů podle view *players_overview* (z dotazu 22)
    ```sql
    select avg(max_level) as average_level from players_overview
    ```
24. Vložení náhodného předměntu
    ```sql
    begin;
    insert into inventory_item (character_inventory_character_n, item_id, level, stack_count, position)
    select
    (
        select character_name from character order by random() limit 1
    ),
    (
        select item_id from item order by random() limit 1
    ),
    (random() * 10)::int as level,
    1 as stack_count,
    (random() * 10)::int as position;

    rollback;
    ```
25. Zvýšení levelu o 3 armorům
    ```sql
    begin;
    update inventory_item i
    set level=level+3
    where i.character_inventory_character_n in (
        select inv.character_inventory_character_n
        from inventory_item inv
        natural join item
        where item_type = 10
    );

    -- ověření - součet by měl být větší
    select sum(level) from inventory_item;

    rollback;
    ```
26. Odstranění příjmutého questu
    ```sql
    begin;
    -- úprava dat (aktualně žádná data nevyhovují podmínce pro delete, tak jeden záznam změníme aby podmínce vyhověl)
    update quest
    set is_offered=false
    where quest_id=1;

    -- delete query
    delete from accepted_quest aq
    where aq.quest_id in (
        select q.quest_id
        from quest q
        where is_offered = false
    );

    --overeni
    select count(*) from accepted_quest;

    rollback;
    ```

---

- Počet RA dotazů: 10
- Počet SQL dotazů 26

## Tabulka pokrytí SQL dotazů
| Kategorie | Kódy dotazů                                         | Charakteristika kategorie                                              |
|-----------|-----------------------------------------------------|------------------------------------------------------------------------|
| A         | 2; 3; 5; 6; 7; 8; 9; 10; 11; 12; 13;                | A - Pozitivní dotaz nad spojením alespoň dvou tabulek                  |
| B         | 4;                                                  | B - Negativní dotaz nad spojením alespoň dvou tabulek                  |
| C         | 13;                                                 | C - Vyber ty, kteří mají vztah POUZE k ...                             |
| D1        | 14;                                                 | D1 - Vyber ty, kteří/které jsou ve vztahu se všemi - dotaz s univerzální kvantifikací |
| D2        | 16;                                                 | D2 - Kontrola výsledku dotazu z kategorie D1                           |
| F1        | 2; 3; 5; 6; 7; 8; 9; 11; 12;                        | F1 - JOIN ON                                                           |
| F2        | 10; 13; 14; 15; 20; 22;                             | F2 - NATURAL JOIN|JOIN USING                                           |
| F3        | 14;                                                 | F3 - CROSS JOIN                                                        |
| F4        | 3; 5; 11; 12;                                       | F4 - LEFT|RIGHT OUTER JOIN                                             |
| F5        | 17;                                                 | F5 - FULL (OUTER) JOIN                                                 |
| G1        | 4; 14;                                              | G1 - Vnořený dotaz v klauzuli WHERE                                    |
| G2        | 6; 15; 16;                                          | G2 - Vnořený dotaz v klauzuli FROM                                     |
| G3        | 19; 24;                                             | G3 - Vnořený dotaz v klauzuli SELECT                                   |
| G4        | 4; 14;                                              | G4 - Vztažený vnořený dotaz (EXISTS, NOT EXISTS)                       |
| H1        | 18; 21;                                             | H1 - Množinové sjednocení - UNION                                      |
| H2        | 7; 15; 21;                                          | H2 - Množinový rozdíl - MINUS nebo EXCEPT                              |
| H3        | 20;                                                 | H3 - Množinový průnik - INTERSECT                                      |
| I1        | 6; 12; 14; 16; 19; 22;                              | I1 - Agregační funkce (count|sum|min|max|avg)                          |
| I2        | 6; 12; 22;                                          | I2 - Agregační funkce nad seskupenými řádky - GROUP BY (HAVING)        |
| J         | 14;                                                 | J - Stejný dotaz ve třech různých formulacích SQL                      |
| K         | 6;                                                  | K - Všechny klauzule v 1 dotazu - SELECT FROM WHERE GROUP BY HAVING ORDER BY |
| L         | 22;                                                 | L - VIEW                                                               |
| M         | 23;                                                 | M - Dotaz nad pohledem                                                 |
| N         | 24;                                                 | N - INSERT                                                             |
| O         | 25;                                                 | O - UPDATE s vnořeným SELECT příkazem                                  |
| P         | 26;                                                 | P - DELETE s vnořeným SELECT příkazem                                  |


## Script

- Script pro vytvoření a naplnění daty: [db init script.sql](../db%20init%20script.sql)
- Schéma pro [kreslítko](https://dbs.fit.cvut.cz/kreslitko/): [database schema.json](../database%20schema.json)

## Závěr

Tato práce byla po dlouhé době studia konečně něco zajímavého.
Možná je schéma rozsáhlejší a složitější, než je třeba, ale aspoň je to trochu challenge a nejen nějaká semetrálka co je za den hotová.
Na práci mi nejvíce času a úsilí zabralo napsat všechny dotazy, a ještě aby pokryli všechny kategorie z tabulky. Při psaní dotazů, mi byla nějvětší pomocí vzorová práce Zoo ve skluzu.
Další dobrou pomoc poskytl i náš [PSQLClient](https://psqlc.db.kii.pef.czu.cz/SQLClient) od pana doktora Pavlíčka,
kde se dá dotaz v relační algebře spustit nebo převést na SQL a můžu si tak ověřit, že RA dotaz bude dělat to co chci.
RA překladač si zatím neporadí se složitějšími dotazy, ale věřím že v budoucnu bude pracovat perfektně.
Se svým ERD diagrem nejsem moc spokojený protože jsem musel můj model rozmotávat, aby vazby nebyli náhodně skrz celý diagram.
A ovládání ERD toolu v PGAdminu nebylo moc přívětivé.
Kreslítko je oproti tomu mnohem více intuitivní a dobře se ovládá.
Bylo fajn si po delší době osvěžit SQL a navrhnout model, který dává smysl a má reálné využití.


## Zdroje
- ČVUT. Data Modeler (Kreslítko). Online. ©2023. Dostupné z: https://dbs.fit.cvut.cz/kreslitko. [cit. 2023-11-29].
- ČZU. Databázové systémy. Online. Moodle. [2023]. Dostupné z: https://moodle.czu.cz/course/view.php?id=4078. [cit. 2023-11-29].
- PAVLÍČEK, Josef. SQLClient. Online. Dostupné z: https://psqlc.db.kii.pef.czu.cz/SQLCLient. [cit. 2023-11-29].
- HUNKA, Jiří. Zoo ve skluzu. Online. Dostupné z: https://users.fit.cvut.cz/~hunkajir/dbs2/main.xml. [cit. 2023-12-01].
- OPENAI. ChatGPT 3.5. [online]. ©2023 [cit. 2023-12-01]. Dostupné z: https://chat.openai.com/.
  Prompts: "dependency tree",
  "Make short description of this schema: [DDL script]",
  "[CSV] Here i give a CSV. Take the content and poste it to a markdown table like this: [md snippet]"
