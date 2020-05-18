# Jak działa generowanie Helpa  
  
Opisane niżej klasy możesz znaleźć w projekcie `Watchman.Discord`, w ścieżce `Areas/Help/`  
Namespace: `Watchman.Discord.Areas.Help.Services`, `Watchman.Discord.Areas.Help.Factories` i `Devscord.DiscordFramework.Services`  
  
### Podsumowanie:  
  
Wraz z uruchomieniem bota zostaje uruchomiony serwis z Frameworka, który zbiera dane o wszystkich kontrolerach.  
Następnie informacje te są mapowane na `CommandInfo` (z uzupełnieniem domyślnych opisów), a na sam koniec wpisywane za pomocą domeny jako `HelpInformation` do bazy.  
W momencie zapytania o Help przez użytkownika, obiekty `HelpInformation` są pobierane z bazy (za pomocą domeny), a następnie na ich podstawie tworzona jest sformatowana wiadomość (jako Embed) i zostaje wysłana na kanał, z którego użytkownik wywołał komendę.  
  
## HelpDBGeneratorService  
  
Ten serwis jest wywoływany tuż po uruchomieniu bota (patrz:  `Watchman.Discord.WatchmanBot/GetWorkflowBuilder`).  
```csharp
public async Task FillDatabase(IEnumerable<CommandInfo> commandInfosFromAssembly)
```  
Przyjmuje wygenerowane obiekty CommandInfo. Opisy komend są filtrowane - zostają tylko te, których jeszcze nie ma w bazie (porównywane na podstawie nazwy metody).  
Nowe opisy wysyłane są do domeny (w niej odbywa się zapisanie do bazy).  
Przy okazji serwis również sprawdza, czy nie ma przestarzałych opisów (dane metody już nie istnieją w kodzie) i ostrzega o nich (za pomocą `Log.Warning`).  
  
## CommandsInfoFactory  
  
Fabryka mapująca kontrolery na informacje dla Helpa.  
```csharp
public IEnumerable<CommandInfo> Create(Type controller)
```  
Wyciąga wszystkie metody z atrybutem `DiscordCommand` i tworzy na ich podstawie obiekty `CommandInfo` (jedna metoda = jeden CommandInfo).  
Opis komendy uzupełniany jest domyślną wartością ("Default").  
  
## HelpDataCollectorService  
  
Serwis zajmujący się zmapowaniem komend na obiekty `CommandInfo` (patrz: `Devscord.DiscordFramework.Services.HelpDataCollectorService`).  
```csharp
public IEnumerable<CommandInfo> GetCommandsInfo(Assembly botAssembly)
```  
Metoda ta najpierw wyszukuje wszystkie kontrolery, a następnie przekazuje je do `CommandsInfoFactory` oczekując obiektów CommandInfo, zawierających informacje o wszystkich komendach (`DiscordCommand`) we wszystkich kontrolerach.  
  
## HelpMessageGeneratorService  
  
Serwis ma za zadanie przetworzyć suche obiekty z bazy na tekst do wyświetlenia użytkownikowi.  
```csharp
public IEnumerable<KeyValuePair<string, string>> MapToEmbedInput(IEnumerable<HelpInformation> helpInformations)
```  
Metoda ta tworzy listę opisów każdej komendy z obiektów `HelpInformation` (obiekt ten pochodzi bezpośrednio z fabryki tworzącej `HelpInformation` na podstawie `CommandInfo`, ponieważ to nim posługujemy się z domenie, np. aby zapisać do bazy).
