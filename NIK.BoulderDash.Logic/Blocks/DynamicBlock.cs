using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic
{
    public class DynamicBlock: Block
    {
        public ref Point tileOldPosition { get => ref tilePosition; }

    }
}
