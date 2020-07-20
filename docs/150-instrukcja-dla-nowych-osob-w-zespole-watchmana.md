# Instrukcja dla nowych osób w zespole Watchmana  
  
## Trello  
  
#### Nazwy kolumn  
  
`ToDo`  
Lista zadań których nikt jeszcze nie ruszył.  
  
`Assigned`  
Lista zadań które są przypisane do konkretnej osoby, ale jeszcze nie są rozpoczęte.  
  
`Acknowledged`  
Osoba do której zadanie jest przypisane zadanie, przeczytała je, rozumie co ma zrobić, jednak fizycznie nie zaczęła pracy.  
  
`Doing`   
Zadanie jest w trakcie wykonywania.   
   
`Feedback`  
Osoba do której zadanie jest przypisane nie może kontynuować pracy z powodu braku informacji od innych osób  
np brak informacji w jaki sposób konkretny element ma działać.  
Celem kolumny feedback jest przede wszystkim poinformowanie osoby zlecającej zadanie, że stoi ono w miejscu i czeka na odpowiedź.  
  
`Review`  
Zadanie jest skończone, aktualnie czeka w postaci pull requesta (lub innej jeśli nie dotyczyło kodu) na ocenę i zatwierdzenie.  
  
`Done`  
Zadanie jest sprawdzone, zaakceptowane i znajduje się na branchu master (jeśli dotyczyło kodu).  
  
`On production`  
Jeśli zadanie dotyczuło kodu, działa już na serwerze produkcyjnym.  
Możliwe że w przyszłości pojawi się błąd i będą wymagane poprawki .  
  
`Closed`  
Zadanie jest uznane za skończone i działające stabilnie na produkcji.  
  
#### Etykiety  
  
Do zadań przydzielone są etykiety - w domyślnym widoku są wyświetlane jako kolorowe pasji, po kliknięciu na pasek lub po wejściu w treść zadania, możesz zobaczyć tytuł etykiety.  
Etykiet używamy to oznaczania poziomu trudności, priorytetu i (czasami) innych istotnych elementów.  
  
`Easy` - Nie powinno sprawić problemów.  
`Medium` - Mogą pojawić się problemy.  
`Medium-Hard` - Występuje kilka problemów wymagających zastanowienia się.  
`Hard` - Nie wiadomo jak to zrobić.  
  
`Low` - Zapisane jako pomysł, jednak nie potrzebujemy tej funkcjonalności aktualnie.  
`Normal` - Nie śpieszy nam się, ale dobrze gdyby było na produkcji.  
`High` - Podobna sytuacja do Normal, jednak dobrze gdyby było zrobione wcześniej.  
`Urgent` - Śpieszy nam się, zadanie powinno być zrobione tak szybko jak to możliwe, ale nic sie nie stanie jak poczeka kilka dni albo tydzień.  
`Immediate` - Pilna sprawa, powinno być zrobione tak szybko jak to możliwe (zazwyczaj jest to związane z błędem w podstawowej funckjonalności, lub oznacza zadanie blokujące wiele innych zadań).  
  
`GoodFirst` - Zadanie nie wymaga znajomości wielu skomplikowanych elementów w naszej aplikacji.   
Pozwala poznać jak ona działa.   
Innymi słowami - jest dobre na start.  
  
`Dokumentacja` - Zadania związane z dokumentacją mają osobnę sekcję w trello.  
Jedno zadanie powinno dotyczyć jednej strony w dokumentacji.  
Zadania z `Done` mogą wracać do `Doing` lub `ToUpdate`, jeśli strona wymaga aktualizacji.  
  
#### Idee  
  
Idee to różnego rodzaju luźne pomysły na rozwój projektu, które jeszcze nie trafiły na liste zadań.  
  
Idee które powinny zostać wprowadzone wpadają do zaakceptowanych, a później stają się zadaniem lub bezpośrednio stają się zadaniem.  
  
Jeśli pomysł jest odrzucony z jakiegoś powodu, przykładowo odbiega od wizji bota, funkcjonalność już istnieje, lub już jest na liście zadań - trafia na liste odrzuconych.  
  
## Github  
  
#### Branche  
  
Po zainstalowaniu wtyczki do google chrome [Link do wtyczki](https://chrome.google.com/webstore/detail/trello-card-numbers/kadpkdielickimifpinkknemjdipghaf), lub po spojrzeniu na url po kliknięciu na zadanie, widzimy ID zadania.  
  
Nazwa brancha powinna zawierać ID zadania + opis po angielsku.  
Opis nie musi być dosłownym tłumaczeniem tytułu zadania, wystarczy że jednym zdaniem opisuje czego to zadanie dotyczy.  
  
Prefixy nazwy brancha:  
Nowa funkcjonalność -> F/  
Naprawa błędu -> B/  
Zmiana w dokumentacji -> D/  
  
Przykład:  
`F/123-add-new-database`  
`B/111-fix-logging`  
  
Jeśli zmiana dokumentacji jest wewnątrz innego zadania (np zmieniasz funkcjonalność serwisu i od razu poprawiasz jego dokumentację) - nie musisz tworzyć osobnego brancha do zmiany w dokumentacji.  
Branche zaczynające się od `D/` powinny być używane tylko wtedy, kiedy dotyczą one jedynie dokumentacji.  
  
#### Jak wykonywać zadania  
  
1. Zrób forka repozytorium.  
2. Stwórz brancha na swoim forku (nazwij go według instrukcji wyżej).  
3. W głównym repozytorium przy próbie dodania pull requesta, zobaczysz opcje dodania pull requesta z brancha, w swoim forku.  
4. Dodaj pull requesta.  
5. Poczekaj na review - żeby przyśpieszyć ten proces, możesz napisać na Discordzie (Jeśli jeszcze nie dołączyłeś - [Zaproszenie do Discorda](https://discord.gg/9R3mUKd)).  
6. Popraw kod lub skomentuj komentarze.  
7. Zaczekaj aż dostaniesz zgode na merge.  
8. Zrób merge, lub zaczekaj aż osoba która wyraziła zgodę to zrobi (czasami merge robimy od razu po wyrażeniu zgody).  

#### Etykiety  
  
Do pull requestów możemy dodawać etykiety  
Przykładowo `ready for review` lub `help needed`  
Zachęcam do używania ich, szczególnie wspomnianego `ready for review`
