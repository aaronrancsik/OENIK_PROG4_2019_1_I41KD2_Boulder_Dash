using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic.Blocks
{
    class Titanium: Block, IVariable
    {
        public Titanium():base(false) { }
        public int Variant { get; set; }
    }
}
