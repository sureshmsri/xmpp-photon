using com.negociosit.XMPP.XMPPphoton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.negociosit.XMPP.XMPPphoton
{
    public class StreamManagement : xmedianet.socketserver.SocketClient
    {
        XMPPClient XMPPClient = null;
        public StreamManagement(XMPPClient client)
            : base()
        {
            XMPPClient = client;
        }
        public StreamManagement(XMPPClient client, xmedianet.socketserver.ILogInterface loginterface)
            : base(loginterface, "")
        {
            XMPPClient = client;
        }
       

        public void EnablingStreamManagement(string versionNo)
        {
            string enableString = "<enable xmlns='urn:xmpp:sm:"+ versionNo+"'/>";
            XMPPClient.SendRawXML(enableString);
        }
        public void EnablingStreamManagement(int versionNo,bool resume)
        {
            string enableString = string.Empty;
            if(resume)
            {
                 enableString = "<enable xmlns='urn:xmpp:sm:" + versionNo + "'" + " resume='true'/>";
            }
            
            else
            {
                enableString = "<enable xmlns='urn:xmpp:sm:" + versionNo + "'" + " resume='false'/>";
            }
            XMPPClient.SendRawXML(enableString);
        }
        public void SendStreamACK(int HstanzaNo)
        {
            string sendACK="<a xmlns='urn:xmpp:sm:3' h='"+HstanzaNo+"'/>"; //<a xmlns="urn:xmpp:sm:2" h="305" />
            XMPPClient.SendRawXML(sendACK);
        }
        public void RequestStreamACK(int versionNo)
        {
            XMPPClient.SendRawXML("<r xmlns='urn:xmpp:sm:"+versionNo+"'/>");
        }
        public void ResumptionRequest(string resumptionID,int sequenceNO)
        { 
            //<resume xmlns='urn:xmpp:sm:3' h='some-sequence-number' previd='some-long-sm-id'/>
            string requestResumption = "<resume xmlns='urn:xmpp:sm:3' h='"+sequenceNO+"' previd='"+resumptionID+"'/>";
                XMPPClient.SendRawXML(requestResumption);
        }
    }
}
