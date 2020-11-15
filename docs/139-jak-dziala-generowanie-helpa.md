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
  
Serwis zajmujący się pobraniem komend z projektu wyszukując po typie `IBotCommand`.  

```csharp
public IEnumerable<BotCommandInformation> GetCommandsInfo(Assembly botAssembly)
```  
  
## HelpMessageGeneratorService  
  
Serwis ma za zadanie przetworzyć suche obiekty z bazy na tekst do wyświetlenia użytkownikowi.  

```csharp
public IEnumerable<KeyValuePair<string, string>> MapToEmbedInput(IEnumerable<HelpInformation> helpInformations)
```  

Metoda ta tworzy listę opisów każdej komendy z obiektów `HelpInformation` (obiekt ten pochodzi bezpośrednio z fabryki tworzącej `HelpInformation` na podstawie `CommandInfo`, ponieważ to nim posługujemy się z domenie, np. aby zapisać do bazy).
