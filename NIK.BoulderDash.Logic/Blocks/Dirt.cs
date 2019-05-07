using NIK.BoulderDash.Logic.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic
{
    public class Dirt : Block, IVariable
    {
        public int Variant { get; set; }
    }
}
