﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic.Blocks
{
    class Wall : Block, IVariable
    {
        public int Variant { get; set; }
    }
}