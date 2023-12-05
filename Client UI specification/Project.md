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
- Jméno: Alex Martinez
- Věk: 28 let
- Pohlaví: Muž:
- Rodinný stav: Svobodný
- Zaměstnání: Zaměstnanec: Vývojář softwaru
- Koníčky: Vývojářský programátor: Bojová umění, čtení fantasy románů, streamování na Twitchi
- Žánry hraných her: RPG, Strategické hry
- Typ počítače: Herní počítač vyšší třídy

Alex přes den pracuje jako softwarový vývojář.
Po večerech cvičí bojová umění a pro inspiraci čte fantasy romány.
Večery tráví streamováním svých herních seancí na Twitchi, kde se věnuje svému publiku.
Alex má rád pohlcující RPG a strategické hry, které jsou výzvou pro jeho intelekt.
Jeho herní seance se obvykle zaměřují na plnění náročných úkolů a prozkoumávání bohatých herních příběhů.

### Persona B
- Jméno: Emily Johnson
- Věk: 22 let
- Pohlaví: Žena
- Rodinný stav: Ve vztahu
- Zaměstnání: Studentka vysoké školy (informatika)
- Záliby: Digitální umění, Cosplay, Hra na housle
- Žánry hraných her: MMORPG, Sandboxové hry
- Typ počítače: Herní notebook střední třídy

Emily je studentka vysoké školy, která studuje informatiku.
Její dny jsou naplněny výukou, ale vždy si najde čas na své kreativní aktivity, jako je digitální umění a cosplay.
Po večerech ráda hraje MMORPG a ponořuje se do rozsáhlých virtuálních světů.
Emily oceňuje hry, které jí umožňují vyjádřit svou kreativitu a spojit se s ostatními hráči v rámci společenského života.

### Persona C
- Jméno: James Rodriguez
- Věk: 35 let
- Pohlaví: Muž
- Rodinný stav: ženatý, dvě děti
- Zaměstnání: IT konzultant
- Záliby: Elektronika pro kutily, stolní hry, jízda na kole
- Žánry hraných her: Taktické RPG hry, Simulační hry
- Typ počítače: Herní počítač na zakázku

James vyvažuje svou kariéru IT konzultanta s rodinným životem.
Rád tráví víkendy v přírodě, jezdí s rodinou na kole a hraje deskové hry.
Po večerech se uchyluje do svého herního doupěte, kde relaxuje u taktických RPG a simulačních her.
James si cení her, které nabízejí strategickou hloubku a pocit úspěchu.
Jeho herní seance jsou kombinací sólových dobrodružství a spolupráce s přáteli ve více hráčích.

---

> Všechny **use cases** a **scénáře** implicitně předpokládají **základní ovládání** a chování okna v běžném desktopovém operačním systému (Windows, Mac, GNU/Linux s GUI),
tj. minimalizace, úprava velikosti, zavření okna.

## Use Case - Přihlašovací obrazovka
Uživatel se chce přihlásit do systému a očekává:
- zadání username
- zadaání hesla
- možnost si heslo zapamatovat
- tlačítko pro přihlášení

Pokud účet nemá očekává možnost registrace.

## Scénář - Přihlašovací obrazovka
- Systém zobrazí přihlašovací obrazovku a čeká na vyplnění požadavků a stisknutí talčítka pro přihlášení.
- Pokud uživatel údaje zadá špatně, systém uživatele informuje o nesprávnosti.
- Pokud se uživatel chce registrovat systém mu zobrazí formulář pro registraci.
- Po úspěšném přihlášení/reguistraci, zobrazí okno pro Výběr postavy.

## Logický design - Přihlašovací obrazovka

## Use Case - Registrace
Uživatel nemá účet a chce se zaregistrovat. Očekává
- zadání svého jména (username)
- hesla
- tlačítko pro registraci

