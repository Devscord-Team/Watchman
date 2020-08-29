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
