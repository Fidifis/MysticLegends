# Semestrální projekt - UI Specifikace
Autor: Filip Digrín

## Motivace
Motivací k vytvoření této UI specifikace je potřeba návrhu rozhraní pro počítačovou hru,
která se prezentuje jako online RPG ve stylu hry Shakes and Fidget s důrazem na ekonomiku.
Uživatel si bude moci vytvořit postavu za kterou hraje, plnit různé úkoly, vydávat se na výpravy a obchodovat s itemy.

## Definice cíle
Hlavním cílem této hry je vylepšování postavy, plnění úkolů, obchodování s předměty s NPC (Non-Player Character) i s jinými hráči.
Díky online serverům se ke hře lze připojit odkudkoliv, pomocí svého účtu.
Systém je realizovaný jako desktopová aplikace s okny.

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

## Scénář - Hlavní obrazovka (město)

## Logický design - Hlavní obrazovka (město)

## Use Case - Přehled postavy

## Scénář - Přehled postavy

## Logický design - Přehled postavy

## Use Case - Svět

## Scénář - Svět

## Logický design - Svět

## Use Case - Obchodník - Nákup

## Scénář - Obchodník - Nákup

## Logický design - Obchodník - Nákup

## Use Case - Obchodník - Prodej

## Scénář - Obchodník - Prodej

## Logický design - Obchodník - Prodej

## Use Case - Obchodník - Úkoly

## Scénář - Obchodník - Úkoly

## Logický design - Obchodník - Úkoly

## Use Case - Trade Market

## Scénář - Trade Market

## Logický design - Trade Market

## Grafický design

## Zdroje
- HAVLÍČEK, Tomáš. Bakalářská práce: Volné téma v oblasti User Experience Design. Online, vedoucí Josef Pavlíček. ČZU, 2014. Dostupné z: https://moodle.czu.cz/pluginfile.php/440218/mod_resource/content/0/UI%20specifikaceHavlicek.pdf. [cit. 2023-12-03].
- OPENAI. ChatGPT 3.5. [online]. ©2023 [cit. 2023-12-03]. Dostupné z: https://chat.openai.com/.
  Prompts: "Make 3 personas for UI design specification of a online rpg computer game. It should include their name, age, gender, marriage, job, hobbies, genres of played games, type of computer, description of usual day"
