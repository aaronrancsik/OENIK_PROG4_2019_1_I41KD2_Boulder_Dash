using NIK.BoulderDash.Logic.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic
{
    public abstract class Enemie : DynamicBlock
    {
        public Enemie() : base(true, false)
        {
        }
        virtual public void Step(bool[,] obstacle) { }
    }
}
