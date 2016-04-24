using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.negociosit.XMPP.XMPPphoton;



namespace tests.com.negociosit.XMPP.XMPPphoton
{
    [TestClass]
    public class XMPPConnectionTest
    {
        [TestMethod]
        public void TestConnection()
        {
            
            XMPPClient xmppClient = new XMPPClient();
            xmppClient.UserName = "Athirarajmr";
            xmppClient.JID = "Athirarajmr@ap.negociosit.com";
            xmppClient.Server = "192.168.1.29s";
            xmppClient.Password = "P3FCf";

            XMPPConnection xmppConnection = new XMPPConnection(xmppClient);
            bool connectionStatus =  xmppConnection.Connect();
            //Assert.Equals(connectionStatus, true);
        }
    }
}
