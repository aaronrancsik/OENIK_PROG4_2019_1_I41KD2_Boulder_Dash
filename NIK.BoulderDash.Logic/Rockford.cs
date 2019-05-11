using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using NIK;
using NIK.BoulderDash.Logic.Blocks;

namespace NIK.BoulderDash.Logic
{
    public enum State
    {
        Up,Down,Left,Right,Stand,Birth
    }
    
    public class Rockford : DynamicBlock
    {
        public bool isLastMoveWasRight { get; private set; } = true;

        private State direction = State.Right;

        public State Direaction
        {
            get { return direction; }
            set {
                if (value == State.Right)
                {
                    isLastMoveWasRight = true;
                }
                else if (value == State.Left)
                {
                    isLastMoveWasRight = false;
                }
                direction = value;
            }
        }


        public Rockford():base(true, false) {}
    }
}
