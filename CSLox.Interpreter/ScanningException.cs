using System;
using System.Collections.Generic;

namespace CSLox.Interpreter
{
    public class ScanningException : Exception 
    {
        public int Line { get; private set; }
        
        public ScanningException(int line,
            string message) : base(message)
        {
            Line = line;
        }
    }
}