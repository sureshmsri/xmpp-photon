using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.negociosit.XMPP.XMPPphoton;

namespace tests.com.negociosit.XMPP.XMPPphoton
{
    [TestClass]
    public class BanUserTest
    {
        [TestMethod]
        public void TestBanUser()
        {
            XMPPClient ObjXmppClient = new XMPPClient();
            //initializing the xmpp client with credentials
            ObjXmppClient.JID = "YOW1j@ap.negociosit.com";
            ObjXmppClient.Password = "RJcp1";
            ObjXmppClient.Server = "54.173.99.54";
            ObjXmppClient.AutoReconnect = true;
            ObjXmppClient.RetrieveRoster = true;
            ObjXmppClient.PresenceStatus = new PresenceStatus() { PresenceType = PresenceType.available, IsOnline = true };
            ObjXmppClient.AutoAcceptPresenceSubscribe = true;
            ObjXmppClient.AttemptReconnectOnBadPing = true;
            //
            XMPPConnection ObjXmppCon = new XMPPConnection(ObjXmppClient);
            ObjXmppCon = new XMPPConnection(ObjXmppClient);
            ObjXmppCon.Connect();
            ObjXmppClient.Connect();
            //muc manager test 
            MucManager mucManager = new MucManager(ObjXmppClient);
            JID roomJID = "library11@conference.ap.negociosit.com";
            JID userJID="r3rjy@ap.negociosit.com";
            mucManager.EnterRoom(roomJID, "XMPPTestNickName");
            mucManager.BanUser(roomJID, userJID);
        }
    }
}
