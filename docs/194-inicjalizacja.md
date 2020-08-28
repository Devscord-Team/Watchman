# Inicjalizacja bota
  
## Punkt wejściowy

Jeśli uruchamiamy bota lokalnie, punktem wejściowym jest plik  "Program" w projekcie "Watchman.Discord". Tutaj
uruchamiana jest tylko czysta aplikacja samego bota, czyli tworzony i konfigurowany obiekt klasy "WatchmanBot".
Na produkcji bot jest uruchamiany w tle przez aplikację webową (projekt "Watchman.Web") i obsługiwany przez Hangfire.
Obiekt klasy "WatchmanBot" jest tworzony w pliku "Watchman.Web/ServiceProviders/AutofacServiceProviderFactory" w funkcji:
```
public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
```
Również w tym miejscu, po stworzeniu bota konfigurowane będą serwisy pracujące w tle, wymagane do jego obsługi:
```
container.Resolve<HangfireJobsService>().SetDefaultJobs(container)
```

## InitializationService

```
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
InitializationService jest używany w klasie "WatchmanBot" w funkcji "GetWorkflowBuilder()".
Metoda "InitServer" zajmuje się wszystkim co potrzebne aby dostać na podanym w argumencie serwerze gotowego do pracy bota.


```
    private async Task MuteRoleInit(DiscordServerContext server)
    {
        await this._muteRoleInitService.InitForServer(server);
        Log.Information("Mute role initialized: {server}", server.Name);
    }
```
tworzy na serwerze rolę "Muted" która może być nadana wybranej osobie przez bota.Aby to zrobić, należy posiadać uprawnienia administratora
oraz użyć komendy "-mute".Osoba z tą rolą nie może wykonywać na serwerze czynności, które wymagają następujących uprawnień:
```
{ Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite }
```


```
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


```
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
ostatniej daty takiej akcji dla wybranego serwera. (o ile istnieje)


```
    private async Task NotifyDomainAboutInit(DiscordServerContext server)
    {
        var command = new AddInitEventCommand(server.Id, endedAt: DateTime.UtcNow);
        await this._commandBus.ExecuteAsync(command);
    }
```
Zapisanie w bazie danych informacji o udanym zainicjowalizowaniu bota na serwerze, więc teraz to będzie dla niego "LastInitDate".