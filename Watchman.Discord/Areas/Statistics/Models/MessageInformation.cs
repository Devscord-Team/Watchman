using System;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Statistics.Models
{
    public class MessageInformation : Entity //todo stworzyć warstwe z encjami na które będziemy mapować nasze obiekty
    {
        //możliwe że powinniśmy trzymać w obiektach niżej mniej informacji
        //zaoszczędzilibyśmy na mocy obliczeniowej, a informacje mogłyby być później uzupełniane na podstawie matchowania
        //w bazie trzymalibyśmy osobno listy z wszystkimi autorami, kanaałami i serwerami które obsługujemy
        //w planach jest żeby bot obsługiwał więcej serwerów niż devscord
        public MessageInformationAuthor Author { get; set; }
        public MessageInformationChannel Channel { get; set; }
        public MessageInformationServer Server { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
    }
}
