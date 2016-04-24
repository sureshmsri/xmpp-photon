using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.negociosit.XMPP.XMPPphoton
{
   public class RosterActivity:xmedianet.socketserver.SocketClient
    {
       public RosterActivity(XMPPClient client)
            : base()
        {
            XMPPClient = client;
        }
       public RosterActivity(XMPPClient client, xmedianet.socketserver.ILogInterface loginterface)
            : base(loginterface, "")
        {
            XMPPClient = client;
        }
        XMPPClient XMPPClient = null;
       //roster Activity
        public IQ RosterLastActivity(JID toJID)
        {
            try
            {
                IQ lastActivity = new IQ();
                lastActivity.Type = IQType.get.ToString();
                lastActivity.From = XMPPClient.JID;
                lastActivity.To = toJID;
                lastActivity.ID = "TAPlastActivityID";
                lastActivity.InnerXML = "<query xmlns='jabber:iq:last'/>";
                IQ response = XMPPClient.SendRecieveIQ(lastActivity, 10000);
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
      /// <summary>
      /// send delivery receipt 
      /// </summary>
      /// <param name="sendToJID"></param>
      /// <param name="toMessageID"></param>
        public void SendChatDeliveryACK(JID sendToJID,string toMessageID)
        {
            try
            {
               string m_strID = Guid.NewGuid().ToString();
               string ackMessage = "<message from='" + XMPPClient.JID + "' id='" + m_strID + "' to='"+sendToJID+"'> <received xmlns='urn:xmpp:receipts' id='"+toMessageID+"'/> </message>";
               XMPPClient.SendRawXML(ackMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
