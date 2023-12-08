# Semestrální projekt - UI Specifikace

Autor: Filip Digrín

## Motivace

Motivací k vytvoření této UI specifikace je potřeba návrhu rozhraní pro počítačovou hru,
která se prezentuje jako online RPG ve stylu hry Shakes and Fidget s důrazem na ekonomiku.
Uživatel si bude moci vytvořit postavu za kterou hraje, plnit různé úkoly, vydávat se na výpravy a obchodovat s itemy.

## Definice cíle

Hlavním cílem této hry je vylepšování postavy, plnění úkolů, porážení nepřátel, obchodování s předměty s NPC (Non-Player Character) i s jinými hráči.
Díky online serverům se ke hře lze připojit odkudkoliv, pomocí svého účtu.
Systém je realizovaný jako desktopová aplikace s okny.

### Postava

Uživatel bude moci získávat zkušenosti za splněné úkoly nebo poražení nepřátel, což mu umožní zvyšování úrovně a odemčení lepších předmětů a úkolů.

### Úkoly

Hra bude poskytovat seznam úkolů, které může hráč plnit.
Po úspěšném dokončení úkolu získá hráč odměny, jako jsou zkušenosti nebo herní měna.

### Souboje

Hra bude mít implementovaný bojový systém, kde hráč může čelit různým nepřátelům.

### Obchodování

Hra umožní hráčům obchodovat s virtuálními postavami v herním světě. NPC obchodníci budou nabízet různé předměty za herní měnu.
Kromě obchodování s NPC bude existovat možnost hráčského obchodování. Hráči budou moci nabízet své předměty ostatním.

### Přihlášení / Registrace

Hra bude vyžadovat, aby uživatel vytvořil účet nebo se přihlásil pomocí existujícího účtu. To umožní uživateli ukládat svůj herní postup a přistupovat k hře z různých zařízení.

## Persony

### Persona A

- Jméno: Alex Rodriguez
- Věk: 27 let
- Pohlaví: Muž
- Rodinný stav: Svobodný
- Zaměstnání: Vývojář softwaru
- Koníčky: hraní, kódování vedlejších projektů, návštěva herních kongresů
- Žánry hraných her: MMORPG, MOBA, střílečky z pohledu první osoby
- Typ počítače: Herní počítač vyšší třídy s nejnovější grafickou kartou a periferiemi

Přes den pracuje na dálku jako vývojář softwaru, večery tráví u online RPG. Rád zkoumá složité herní mechanismy a oceňuje dobře navržené uživatelské rozhraní, které umocňuje herní zážitek. V RPG hrách vyhledává především poutavý a náročný příběh.

### Persona B

- Jméno: Sarah Williamsová
- Věk: 34 let
- Pohlaví: žena
- Rodinný stav: Vdaná se dvěma dětmi
- Zaměstnání: Marketingová manažerka
- Záliby: Čtení fantasy románů, zahradničení, příležitostné hraní her
- Žánry hraných her: logické hry, příležitostné RPG hry
- Typ počítače: Notebook střední třídy pro práci i hraní her

Balancuje mezi náročnou prací v marketingu a rodinnými povinnostmi. Ráda si odpočine několika hodinami hraní po uložení dětí ke spánku. Preferuje hry s poutavým příběhem a vizuálně příjemnou estetikou. Oceňuje uživatelsky přívětivá rozhraní a zjednodušenou hratelnost, aby se přizpůsobil omezenému hernímu času. Zajímá ji kooperativní hraní a sociální interakce ve hře.

### Persona C

- Jméno: Richard Thompson
- Věk: 45 let
- Pohlaví: Muž
- Rodinný stav: Rozvedený
- Zaměstnání: IT konzultant
- Žánry hraných her: Zřídkakdy hraje hry, skeptický k online hrám a pohlcujícím zážitkům
- Typ počítače: Starý stolní počítač pro práci, skeptický k hernímu hardwaru

Většinu dne tráví řešením problémů s IT. Online RPG hry považuje za ztrátu času a zpochybňuje hodnotu pohlcujících herních zážitků. Dává přednost tradičním formám zábavy a brání se přijímání nových technologií v oblasti her. Obecně je skeptický k času a penězům vynaloženým na videohry.

---

## Use Case - Přihlašovací obrazovka

Uživatel se chce přihlásit do systému a očekává:
- zadání username
- zadaání hesla
- možnost si heslo zapamatovat

