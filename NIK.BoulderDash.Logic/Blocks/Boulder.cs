using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic.Blocks
{
    public class Boulder: DynamicBlock, IVariable
    {
        public Boulder():base(false, true)
        {
        }

        public int Variant { get; set; }
    }
}
