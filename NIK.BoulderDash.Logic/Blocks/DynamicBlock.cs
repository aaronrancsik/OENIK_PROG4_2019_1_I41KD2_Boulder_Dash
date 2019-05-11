using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic.Blocks
{
    public class DynamicBlock: Block
    {
        private Point tilePosition;
        public ref Point TilePosition { get => ref tilePosition; }

        public DynamicBlock(bool explosive, bool isRounded) :base(isRounded)
        {
            this.Explosive = explosive;
        }
        private Point tileOldPosition;
        public ref Point TileOldPosition { get => ref tileOldPosition; }

    }
}
