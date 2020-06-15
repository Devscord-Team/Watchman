# Serwisy w frameworku  

Opisane niżej serwisy możesz znaleźć w projekcie Devscord.DiscordFramework, w ścieżce Services/  
Namespace: Devscord.DiscordFramework.Services  

## ChannelsService   

Serwis, który służy do obsługi kanałów.  
Aktualnie jest wykorzystywany jedynie do ustawiania uprawnień konkretnych roli na wybranych kanałach.  
Przykładowo, że osoby nie mające roli "admin" nie mogą pisać na kanałach dla administratorów.  
Zawiera metody   
```csharp  
public Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole userRole)  
```  
Do dodawania uprawnień dla jednej roli, jednemu kanałowi.  
```csharp  
public Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole userRole)  
```  
Do dodawania uprawnień jednej roli, wielu kanałom - przykładowo ustawienie wszystkim kanałom zakazu wysyłania wiadomości, dla roli "muted".  

## DirectMessagesService  

Serwis służący do wysyłania wiadomości bezpośrednio do użytkowników, w wiadomości prywatnej.  
Zwiera metody   
```csharp  
public Task<bool> TrySendMessage(ulong userId, Func<ResponsesService, string> response, Contexts contexts)  
```  
Do wysyłania wiadomości z pomocą naszego serwisu do automatycznego renderowania wiadomości, według z góry ustalonego wzoru.  
```csharp  
public async Task<bool> TrySendMessage(ulong userId, string message, MessageType messageType = MessageType.NormalText)  
```  
Do wysyłania wiadomości bez używania systemu do renderowania wiadomości - użytkownik dostanie dokładnie taką wiadomość, jaka jest zawartość parametru message.  

## DiscordServersService  

Serwis służący do pobierania listy wszystkich serwerów discord, do których dostęp ma aktualna instancja bota.  
Zawiera metodę  
```csharp  
public Task<IEnumerable<DiscordServerContext>> GetDiscordServers()  
```  
Zwraca typ `DiscordServerContext` posiadający podstawowe informacje o serwerze, takie jak jego ID, nazwa, informacja który użytkownik jest jego właścicielem.  

## HelpDataCollectorService  

Serwis służący do pobierania informacji o wszystkich komendach dostępnych w aplikacji.  
Zawiera metodę  
```csharp  
public IEnumerable<CommandInfo> GetCommandsInfo(Assembly botAssembly)  
```  
Za pomocą przekazanego assembly, w którym powinny znajdować się kontrolery (typy dziedziczące po `IController` - w naszym przypadku jest to `Watchman.Discord`) wyciąga wszystkie typy kontrolerów, a następnie za pomocą klasy `CommandsInfoFactory` wyciąga z typów kontrolerów informacje o tym, jakie komendy są w stanie obsłużyć.  

Możemy się tego dowiedzieć na podstawie atrybutu `DiscordCommand`, z którego korzystają metody kontrolerów, aby wskazać na którą komendę powinna reagować ta metoda.  

## MessagesHistoryService  

Serwis służący do pobierania historii wiadomości z podanego przez nad kanału, na wybranym przez nas serwerze.  
Zawiera metody  
```csharp  
public async Task<IEnumerable<Message>> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit)  
```  
Pozwala na pobranie określonej ilości ostatnich wiadomości.  
```csharp  
public async Task<IEnumerable<Message>> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit, ulong fromMessageId, bool goBefore)  
```  
Pozwala na dokładniejsze określenie które wiadomości chcemy pobrać - pozwala to na zwiększenie wydajności w sytuacji kiedy dokładnie wiemy których wiadomości potrzebujemy.  

## MessageSplittingService  

Serwis służący do przycinania wiadomości w taki sposób, aby były możliwe do wysłania na discordzie, nie psując jednocześnie ich wyglądu.  
Takie rozwiązanie jest konieczne, ponieważ discord posiada limit ilości znaków w jednej wiadomości.  
Zawiera metodę   
```csharp  
public IEnumerable<string> SplitMessage(string fullMessage, MessageType messageType)  
```  
Która pod spodem używa różnych metod prywatnych, w zależności od tego jaki `MessageType` podamy.  
Ma to na celu zachowanie ostrożności przy przycinaniu formatów innych niż zwykła wiadomość tekstowa.  
Przykładowo w przypadku wiadomości w bloku, nie chcemy przerywać bloku, a w przypadku wiadomości w formacie JSON nie chcemy zniekształcać wyglądu formatu.  
Obsługiwane formaty to   
`MessageType.NormalText` - zwykła wiadomość tekstowa  
`MessageType.BlockFormatted` - wiadomość używająca formatowania tekstu (np blok kodu)  
`MessageType.Json` - wiadomość zawierająca dane w formacie JSON  

