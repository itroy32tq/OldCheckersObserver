using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal interface ISerializable
    {
        public event Action<BaseClickComponent> ChipDestroyed;

        public event Action ObjectsMoved;

        public event Action<ColorType> GameEnded;

        public event Action StepOver;
    }
}
