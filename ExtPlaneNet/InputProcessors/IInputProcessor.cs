using System;
using System.Collections.Generic;

namespace ExtPlaneNetCore.InputProcessors
{
    public interface IInputProcessor
    {
        string Evaluator { get; }
        void Process(string data);
    }
}
