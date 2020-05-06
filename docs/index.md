## Witaj w dokumentacji Watchmana!  

Poniżej znajdziesz najważniejsze linki, które są niezbędne do pracy nad Watchmanem.  

[Repozytorium na githubie](https://github.com/Devscord-Team/Watchman)  

[Trello](https://trello.com/b/xpN9bYtR/watchman)  

## Getting Started  


Poniżej znajdziesz instrukcję uruchomienia bota na własnym hostingu (np. na własnym PC), co daje możliwość samodzielnej ingerencji w kod i dostosowanie bota do własnych preferencji.  

**Jeśli jednak zależy Ci jedynie na skorzystaniu z domyślnego bota na swoim serwerze Discord, użyj tego linku:**  
[Zaproszenie Bota na własny serwer](https://discordapp.com/api/oauth2/authorize?client_id=636274997786312723&permissions=2147483127&scope=bot)  

***  

#### Potrzebne narzędzia:  

* .NET Core 3.1  
* Lokalna baza MongoDB  

#### Przygotowanie bota do uruchomienia  

Jeśli masz już zainstalowane powyższe narzędzia, możesz przejść do sklonowania repozytorium:  

##### Klonowanie repozytorium  

```  
git clone https://github.com/Devscord-Team/Watchman.git  
```  
##### Dodawanie pliku konfiguracyjnego  

Następnie w folderze `Watchman/Watchman.Discord/` utwórz plik `config.json`, w którym powinny się znaleźć:  
* token bota utworzony na stronie [Discord Developer Portal](https://discordapp.com/developers/applications)  
* mongoDB connection string (jeśli podłączasz się do lokalnej bazy danych to prawdopodobnie twój connectionString będzie wyglądać tak jak w poniższym przykładzie.  

Przykład takiego pliku:  
```json  
{  
    "token": "ABCDefgh.123456789",  
    "MongoDbConnectionString": "mongodb://localhost:27017"  
}  
```  

##### Kompilowanie bota i uruchomienie  

Przejdź w konsoli do folderu `Watchman/Watchman.Discord`, następnie skompiluj i uruchom bota za pomocą komend:  

```  
cd Watchman/Watchman.Discord  
dotnet run  
```  

#### Dodawanie bota do swojego serwera  

Link który umożliwi dodanie bota do serwera będzie wyglądać następująco:  

```  
https://discordapp.com/api/oauth2/authorize?client_id=id_twojego_bota&scope=bot&permissions=2147483127  
```  

Z tą różnicą, że musisz przekopiować w miejsce `id_twojego_bota` ClientID twojej aplikacji z podanej wcześniej strony [Discord Developer Portal](https://discordapp.com/developers/applications).
Przejdź po ten adres, wybierz serwer oraz przydziel uprawnienia.  

Od tego momentu możesz już korzystać z bota na własnym serwerze.  
Aby dowiedzieć się więcej o działaniu bota - kliknij w zakładkę "Dokumentacja techniczna" w głównym  menu.  
