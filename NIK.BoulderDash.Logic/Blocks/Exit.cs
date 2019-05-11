using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic.Blocks
{
    public class Exit: DynamicBlock
    {
        public Exit() : base(false, false) { }
        public bool IsOpen { get; private set; }

        public void Open()
        {
            IsOpen = true;
        }
    }
}
