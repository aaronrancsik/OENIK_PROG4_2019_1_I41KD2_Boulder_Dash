using NIK.BoulderDash.Logic.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic
{
    public class Block : IVariable
    {
        public ref Point tilePosition { get => ref tilePosition; }
        public static int Set { get; set; }
        public int Variant { get; set; }
    }
}
