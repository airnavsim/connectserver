using System;
using System.Collections.Generic;
using System.Text;


/*
 * can this be used?
 * https://github.com/eikcam/XPlaneUdpData
 * */
namespace Cs.Software.SimConnector
{
    public class Xplane : SimInterface
    {
        private ExtPlaneNetCore.ExtPlaneInterface extPlane { get; set; }
        public void Connect()
        {

            if (this.extPlane == null)
                this.extPlane = new ExtPlaneNetCore.ExtPlaneInterface();


        }
    }
}
