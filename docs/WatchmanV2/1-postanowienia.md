# Postanowienia projektu WatchmanV2

## Czym jest WatchmanV2

Jak nazwa wskazuje, to druga, odświeżona wersja projektu Watchman. W rozwoju projektu była przerwa trwająca ponad rok, w tym czasie zmieniły się częściowo plany dotyczące projektu - zarówno podejście do tworzenia oprogramowania jak i tego, jak powinien działać produkt końcowy.

## 1. Jasno określone konwencje pisania kodu

- Stosowanie się do powszechnie znanych dobrych praktyk, takich jak SOLID, DRY, KISS, YAGNI, GRASP itd
- Używanie `this` przy odwoływaniu się do elementów (pola, właściwości, metody) z obecnego obiektu
- Używanie `this` zamiast `_` do prywatnych pól
- W przypadku możliwości zaoszczędzenia na wydajności zerowym kosztem, staramy się to robić, jeśli gdzieś możemy stworzyć mniej instancji a czytelność kodu się nie zmieni (lub będzie lepsza), powinniśmy to zrobić

## 2. Design nastawiony na łatwe pisanie testów i duże pokrycie w testach

- Używanie singletonów zamiast klas statycznych
- Używanie interfejsów do wstrzykiwania zależności, zależnościami nie powinny być klasy, jeśli jest to możliwe do wykonania
- Każdy test jednostkowy powinien testować tylko jedną metodę
- Klasy statyczne powinny służyć jedynie do wykonywania prostych funkcji, głównie extension methods
- Testy integracyjne służą do testowania interakcji między kilkoma klasami, granica "odkąd dokąd" testujemy powinna być zależna od konkretnego przypadku testowego
- Nazwy testów jednostkowych powinny zawierać nazwę metody która jest testowana, według wzoru `{Method}_{TestCase}`, przykładowo `RenderProperty_ShouldContainTypeName()`
- Pull requesty powinny zawierać testy do kodu, który został poprawiony/dodany
- W przypadku znalezienia błędu, powinny zostać dodane testy, gwarantujące że ten konkretny problem więcej się nie powtórzy
- Nazwy klas zawierających testy jednostkowe, powinny odpowiadać klasom testowanym i powinny mieć suffix "Tests", przykładowo dla `UsersService` powinna być klasa `UsersServiceTests`

## 3. Korzystanie z issues na githubie do wystawiania prostych zadań

Teraz osoby z zewnątrz miały trudniejszy proces dołączania do projektu, bo każdy musiał być dodany do trello i discorda, a wszystkiego wymagała prywatna komunikacja - to było dość duże utrudnienie i myśle, że istotny czynnik zniechęcający do pomocy przy projekcie. Dużo prościej by było, gdyby część zadań (te które może robić nawet ktoś, kto nie zna bardzo dobrze projektu), było przeniesionych na issues na githubie.

## 4. Obsługa klienta

Bot powinien zawierać możliwość obsługi prywatnych wiadomości, w których użytkownicy mogliby (poza używaniem standardowych komend) zgłaszać problemy i propozycje. Bot powinien mieć możliwość odpowiadania użytkownikom (wiadomości pisane przez osoby z zespołu Watchmana).