Pokud účet nemá očekává možnost registrace.

## Scénář - Přihlašovací obrazovka

- Systém zobrazí přihlašovací obrazovku a čeká na vyplnění údajů (username, heslo, popř. server)
a stisknutí talčítka pro přihlášení.
- Pokud uživatel údaje zadá špatně, systém uživatele informuje o nesprávnosti.
- Pokud se uživatel chce registrovat systém mu zobrazí formulář pro registraci.
- Po úspěšném přihlášení/reguistraci, zobrazí okno pro Výběr postavy.

## Logický design - Přihlašovací obrazovka

![login](logic%20design/Login.png)

## Use Case - Registrace

Uživatel nemá účet a chce se zaregistrovat. Očekává
- zadání svého jména (username)
- hesla

## Scénář - Registrace

- Systém zobrazí registrační okno s:
  - políčkem pro username
  - políčkem pro heslo
  - políčkem pro opětovné zadání hesla
  - a čeká na vyplnění požadavků a stisknutí tlačítka pro registraci.
  - Ve formuláři musí být zapsané username, který není prázdný a neobsahuje speciální znaky.
  - Obě hesla musejí být stejná
  - Heslo nesmí být prázdné a musí být alespoň 7 znaků
  - Musí být příjmuty podmínky použití a ochrana osobních údajů
- Pokud něco nevyhovuje sytém uživatele upozorní.
- Pokud je vše v pořádku sytém uživatele registruje a zobrazí okno pro výběr postavy

## Logický design - Registrace

![Register](logic%20design/Register.png)

## Use Case - Výběr postavy

Uživatel vstupuje do hry a chce:
- vidět své postavy
- vybrat si postavu
- vytvořit novou postavu
  - vybrat si *class* postavy
  - zadat jméno postavy
  - zrušit akci a vrátit se zpět na výběr postav
  - potvrdit akci
- po výběru / vytvoření postavy vstoupit do hry

## Scénář - Výběr postavy

- Systém uživateli zobrazí seznam postav s možností pro vybrání
- tlačítko pro vytvoření nové postavy
- Zobrazí hlášku, že uživatel žádnou postavu nemá

Při vytváření nové postavy
- ukáže tlačítko pro návrat do výběru postav
1. zobrazí seznam *class* ze kterých si uživatel může vyrat
2. vyzve uživatele pro zadání jména postavy
3. pokud taková postavu již existuje upozorní uživatele
4. vpustí uživatele do hry

## Logický design - Výběr postavy

![Character select](logic%20design/Characer%20select.png)

## Use Case - Přehled (Hlavní obrazovka / město)

Uživatel chce mít základní přehled o jeho prostředcích.
- jaký má jeho postava level
- kolik má peněz
- kde se nachází (město)

## Scénář - Přehled (Hlavní obrazovka / město)

Systém zobrazí:
- název aktuálního města kde se uživatel nachází
- level postavy a progress k dalšímu levelu
- kolik má uživatel herní měny
- tlačítko otevření detailu postavy

## Use Case - NPCs (Hlavní obrazovka / město)

Uživatel se chce dostat k různým postavám a funkcím samotné hry.
- očekává možnosti co může ve městě dělat
- možnost pro změnu postavy
- možnost pro odhlášení

## Scénář - NPCs (město)

Systém zobrazí:
- seznam postav, které se ve městě nacházejí s možností otevřít jejich nabídku.
- tlačíko pro otevření mapy světa
- tlačítko s menu, kde je možnost změny postavy, odhlášení se

## Logický design - Hlavní obrazovka (město)

![Main Window](logic%20design/Main%20Window.png)

## Use Case - Přehled postavy

Uživatel se chce:
- podívat na statistiky své postvy
- předměty které má u sebe
- statisktiky těchto předmětů

## Scénář - Přehled postavy

Systém uživateli zobrazí:
- výčet statistik jeho postavy
- předměty které má jeho postava nasazené
- předměty v inventáři

## Logický design - Přehled postavy

![Character](logic%20design/Character.png)

## Use Case - Svět

Uživatel očekává mapu světa na které uvidí místa kam může cestovat

## Scénář - Svět

Systém uživateli ukáže mapu světa na které uvidí místa kam může cestovat.
- po kliknutí na nějaké místo se uživatele zeptá jestli si je jistý
- při kladné odpovědi se zavře okno města a jeho podokna a ukáže se obrazovka s cesttováním

