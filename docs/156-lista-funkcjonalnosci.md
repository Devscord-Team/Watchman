# Lista funkcjonalności  
  
## Zarządzanie rolami  
  
Zarządzanie rolami jest oparte o tak zwane "bezpieczne role" (safe roles), czyli liste roli, które na danym serwerze są ustawione jako bezpieczne.  
Jeśli rola jest ustawiona jako bezpieczna, użytkownik może ją sobie dodać bez pomocy admina.  
Listy bezpiecznych roli są indywidualne dla wszystkich serwerów, nie istnieje lista z domyślnymi rolami.  
  
```  
-roles  
```  
Wyświetla liste wszystkich bezpiecznych roli na serwerze.  
```  
-set role {role} -safe  
```  
 Dodaje role do listy bezpiecznych roli.  
 Przykład użycia: `-set role testowa safe`  
```  
-set role {role} -unsafe  
```  
 Dodaje role jako bezpieczną.  
 Przykład użycia: `-set role testowa unsafe`  
```  
-add role {role}  
```  
 Przypisuje role do użytkownika, który wpisał komende.  
 Przykład użycia: `-add role testowa`  
```  
-remove role {role}  
```  
Usuwa role użytkownikowi, który wpisał komende.  
Przykład użycia: `-remove role testowa`  
  
## Statystyki  
  
```  
-stats {period}  
```  
Generuje wykres statystyk podający dokładność co podanego okresu czasu.  
Dostępne okresy:  
- hour  
- day  
- week  
- month  
  
Przykład użycia: `-stats month`.  
  
## Konfigurowalne odpowiedzi  
  
Bot działa w oparciu o templatki, na podstawie których renderujemy odpowiedzi.  
Przy określonych wydarzeniach, zwracamy określone odpowiedzi. Jednak to co znajdzie się w tej odpowiedzi, zależy już od indywidualnej konfiguracji serwera.  
Odpowiedzi dzielą się na domyślne i skonfigurowane przez serwer (nadpisane przez niego).  
  
```  
-add response -onevent {onEvent} -message {message}  
```  
Jeśli serwer nie nadpisał domyślnej odpowiedzi, dodaje jej nadpisanie.  
Przykład użycia: `-add response -onevent PrintHelp -message "help print2"`  
```  
-update response -onevent {onEvent} -message {message}  
```  
Jeśli serwer nadpisał domyślną odpowiedź, aktualizuje nadpisaną odpowiedź.  
Przykład użycia: `-update response -onevent PrintHelp -message "help print3"`  
```  
-remove response -onevent {onEvent}  
```  
Jesli serwer nadpisał domyślną odpowiedź, usuwa nadpisanie (teraz będzie korzystać z domyślnej).  
Przykład użycia: `-remove response -onevent PrintHelp`  
Parametry:  
`onEvent` - identyfikator wydarzenia przy którym jest wywoływana komenda.  
`message` - treść wiadomości, jeśli jest dłuższy niż jedno słowo (zawiera spacje), musi być wewnątrz cudzysłowów.  
  
