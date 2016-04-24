using System;
using System.Net;
using System.Collections.Generic;
using xmedianet.socketserver;
using System.Net.Sockets;
using Windows.Networking.Sockets;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;

using System.Linq;
using System.Text;

using System.Xml.Linq;
using Windows.Foundation;
using Windows.Networking;

using Windows.Storage.Streams;

namespace com.negociosit.XMPP.XMPPphoton
{
    public class XMPPConnection : xmedianet.socketserver.SocketClient
    {
        
        
        public XMPPConnection(XMPPClient client) : base()
        {
            XMPPClient = client;
            
        }

        public XMPPConnection(XMPPClient client, xmedianet.socketserver.ILogInterface loginterface)
            : base(loginterface, "")
        {
            XMPPClient = client;
        }

        XMPPClient XMPPClient = null;
        public bool Connect()
        {
            bool bStarted;
            try
            {
                XMPPClient.XMPPState = XMPPState.Connecting;
                if (XMPPClient.XMPPAccount.UseSOCKSProxy == true)
                    this.SetSOCKSProxy(XMPPClient.XMPPAccount.SOCKSVersion, XMPPClient.XMPPAccount.ProxyName, XMPPClient.XMPPAccount.ProxyPort, "User");

                 bStarted = ConnectAsync(XMPPClient.Server, XMPPClient.Port);
                if (bStarted == false)
                {
                    XMPPClient.XMPPState = XMPPState.Unknown;
                }
                
            }
             catch(TimeoutException tx)
            {
                throw tx;
            }
            catch(Exception ex)
            {
                throw ex;
            }          
            return bStarted;
        }

        public new bool Connected
        {
            get
            {
                if (Client == null)
                    return false;
                return Client.Connected;
            }
        }

        public void GracefulDisconnect()
        {
            XMPPClient.XMPPState = XMPPState.Unknown;
            if (Client.Connected == true)
            {
                Send("</stream>");
            }
        }