## Logický design - Svět

![World](logic%20design/Map.png)

## Use Case - Obchodník - Nákup

Uživatel chce:
- vidět co si může koupit
- cenu
- level, a další atributy
- provést nákup

## Scénář - Obchodník - Nákup

- Systém zobrazí nabídku předmětů k nákupu
- Umožní provést nákup klepnutím na předmět a potvrzením nákupu
- Umožní provést nákup přetažením předmětu do inventáře postavy

## Logický design - Obchodník - Nákup

![buy](logic%20design/NPC%20Buy.png)

## Use Case - Obchodník - Prodej

Uživatel chce prodat své předměty a vidět cenu, za kterou je může prodat

## Scénář - Obchodník - Prodej

- Systém uživateli poskytne pole kam může umístit předměty které chce prodat
- po vložení / vyjmutí předmětu se přepočítá cena za prodej
- tlačítko pro prodej tyto předměty prodá a systém hráči připíše herní měnu

## Logický design - Obchodník - Prodej

![sell](logic%20design/NPC%20Sell.png)

## Use Case - Obchodník - Úkoly

Uživatel chce vidět:
- seznam úkolů co daný obchodník (NPC) nabízí
- u každého úkolu vidět přibližně o čem je
- zda ho již přijmul nebo ještě ne
- zobrazit si podrobnosti

## Scénář - Obchodník - Úkoly

Systém uživateli zobrazí seznam úkolů.

Každý úkol zobrazuje:
- název
- krátků úrivek z popisku
- stav (accepted / not accepted)
- možnost otevřít podrobnosti

## Logický design - Obchodník - Úkoly

![quests](logic%20design/NPC%20Quests.png)

## Use Case - Podrobnosti úkolu

Uživatel se chce dozvědět o úkolu více a očekává:
- o čem úkol je
- požadavky na splnění
- odměnu
- možnost úkol příjmout nebo opustit

## Scénář - Podrobnosti úkolu

Systém uživateli zobrazí okno s podrobnostmi o úkolu
- název
- celý popisek
- požadované předměty, které musí získat pro splnění úkolu a jejich počet
- výší odběny ve zkušenostech a herní měně
- požadovaný level, aby úkol mohl přijmout
- tlačítko pro přijmutí / opuštění / dokončení úkolu (záleží na aktuálním stavu úkolu)

## Logický design - Podrobnosti úkolu

![quest details](logic%20design/Quest%20Details.png)

## Use Case - Trade Market

Uživatel chce:
- vidět předměty co ostatní nabízejí a za kolik
- koupit předmět
- prodat vlastní předmět

## Scénář - Trade Market

Systém zobrazí:
- seznam předmětů které je možné si koupit.
- po kliknutí zobrazí detail předmětu s jeho cenou a možností pro koupi
- políčko pro vložení vlastního předmětu
- políčko pro stanovení ceny
- tlačítko pro prodej

## Logický design - Trade Market

![trade market](logic%20design/Trade%20Market.png)

## Grafický design

![](graphic%20design/login.png)
![](graphic%20design/characterselect.png)
![](graphic%20design/city.png)
![](graphic%20design/character.png)
![](graphic%20design/buy.png)
![](graphic%20design/sell.png)
![](graphic%20design/quests.png)
![](graphic%20design/details.png)

## Figma Projekt

[figma.com](https://www.figma.com/file/whiAqWHlBIiq8EHYUEyMl8/MysticLegends-UI?type=design&node-id=0%3A1&mode=design&t=YWiBlLBAOVQU686F-1)

## Zdroje

- HAVLÍČEK, Tomáš. Bakalářská práce: Volné téma v oblasti User Experience Design. Online, vedoucí Josef Pavlíček. ČZU, 2014. Dostupné z: https://moodle.czu.cz/pluginfile.php/440218/mod_resource/content/0/UI%20specifikaceHavlicek.pdf. [cit. 2023-12-03].
- OPENAI. ChatGPT 3.5. [online]. ©2023 [cit. 2023-12-03]. Dostupné z: https://chat.openai.com/.
  Prompts: "Make 3 personas for UI design specification of a online rpg computer game. It should include their name, age, gender, marriage, job, hobbies, genres of played games, type of computer, description of usual day",
  "[snippet] můžeš rozvést jednotlivé body?"
