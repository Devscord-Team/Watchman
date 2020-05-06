# Do czego używamy klasy Server  
  
Klase Server można znaleźć w projekcie Devscord.DiscordFramework  
Namespace: Devscord.DiscordFramework.Framework  
  
## ServerInitializer  
  
Klasa służąca do inicjalizacji serwera.   
Pozwala na automatyczne skonfigurowanie klasy Server w taki sposób, żeby była zintegrowana z biblioteką Discord.NET  
  
## Server  
    
Klasa statyczna której używamy do komunikacji z discordem, jest zintegrowana z biblioteką Discord.NET i staramy się, żeby była jedynym punktem w aplikacji, który ma kontakt z tą biblioteką.  
Nie zawsze jest możliwe, idąc na skróty, w kilku miejscach w frameworku łamiemy tą zasadę, ponieważ mapowanie wszystkich typów na poziomie klasy Server byłoby zbyt pracochłonne.  
Jednak nigdzie nie łamiemy zasady - **operacje na klasach z Discord.NET mogą występować jedynie na terenie frameworka.**  
Taka izolacja pozwala nam na bardzo prostą podmianę kluczowych elementów, związanych z komunikacją między botem a discordem.  
Nie musimy zmieniać połowy projektu chcąc zmienić sposób wysyłania wiadomości na serwer - wystarczy zmiana kilku linijek w tej klasie.  
Zawiera metody  
```csharp  
public static async Task SendDirectMessage(ulong userId, string message)  
```  
Do wysyłania wiadomości prywatnych do użytkowników.  
```csharp  
public static IEnumerable<SocketRole> GetSocketRoles(ulong guildId)  
```  
Do pobierania listy roli dostępnych na serwerze o podanym ID.  
```csharp  
public static IEnumerable<UserRole> GetRoles(ulong guildId)  
```  
Do pobierania listy roli dostępnych na serwerze o podanym ID.  
Mapuje role na nasz obiekt.  
```csharp  
public static async Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null)  
```  
Do pobierania referencji do kanału o określonym ID, z określonego serwera (jeśli nie podane, pobierze z tego, z którego została wysłana komenda).  
```csharp  
public static async Task<RestUser> GetUser(ulong userId)  
```  
Do pobierania referencji do użytkownika na serwerze.  
```csharp  
public static async Task<RestGuild> GetGuild(ulong guildId)  
```  
Do pobierania referencji do serwera.  
```csharp  
public static async Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)  
```  
Do pobierania referencji użytkownika w kontekście serwera.  
```csharp  
public static async Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId)  
```  
Do pobierania referencji użytkowników w kontekście serwera.  
```csharp  
public static async Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)  
```  
Do tworzenia nowej roli na serwerze.  
```csharp  
public static async Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole role)  
```  
Do zmiany uprawnień określonej roli.  
```csharp  
public static async Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)  
```  
Do zmiany uprawnień wielu roli, na jednym serwerze.  
```csharp  
public static async Task<IEnumerable<DiscordServerContext>> GetDiscordServers()  
```  
Do pobierania listy serwerów do których obecna instancja bota ma dostęp (zależne od tokenu w konfiguracji).  
```csharp  
public static async Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)  
```  
Do pobierania listy wiadomości z określonego kanału na określonym serwerze.  
```csharp  
public static async Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)  
```  
Do pobierania aktywnych linków z zaproszeniami.
