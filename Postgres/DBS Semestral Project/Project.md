# Semestrální projekt - Databáze
## Popis
Jedná se databázový systém ke hře, ve které si vytvoříte postavu a vydáte se na dobrodružství.
Můžete prozkoumávat různá města, plnit úkoly a obchodovat s postavami.
Každý úkol má vlastní sadu předmětů, potřebnou získat ke splnění úkolu a získání odměny.
K získání úkolových předmětů se musíte vydat do divočiny a porazit různé nepřátele, ze kterých je můžete dostat.
Všechny předměty, které můžete získat, lze vyměnit s ostatními hráči na trhu.
Hra také zajišťuje, že hrajete skutečně vy, protože vyžaduje uživatelské jméno a heslo.

## Konceptuální schéma
![schéma](Conceptual%20schema.jpg)

## Diskuse smyček
Ve schématu se nachází smyčky (hodně).

Vzhledem k velkému množstvý smyček jsem se je rozhodl zakreslit do obrázku, jelikož vypsání všech možných kombinací není v mých silách.

![schéma_se_smyčkai](Cycles.jpg)

### Dependency tree
Schéma jsem se pokusil přetransformovat do jiné podoby a udělat jakoby dependency tree. S tím že ořežu entity které nejsou ve smyčce (třeba user je mimo smyčku) a nechám jen ty které ve smyčce jsou.

Na vrcholu jsou entity které na ničem nezávisí. Každý řádek tvoří jednu úroveň. Entity na jedné úrovni mohou být vytvořeny jen v případě že existují entity z vyší úrovně. Na sousedech a nižších úrovní nezáleží. Vazba *required* vždy směřuje nahoru. Cardinalita *Many* je vždy dole.

Vzhledem k tomu že tento graf bylo možné sestavit nevidím ve smyčkách problém.

![dependency_tree](dependency.jpg)
