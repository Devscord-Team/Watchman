# Co testować i jak testować  
  
W obecnej chwili mamy problem z zbyt małą ilością testów - przydałoby się coś z tym zrobić.  
Uznałem że dobrym pomysłem będzie pisanie testów do funkcjonalności na bieżąco podczas tworzenia ich, czyli **jeśli tworzysz pull requesta, upewnij się, że są napisane testy do funkcjonalności którą dodawałeś lub zmieniałeś** - no chyba że nie warto tego testować.  
  
## W jakich stuacjach nie powinniśmy pisać testów  
  
Jeśli mamy za mało testów - jest źle bo łatwo coś zepsuć i się o tym nie dowiedzieć.  
Oznacza to, że musimy więcej czasu poświęcać na manualne testowanie funkcjonalności a ostatecznie i tak nie mamy pewności czy wszystko działa prawidłowo.  
Jednak jeśli mamy za dużo testów - jest źle, bo nie dość że continuous integration działa wolniej niż powinno, to jeszcze straciliśmy dużo czasu.  
Dlatego właśnie powinniśmy pisać testy jedynie wtedy, kiedy mają one jakieś sensowne uzasadnienie.  
  
Przykładowo - jeśli piszemy bardzo prosty mechanizm który jedyne co robi to zwraca prosty tekst, przykładem mogą być komendy `-dontask` albo `-google`, nie ma sensu pisać testów bo kod jest zbyt prosty, pull requesty zatwierdzają zawsze dwie osoby więc jeśli cały kod wygląda w taki sposób a w dodatku jest odpowiedzialny za działanie bardzo małej części systemu, pisanie testów nie ma tutaj żadnego sensu.  
  
Można powiedzieć, że pisanie testów do kontrolerów z definicji nie ma sensu, bo kod kontroletów powinien być na tyle prosty, że nie powinno być potrzeby testowania go.  
Jeśli kod kontrolera jest na tyle skomplikowany, że czujesz potrzebę przetestowania go - przenieś tą logikę do serwisu.  
Pamiętaj również o tym, żeby przypadkiem nie popaść w drugą skrajność, czyli przenoszenie całej logiki do serwisu.  
Kontroler powinien odpowiadać na pytanie `"co ta komenda robi?"` a serwis powinien odpowiadać na pytanie `"jak to się dzieje, że to działa?"` - i właśnie to "jak to się dzieje że to działa" zazwyczaj jest na tyle skomplikowane, że powinno być testowane.  
  
