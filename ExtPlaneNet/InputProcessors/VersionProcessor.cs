﻿using System;

namespace ExtPlaneNetCore.InputProcessors
{
    public class VersionProcessor : IInputProcessor
    {
        public string Evaluator
        {
            get { return "^EXTPLANE (.+)"; }
        }

        public void Process(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return;
        }
    }
}
