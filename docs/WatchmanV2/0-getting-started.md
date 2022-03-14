# Getting started

## Potrzebne narzędzia

- .NET 6.0
- MongoDB
- Konto discord z włączonym trybem developera (pozwala na kopiowanie ID serwerów/kanałów/użytkowników)
- Własny serwer discord z stworzoną instancją bota (polecam skorzystać z [Dokumentacji od Discorda](https://discord.com/developers/docs/intro)

## Konfiguracja własnego serwera discord

1. Stwórz kanał `debug-logs`
2. Stwórz kanał zawierający słowo `test`, przykładowo `test1` lub po prostu `test`

**WAŻNE!!**
Tryb `Debug` w którym będziesz domyślnie testować aplikacje, obsługuje jedynie te komendy, które są na kanale z `test` w nazwie.

## Konfiguracja Watchmana

1. Wchodzimy w Watchman.Web
2. Tworzymy pliki `appsettings.json` i `appsettings.Development.json`

Ich struktura powinna być identyczna, `appsettings.json` jest używany jeśli odpalamy aplikacje w trybie `release`, a `appsettings.Development.json` jeśli w trybie `debug` (domyślny).

Plik `appsettings` uzupełniamy
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "ExceptionServerId": 123,
    "ExceptionChannelId": 123
  },
  "ConnectionStrings": {
    "Mongo": "mongodb://localhost:27017/devscord",
    "Lite": "watchman.db"
  },
  "Discord": {
    "Token": "",
    "SendOnlyUnknownExceptionInfo":  true
  }
}
```
- `ExceptionServerId` - tutaj wpisujesz ID swojego serwera
- `ExceptionChannelId` - tutaj wpisujesz ID swojego kanału `debug-logs` (nazwa kanału nie ma znaczenia)
- `Token` - tutaj wpisujesz token bota, informację jak go stworzyć i dodać na swój serwer możesz znaleźć w [dokumentacji discorda](https://discord.com/developers/docs/intro)
- `SendOnlyUnknownExceptionInfo` (bool) - określasz czy do `debug-logs` mają trafiać wszystkie błędy, w tym te które dotyczą przykładowo błędnie wpisanej komendy przez użytkownika (true = ignorowanie błędów użytkownika)

## To wszystko

Teraz pozostało odpalić aplikację, upewnij się że projekt który chcesz odpalić to `Watchman.Web`. Jeśli po drodze natrafiłeś na jakieś niejasności, napisz nam o tym na naszym serwerze discord [Zaproszenie](https://discord.gg/w2NtcJB).

Aby sprawdzić czy bot na pewno działa prawidłowo, wpisz komendę `-help` na swoim kanale `test`, po tym jak zauważysz na swoim serwerze discord, że bot jest aktywny.
