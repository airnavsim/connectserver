using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Software
{
    public class Software
    {
        public Cs.Debug Debug { get; set; }
        public void Run()
        {

            this.Debug.Message("Class running");


        }
    }
}
