# Jak rozwijać dokumentacje

Dzięki systemowi jaki oferuje nam [https://readthedocs.org/](https://readthedocs.org/) rozwijanie dokumentacji jest bardzo proste i intuicyjne, bo jedyne o czym musimy pamiętać to dodawanie plików z rozszerzeniem `.md` do folderu `/docs`, oraz konfiguracji pliku `mkdocs.yml` w ścieżce bazowej.

## Folder /docs

W folderze [/docs](https://github.com/Devscord-Team/Watchman/tree/master/docs) znajdują się wszystkie pliki `.md` odpowiadające stronom w dokumentacji.

#### O czym warto pamiętać przy pisaniu plików .md

- umieszczaj kod w przeznaczonych do tego blokach
- symbol nowej linii to podwójny enter lub podwójna spacja, my zdecydowaliśmy się na dodawanie podwójnej spacji na koniec każdej linijki tekstu, można ten proces zautomatyzować używając dowolnego edytora tekstu (np vs code)  
  wchodząc w opcje podmiany tekstu i zamieniając tekst złapany regexem `\s*$` na dwie spacje

Dodatkowo polecam przeczytanie [https://guides.github.com/features/mastering-markdown/](https://guides.github.com/features/mastering-markdown/).

## Plik mkdocs.yml

Wewnątrz pliku [mkdocs.yml](https://github.com/Devscord-Team/Watchman/blob/master/mkdocs.yml) możesz łatwo zauważyć konfigurację który z plików `.md` we wspomnianym folderze `/docs` odpowiada któremu miejscu w menu.  
Jest to konfiguracja struktury menu - informacje który odnośnik do dokumentu powinien być w którym miejscu menu.

Dodatkowo plik `mkdocs.yml` zawiera konfiguracje tego, jak powinna wyglądać i zachowywać się dokumentacja - z których plików stylu powinna korzystać, w jakim języku powinny być wbudowane elementy, jakiej czcionki chcemy używać itd.  
Nie jest ona duża, ponieważ to co dostarcza nam [https://readthedocs.org/](https://readthedocs.org/) w połączeniu z wybranym przez nas stylem graficznym, jest po prostu wystarczające.

## O czym warto pamiętać pisząc dokumentację

- dokumentacji nie piszesz dla siebie, tylko dla osoby która poznaje temat od zera
- od tematów które każdy umie wytłumaczyć w minute dużo ważniejsze są te, które mało kto kojarzy
- ktoś kto czyta dokumentacje, niekonieczne wie dokładnie jak działa cały projekt, a tym bardziej co znajduje sie w którym pliku - staraj się tłumaczyć wszystko dokładnie i unikaj skrótów myślowych
- odpowiednie formatowanie tekstu potrafi drastycznie poprawić lub zepsuć jego czytelność
