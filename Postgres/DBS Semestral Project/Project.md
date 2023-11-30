# Semestrální projekt - Databáze
## Popis
Jedná se o databázový systém k online RPG hře. Můžete se registrovat nebo přihlásit do systému.
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

## Dotazy
1. postavy které mají level větší roven 5
    - ```
      character(level>=5)[character_name, level]
      ```
    - ```
      SELECT DISTINCT character_name, level FROM  character WHERE (level>=5)
      ```
2. item na prodej u obchodníka, které stojí více jak 50
    - ```
      select it.invitem_id, p.price_gold from inventory_item as it join price as p on it.invitem_id=p.invitem_id where p.price_gold > 50
      ```
3. itemy u obchodníka, které nejsou na prodej
    - ```
      select it.invitem_id, it.npc_id from inventory_item as it left join price as p on it.invitem_id = p.invitem_id where p.invitem_id is null and it.npc_id is not null
      ```
4. uživatel který nemá refresh token
    - ```
      users!*>refresh_token
      ```
    - ```
      select distinct * from users u where not exists (select * from refresh_token r where u.username=r.username)
      ```
5. úkol společně s jeho requirements a reward
    - ```
      select q.*, qr.item_id, qr.amount, qrd.currency_gold, qrd.xp
      from quest q
      left join quest_requirement qr on q.quest_id = qr.quest_id
      left join quest_reward qrd on q.quest_id = qrd.quest_id
      ```
6. celkové množství peněz všech npcs v jednotlivých městech
    - ```
      select f.city_name as "city name", sum(f.currency_gold) as "total gold" from (select npc.city_name, npc.currency_gold from npc join city on npc.city_name = city.city_name) as f group by f.city_name order by f.city_name
      ```


## Zdroje
- ČVUT. Data Modeler (Kreslítko). Online. BI-DBS. © 2023. Dostupné z: https://dbs.fit.cvut.cz/kreslitko. [cit. 2023-11-29].
- OPENAI. ChatGPT 3.5. ChatGPT [online]. © 2023 [cit. 2023-11-29]. Dostupné z: https://chat.openai.com/. Prompt: "dependency tree", "Make short description of this schema: [DDL script]".
- ČVUT. Zoo ve skluzu. Online. BI-DBS. Dostupné z: https://users.fit.cvut.cz/~hunkajir/dbs2/main.xml. [cit. 2023-11-29].
- ČZU. Databázové systémy. Online. Moodle. [2023]. Dostupné z: https://moodle.czu.cz/course/view.php?id=4078. [cit. 2023-11-29].
- PAVLÍČEK, Josef. SQLClient. Online. Dostupné z: https://psqlc.db.kii.pef.czu.cz/SQLCLient. [cit. 2023-11-29].
