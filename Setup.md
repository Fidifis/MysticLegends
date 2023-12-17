# Semestrální projekt - Komponentová tvorba
Autor: Filip Digrín

## Jak spustit klienta
Projekt klienta se nachází v [MysticLegendsClient](MysticLegendsClient/)
- Pro fungování programu je potřeba mít stažený obsah z git LFS. Jinak bude padat při načítání obrázků.
- Doporučuji stáhnout source code z [releases](https://github.com/Fidifis/MysticLegends/releases)
- Nebo mít nainstalovaný Git + LFS a spustit `git clone https://github.com/Fidifis/MysticLegends.git`

## Jak spustit server
Projekt serveru se nachází v [MysticLegendsServer](MysticLegendsServer/)

### Možnosti spuštění
- lokálně - vyžaduje connection string k databázi
- v dockeru - vyžaduje docker, databáze je součástí docker-compose
- cloud - server běží i v cloudu. Bude vám stačit klient a připojení k internetu. Pokud zrovna neběží můžu na vyžádání server spustit kdykoliv.
  Status je možno ověřit [zde](https://mysticlegends.fidifis.com/api/health)

### Spuštění v dockeru
Stačí pustit příkaz a server s databází se spustí sám (\**magick*\*)
```
docker compose up
```

### Nastavení connection pro databázi
V případě lokálního spuštění je potřeba vytvořit connection string podle zde uvedeného vzoru a vložit správné heslo.

```
Server=db.kii.pef.czu.cz;Port=5432;Database=xdigf001;User Id=xdigf001;Password=pastePasswordHere;SSL Mode=prefer;Pooling=true;MaxPoolSize=3
```

Tento connection string je potřeba jako environment variable se jménem `CONNECTIONSTRING`

Nebo ve Visual Studiu zadat do Connected services (pod projektem) do Secrets.json:

```json
{
  "ConnectionStrings:GameDB": "Server=db.kii.pef.czu.cz;Port=5432;Database=xdigf001;User Id=xdigf001;Password=pastePasswordHere;SSL Mode=prefer;Pooling=true;MaxPoolSize=3"
}
```
