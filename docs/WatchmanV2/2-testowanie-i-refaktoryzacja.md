# Testowanie i refaktoryzacja

## Jakich bibliotek używamy do testów

- [NUnit](https://nunit.org/)
- [Fluent Assertions](https://fluentassertions.com/)
- [Moq](https://github.com/Moq/moq4/wiki/Quickstart)
- [Auto Fixture](https://github.com/AutoFixture/AutoFixture)

## Nazewnictwo

Klasy testów mają taką samą nazwę jak klasy, które są w nich testowane. Przykładowo testy do klasy `Calculator` będą w klasie `CalculatorTests`.

Nazwy testów piszemy w konwencji `NazwaMetody_CoMaSprawdzaćTest`, przykładowo metoda `Divide` powinna mieć testy `Divide_ShouldDivideTwoNumbersProperly` i `Divide_ShouldThrowExceptionIfDivideByZero`.

Staramy się umieszczać testy w takich samych przestrzeniach nazw, co ich odpowiedniki w kodzie, przykładowo jeśli klasa `Calculator` ma namespace `Application.Services`, test powinien być w `UnitTests.Services`. W niektórych sytuacjach dopasowywanie się 1:1 do oryginalnej przestrzeni nazw może być przerostem formy nad treścią więc dopuszczamy wyjątki, ale jeśli nie ma konkretnego argumentu żeby zrobić inaczej, staramy się robić według wyżej opisanego sposobu.

## AAA

W testach stosujemy się do dobrej praktyki AAA, czyli
- **Arrange** - przygotowanie potrzebnych danych
- **Act** - wykonanie akcji
- **Assert** - sprawdzenie wyników

Dobrym przykładem może być jeden z prostszych testów, który dobrze pokazuje jak taki podział powinien wyglądać.
```csharp
[Test, AutoData]
public async Task Send_ShouldSendMessage(SendCommand command, Contexts contexts)
{
    //Arrange
    var messagesServiceMock = new Mock<IMessagesService>();
    var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
    messagesServiceFactoryMock
        .Setup(x => x.Create(It.IsAny<Contexts>()))
        .Returns(messagesServiceMock.Object);
    var controller = new SendController(messagesServiceFactoryMock.Object);

    //Act
    await controller.Send(command, contexts);

    //Assert
    messagesServiceMock.VerifySet(x => x.ChannelId = command.Channel, Times.Once);
    messagesServiceMock.Verify(x => x.SendMessage(command.Message, It.IsAny<MessageType>()), Times.Once);
}
```

## Mock i Stub

Mockowanie, do którego używamy biblioteki [Moq](https://github.com/Moq/moq4/wiki/Quickstart), to generowanie specjalnych obiektów, których działanie możemy skonfigurować na potrzeby specyficznego testu. Poza konfiguracjami które są dedykowane dla testów, możemy sprawdzać czy na pewno były użyte w prawidłowy sposób wewnątrz testowanej metody, możemy zweryfikować co było używane, jak dużo razy, z użyciem jakich danych itd. Możliwości jest dużo.

Stub to rzeczywista klasa, która imituje działanie innej klasy, ale w bardzo prymitywny sposób, bo nie zawiera logiki, zwraca zawsze sztywne i stałe wartości, nie pozwala na weryfikacje.

Można powiedzieć że stub to taki biedny mock, albo że mock to stub na sterydach.

A więc kiedy którego używać? Mock daje nam bardzo dużo możliwości, ale wymusza tworzenie konfiguracji w przypadku każdego testu, często jest to bardzo przydatne, ale są też sytuacje w których wystarczy nam bardzo prymitywny obiekt, który jedynie zwraca sztywne dane, a jego oryginalny odpowiednik ma proste i przewidywalne działanie.

**Ważne**
Jeśli klasa którą testujemy ma bardzo dużo zależności, używamy specjalnych fabryk, które można znaleźć w ścieżce `TestObjectFactories`. Fabryki te, zawierają metody tworzące klasy które chcemy testować, ale już uzupełnione o mocki. Mocki są uzupełniane w taki sposób, że w prosty sposób możemy podmieniać je własnymi.
```csharp
internal AdministrationController CreateAdministrationController(
    Mock<IQueryBus> queryBusMock = null, Mock<IUsersService> usersServiceMock = null, 
    Mock<IDirectMessagesService> directMessagesServiceMock = null, Mock<IMessagesServiceFactory> messagesServiceFactoryMock = null, 
    Mock<IRolesService> rolesServiceMock = null, Mock<ITrustRolesService> trustRolesServiceMock = null,
    Mock<ICheckUserSafetyService> checkUserSafetyServiceMock = null, Mock<IUsersRolesService> usersRolesServiceMock = null, 
    Mock<IConfigurationService> configurationServiceMock = null)
{
    queryBusMock ??= new Mock<IQueryBus>();
    usersServiceMock ??= new Mock<IUsersService>();
    directMessagesServiceMock ??= new Mock<IDirectMessagesService>();
    messagesServiceFactoryMock ??= new Mock<IMessagesServiceFactory>();
    rolesServiceMock ??= new Mock<IRolesService>();
    trustRolesServiceMock ??= new Mock<ITrustRolesService>();
    checkUserSafetyServiceMock ??= new Mock<ICheckUserSafetyService>();
    usersRolesServiceMock ??= new Mock<IUsersRolesService>();
    configurationServiceMock ??= new Mock<IConfigurationService>();

    return new AdministrationController(
        queryBusMock.Object,
        usersServiceMock.Object,
        directMessagesServiceMock.Object,
        messagesServiceFactoryMock.Object,
        rolesServiceMock.Object,
        trustRolesServiceMock.Object,
        checkUserSafetyServiceMock.Object,
        usersRolesServiceMock.Object,
        configurationServiceMock.Object);
}
```
Przykładowe wykorzystanie
```csharp
var rolesServiceMock = new Mock<IRolesService>();
var controller = this.testControllersFactory
    .CreateAdministrationController(rolesServiceMock: rolesServiceMock);
```

Dzięki temu rozwiązaniu, możemy w prosty sposób stworzyć obiekt uzupełniony wszystkimi zależnościami i przekazać mu tylko tą zależność, nad którą chcemy mieć kontrolę i która ma zachowywać się w sposób opisany w teście.
(**uwaga:** bardzo możliwe, że to rozwiązanie będzie poprawione w przyszłości)

## Dane testowe

Dane testowe można podzielić na te, które wpływają na wynik i na te, które niezależnie jakie będą, nie wpłyną na wynik. 

W przypadku danych które mają znaczenie, musimy wpisywać je ręcznie. Testy które testują tylko jeden zestaw danych, mogą mieć dane wpisane na sztywno jako zmiennie, w przypadku testów które testują dużo zestawów danych, musimy te dane umieścić w parametrach testu i uzupełniać je za pomocą atrybutu `[TestCase]`.

Jeśli dane nie mają znaczenia (muszą jedynie istnieć), możemy użyć biblioteki [Auto Fixture](https://github.com/AutoFixture/AutoFixture).

Jeśli test ma jeden zestaw danych, możemy użyć atrybutu `[AutoData]` i AutoFixture samo uzupełni nam parametry testu. 
```csharp
[Test, AutoData]
public async Task Send_ShouldSendMessage(SendCommand command, Contexts contexts)
```
W przypadku kiedy to my chcemy uzupełnić parametry testu, powinniśmy skorzystać z klasy `Fixture`, która pozwala na konfigurowanie i tworzenie obiektów.
```csharp
[Test]
[TestCase(true, true, 1, typeof(InvalidArgumentsException))]
[TestCase(false, false, 1, typeof(NotEnoughArgumentsException))]
[TestCase(true, false, 6, typeof(InvalidArgumentsException))]
[TestCase(true, false, 0, typeof(InvalidArgumentsException))]
[TestCase(true, false, 5, null)]
[TestCase(true, false, 1, null)]
public void SetRoleAsSafe_ShouldThrowExceptionWhenCommandNotMatchRules(bool safeParam, bool unsafeParam, int rolesCount, Type exceptionType)
{
    //Arrange
    var fixture = new Fixture();
    var contexts = fixture.Create<Contexts>();
```

## Fluent Assertions vs NUnit

Proponuję używanie [Fluent Assertions](https://fluentassertions.com/) we wszystkich miejscach, w których to jest możliwe, ponieważ zwiększa czytelność testów. [Fluent Assertions](https://fluentassertions.com/) traktujemy jako alternatywę do `Assert` z [NUnit](https://nunit.org/), więc w przypadku testów mocków, musimy korzystać z weryfikacji którą dostarcza nam [Moq](https://github.com/Moq/moq4/wiki/Quickstart), której styl jest w pewnym stopniu podobny do [Fluent Assertions](https://fluentassertions.com/).

## Refaktoryzacja kodu podczas pisania testów

Podczas pisania testów do kodu, który nigdy nie był testowany, możliwe że bez poprawienia go, testy będą trudne do napisania. Możliwe że niektóre metody trzeba będzie podzielić na mniejsze, a niektóre klasy powinny być podzielone na kilka niezależnych. Pamiętaj jednak żeby przed zrobieniem wielu zmian w kodzie na potrzeby testu, zapytać nas czy to na pewno jest dobry pomysł (jako komentarz w issue lub na odpowiednim kanale na naszym serwerze discord).

Poprawianie kodu przy okazji pisania testów jest dobrym pomysłem, ale przebudowywanie połowy aplikacji na potrzeby jednego testu, nie zawsze ma sens. Czasami można osiągnąć podobny poziom testowalności przy mniejszej ilości zmian.
