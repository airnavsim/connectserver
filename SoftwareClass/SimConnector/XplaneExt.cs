using System;
using System.Collections.Generic;
using System.Text;


/*
 * can this be used?
 * https://github.com/eikcam/XPlaneUdpData
 * */
namespace Cs.Software.SimConnector
{
    public class XplaneExt : ISimInterface
    {
        private ExtPlaneNetCore.ExtPlaneInterface ExtPlane { get; set; }
        public void Connect()
        {

            if (this.ExtPlane == null)
                this.ExtPlane = new ExtPlaneNetCore.ExtPlaneInterface();



        }
    }
}
