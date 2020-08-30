# Czym są CustomCommands

Aktualnie to jak wyglądają komendy definiują modele, a dokładniej to jakie mają propertiesy i jakich typów są te propertiesy (przykładowo typ numeryczny, tekstowy, czasowy).  
Przykładowo jeśli komenda zawiera `public int Test { get; set; }` - będzie wymagała argumentu `-test jakaśLiczba`.

Jednak co w przypadku kiedy właściciele/moderatorzy pewnego serwera by zdecydowali, że chcą aby ich komendy nazywały się inaczej?  
Przykładowo chcieliby zastąpić `-addrole -roles test` komendą `-add role test`, albo idąc dalej `daj dla mnie role test`.  
Domyślnym sposobem się nie da - na szczęście przychodzą nam z pomocą komendy niestandardowe, lub tak jak my to nazywamy w systemie - CustomCommands.

## Jak działają CustomCommands

Idea jest bardzo prosta - pomysłodawca wyglądu komendy pisze regexa, który ma grupy.  
Grupy nazywają się tak jak propertiesy w modelu.  
A wartości które do nich wpadają, są prawidłowego typu.

Później następuje proces walidacji - czy na pewno wszystkie wymagane wartości są prawidłowo ustawione oraz czy na pewno wszystkie wartości są prawidłowego typu.  
A po nim proces konwersji na model, który trafia już do kontrolera.

Proces dzieje się można powiedzieć że dosłownie obok obsługi IBotCommands (z kilkoma elementami wspólnymi).  
Oznacza to że nie ma sensu dokładnie tłumaczyć co dzieje się z CustomCommands krok po kroku - jeśli ktoś wie w jaki sposób obsługujemy IBotCommands, automatycznie będzie rozumiał jak przetwarzamy CustomCommands.  
Jedyna zauważalna różnica to to, że w przypadku IBotCommands wyciągamy wartości według automatycznie wygenerowanego schematu mówiącego jak powinna wyglądać komenda, a w przypadku CustomCommands bierzemy te wartości z regexa, którego wcześniej przypisał moderator do konkretnej komendy.