## Scénář - Registrace
- Systém zobrazí registrační okno s:
  - políčkem pro username
  - políčkem pro heslo
  - políčkem pro opětovné zadání hesla
  - a čeká na vyplnění požadavků a stisknutí tlačítka pro registraci.
  - Ve formuláři musí být zapsané username, který není prázdný a neobsahuje speciální znaky.
  - Obě hesla musejí být stejná
  - Heslo nesmí být prázdné a musí být alespoň 7 znaků
- Pokud něco nevyhovuje sytém uživatele upozorní.
- Pokud je vše v pořádku sytém uživatele registruje a zobrazí okno pro výběr postavy

## Logický design - Registrace

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

## Use Case - Hlavní obrazovka (město)
Uživatel chce mít základní přehled o jeho prostředcích.
- jaký má jeho postava level
- kolik má peněz
- kde se nachází (město)

Dále se chce dostat k různým postavám a funkcím samotné hry.
- očekává možnosti co může ve městě dělat
- tlačítko pro změnu postavy
- tlačítko pro odhlášení

## Scénář - Hlavní obrazovka (město)
Systém zobrazí hlavní okno. Pokud je toto okno zavřeno křížkem, celý program končí včetně podoken.
- zobrazuje název aktuálního města kde se uživatel nachází
- level postavy a progress k dalšímu levelu
- kolik má uživatel herní měny
- seznam postav, které se ve městě nacházejí s možností otevřít jejich nabídku.
- tlačíko pro otevření mapy světa
- tlačítko otevření detailu postavy
- tlačítko s menu, kde je možnost změny postavy, odhlášení se

## Logický design - Hlavní obrazovka (město)

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

## Use Case - Svět
Uživatel očekává mapu světa na které uvidí místa kam může cestovat

## Scénář - Svět
Systém uživateli ukáže mapu světa na které uvidí místa kam může cestovat.
- po kliknutí na nějaké místo se uživatele zeptá jestli si je jistý
- při kladné odpovědi se zavře okno města a jeho podokna a ukáže se obrazovka s cesttováním

## Logický design - Svět

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

## Use Case - Obchodník - Prodej
Uživatel chce prodat své předměty a vidět cenu, za kterou je může prodat

## Scénář - Obchodník - Prodej
- Systém uživateli poskytne pole kam může umístit předměty které chce prodat
- po vložení / vyjmutí předmětu se přepočítá cena za prodej
- tlačítko pro prodej tyto předměty prodá a systém hráči připíše herní měnu

## Logický design - Obchodník - Prodej

## Use Case - Obchodník - Úkoly
Uživatel chce vidět:
- seznam úkolů co daný obchodník (NPC) nabízí
- u každého úkolu vidět přibližně o čem je
- zda ho již přijmul nebo ještě ne
- rozkliknout si podrobnosti

## Scénář - Obchodník - Úkoly
Systém uživateli zobrazí seznam úkolů.

Každý úkol zobrazuje:
- název
- krátků úrivek z popisku
- stav (accepted / not accepted)
- možnost otevřít podrobnosti

## Logický design - Obchodník - Úkoly

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

## Use Case - Trade Market

## Scénář - Trade Market

## Logický design - Trade Market

## Grafický design

## Zdroje
- HAVLÍČEK, Tomáš. Bakalářská práce: Volné téma v oblasti User Experience Design. Online, vedoucí Josef Pavlíček. ČZU, 2014. Dostupné z: https://moodle.czu.cz/pluginfile.php/440218/mod_resource/content/0/UI%20specifikaceHavlicek.pdf. [cit. 2023-12-03].
- OPENAI. ChatGPT 3.5. [online]. ©2023 [cit. 2023-12-03]. Dostupné z: https://chat.openai.com/.
  Prompts: "Make 3 personas for UI design specification of a online rpg computer game. It should include their name, age, gender, marriage, job, hobbies, genres of played games, type of computer, description of usual day",
  "[snippet] můžeš rozvést jednotlivé body?"
