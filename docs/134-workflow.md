# Jak działa workflow  

Workflow znajdziecie w projekcie Devscord.DiscordFramework  
W namespace: Devscord.DiscordFramework  

## WorkflowBuilder  

Zanim zaczniemy omawiać czym tak na prawde jest klasa Workflow, omówmy w jaki sposób można ją stworzyć.  

Workflow builder - jak nazwa wskazuje, to klasa służąca do pełnego skonfigurowania klasy Workflow przed jej użyciem.  
Jest to ważne, ponieważ klasa Workflow jest skomplikowana i konfigurują wszystko ręcznie, łatwo zapomnieć o istotnych elementach - nasz WorkflowBuilder pilnuje, żeby Workflow zostało stworzone w prawidłowy sposób.  

Przykład użycia WorkflowBuildera  
```csharp  
WorkflowBuilder.Create(_configuration.Token, this._context, typeof(WatchmanBot).Assembly)  
    .SetDefaultMiddlewares()  
    .AddOnReadyHandlers(builder =>  
    {  
        builder  
            .AddHandler(() => Task.Run(() => Log.Information("Bot started and logged in...")))  
            .AddFromIoC<HelpDataCollectorService, HelpDBGeneratorService>((dataCollector, helpService) => () =>  
            {  
                Task.Run(() => helpService.FillDatabase(dataCollector.GetCommandsInfo(typeof(WatchmanBot).Assembly)));  
                return Task.CompletedTask;  
            })  
            .AddFromIoC<UnmutingExpiredMuteEventsService, DiscordServersService>((unmutingService, serversService) => async () =>  
            {  
                var servers = (await serversService.GetDiscordServers()).ToList();  
                servers.ForEach(unmutingService.UnmuteUsersInit);  
            })  
            .AddFromIoC<CyclicStatisticsGeneratorService>(cyclicStatsGenerator => () =>  
            {  
                _ = cyclicStatsGenerator.StartGeneratingStatsCacheEveryday();  
                return Task.CompletedTask;  
            })  
            .AddFromIoC<ResponsesInitService>(responsesService => async () =>  
            {  
                await responsesService.InitNewResponsesFromResources();  
            })  
            .AddFromIoC<InitializationService, DiscordServersService>((initService, serversService) => () =>  
            {  
                var stopwatch = Stopwatch.StartNew();  

                var servers = serversService.GetDiscordServers().Result;  
                Task.WaitAll(servers.Select(async server =>  
                {  
                    Log.Information($"Initializing server: {server.Name}");  
                    await initService.InitServer(server);  
                    Log.Information($"Done server: {server.Name}");  
                }).ToArray());  

                Log.Information(stopwatch.ElapsedMilliseconds.ToString());  
                return Task.CompletedTask;  
            })  
            .AddHandler(() => Task.Run(() => Log.Information("Bot has done every Ready tasks.")));  
    })  
    .AddOnUserJoinedHandlers(builder =>  
    {  
        builder  
            .AddFromIoC<WelcomeUserService>(x => x.WelcomeUser)  
            .AddFromIoC<MutingRejoinedUsersService>(x => x.MuteAgainIfNeeded);  
    })  
    .AddOnDiscordServerAddedBot(builder =>  
    {  
        builder  
            .AddFromIoC<InitializationService>(initService => initService.InitServer);  
    })  
    .AddOnWorkflowExceptionHandlers(builder =>  
    {  
        builder  
            .AddFromIoC<ExceptionHandlerService>(x => x.LogException)  
            .AddHandler(this.PrintDebugExceptionInfo, onlyOnDebug: true)  
            .AddHandler(this.PrintExceptionOnConsole);  
    })  
    .Build();  
```  
(Przykład to kopiuj-wklej z kodu Watchmana, wersja z dnia 6 maja 2020)  
Prawdopodobnie dla osoby która pierwszy raz widzi coś takiego, ten kod może wyglądać strasznie, ale w rzeczywistości jest bardzo czytelny i przejrzysty - dokładnie widać w jaki sposób mamy wszystko skonfigurowane.  

