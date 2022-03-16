# Flow releasowe

Opis procesu dzięki któremu Twoje zmiany w kodzie trafiają na środowisko produkcyjne.

## Pull request do mastera

Na tym etapie, poza manualną weryfikacją kodu, odpalamy automaty mające sprawdzić czy kod spełnia określone warunki.

- Czy projekt da się zbudować?
- Czy wszystkie testy przechodzą?
- Czy jakość kodu nie uległa pogorszeniu?

Jeśli wszystko przeszło prawidłowo i pull request został zmergowany, następuje automatycznie:

## Commit w branchu master

Każdy commit poza ponownym odpaleniem (dla pewności) wcześniej wspomnianych automatów, dodatkowo

- Buduje i aktualizuje obraz dockera na portalu DockerHub [link do obrazu](https://hub.docker.com/repository/docker/marcin99b/watchman-web)

Oznacza to, że obraz dockerowy na DockerHubie jest zawsze aktualny z kodem na branchu master.

## Pull request master -> release

W tym kroku, poza tym, że ponownie odpalamy automaty do weryfikacji czy kod na pewno jest prawidłowy

- Przeglądamy manualnie kod, żeby upewnić się że nie wrzucimy na produkcję czegoś nieprawidłowego. Teoretycznie podczas sprawdzania pull requesta powinny wyjść wszystkie błędy, jednak praktyka pokazuje że czasami lubi się tam coś prześlizgnąć, warto sprawdzać kod dwa razy

Jeśli zaakceptujemy pull requesta:

## Commit w branchu release

Poza ponownym odpaleniem automatów weryfikujących kod

- Odpalamy automat, który loguje się do serwera na którym działa Watchman, aktualizuje lokalną wersje obrazu dockerowego, a następnie podmienia ją z obecnie działającą

Dzięki takiemu rozwiązaniu, czas w którym bot jest fizycznie wyłączony jest na tyle krótki, że powinien być niezauważalny dla użytkownika.