## MessagesService  

Serwis służący do wysyłania wiadomości, na wcześniej wybrany przez nas kanał, na wybranym przez na serwerze.  
Do wyboru ID serwera służy properties  
```csharp  
public ulong GuildId { get; set; }  
```  
A do wyboru ID kanału służy   
```csharp  
public ulong ChannelId { get; set; }  
```  
W większości przypadków nie przydzielamy tych wartości ręcznie - robi to za nas często używana fabryka `MessagesServiceFactory`, która za pomocą podanych przez nas wartości, jest w stanie stworzyć obiekt MessagesService.  
Jest to dużo wygodniejsza i bezpieczniejsza opcja, niż ręczne przypisywanie wartości propertiesów, ponieważ używając fabryki - mamy pewnośc że obiekt zawsze będzie tworzony prawidłowo.  

Serwis za pomocą wstrzykiwania zależności dostaje   
`ResponsesService` - serwis służący do obsługi automatycznego generowania wiadomości, na wcześniej wybranego szablonu wiadomości  
`MessageSplittingService` - wcześniej wspomniany serwis, służący do automatycznego dzielenia wiadomości, które przekraczają limit znaków w jednej wiadomości  

Zawiera metody  
```csharp  
public Task SendMessage(string message, MessageType messageType = MessageType.NormalText)  
```  
Do wysyłania wiadomości dokładnie takich, jakie wcześniej wygenerowaliśmy - wiadomość jest dokładnie taka, jak zawartość parametru message.  
Dodatkowo możemy określić typ wiadomości, co zostało omówione przy opisie `MessageSplittingService`.  
```csharp  
public Task SendResponse(Func<ResponsesService, string> response, Contexts contexts)  
```  
Do wysyłania wiadomości, za pomocą wybranego szablonu odpowiedzi, któremu przekazujemy dane.  
W `Func<ResponsesService, string>` jesteśmy w stanie wybrać szablon odpowiedzi, ponieważ ResponsesService zawiera metody rozszerzające - służące do wyboru z którego identyfikatora szablonu chcemy korzystać.  
Wiedząc jaki identyfikator szablonu wybraliśmy, serwis jest w stanie sprawdzić czy serwer nadpisał odpowiedź swoim szablonem, czy system powinien użyć domyślnego szablonu.  
```csharp  
public async Task SendFile(string filePath)  
```  
Do wysyłania plików. Przykład zastosowania to wysyłanie wyniku statystyk w formie wykresu.  

## UsersRolesService  

Serwis służący do zarządzania rolami na wybranym przez nas serwerze.  
Zawiera metody  
```csharp  
public UserRole CreateNewRole(DiscordServerContext server, NewUserRole userRole)  
```  
Do stworzenia nowej roli na wybranym przez nas serwerze.  W skład obiektu `NewUserRole` wchodzi informacja o nazwie roli, oraz lista z uprawnieniami które postanowiliśmy wybrać.  
```csharp  
public UserRole GetRoleByName(string name, DiscordServerContext server)  
```  
Do pobrania roli o określonej przez nas nazwie, na wybranym przez nas serwerze.  
```csharp  
public IEnumerable<UserRole> GetRoles(DiscordServerContext server)  
```  
Do pobrania informacji o wszystkich rolach, które są dostępne na wybranym przez nas serwerze.  

## UsersService  

Serwis służący do zarządzania użytkownikami na wybranym przez nas serwerze.  
Zawiera metody  
```csharp  
public Task AddRole(UserRole role, UserContext user, DiscordServerContext server)  
```  
Do przydzielenia roli wybranemu użytkownikowi.  
```csharp  
public Task RemoveRole(UserRole role, UserContext user, DiscordServerContext server)  
```  
Do usunięcia roli wybranemu użytkownikowi.  
```csharp  
public IEnumerable<UserContext> GetUsers(DiscordServerContext server)  
```  
Do pobrania listy wszystkich użytkowników na wybranym przez nas serwerze.  
```csharp  
public UserContext GetUserByMention(DiscordServerContext server, string mention)  
```  
Pobranie informacji o użytkowniku, na podstawie tego jak wygląda wzmianka o nim.   
Przykładowo `@Watchman` - discord będzie wyświetlać tą wiadomość w specjalny, wyróżniony sposób, jednak pod spodem, jako suchy tekst, będzie informacja o tym jakie jest ID tego użytkownika.