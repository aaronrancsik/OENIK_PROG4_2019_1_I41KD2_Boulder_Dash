﻿using NIK.BoulderDash.Logic.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic.Blocks
{
    public class Dirt : Block, IVariable
    {
        public Dirt():base(false)
        {

        }
        public int Variant { get; set; }
    }
}
