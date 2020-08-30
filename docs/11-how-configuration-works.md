# Jak działa konfiguracja

Szerokie możliwości konfiguracyjne to jeden z naszych priorytetów.  
Poza funkcjonalnościami takimi jak konfigurowalne komendy czy konfigurowalne odpowiedzi bota, ważne są też konfigurowalne... wszystkie inne elementy.

Tutaj skupimy się na tym jak działa konfiguracja w miejscach, które nie mają swojej dedykowanej konfiguracji.  
Jako dedykowaną konfigurację rozumiem przykładowo to, co się dzieje przy konfigurowalnych odpowiedziach.

## ConfigurationItems i MappedConfiguration\<T\>

Bazowym obiektem jest są tutaj tak zwane ConfigurationItems, czyli pojedyncze obiekty, trzymające wartości konfiguracji.  
Każdy z tych obiektów trzyma wartość która ma ściśle określony typ - przykładowo `MinAverageMessagesPerWeek` trzyma wartość `int`, więc do czasu kiedy tego nie zmienimy, nie będzie się dało wrzucić tam innego typu danych.

Każdy z obiektów dziedziczy po klasie `MappedConfiguration<T>`, gdzie `T` to wspomniany typ danych, jaki trzyma obiekt konfiguracyjny.

```csharp
    public abstract class MappedConfiguration<T> : IMappedConfiguration
    {
        public abstract T Value { get; set; }
        public ulong ServerId { get; }
        public string Name { get; }

        public MappedConfiguration(ulong serverId)
        {
            this.Name = this.GetType().Name;
            this.ServerId = serverId;
        }
    }
```

Przez dziedziczenie, każdy z obiektów posiada również `ServerId` - oznacza to, że tak jak w przypadku m.in konfigurowalnych odpowiedzi, występuje tu podział na wartości domyślne i wartości nadpisane przez konkretny serwer (jeśli serwer czegoś nie nadpisał, używane są wartości domyślne).

## Obsługa konfiguracji

Dzięki serwisowi `ConfigurationService` obsługa konfiguracji jest bardzo prosta i wygodna - ponieważ robi on wszystko czego potrzebujemy pod spodem, razem z obsługą pamięci podręcznej (cache).

Udostępnia on takie metody jak

```csharp
public async Task SaveNewConfiguration(IMappedConfiguration changedConfiguration)
```

Do zapisu konfiguracji - system na podstawie danych w obiekcie domyśli się co do za obiekt i do którego serwera należy.

```csharp
public IEnumerable<IMappedConfiguration> GetConfigurationItems(ulong serverId)
```

Do pobrania wszystkich konfiguracji dla określonego serwera.

```csharp
public IEnumerable<IMappedConfiguration> GetConfigurationItems(ulong serverId)
```

Do pobrania konkretnej konfiguracji z określonego serwera.

Jeszcze raz przypomnę - jeśli serwer nie nadpisał jednej z konfigurowalnych opcji, system zwróci domyślną wartość.
