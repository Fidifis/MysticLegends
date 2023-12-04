# Semestrální projekt - Komponentová tvorba
Autor: Filip Digrín

## Jak spustit klienta
Projekt klienta se nachází v [MysticLegendsClient](MysticLegendsClient/)
1. build ve Visual Studiu
2. zapnout

## Jak spustit server
Projekt serveru se nachází v [MysticLegendsServer](MysticLegendsServer/)

### Možnosti spuštění
- lokálně - vyžaduje connection string k databázi
- v dockeru - vyžaduje docker, databáze je součástí docker-compose
- cloud - na den zápočtu a zkoušky spustím server v cloudu. Bude vám stačit klient a připojení k internetu. Pokud bude potřeba můžu na vyžádání server spustit kdykoli.

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