Podobnie wygląda w przypadku elemenów `core'owych` - czyli takich które są kluczowe dla działania aplikacji, bo wiele mechanizmów jest od nich zależnych.  
Przykładowo parsowanie komend powinno być dobrze przetestowane, bo od tego zależy działanie prawie całego projektu.  
Obsługa odpowiedzi (Responses) powinna być dobrze przetestowana, bo korzysta z niej praktycznie każda komenda.  
I tak dalej... takich przykładów można wymienić sporo.  
Jednak są tutaj pewne wyjątki, a dokładniej (tak jak to było z kontrolerami) - prosty kod.  
**Nie ma sensu testować prostego kodu!!** - nie ma sensu testować przykładowo CommandHandlerów bo ich kod jest bardzo prosty. Co prawda zależy od nich działanie wielu krytycznych elementów w systemie, ale czy serio jest sens sprawdzać czy kod który wygląda tak:  
```csharp  
namespace Watchman.DomainModel.Responses.Commands.Handlers  
{  
    public class AddOrUpdateResponsesCommandHandler : ICommandHandler<AddOrUpdateResponsesCommand>  
    {  
        private readonly ISessionFactory _sessionFactory;  
  
        public AddOrUpdateResponsesCommandHandler(ISessionFactory sessionFactory)  
        {  
            this._sessionFactory = sessionFactory;  
        }  
  
        public async Task HandleAsync(AddOrUpdateResponsesCommand command)  
        {  
            using var session = this._sessionFactory.CreateMongo();  
            foreach (var response in command.ResponsesToAddOrUpdate)  
            {  
                await session.AddOrUpdateAsync(response);  
            }  
        }  
    }  
}  
```  
rzeczywiście ma sens... moim zdaniem nie ma żadnego sensu, bo na pierwszy rzut oka dokładnie widzimy co się tam stanie.  
Nie ma tutaj logiki która mogłaby sprawić, że mechanizm zachowa sie inaczej niż myślimy.  
Co innego w przypadku takiej logiki:  
```csharp  
        public object GetValueByNameFromCustomCommand(string key, bool isList, BotCommandTemplate template, Match match)  
        {  
            var value = match.Groups[key].Value.Trim();  
            var argType = template.Properties.First(x => x.Name.ToLowerInvariant() == key.ToLowerInvariant()).Type;  
            if (argType == BotCommandPropertyType.Bool)  
            {  
                return string.IsNullOrWhiteSpace(value) ? bool.FalseString : bool.TrueString;  
            }  
            if (argType == BotCommandPropertyType.SingleWord)  
            {  
                return value.Split().First().Trim('\"');  
            }  
            if (argType == BotCommandPropertyType.Text)  
            {  
                return value.Trim('\"');  
            }  
            if (!isList)  
            {  
                return value;  
            }  
  
            if (!value.Contains('\"'))  
            {  
                return value.Split().Where(x => !string.IsNullOrEmpty(x))  
                    .ToList();  
            }  
            var splittedResults = value.Split('"').Where(x => !string.IsNullOrWhiteSpace(x));  
            var results = new List<string>();  
            foreach (var toRemove in splittedResults)  
            {  
                if (value.Contains($"\"{toRemove}\""))  
                {  
                    // here we're adding text with quotation marks to the results, but texts without quotation marks are added later  
                    results.Add(toRemove);  
                }  
                value = value.Replace($"\"{toRemove}\"", string.Empty);  
            }  
            var otherResultsWithoutQuote = value.Split().Where(x => !string.IsNullOrWhiteSpace(x));  
            results.AddRange(otherResultsWithoutQuote);  
            return results;  
        }  
```  
tutaj bardzo łatwo zrobić błąd, więc kod powinien być solidnie przetestowany.  
  
## Odróżniaj rodzaje testów  
  
Dużo osób mających małe doświadczenie z testami, nie odróżnia testów jednostkowych od testów integracyjnych itd.  
Zasada jest dość prosta  
  
### Testy jednostkowe  
  
Powinny zawsze sprawdzać tylko jedną funkcjonalność a ta funkcjonalność powinna robić tylko jedną rzecz (zasada SRP z zasad SOLID).  
Jeśli sprawdzamy funkcjonalność i przy okazji jej dodatkowe zależności, albo tworzymy dodatkowy kod który jest właśnie tymi dodatkowymi zależnościami (i potencjalnym miejscem z błędami), to to już nie są testy jednostkowe.  
Do wstrzykiwania zależności nie powinniśmy używać rzeczywistych klas, powinniśmy używać jedynie mocków.  
Jeśli musimy użyć innej klasy która ma jakąś logike albo (co gorsze) musimy samodzielnie taką klase napisać... to znaczy że koncepcja jest wadliwa u podstaw.  
**Do symulowania działania zależności powinniśmy zawsze używać mocków**.  
Możliwe że aktualny kod często nie będzie na to pozwalał, bo w wielu miejscach są wstrzykiwane bezpośrednio klasy zamiast interfejsów - w takim przypadku powinniśmy zamienić te klasy na interfejsy i przetestować je prawidłowo, zamiast robienia syfu w testach.  
Może być też tak że funkcjonalność nie spełnia zasady SRP więc nie da się jej przetestować jednostkowo - wtedy powinniśmy zrefaktoryzować tą funkcjonalność.  
Może być też tak że wydaje nam się, że coś nie spełnia zasady SRP, ale w tym przypadku ją spełnia, a dokładniej `tą odpowiedzialnością jest zarządzanie czymś lub podejmowanie decyzji na podstawie kilku zależności` - wtedy wszystko jest jak najbardziej w porządku... no chyba że ta funkcjonalność robi coś poza wspomnianym podejmowaniem decyzji - `przykładowo najpierw oblicza te zależności a później podejmuje decyzje na podstawie wyników`.  
  