Lista dostępnych wydarzeń z danymi domyślnymi:  
```  
AvailableSafeRoles  
```  
domyślna wartość - `Dostępne role: {{roles}}`  
```  
MutedUser  
```  
domyślna wartość - `Użytkownik {{user}} został zmutowany do {{timeEnd}}UTC`  
```  
NewUserArrived  
```  
domyślna wartość - `Cześć {{user}}! Witamy cię na serwerze {{server}}`  
```  
NotEnoughArguments  
```  
domyślna wartość - `Podano niewystarczającą liczbę argumentów!`  
```  
NumberOfMessagesIsHuge  
```  
domyślna wartość - `Liczba wiadomości w podanym zakresie jest ogromna ({{messagesCount}} wiadomości). Czy na pewno chcesz wszystkie przeczytać? (użyj parametru -force)`  
```  
PrintHelp  
```  
domyślna wartość - `Dostępne komendy: ```{{help}}````  
```  
ReadingHistoryDone  
```  
domyślna wartość - `Zakończono wczytywanie historii wiadomości.`  
```  
ResponseAlreadyExists  
```  
domyślna wartość - `Odpowiedź {{onEvent}} już istnieje dla serwera **{{server}}**.`  
```  
ResponseHasBeenAdded  
```  
domyślna wartość - `Odpowiedź {{onEvent}} została dodana dla serwera **{{server}}**.`  
```  
ResponseHasBeenRemoved  
```  
domyślna wartość - `Odpowiedź {{onEvent}} została usunięta dla serwera **{{server}}**.`  
```  
ResponseHasBeenUpdated  
```  
domyślna wartość - `Odpowiedź {{onEvent}} została zaktualizowana dla serwera **{{server}}**.\nStara odpowiedź: *{{oldMessage}}*.\nNowa odpowiedź: *{{newMessage}}*.`  
```  
ResponseNotFound  
```  
domyślna wartość - `Odpowiedź {{onEvent}} nie została odnaleziona dla serwera **{{server}}**.`  
```  
RoleAddedToUser  
```  
domyślna wartość - `Dodano role: {{role}} użytkownikowi {{user}}`  
```  
RoleIsInUserAlready  
```  
domyślna wartość - `Użytkownik {{user}} posiada już role {{role}}`  
```  
RoleNotFound  
```  
domyślna wartość - `Nie znaleziono roli {{role}}.`  
```  
RoleNotFoundInUser  
```  
domyślna wartość - `Użytkownik {{user}} nie posiada roli {{role}}`  
```  
RoleNotFoundOrIsNotSafe  
```  
domyślna wartość - `Nie znaleziono roli {{role}} lub wybrana rola musi być zmieniona ręcznie przez członka administracji`  
```  
RoleRemovedFromUser  
```  
domyślna wartość - `Usunięto role {{role}} użytkownikowi {{user}}`  
```  
RoleSettingsChanged  
```  
domyślna wartość - `Zmieniono ustawienia roli {{role}} pomyślnie!`  
```  
SentByDmMessagesOfAskedUser  
```  
domyślna wartość - `Wysłano {{messagesCount}} wiadomości użytkownika {{user}} w wiadomości prywatnej.`  
```  
ServerDoesntHaveAnySafeRoles  
```  
domyślna wartość - `Serwer nie ma żadnych dostępnych bezpiecznych ról (jeśli jesteś adminem, ustaw bezpieczne role za pomocą komendy -set role &lt;nazwa_roli&gt; safe) `  
```  
SpamAlertRecognized  
```  
domyślna wartość - `Spam alert! Wykryto spam u użytkownika {{user}} na kanale {{channel}}. Poczekaj chwile zanim coś napiszesz.`  
```  
SpamAlertUserIsMuted  
```  
domyślna wartość - `Spam alert! Uzytkownik {{user}} został zmutowany.`  
```  
SpamAlertUserIsMutedForLong  
```  
domyślna wartość - `Spam alert! Użytkownik {{user}} został zmutowany na dłużej`  
```  
TimeCannotBeNegative  
```  
domyślna wartość - `Czas nie może być ujemny!`  
```  
TimeIsTooBig  
```  
domyślna wartość - `Podano za dużą wartość czasu!`  
```  
TimeNotSpecified  
```  
domyślna wartość - `Nie określiłeś przedziału czasu!`  
```  
UnmutedUser  
```  
domyślna wartość - `Użytkownik {{user}} może pisać ponownie.`  
```  
UnmutedUserForUser  
```  
domyślna wartość - `Cześć {{user}}! Już możesz pisać ponownie na serwerze {{server}}`  
```  
UserDidntMentionAnyUser  
```  
domyślna wartość - `Nie wskazałeś żadnego użytkownika!`  
```  
UserDidntWriteAnyMessageInThisTime  
```  
domyślna wartość - `Użytkownik {{user}} nie napisał żadnej wiadomości w podanym czasie.`  
```  
UserDoesntHaveAvatar  
```  
domyślna wartość - `Użytkownik {{user}} nie posiada avatara.`  
```  
UserIsNotAdmin  
```  
domyślna wartość - `Nie masz wystarczających uprawnień do wywołania tej komendy.`  
```  
UserNotFound  
```  
domyślna wartość - `Użytkownik {{user}} nie istnieje.`  
  
## Automatyczne wykrywanie spamu  
  
Bot po wykryciu spamu, powiadamia podejrzaną osobę, żeby na chwilę przestała pisać - pozwoli to na rozwianie podejrzeń czy ta osoba jest spamerem.  
Jeśli ta osoba będzie pisać dalej (mimo ostrzeżenia), zostanie zmutowana na krótki okres czasu.  
Po tym okresie czasu, osoba ta przestaje być zmutowana, ale trafia na listę osób podejrzanych - będąc na tej liście, przy kolejnym zmutowaniu w wyniku spamu, okres zmutowania będzie trwał zauważalnie dłużej niż poprzedni.  
Po dłuższym czasie w którym nie wykryto spamu u tej osoby, ta osoba znika z listy osób podejrzanych.  
  
Istnieje również coś takiego jak lista osób zaufanych, czyli osoby które długi czas udzielały się na serwerze, są objęte mniej rygorystycznymi zasadami dotyczącymi spamu (mogą pozwolić sobie na większą częstotliwość wysyłania wiadomości - zakładamy że bezmyślny spamer prędzej sie podda niż zawalczy o dostanie się na tą listę).  
  
## Mutowanie  
  
```  
-mute {user} -t {time} -reason {reason}  
```  
Mutuje określonego użytkownika, na określony czas, z określonego powodu.  
Parametry:  
`user` - wzmianka użytkownika  
`time` - czas podany w formacie `liczba(h|m|s)`  
`reason` - tekst, jeśli jest dłuższy niż jedno słowo (zawiera spacje), musi być wewnątrz cudzysłowów  
Przykłady użycia:  
`-mute @Testowy123 -t 10h -reason "łamanie regulaminu serwera"`  
`-mute @Testowy123 -t 5m -reason testowy`  
`-mute @UserABC -t 30s -reason "przykładowy powód"`  
  
## Inne  
  
```  
-avatar  
```  
Wyświetla awatar użytkownika, który wpisał komende.  
```  
-marchew  
```  
Odpowiedź na życie   
```  
-init  
```  
Inicjalizacja serwera (załadowanie informacji o serwerze do bazy danych bota).  
Uruchamia się automatycznie przy starcie bota, jednak jeśli dopiero go dodałeś do serwera - możliwe że jeszcze nie przeszedł inicjalizacji - wtedy warto zrobić ją manualnie.  
