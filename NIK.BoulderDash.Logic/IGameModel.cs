using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic
{
    public interface IGameModel
    {
        int Time { get; }
        Player Player { get; }
        Map Map { get; }
        Camera Camera { get; }
        List<Enemie> Enemies { get; }
    }
}
