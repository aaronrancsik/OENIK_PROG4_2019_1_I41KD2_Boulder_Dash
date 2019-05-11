using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic.Blocks
{
    public class Wall : Block, IVariable
    {
        public Wall() : base(true) { }
        public int Variant { get; set; }
    }
}
