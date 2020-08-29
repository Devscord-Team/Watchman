# Jak działa Anty Spam

System do wykrywania spamu jest jedną z kluczowych i najbardziej priorytetowych funkcjonalności Watchmana.  
**Dlaczego?** Odpowiedź jest bardzo prosta - Watchman docelowo ma być botem, mogącym zastąpić inne boty.  
Oznacza to, że poza funkcjonalnościami takimi jak rozbudowane statystyki czy rozbudowane opcje konfiguracji, powinniśmy również dostarczać sprawne zabezpieczenia przed spamem - który jest zagrożeniem, którego doświadcza myślę że każdy większy serwer raz na jakiś czas.

## System bezpiecznych i niebezpiecznych użytkowników

Uznaliśmy że bardzo trudno ustalić sztywne reguły wykrywania spamu, które będą sprawiedliwe w każdym przypadku, a dodatkowo nikomu nie będą przeszkadzać.  
Dlatego powstał system klasyfikacji użytkowników, który działa na bardzo prostej zasadzie:

- Jeśli jesteś nowy, traktujemy cie neutralnie
- Jeśli jesteś aktywny i nie ma z tobą problemów, traktujemy cie lżej
- Jeśli sprawiasz problemy, traktujemy cie bardziej rygorystycznie

Aby zostać uznanym za niebezpiecznego użytkownika, trzeba zostać wykrytym jako potencjalny spamer - dzieje się tak przykładowo po zostaniu wyciszonym za spam, w ostatnim czasie.  
Jeśli ktoś nie sprawiał problemów, po pewnym czasie przestaje być uznawany za niebezpiecznego.

Aby zostać uznanym za bezpiecznego... tutaj mechanizm jest bardziej skomplikowany - te osoby również mogą zostać ukarane za spam, jednak żeby do tego doszło, muszą się bardziej postarać, więc przydzielenie komuś etykiety "bezpieczny" powinno następować jedynie wtedy, kiedy ta osoba na to zasłużyła.  
Dlatego przy tym mechanizmie mamy sporo wartości konfiguracyjnych - takich jak wymagana średnia dzienna ilość wiadomości w ostatnim czasie, czas bycia na serwerze, minimalna suma wszystkich wiadomości i inne.  
Wszystko to, aby zminimalizować ryzyko przypięcia tej etykiety do użytkownika, który nie jest aktywny regularnie (z naciskiem na regularnie), przy czym jednocześnie nie sprawia problemów.

## Czym są detektory

Powiedzieliśmy sobie o tym jak dostosowujemy kary do różnych typów użytkowników, teraz pora omówić skąd te kary w ogóle się biorą.

Detektory zajmują się rozpoznawaniem konkretnych specyficznych zagrożeń i zwracaniem informacji jaka jest szansa na to zagrożenie - przykładowo zerowa, niska, średnia... lub że są pewne.  
Na podstawie tego jak dużo detektorów w jakim stopniu jest pewnych tego, za co są odpowiedzialne, system decyduje o ostatecznej decyzji.  
Po otrzymaniu podsumowania wyników detektorów, ustalamy kare dla użytkownika na podstawie wspomnianego podsumowania i wspomnianego typu użytkownika (niebezpieczny/neutralny/bezpieczny).

Przyjęliśmy zasadę że zawsze pierwszym krokiem jest ostrzeżenie o wykrytym potencjalnym spamie, które nie jest niebezpieczne - jednak jeśli ktoś zignoruje ostrzeżenie, zostaje wyciszony.  
Czas wyciszenia jest zależny od tego jak dużo razy został wyciszony w ostatnim czasie.

## Lista domyślnych detektorów

#### LinksDetectorStrategy

Sprawdza czy wiadomość zawiera link - chcemy aby takie wiadomości zwiększały szanse na bycie wykrytymi jako spam, żeby ewentualny spam groźnymi linkami został wykryty możliwie najwcześniej.

#### DuplicatedMessagesDetectorStrategy

Porównuje aktualną wiadomość z poprzednimi - jeśli ktoś wysłał wiele razy tą samą wiadomość, istnieje duże prawdopodobieństwo że jest to nie dość że spam, to jeszcze bezmyślny.  
Wewnątrz tego detektora działa również algorytm porównywania wiadomości, więc nawet jeśli ktoś nie wysyła identycznych tekstów, ale są one bardzo podobne - zostaną rozpoznane jako duplikaty, z tym że nie z 100% prawdopodobieństwem.

#### CapslockDetectorStrategy

Osoby które nie dość że spamują, to w dodatku robią to z caps lockiem, powinny być szybciej wykrywane.  
Powód - dla wielu osób wiadomości pisane głównie caps lockiem są irytujące.  
Im szybciej ktoś przestanie denerwować innych, tym lepiej.

#### FloodDetectorStrategy

Detektor będący tym, co wiele osób rozumie jako system anty spamowy - czyli sprawdzanie jak dużo wiadomości zostało wysłanych w określonym czasie.

Dodatkowo część detektorów jest ustawionych na sprawdzanie również wiadomości innych użytkowników - ma to na celu chronienie serwera przed sytuacją, gdzie nie atakuje nas jeden bot/użytkownik spamujący, tylko jest ich bardzo dużo, a każdy z nich wysyła wiadomość w dużych odstępach czasu.

Wiele prostych systemów anty spamowych ma ustawione wykrywanie spamu kiedy przykładowo jeden użytkownik wysłał +5 wiadomości w ostatnich 5 sekundach - ale co jeśli każdy z użytkowników wysyła jedną wiadomość na 5 sekund, a tych użytkowników jest 20?  
Wtedy będziemy mieć 100 wiadomości na 5 sekund, czyli 20 na sekunde... których bot nie będzie w stanie wykryć.  
No chyba że ktoś użyje Watchmana - wtedy takie osoby/boty zostaną wykryte tak szybko jak to możliwe.
