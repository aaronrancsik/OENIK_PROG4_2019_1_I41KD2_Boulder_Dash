using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic
{
    interface IGameModel
    {
        int Time { get; }
        IPlayer Player { get; }
        IMap Map { get; }
        ICamera Camera { get; }
        ObservableCollection<IEnemie> Enemies { get; }
    }
}