### Testy integracyjne  
  
Powinny sprawdzać czy ciągi zdarzeń działają prawidłowo.  
Dzięki temu piszemy mniej takich testów niż testów integracyjnych, ale też sprawdzamy kod z mniejszą dokładnością i w przypadku wykrycia błędu, dostajemy mniej informacji co było źle.  
Test jednostkowy powie nam dokładnie co się psuje i w jakim przypadku, a test integracyjny powie ogólnie "coś w tym obszarze sie zepsuło, sprawdź co konkretnie".  
  
W idealnych warunkach nie pisalibyśmy testów integracyjnych, pisalibyśmy jedynie bardzo dużo testów jednostkowych.  
Jednak... takie warunki nie istnieją, bo nie mamy aż tyle czasu żeby pisać do wszystkiego testy jednostkowe, nie mamy aż tyle czasu żeby czekać aż wszystkie te testy jednostkowe się wykonają podczas continuous integration i... nie wszystko da się przetestować testami jednostkowymi.  
  
Przykładowo - nie da się przetestować testami jednostkowymi integracji z zewnętrznymi usługami/bibliotekami.  
Przykładowo nie da się sprawdzić czy w prawidłowy sposób korzystamy z zewnętrznego api ani czy nasz sposób używania zewnętrznej biblioteki jest na pewno taki, jaki powinien być. Może robimy coś nie tak jak twórca przewidział i przez to dostajemy dziwne wyniki - nie dlatego że nasz kod ma błędy, tylko dlatego, że źle zrozumieliśmy sposób działania zewnętrznej usługi.  
  
Co do braku czasu na testowanie wszystkiego - w niektórych przypadkach nie ma sensu testowanie wielu prostych kawałków kodu, ale te proste kawałki kodu po połączeniu zaczynają tworzyć skomplikowany mechanizm... który już powinniśmy przetestować.  
Moglibyśmy to samo osiągnąć pisząc testy jednostkowe do wszystkich prostych kawałków kodu, ale co jeśli jest ich bardzo dużo i w 90% to będzie jedynie strata czasu? (a zazwyczaj tak jest)  
  
### Testy end to end (e2e)  
  
W przypadku testów integracyjnych sprawdzających jakiś proces od początku do końca, nazywamy je testami end to end.  
Przykładowo test api gdzie sprawdzamy cały proces od obsłużenia endpointa, przez sprawdzenie autoryzacji/autentykacji aż po sprawdzenie działania całej funkcjonalności, albo procesu kilku funkcjonalności (np stworzenie użytkownika, weryfikacja konta, logowanie) - będzie testem end to end, bo sprawdzamy cały proces.  
Co więcej - takie testy powinny być oparte o symulowanie ruchu użytkownika, bo tylko wtedy możemy powiedzieć o procesie przetestowanym od początku do końca.  
  
Testem integracyjnym będzie sprawdzenie procesu który jest wycinkiem z całości, przykładowo sam proces rejestracja-weryfikacja-logowanie, ale już bez sprawdzania routingu i obsługi api, przetestujemy wszystko używając jedynie wewnętrznych klas, a nie symulując ruch potenchalnego użytkownika (w przypadku api będzie to symulowanie aplikacji client).  
  
### Testy manualne  
  
Jak nazwa wskazuje - testy które wykonuje człowiek zamiast automatu.  
Mają taki plus że w niestandardowych przypadkach człowiek lepiej zauważy pewne dziwne zachowania... no i nie trzeba pisać do nich testów.  
Chociaż można pisać do nich scenariusze - w przypadku oprogramowania którego działanie jest krytycznie ważne, są zespoły testerów manualnych którzy "wyklikują" wszystko w poszukiwaniu potencjalnych błędów, ale także wykonują scenariusze, których nie można łatwo zastąpić testami automatycznymi, bo wyniki musi zweryfikować człowiek. W przypadku testów automatycznych jest duża szansa że zapomnimy o sprawdzeniu konkretnego przypadku albo automat uzna nieprawidłowy wynik za prawidłowy, bo tester jeszcze nigdy się z nim nie spotkał - więc nie wie że powinien na niego uważać.  
  
