# Inicjalizacja bota
  
### Watchman.Discord

Jeśli uruchamiamy bota lokalnie, punktem wejściowym programu jest plik "Program" w projekcie "Watchman.Discord".  Tutaj też
uruchamiana jest czysta aplikacja samego bota, czyli tworzony i konfigurowany obiekt klasy "WatchmanBot".

### Watchman.Web

Na produkcji bot jest uruchamiany w tle przez aplikację webową (projekt "Watchman.Web").  Za pomocą Hangfire jest obsługiwana część jego funkcji które mają być wykonywane okresowo.

W tym przypadku nasz obiekt "WatchmanBot" jest tworzony w pliku "Watchman.Web/ServiceProviders/AutofacServiceProviderFactory" w funkcji:
```csharp
0  public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
1  {
2      var container = containerBuilder.Build();
3
4      var workflowBuilder = new WatchmanBot(new DiscordConfiguration
5      {
6          MongoDbConnectionString = this._configuration.GetConnectionString("Mongo"),
7          Token = this._configuration["Discord:Token"]
8      }, container.Resolve<IComponentContext>()).GetWorkflowBuilder();
9
10     workflowBuilder.Build();
11     container.Resolve<HangfireJobsService>().SetDefaultJobs(container);
12     return new AutofacServiceProvider(container);
13 }
```
Za inicjalizację serwisów obsługiwanych przez HangFire odpowiada linia 11.  

## InitializationService

```csharp
public async Task InitServer(DiscordServerContext server)
{ 
    Log.Information("Initializing server: {server}", server.ToJson());
    await this.MuteRoleInit(server);
    var lastInitDate = this.GetLastInitDate(server);
    await this.ReadServerMessagesHistory(server, lastInitDate);
    await this._cyclicStatisticsGeneratorService.GenerateStatsForDaysBefore(server, lastInitDate);
    await this.NotifyDomainAboutInit(server);
    Log.Information("Done server: {server}", server.ToJson());
}
```
InitializationService jest używany w klasie "WatchmanBot" w funkcji "GetWorkflowBuilder()".  Metoda "InitServer" zajmuje się wszystkim co potrzebne aby dostać na podanym w argumencie serwerze gotowego do pracy bota.  


```csharp
private async Task MuteRoleInit(DiscordServerContext server)
{
    await this._muteRoleInitService.InitForServer(server);
    Log.Information("Mute role initialized: {server}", server.Name);
}
```
Tworzy na serwerze rolę "Muted" (wraz z ustawieniem odpowiednich uprawnień roli, np. brak możliwości wysyłania wiadomości), która może być nadana wybranej osobie przez bota za pomocą komendy "-mute" lub przez antispam.  

```csharp
private async Task ReadServerMessagesHistory(DiscordServerContext server, DateTime lastInitDate)
{
    foreach (var textChannel in server.GetTextChannels())
    {
        await this._serverScanningService.ScanChannelHistory(server, textChannel, lastInitDate);
    }
    Log.Information("Read messages history: {server}", server.Name);
}
```
Przeszukuje wszystkie kanały na danym serwerze i nowe (niezapisane jeszcze w bazie) wiadomości zapisuje do bazy danych bota.  

```csharp
private DateTime GetLastInitDate(DiscordServerContext server)
{
    var query = new GetInitEventsQuery(server.Id);
    var initEvents = this._queryBus.Execute(query).InitEvents.ToList();
    if (!initEvents.Any())
    {
        return DateTime.UnixEpoch;
    }

    var lastInitEvent = initEvents.Max(x => x.EndedAt);
    return lastInitEvent;
}
```
Każda inicjalizacja bota na danym serwerze zostawia o sobie informację w bazie danych, w tej metodzie szukamy
ostatniej daty włączenia bota dla wybranego serwera.  

```csharp
private async Task NotifyDomainAboutInit(DiscordServerContext server)
{
    var command = new AddInitEventCommand(server.Id, endedAt: DateTime.UtcNow);
    await this._commandBus.ExecuteAsync(command);
}
```
Zapisanie w bazie danych informacji o udanym zainicjowalizowaniu bota na serwerze, więc teraz to będzie dla niego "LastInitDate".
