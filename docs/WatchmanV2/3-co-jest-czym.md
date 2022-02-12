# Co jest czym

Widząc te wszystkie warstwy, foldery, pliki itd, można łatwo poczuć się przytłoczonym. Często nowe osoby potrzebują krótkiego wytłumaczenia jak to mniej więcej działa i z myślą o takich osobach, postaram się wszystko opisać.

### DiscordFramework

Zdecydowaliśmy się napisać własny, prosty framework, oparty na bibliotece [Discord.NET](https://github.com/discord-net/Discord.Net). Zawiera on przede wszystkim logikę służącą do komunikowania się z discordem, ale też odpowiada za wszelkiego rodzaju logikę, przez którą to wszystko działa tak jak działa. Innymi słowami, jest sercem naszego projektu, dzięki niemu komendy są prawidłowo parsowane i walidowane, dzięki niemu odpowiednie kontrolery dostają prawidłowe informacje, dzięki niemu wszystko spina się w jedną, prawidłowo działającą całość.

Jest również jedynym miejscem, w którym obsługujemy bibliotekę [Discord.NET](https://github.com/discord-net/Discord.Net), ma to na celu prostą ewentualną podmianę na własną implementację w przyszłości.

### Discord

Warstwa w której jest konfiguracja frameworka, kontrolery i logika wokół kontrolerów. Dawniej ta warstwa była głównym punktem odpalania aplikacji.

Kontrolery opisują jak konkretne komendy powinny działać, mogą komunikować się z serwisami z naszego frameworka, z warstwą integracji, lub warstwą domeny. Kontrolery mogą mieć swoje dedykowane modele i serwisy, które służą do wydzielenia kodu który jest blisko związany z komunikacją z discordem, ale nie jest uniwersalny, jest specyficzny dla działania konkretnych komend.

### Web

API służące do zarządzania botem, za jego pomocą można ustawiać konfiguracje Watchmana, pobierać informacje (np statystyki), odpalać akcje, takie jak wysyłanie wiadomości w imieniu bota. Elementy które są możliwe do zrobienia za pomocą komend, ale w formie api.

Ta warstwa jest punktem startowym projektu, czyli odpala ona bota w tle, przy okazji odpalania api. Jest plan żeby w przyszłości była też strona internetowa, jako panel do zarządzania botem.

### DomainModel

Warstwa w której opisujemy jak mają działać procesy aplikacji, a dokładniej mówimy "co jest czym", "co jest po co". Przykładowo nie będzie tutaj logiki mówiącej że coś ma zostać teraz wyświetlone, ale możemy znaleźć logikę mówiącą czym jest "zaufany użytkownik" i jak powinien się zachowywać.

W prostszym języku to kod, który może działać niezależnie od discorda, gdybyśmy chcieli przenieść działanie Watchmana na przykładowo stronę internetową, bylibyśmy w stanie wykorzystać kod z tej warstwy, czego już nie można powiedzieć o warstwie `Discord`, w której mogą znajdować się takie elementy jak "teraz wyświetl ten tekst na tym kanale", "teraz wyślij taką wiadomość prywatną temu użytkownikowi".

### Integrations

Integracje z zewnętrznymi usługami... tak po prostu. Obsługa zewnętrznych api, baz danych, plików na dysku itd.

### IoC

Konfiguracja IoC, do której używamy biblioteki [Autofac](https://autofac.org/)

### Cqrs

Własna implementacja wzorca CQRS.

### Common

Zestaw klas pomocniczych, które mogą być użyte w dowolnym miejscu w aplikacji, poza `DiscordFramework`.

## Co wie o czym

Jedną z zalet posiadania wielu warstw, jest możliwość zdefiniowania która warstwa może być widoczna dla której warstwy. W niektórych momentach jest to dodatkowym utrudnieniem, bo pewne rzeczy trzeba "obchodzić', ale ostatecznie zyskujemy na tym większą czytelność i przejrzystosć aplikacji.

### DiscordFramework

Docelowo ma być w innym repozytorium, ta warstwa docelowo nie powinna mieć zależności z niczym.

### Discord

Może korzystać z wszystkich warstw. Używa głównie DiscordFramework i DomainModel.

### Web

Nie może wiedzieć o DiscordFramework, używa DomainModel i Discord.

### DomainModel

Staramy się żemy DomainModel był jak najbardziej niezależny, ale może używać Integrations i Common.

### Integrations, IoC, Cqrs

Powinny być jak najbardziej niezależne, ale może używać Common.

### Common

Jest niezależne, może być używane przez wszystko poza DiscordFramework.