        public override bool Disconnect()
        {
            try
            {
                XMPPClient.XMPPState = XMPPState.Unknown;
                if ((Client != null) && (Client.Connected == true))
                {
                    Send("</stream>");
                    bool bRet = base.Disconnect();

                    return bRet;
                }
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        public virtual int SendStanza(XMPPStanza stanza)
        {
            string strSend = stanza.XML;
            byte[] bStanza = System.Text.UTF8Encoding.UTF8.GetBytes(strSend);
            return this.Send(bStanza);
        }


        public delegate void DelegateStanza(XMPPStanza stanza, object objFrom);
        public event DelegateStanza OnStanzaReceived = null;

        internal void FireStanzaReceived(XMPPStanza stanza)
        {
            if (OnStanzaReceived != null)
            {
                OnStanzaReceived(stanza, this);
            }
        }
        protected override void OnConnected(bool bSuccess, string strErrors)
        {
            try
            {
                if ((bSuccess == true) && (Client.Connected == true))
                {

                    this.Client.NoDelay = true;
                   
#if !WINDOWS_PHONE
                this.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, Windows.Networking.Sockets.SocketOptionName.KeepAlive, true);
  
#endif
#if WINDOWS_PHONE
                    var cancellationTokenSource = new System.Threading.CancellationTokenSource();
                    var task = Repeat.Interval(
                       TimeSpan.FromSeconds(60),
                       () => OnKeepAlive(),
                       cancellationTokenSource.Token
                   );
              //  this.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, Windows.Networking.Sockets.SocketOptionName.KeepAlive, true);
                    
#endif                   

                    XMPPClient.XMPPState = XMPPState.Connected;
                    XMPPClient.FireConnectAttemptFinished(true);
                    System.Diagnostics.Debug.WriteLine(string.Format("Successful TCP connection"));

                }
                else
                {
                    XMPPClient.XMPPState = XMPPState.Unknown;
                    XMPPClient.FireConnectAttemptFinished(false);
                    System.Diagnostics.Debug.WriteLine(string.Format("Failed to connect: {0}", strErrors));
                    return;
                }

                if (XMPPClient.UseOldStyleTLS == true)
                {
                    StartTLS();
                }


                /// Send stream header if we haven't yet
                XMPPClient.XMPPState = XMPPState.Authenticating;

                OpenStreamStanza open = new OpenStreamStanza(this.XMPPClient);
                string strSend = open.XML;
                byte[] bStanza = System.Text.UTF8Encoding.UTF8.GetBytes(strSend);
                this.Send(bStanza);
            }
            catch(TimeoutException tx)
            {
                Console.WriteLine(tx.InnerException.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
           
        }

        bool m_bStartedTLS = false;
        public void StartTLS()
        {
            if ((XMPPClient.UseTLS == true) && (m_bStartedTLS == false) )
            {
                m_bStartedTLS = true;
                this.StartTLS(XMPPClient.Server);
            }
        }

        public void OnDisconnect(string strReason)
        {
            XMPPClient.XMPPState = XMPPState.Unknown;
            m_bStartedTLS = false;
            System.Diagnostics.Debug.WriteLine(string.Format("TCP disconnected: {0}", strReason));
            XMPPClient.FireDisconnectedFromServer();
            base.OnDisconnect(strReason);
            
        }

        public void OnDisconnected(string strReason)
        {
            XMPPClient.XMPPState = XMPPState.Unknown;
            m_bStartedTLS = false;
            System.Diagnostics.Debug.WriteLine(string.Format("TCP disconnected: {0}", strReason));
            XMPPClient.FireDisconnectedFromServer();
            //base.OnDisconnected(strReason);
            base.Disconnect();
            OnDisconnected(strReason);
        }

        public override int Send(byte[] bData, int nLength, bool bTransform)
        {
            
                int nRet = base.Send(bData, nLength, bTransform);
            
                try
                {
                if ((bTransform == true) && (nRet == nLength))
                {
                    string strSend = System.Text.UTF8Encoding.UTF8.GetString(bData, 0, nLength);
                    XMPPClient.FireXMLSent(strSend);
                }
               
                ////suresh keep alive
                //if (m_KeepAlive && m_KeepaliveTimer != null)
                //    m_KeepaliveTimer.Change(m_KeepAliveInterval * 1000, m_KeepAliveInterval * 1000);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
           return nRet;
        }

        //keep alive

        private void OnKeepAlive()
        {
            Send(" ");
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:tt") + " KeepAlive");
        }
               
        
        //#region << Keepalive Timer functions >>
        //protected void CreateKeepAliveTimer()
        //{
        //    // Create the delegate that invokes methods for the timer.
        //    TimerCallback timerDelegate = new TimerCallback(KeepAliveTick);
        //    int interval = m_KeepAliveInterval * 1000;
        //    // Create a timer that waits x seconds, then invokes every x seconds.
        //    m_KeepaliveTimer = new Timer(timerDelegate, null, interval, interval);
        //}

        //protected void DestroyKeepAliveTimer()
        //{
        //    if (m_KeepaliveTimer == null)
        //        return;

        //    m_KeepaliveTimer.Dispose();
        //    m_KeepaliveTimer = null;
        //}

        //private void KeepAliveTick(Object state)
        //{
        //    // Send a Space for Keep Alive
        //    Send(" ");
        //}
        ///// <summary>
        ///// <para>
        ///// the keep alive interval in seconds.
        ///// Default value is 120
        ///// </para>
        ///// <para>
        ///// Keep alive packets prevent disconenct on NAT and broadband connections which often
        ///// disconnect if they are idle.
        ///// </para>
        ///// </summary>
        //public int KeepAliveInterval
        //{
        //    get
        //    {
        //        return m_KeepAliveInterval;
        //    }
        //    set
        //    {
        //        m_KeepAliveInterval = value;
        //    }
        //}
        ///// <summary>
        ///// Send Keep Alives (for NAT)
        ///// </summary>
        //public bool KeepAlive
        //{
        //    get
        //    {
        //        return m_KeepAlive;
        //    }
        //    set
        //    {
        //        m_KeepAlive = value;
        //    }
        //}
        //internal void RaiseOnLogin()
        //{
        //    if (KeepAlive)
        //        CreateKeepAliveTimer();
        //}
        //#endregion
        XMPPStream XMPPStream = new XMPPStream();
        protected override void OnMessage(byte[] bData)
        {

            string strXML = System.Text.UTF8Encoding.UTF8.GetString(bData, 0, bData.Length);

            
            XMPPClient.FireXMLReceived(strXML);

            XMPPStream.Append(strXML);
            XMPPStream.ParseStanzas(this, XMPPClient);
            //XMPPStream.Flush();
            /// Parse out our stanza's
            /// 

        }
        
    }
}
