# Jak działa generowanie Helpa  
  
Wraz z uruchomieniem bota zostaje uruchomiony serwis, który zbiera dane o wszystkich komendach (klasy typu IBotCommand).  
Następnie informacje te są mapowane na `BotCommandInformation` (z uzupełnieniem kilku informacji na temat przyjmowanych argumentów przez daną komendę).  
Na koniec z obiektów `BotCommandInformation` tworzone są obiekty `HelpInformation`, które wpisywane są za pomocą domeny do bazy.  
  
W momencie wpisania przez użytkownika komendy `-help`, obiekty `HelpInformation` są pobierane z bazy (za pomocą domeny), a następnie na ich podstawie tworzona jest sformatowana wiadomość (jako Embed), która zostaje wysłana na kanał, z którego użytkownik wywołał komendę.  
  
W przypadku wywołania komendy `-help <nazwa_komendy>` np. `-help addrole` wyświetlana jest pomoc dla konkretnej jednej komendy wraz z oczekiwanymi parametrami i przykładowym użyciem (który może być wygenerowany przez `HelpExampleUsageGenerator`).  
  
## HelpDBGeneratorService  
  
Ten serwis jest wywoływany tuż po uruchomieniu bota (patrz: `Watchman.Discord.WatchmanBot.GetWorkflowBuilder`).  

```csharp
public Task FillDatabase(IEnumerable<BotCommandInformation> commandInfosFromAssembly)
```  

Przyjmuje wygenerowane obiekty `BotCommandInformation`. Opisy komend są filtrowane - zostają tylko te, których jeszcze nie ma w bazie (porównywane na podstawie nazwy metody).  
Nowe opisy wysyłane są do domeny (w niej odbywa się zapisanie do bazy).  
Przy okazji serwis również sprawdza, czy nie ma przestarzałych opisów (dane metody już nie istnieją w kodzie) i ostrzega o nich (za pomocą `Log.Warning`).  
  
## BotCommandInformationFactory  
  
Fabryka mapująca `BotCommand` na informacje potrzebne dla `HelpInformation`.  

```csharp
public BotCommandInformation Create(Type controller)
```  

Wyciąga wszystkie properties z `IBotCommand` i tworzy na ich podstawie obiekt `BotCommandInformation`.  
  
## HelpDataCollectorService  
  
Serwis zajmujący się pobraniem komend z projektu za pomocą refleksji wyszukując po typie `IBotCommand`.  

```csharp
public IEnumerable<BotCommandInformation> GetCommandsInfo(Assembly botAssembly)
```  
  
## HelpMessageGeneratorService  
  
Serwis ma za zadanie przetworzyć suche obiekty z bazy na wiadomość do wyświetlenia użytkownikowi.  

```csharp
public IEnumerable<KeyValuePair<string, string>> MapHelpForAllCommandsToEmbed(IEnumerable<HelpInformation> helpInformations, DiscordServerContext server)
```  

Metoda ta tworzy listę opisów każdej komendy z obiektów `HelpInformation` pobranych z bazy.  

```csharp
public IEnumerable<KeyValuePair<string, string>> MapHelpForOneCommandToEmbed(HelpInformation helpInformation, DiscordServerContext server)
```

Metoda ta tworzy opis dla jednej komendy na podstawie obiektu `HelpInformation` pobranego z bazy.  
Jeśli przykład użycia w `HelpInformation` jest pusty to metoda skorzysta z klasy `HelpExampleUsageGenerator` aby wygenerować automatycznie taki przykład.  
  
## HelpExampleUsageGenerator  
  
Serwis generujący domyślny przykład użycia danej komendy, na podstawie parametrów jakie ta komenda posiada.  