Omówmy sobie ten kod krok po kroku.  
#### Tworzenie obiektu  
```csharp  
public static WorkflowBuilder Create(string token, IComponentContext context, Assembly botAssembly)  
```  
W tej linii tworzymy podstawowy obiekt naszego bota, przekazujemy mu wszystkie informacje które są absolutnie niezbędne do stworzenia go.  
Metoda Create() nie jest niczym innym, jak odpaleniem prywatnego konstruktora. Ten mechanizm to jedynie zabieg stylistyczny.  
Przekazawywane parametry to  
`token` - token bota discordowego, po stworzeniu własnego bota na stronie discorda, mamy dostęp do jego tokenu.  
Bota możemy stworzyć tutaj [Discord Developer Portal](https://discordapp.com/developers/applications).  
`context` - obiekt za pomocą którego jesteśmy w stanie pobierać instancje zarejestrowanych typów, z kontenera IoC.  

#### Ustawianie Middlewares  
```csharp  
public WorkflowBuilder SetDefaultMiddlewares()  
```  
Aktualnie WorkflowBuilder nie posiada opcji ustawiania niestandardowych middlewares.   
Istnieją jedynie domyślne w frameworku takie jak `ChannelMiddleware` `ServerMiddleware` `UserMiddleware`.  
Middlewares służą do zbierania informacji na temat tego jakie dane zawiera  wiadomość (z którego kanału jest wysłana, przez jakiego użytkownika itd).  

#### Ustawianie handlerów  
Do ustawiania handlerów służy wiele metod, jednak wszystkie z nich działają według tego samego schematu.  
Jako handlery rozumiemy metody/funkcje, które mają się wykonać kiedy zajdzie określone zdarzenie.  
Przykładowo kiedy nowy użytkownik dołączy do serwera, kiedy w aplikacji wystąpi wyjątek, albo kiedy w aplikacji wystąpi wyjątek.  

To w której sytuacji się wykonają, jesteśmy w stanie rozpoznać na podstawie nazwy metody.  
Lista dostępnych handlerów:  
```csharp  
public WorkflowBuilder AddOnReadyHandlers(Action<WorkflowBuilderHandlers<Func<Task>>> action)  
```  
Do dodawania akcji, które mają stać się jak status połączenia bota z discordem będzie na poziomie `Ready`.  
```csharp  
public WorkflowBuilder AddOnUserJoinedHandlers(Action<WorkflowBuilderHandlers<Func<Contexts, Task>>> action)  
```  
Do dodawania akcji, które mają się stać jak do serwera dołączy nowy użytkownik.  
```csharp  
public WorkflowBuilder AddOnMessageReceivedHandlers(Action<WorkflowBuilderHandlers<Func<SocketMessage, Task>>> action)  
```  
Do dodawania akcji, które mają się stać jak któryś z użytkowników wyśle wiadomość.  
```csharp  
public WorkflowBuilder AddOnDiscordServerAddedBot(Action<WorkflowBuilderHandlers<Func<DiscordServerContext, Task>>> action)  
```  
Do dodawania akcji, która mają się stać jak bot zostanie dodany do nowego serwera.  
```csharp  
public WorkflowBuilder AddOnWorkflowExceptionHandlers(Action<WorkflowBuilderHandlers<Action<Exception, Contexts>>> action)  
```  
Do dodawania akcji, która mają się stać jak wewnątrz aplikacji zostanie wyrzucony wyjątek.  

Sposób używania tych metod jest prosty, przekazujemy tam referencję do metody (najczęściej jako funkcję anonimową), której parametry są zależne od tego, co chcemy obsłużyć.  
Przykładowo `AddOnWorkflowExceptionHandlers` będzie dostarczać informacje o wyjątku a `AddOnMessageReceivedHandlers` będzie dostarczać informacje o wiadomości.  

W zbudowaniu prawidłowego handlera pomaga nam klasa `WorkflowBuilderHandlers` posiadająca wewnątrz dostęp do kontenera IoC, co pozwala nam na proste wyciąganie typów z niego.  
Wewnątrz wspomnianej klasy znajdują się metody  
```csharp  
public WorkflowBuilderHandlers<T> AddHandler(T handler, bool onlyOnDebug = false)  
```  
Do dodawania handlera, który ma zwracać wynik o typie T, gdzie T to typ od którego klasa WorkflowBuilderHandlers jest generyczna.  
Przykładowo w przypadku `WorkflowBuilderHandlers<Func<SocketMessage, Task>>` typem T będzie `Func<SocketMessage, Task>`, tak więc powinniśmy przekazać jako handler typ `Func<SocketMessage, Task>`.  
Drugi parametr to informacja czy nasz handler ma być uruchamiany jedynie, kiedy aplikacja jest uruchomiona w trybie `debug`.  
```csharp  
public WorkflowBuilderHandlers<T> AddFromIoC<A>(Func<A, T> func, bool onlyOnDebug = false)  
```  
Do ruchamiania wcześniej wspomnianej metody `AddHandler`, jednak dodatkowo pozwala na operowanie na typie pobranym z kontenera IoC.  

#### Budowanie   
Za pomocą metody   
```csharp  
public WorkflowBuilder Build()  
```  
Jesteśmy w stanie przekazać ustawioną wcześniej konfiguracje, do obiektu Workflow - dzięki czemu będzie dokładnie wiedzieć, w jaki sposób powinno reagować na określone czynniki zewnętrzne.  

Istnieje jeszcze metoda  
```csharp  
public async Task Run()  
```  
Jednak jest to jedynie element estetyczny.   
Jeśli bot jest jedyną działajacą aplikacją (a nie jest jedynie aplikacja działającą w tle), pozwala ona na wieczne czekanie, do momentu aż bot przestanie działać.  
Ma to sprawić, że - przykładowo - aplikacja konsolowa nie wyłączy się chwilę po uruchomieniu bota.  

## Workflow  

Klasa Workflow służy do zarządzania botem.  
Jak nazwa wskazuje - steruje ona przepływem danych w aplikacji wysokopoziomowo, a prostszymi słowami - steruje operacjami, które wykonują się od chwili odebrania informacji o zdarzeniu na serwerze discorda, do chwili kiedy kontroler otrzymuje gotowe, opakowane dane, które są proste do obsłużenia.  

Pozwala ona na zapisanie handlerów skonfigurowanych w WorkflowBuilderze w taki sposób, aby były odpalane wtedy, kiedy takie zdarzenie wystąpi.  

Zawiera metody  
```csharp  
internal void Initialize()  
```  
Przypisuje handlery, które są domyślne w Workflow i powinny być zawsze przypisane.  
Przykładowo metoda do odbierania wiadomości przez Workflow, żeby Workflow mogło je przetwarzać.  
```csharp  
internal void MapHandlers(DiscordSocketClient client)  
```  
Przypisuje handlery ustawione przez WorkflowBuildera oraz domyślne z Workflow do klienta discorda => w tym przypadku klienta z biblioteki Discord.NET którą się wspomagamy, jednak staramy się być od niej jak najbardziej niezależni, co pozwala na prostą podmianę kodu w kluczowych miejscach.  
```csharp  
private async void MessageReceived(SocketMessage socketMessage)  
```  
Odbiera i przetwarza wiadomości  
Przygotowuje dane dla klasy `ControllersService`, w której uruchamia metodę `Run`, której przekazuje przygotowane wcześniej dane.  
Te dane to   
`DiscordRequest` - szczegółowe informacje o komendzie wyciągnięte z wiadomości  
`Contexts` - zestaw metadanych wiadomości, które zdobyły `Middlewares`  
```csharp  
private DiscordRequest ParseRequest(SocketMessage socketMessage)  
```  
Używa klasy `CommandParser` w celu zdobycia szczegółowych informacji o komendzie.  
```csharp  
private Contexts GetContexts(SocketMessage socketMessage)  
```  
Używa klasy `MiddlewaresService` w celu zdobycia obiektu `Contexts` - zestawu informacji o wiadomości (z którego kanału wysłana, na którym serwerze, przez którego użytkownika itd).  

Dodatkowo Workflow zawiera metody  
```csharp  
private bool ShouldIgnoreMessage(SocketMessage socketMessage)  
```  
Lista warunków pozwalająca na sprawdzenie czy bot powinien obsłużyć wiadomość.  
Nie powinien obsługiwać wiadomości wysłanych na kanałach `test`, `logs` oraz wiadomości wysłanych przez bota.  
```csharp  
private async Task CallUserJoined(SocketGuildUser guildUser)  
```  
Przygotowuje dane dla handlera oczekującego na dołączenie nowego użytkownika, a następnie odpala go.  
```csharp  
private async Task CallServerAddedBot(SocketGuild guild)  
```  
Przygotowuje dane dla handlera oczekującego na dodanie bota do nowego serwera, a następnie odpala go.  
(Chodzi o sytuacje gdzie bot jest dodany do innego serwera, a nie gdzie obecny serwer doda kolejnego bota - ta nazwa może być myląca).  

## CommandParser  

Klasa służąca do rozpoznania czy wiadomość jest komendą, a następnie parsuje ją na obiekt `DiscordRequest` - posiadający informacje o komendzie (nazwa, prefix, argumenty).  
Posiada metodę  
```csharp  
public DiscordRequest Parse(string message, DateTime sentAt)  
```  
Zbiera informacje o komendzie a następnie je zwraca.  

## MiddlewaresService  

Klasa gromadząca listę `Middlewares` - czyli klas z interfejsem `IMiddleware`, które posiadają metodę  
```csharp  
IDiscordContext Process(SocketMessage data);  
```  
Zwraca informacje o wiadomości z discorda. To jakie informacje zwraca jest dowolne, określone przez obiekt `Middleware`.  

Zawiera metody  
```csharp  
public void AddMiddleware<T>() where T : IMiddleware  
```  
Do dodawania Middleware.  
```csharp  
public Contexts RunMiddlewares(SocketMessage socketMessage)  
```  
Do odpalania wszystkich Middlewares, które zwracają dane na podstawie danych z `SocketMessage` - typu udostępnianego przez bibliotekę Discord.NET.  
Następnie wyniki są pakowane do obiektu `Contexts` - który jest zbiorem wyników Middlewares.  

## ControllersService  

Serwis który decyduje która metoda którego kontrolera powinna zostać uruchomiona, na podstawie wcześniej zebranych informacji.  
Zawiera metody  
```csharp  
public async Task Run(DiscordRequest request, Contexts contexts)  
```  
Na podstawie informacji z `DiscordRequest` zbiera informacje o kontrolerach i metodach które może użyć, a następnie je odpala.  
Pomaga mu w tym `ControllersContainer` - klasa która zawiera informacje o wszystkich kontrolerach, które są dodatkowo wcześniej przefiltrowane, co pozwala na szybki dostęp do wybranego typu kontrolerów. Przykładowo tych których metody zawierają atrybut `ReadAlways` lub tych które zawierają atrybut `DiscordCommand`.  
```csharp  
private void RunMethods(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)  
```  
Szuka takiej metody kontrolera, do której pasuje `DiscordRequest`.  
Przed użyciem jej, sprawdza czy może jej użyć.  
```csharp  
private void RunMethodsIBotCommand(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)  
```  
Odpowiednik `RunMethods` dla nowego sposobu obsługi komend - za pomocą `BotCommandsService`.  
```csharp  
private bool IsValid(Contexts contexts, MethodInfo method)  
```  
Sprawdza na podstawie danych z `Contexts`, czy określoną metodą można użyć.  
```csharp  
private bool IsMatchedCommand(IEnumerable<DiscordCommand> commands, DiscordRequest request)  
```  
Na podstawie atrybutów `DiscordCommand` które posiada metoda, sprawdza czy określony `DiscordRequest` może jej użyć.  
```csharp  
private void CheckPermissions(MethodInfo method, Contexts contexts)  
```  
Sprawdza czy użytkownik posiada uprawnienia do użycia określonej metody.  
Jeśli nie posiada, wyrzuca wyjątek.  
```csharp  
private static Task InvokeMethod(DiscordRequest request, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)  
```  
Uruchamia metodę.  

## BotCommandsService  
Serwis służący go zbierania informacji o komendach nowym sposobem - pozwalającym na zebranie bardziej szczegółowych danych.  
Posiada metody  
```csharp  
public string RenderTextTemplate(BotCommandTemplate template)  
```  
Konwertuje schemat według którego powinna wyglądać komenda do tekstu.  
```csharp  
public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template)  
```  
Zwraca informację czy określony schemat pasuje do komendy.  
Podczas matchowania, jeśli nazwa modelu IBotCommand kończy się słowem "Command", słowo to jest ignorowane.  
Ignorowana jest również wielkość znaków.  
Przykładowo do komendy `AddRoleCommand` będzie pasować komenda `-addrole`.  
```csharp  
public BotCommandTemplate GetCommandTemplate(Type commandType)  
```  
Na podstawie typu - modelu komendy który dziedziczy po IBotCommand, generuje wzór komendy która do niego pasuje, z której jest w stanie dopasować dane.  
```csharp  
public T ParseRequestToCommand<T>(DiscordRequest request, BotCommandTemplate template) where T : IBotCommand  
```  
Parsuje dane z `DiscordRequest` na model dziedziczący po `IBotCommand`.  
Pod spodem występuje inteligentne parsowanie typów za pomocą klasy `BotCommandsPropertyConversionService`.  
Aktualnie pozwala ona na parsowanie liczb do typu `int`, oraz czasu w formacie `{czas}(h|m|s)` - przykładowo `10h` lub `5s`, na typ TimeSpan.  

## IBotCommand  

Nowy odpowiednik `DiscordRequest`, zawierający dokładniejsze, sparsowane do określonych typów, dane z komendy.  
Wzory komend generowane są według propertiesów i na podstawie dostępnych typów i atrybutów.  
Przykład komendy  
```csharp  
public class PrintCommand : IBotCommand  
{  
    [Text]  
    public string Message { get; set; }  

    [Number]  
    public int Times { get; set; }  

    [Time]  
    [Optional]  
    public TimeSpan Delay { get; set; }  
}  
```  
Przykład użycia  
`-print -message "losowy tekst" -times 10 -delay 5s`  
Dane zostaną prawidłowo przekonwertowane na `PrintCommand` za pomocą `BotCommandsService`.  

Aby użyć IBotCommand w kontrolerze, wystarczy dodać ją jako pierwszy parametr tej metody.  
Nie trzeba tutaj używać atrybutu `DiscordCommand` - który był wymagany wcześniej.  
Przykład takiej metody  
```csharp  
public async Task PrintMyMessage(PrintCommand command, Contexts contexts)  
```  

Dostępne atrybuty  
`ChannelMention` - wzmianka kanału  
`UserMention` - wzmianka użytkownika  
`Number` - liczba, jest automatycznie konwertowana na typ int  
`SingleWord` - pojedyncze słowo, nie wymaga cudzysłowów " " - jednak są dopuszczalne, nie może zawierać spacji  
`Text` - tekst zawarty między cudzysłowamy " "  
`Time` - tekst w formacie `{czas}(h|m|s)` - przykładowo `10h`, dane są automatycznie konwertowane na typ TimeSpan  
`Optional` - wartość jest opcjonalna  