### Piramida testów  
  
Jest również coś takiego jak piramida testów - przypominająca trochę piramidę Maslova (znana również jako piramida potrzeb).  
Piramida ta może przyjmować różne formy, przykładowo prostokąta, klepsydry, odwróconej piramidy lub właśnie klasycznej piramidy - czyli trójkąta, szeroki na dole, średni w środku i wysoki na górze.  
  
Koncepcja piramidy testów jest taka, że  
- im wyżej jest punkt, tym testy dają nam mniej informacji, ale sprawdzają większy obszar (więc może być ich mniej)  
- im niżej jest punkt, tym testy dają nam więcej informacji, ale sprawdzają mniejszy obszar (więc powinno być ich więcej)  
  
Czyli na samym dole są testy jednostkowe, wyżej testy integracyjne, wyżej testy e2e a na szczycie testy manualne.  
  
Szerokość piramidy w tym punkcie określa ilość testów w aplikacji.  
Przykładowo jeśli będziemy mieć porównywalną ilość testów każdego rodzaju - piramida będzie przypominać kwadrat.  
Jeśli najwięcej będzie testów manualnych i e2e, sporo integracyjnych i mało jednostkowych - będzie to odwrócony trójkąt lub inaczej odwrócona piramida.  
  
Są różne rodzaje systemów i różne systemy będą wymagać różnych proporcji między testami, jednak w zdecydowanej większości (m.in w naszym projekcie) idealny kształt to będzie (jak nazwa wskazuje) piramida, czyli trójkąt - dużo testów jednostkowych, troche testów integracyjnych i moooże jakieś testy e2e albo manualne.  
**Nawet jeśli aktualnie tak nie jest - starajmy się do tego dążyć.**  
  
## Umieszczaj testy w prawidłowych DLL  
  
Gdyby ktoś nie wiedział -> dll = csproj = projekt w solucji.  
  
Testy do dll powinny być w dll o nazwie `TestowanaDLL.IntegrationTests` lub `TestowanaDLL.UnitTests` w zależności jakie testy chcemy pisać.  
A namespace wewnątrz dll z testami powinnien być zbliżony do namespace testowanej klasy w testowanej dll.  
Przykładowo  
  
Jeśli chcemy przetestować klase `ExampleService` która znajduje sie w dll `ExampleProject.Core`  
I ma namespace `ExampleProject.Core.Services`  
Testy jednostkowe powinny się znajdować w `ExampleProject.Core.UnitTests` i w namespace `ExampleProject.Core.UnitTests.Services`.  
Testy integracyjne powinny się znajdować w `ExampleProject.Core.IntegrationTests` i namespace `ExampleProject.Core.IntegrationTests.Services`.  
  
## Inni nie zawsze piszą prawidłowy kod  
  
Większość początkujących lubi patrzeć na kod osób bardziej doświadczonych jak na wzór do naśladowania.  
Przez co kod słabej jakości zaczyna się rozprzestrzeniać i z małego kawałka (będącego dla niektórych "dobrym przykładem") staje się większością.  
  
Wielu dobrych programistów często pisze kod który nie jest najlepszej jakości z wielu powodów  
- podczas pisania go nie było wystarczająco dużo czasu  
- to był mały kawałek kodu więc nie było potrzeby przesadnego starania sie  
- to kawałek aplikacji do którego niedługo wrócą i go poprawią, na razie jest jedynie prototyp  
(brzmi jak wymówka, ale niestety czasami tak trzeba)  
  
Więc zanim zaczniecie inspirować się kodem innym, pomyślcie sami czy na pewno będzie to dobry pomysł, ewentualnie zapytajcie kogoś bardziej doświadczonego czy to co robicie na pewno będzie prawidłowe - żeby nie stracić czasu na pisanie ogromnego kawałka kodu, który będzie błędny u podstaw - więc będzie prawie cały do wyrzucenia.  
