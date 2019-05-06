using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using NIK;

namespace NIK.BoulderDash.Logic
{
    public class Player 
    {
        public ref Point tilePosition { get => ref tilePosition; }
        public ref Point tileOldPosition { get => ref tilePosition; }
    }
}
