using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Discord.Framework.Architecture.Controllers;

namespace Watchman.Discord.Areas.Statistics.Controllers
{
    public class StatisticsController : IController
    {
        //TODO kontroler
        //zapisywanie wiadomości -> możliwe że w innym kontrolerze
        //generowanie statystyk jako excel, który idzie do google drive
        //generowanie statystyk jako ładny obraz .png, który jest pokazywany użytkownikom 
        //statystyki -> per kanał, per osoba, per serwer, wszystko z wybranego przedziału dat, domyślnie 30 dni

        //tool dla administracji 
        //mechanizm przewidujący kolejne miesiące i szukający zależności 
        //do tego trzeba dodać informacje o datach i miejscach reklamy
        //można to zrobić jako narzędzie w pythonie, bo tam jest dużo bibliotek do ML, 
        //ewentualnie sprawdzić jak radzi sobie ML.NET
    }
}
