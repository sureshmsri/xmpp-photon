using System;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text.RegularExpressions;


namespace com.negociosit.XMPP.XMPPphoton
{

    

    [XmlRoot(ElementName = "x", Namespace="vcard-temp:x:update")]
    public class VCardUpdate
    {
        public VCardUpdate()
        {
        }

        private string m_strPhotoHash = null;
        [XmlElement(ElementName = "photo")]
        public string PhotoHash
        {
            get { return m_strPhotoHash; }
            set { m_strPhotoHash = value; }
        }
    }

    [XmlRoot(ElementName = "c", Namespace = "http://jabber.org/protocol/caps")]
    public class Capabilities
    {
        public Capabilities()
        {
        }

        private string m_strNode = null;
        [XmlAttribute(AttributeName="node")]
        public string Node
        {
            get { return m_strNode; }
            set { m_strNode = value; }
        }

        private string m_strExtensions = null;
        [XmlAttribute(AttributeName = "ext")]
        public string Extensions
        {
            get { return m_strExtensions; }
            set { m_strExtensions = value; }
        }

        private string m_strVersion = "1.1";
        [XmlAttribute(AttributeName = "ver")]
        public string Version
        {
            get { return m_strVersion; }
            set { m_strVersion = value; }
        }
    }

    [XmlRoot(ElementName = "presence")]
    public class PresenceMessage : XMPPMessageBase
    {
        public PresenceMessage()
            : base(null, "presence")
        {
            m_objPresenceStatus.PresenceType = PresenceType.available;
        }

        public PresenceMessage(string strXML)
            : base(strXML, "presence")
        {
            m_objPresenceStatus.PresenceType = PresenceType.available;
        }

        private PresenceStatus m_objPresenceStatus = new PresenceStatus();
        [XmlIgnore()]
        public PresenceStatus PresenceStatus
        {
            get { return m_objPresenceStatus; }
            set { m_objPresenceStatus = value; }
        }


        [XmlAttribute(AttributeName="type")]
        public override string Type
        {
            get
            {
                if (m_objPresenceStatus.PresenceType == PresenceType.available)
                    return null;
                return m_objPresenceStatus.PresenceType.ToString();
            }
            set
            {
                if (value == null)
                    m_objPresenceStatus.PresenceType = PresenceType.available;
                else if (value == PresenceType.error.ToString())
                    m_objPresenceStatus.PresenceType = PresenceType.error;
                else if (value == PresenceType.probe.ToString())
                    m_objPresenceStatus.PresenceType = PresenceType.probe;
                else if (value == PresenceType.subscribe.ToString())
                    m_objPresenceStatus.PresenceType = PresenceType.subscribe;
                else if (value == PresenceType.subscribed.ToString())
                    m_objPresenceStatus.PresenceType = PresenceType.subscribed;
                else if (value == PresenceType.unavailable.ToString())
                    m_objPresenceStatus.PresenceType = PresenceType.unavailable;
                else if (value == PresenceType.unsubscribe.ToString())
                    m_objPresenceStatus.PresenceType = PresenceType.unsubscribe;
                else if (value == PresenceType.unsubscribed.ToString())
                    m_objPresenceStatus.PresenceType = PresenceType.unsubscribed;
            }
        }

        [XmlElement(ElementName = "show")]
        public string Show
        {
            get
            {
                return PresenceStatus.PresenceShow.ToString();
            }
            set
            {
                if (value == PresenceShow.away.ToString())
                    PresenceStatus.PresenceShow = PresenceShow.away;
                else if (value == PresenceShow.chat.ToString())
                    PresenceStatus.PresenceShow = PresenceShow.chat;
                else if (value == PresenceShow.dnd.ToString())
                    PresenceStatus.PresenceShow = PresenceShow.dnd;
                else if (value == PresenceShow.xa.ToString())
                    PresenceStatus.PresenceShow = PresenceShow.xa;
            }
        }

        [XmlElement(ElementName = "status")]
        public string Status
        {
            get
            {
                return PresenceStatus.Status;
            }
            set
            {
                PresenceStatus.Status = value;
            }
        }

        [XmlElement(ElementName = "priority")]
        public string Priority
        {
            get
            {
                return PresenceStatus.Priority.ToString();
            }
            set
            {
                PresenceStatus.Priority = Convert.ToSByte(value);
            }
        }

        private VCardUpdate m_objVCardUpdate = null;
        [XmlElement(ElementName = "x", Namespace="vcard-temp:x:update")]
        public VCardUpdate VCardUpdate
        {
            get { return m_objVCardUpdate; }
            set { m_objVCardUpdate = value; }
        }


        private Capabilities m_objCapabilities = null;
        [XmlElement(ElementName = "c", Namespace = "http://jabber.org/protocol/caps")]
        public Capabilities Capabilities
        {
            get { return m_objCapabilities; }
            set { m_objCapabilities = value; }
        }

        private IQAvatar m_objAvatar = null;
        [XmlElement(ElementName = "x", Namespace = "jabber:x:avatar")]
        public IQAvatar AvatarHash
        {
            get { return m_objAvatar; }
            set { m_objAvatar = value; }
        }


        //[XmlIgnore()]
        //public override string MessageXML
        //{
        //    get
        //    {
        //        return Utility.GetXMLStringFromObject(this);

        //    }
        //    set
        //    {
        //        if ((value == null) || (value.Length <= 0))
        //            return;

        //        PresenceMessage msg = Utility.ParseObjectFromXMLString(value, typeof(PresenceMessage)) as PresenceMessage;
        //        if (msg != null)
        //        {
        //            this.ID = msg.ID;
        //            this.Type = msg.Type;
        //            this.From = msg.From;
        //            this.To = msg.To;

        //            this.PresenceStatus = msg.PresenceStatus;
        //            this.VCardUpdate = msg.VCardUpdate;
        //            this.Capabilities = msg.Capabilities;
        //        }


        //    }
        //}

    }


    /// <summary>
    /// Responsible for setting our sessions' presence
    /// </summary>
    public class PresenceLogic : Logic
    {
        public PresenceLogic(XMPPClient client)
            : base(client)
        {
        }


        public void SetPresence(PresenceStatus status, Capabilities caps, string strImageHash)
        {
            PresenceMessage pres = new PresenceMessage(null);
            pres.From = XMPPClient.JID;
            pres.To = null;
            pres.PresenceStatus = status;
            pres.Capabilities = caps;
            
            if ((strImageHash != null) && (strImageHash.Length > 0))
            {
                pres.VCardUpdate = new VCardUpdate();
                pres.VCardUpdate.PhotoHash = strImageHash;
            }
            XMPPClient.SendObject(pres);
        }


        public void SubscribeToPresence(JID jidto)
        {
            PresenceMessage pres = new PresenceMessage(null);
            pres.To = jidto;
            pres.From = null;
            pres.Type = "subscribe";
            pres.Show = null;
            pres.Status = null;
            pres.PresenceStatus.PresenceType = PresenceType.subscribe;
            XMPPClient.SendObject(pres);
        }

        public void UnsubscribeToPresence(JID jidto)
        {
            PresenceMessage pres = new PresenceMessage(null);
            pres.To = jidto;
            pres.From = null;
            pres.Type = "unsubscribe";
            pres.Show = null;
            pres.Status = null;
            pres.PresenceStatus.PresenceType = PresenceType.unsubscribe;
            XMPPClient.SendObject(pres);
        }

        /// <summary>
        ///  Allow the remote user to see our presence, we should add them to our roster as well at this poitn?
        /// </summary>
        /// <param name="pres"></param>
        public void AcceptUserPresence(PresenceMessage pres, string strNickName, string strGroup)
        {
            pres.PresenceStatus.PresenceType = PresenceType.subscribed;
            pres.Type = "subscribed";
            pres.To = pres.From.BareJID;
            pres.From = null;

            XMPPClient.SendObject(pres);
        }

        /// <summary>
        /// Deny the remote user the ability to see our presence
        /// </summary>
        /// <param name="pres"></param>
        public void DeclineUserPresence(PresenceMessage pres)
        {
            pres.PresenceStatus.PresenceType = PresenceType.unsubscribed;
            pres.Type = "unsubscribed";
            pres.To = pres.From.BareJID;
            pres.From = null;
            XMPPClient.SendObject(pres);
        }

       


        /// <presence id="AWAQt-31" from="brianbonnett@ninethumbs.com/calculon" to="test@ninethumbs.com/phone"><status>Away</status><priority>0</priority><show>away</show></presence>

        public override bool NewPresence(PresenceMessage pres)
        {
            if ( (pres.Type == "") || (pres.Type == "unavailable") )
            {
                /// Means avialbe
                /// 
                RosterItem  item = XMPPClient.FindRosterItem(pres.From);
                if (item != null)
                {
                    item.SetPresence(pres);
                }
            }
    
            else if (pres.Type == "subscribe")
            {
                Answer answer = XMPPClient.ShouldSubscribeUser(pres);

                if (answer == Answer.Yes)
                {
                    AcceptUserPresence(pres, "", "");
                }
                else if (answer == Answer.No) /// reject if the user has responded
                {
                    DeclineUserPresence(pres);
                }
              

            }
            else if (pres.Type == "subscribed")
            {
            }
            else 
            {
                RosterItem item = XMPPClient.FindRosterItem(pres.From);
                if (item != null)
                {
                    item.SetPresence(pres);
                    //item.Presence = pres.PresenceStatus;

                    /// Commented out because no one seems to support this method
                    //if (pres.AvatarHash != null)
                    //{
                    //    if (pres.AvatarHash.Hash != null)
                    //    {
                    //        /// May have a new avatar, check against our current file system
                    //        /// 
                    //        if (XMPPClient.AvatarStorage.AvatarExist(pres.AvatarHash.Hash) == false)
                    //           DownloadAvatarJabberIQMethod(pres.From);
                    //    }
                    //}

                    if (pres.VCardUpdate != null)
                    {
                        if (pres.VCardUpdate.PhotoHash != null)
                        {

                            //byte [] bURL = Convert.FromBase64String(pres.VCardUpdate.PhotoHash);
                            //string strURL = System.Text.UTF8Encoding.UTF8.GetString(bURL, 0, bURL.Length);
                            //item.AvatarImagePath = strURL;

                            

                            /// XEP-153 - vcard-based avatars... sha1 hash of the current avatar
                            if ((XMPPClient.AvatarStorage.AvatarExist(pres.VCardUpdate.PhotoHash) == false) && (XMPPClient.AutomaticallyDownloadAvatars == true) )
                                RequestVCARD(pres.From);
                            else
                            {
                                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ThreadedUpdateAvatarImageHash),
                                    new RosterItemImageHash() {RosterItem = item, ImageHash=pres.VCardUpdate.PhotoHash});
                                //item.AvatarImagePath = pres.VCardUpdate.PhotoHash;
                            }
                                
   
                                //RequestVCARD(pres.From);


                            /// or XEP-0008 IQ based avatars
                            /// 
                            

                            /// or XEP- jaber fldlkjsl
                            //string strRelativeImage = string.Format("Avatars/{0}", pres.VCardUpdate.PhotoHash);
                            ///// See if this file exists, if it doesn't, download the file
                            ///// 
                            //if (System.IO.File.Exists(strRelativeImage) == true)
                            //    item.AvatarImagePath = strRelativeImage;
                            //else
                            //    //XMPPClient.PersonalEventingLogic.DownloadDataNode(jidfrom, "urn:xmpp:avatar:data", strItem);
                            //    XMPPClient.DownloadAvatar(pres.From, pres.VCardUpdate.PhotoHash);
                        }
                    }


                    System.Diagnostics.Debug.WriteLine(item.ToString());
                    XMPPClient.FireListChanged(1);

                }
            }

            return true;
        }

        /// <summary>
        ///  Update our image path in a thread since the GUI will be called
        /// </summary>
        /// <param name="obj"></param>
        void ThreadedUpdateAvatarImageHash(object obj)
        {
            RosterItemImageHash itemhash = obj as RosterItemImageHash;
            itemhash.RosterItem.AvatarImagePath = itemhash.ImageHash;

        }

        public void DownloadAvatarJabberIQMethod(JID jidfor)
        {
            IQ iq = new IQ();
            iq.From = XMPPClient.JID;
            iq.To = jidfor;
            iq.Type = IQType.get.ToString();
            iq.InnerXML = "<query xmlns='jabber:iq:avatar' />";
            //iq.InnerXML = "<query xmlns='storage:client:avatar' />";

            XMPPClient.SendXMPP(iq);
        }

        public void  RequestVCARD(JID jidfor)
        {
            IQ iq = new IQ();
            iq.From = XMPPClient.JID;
            iq.To = jidfor.BareJID;
            iq.Type = IQType.get.ToString();
            iq.InnerXML = "<vCard xmlns='vcard-temp' />";

            XMPPClient.SendXMPP(iq);
          
        }

        IQ iqGetOurVCARD = null; 
        public void RequestOurVCARD()
        {
            iqGetOurVCARD = new IQ();
            iqGetOurVCARD.From = null;
            iqGetOurVCARD.To = XMPPClient.JID.BareJID;
            iqGetOurVCARD.Type = IQType.get.ToString();
            iqGetOurVCARD.InnerXML = "<vCard xmlns='vcard-temp' />";

            XMPPClient.SendXMPP(iqGetOurVCARD);
        }

        public void UdpateVCARD(vcard vcard)
        {
            IQ iq = new IQ();
            iq.From = XMPPClient.JID;
            iq.To = null;
            iq.Type = IQType.set.ToString();
            iq.InnerXML = Utility.GetXMLStringFromObject(vcard);

            XMPPClient.SendXMPP(iq);
            
        }
        public void UpdateRosterImage(vcard vcars)
        {
            IQ iq = new IQ();
            iq.From = XMPPClient.JID;
            iq.To = null;
            iq.Type = IQType.set.ToString();
            //ser
            XmlSerializer serilise = new XmlSerializer(typeof(vcard));
            TextWriter txtWriter = new StreamWriter(vcars.ToString());
            serilise.Serialize(txtWriter, vcars);
            txtWriter.Close();
            iq.InnerXML = serilise.ToString();
            XMPPClient.SendXMPP(iq);
        }

        // Look for subscribe message to subscribe to presence
        public override bool NewIQ(IQ iq)
        {

            if ( (iqGetOurVCARD != null) && (iq.ID == iqGetOurVCARD.ID))
            {
                foreach (XElement vcard in iq.InitalXMLElement.Descendants("{vcard-temp}vCard"))
                {
                    vcard card = Utility.ParseObjectFromXMLString(vcard.ToString(), typeof(vcard)) as vcard;
                    if (card != null)
                    {
                        XMPPClient.vCard = card;
                    }
                    
                }
                return true;
            }

            if (iq.InitalXMLElement != null)
            {
                foreach (XElement vcard in iq.InitalXMLElement.Descendants("{vcard-temp}vCard"))
                {
                    vcard card = Utility.ParseObjectFromXMLString(vcard.ToString(), typeof(vcard)) as vcard;
                    if (card != null)
                    {
                        RosterItem item = XMPPClient.FindRosterItem(iq.From);
                        if (item != null)
                            item.vCard = card;
                        else if (iq.From.BareJID == XMPPClient.JID.BareJID)
                            XMPPClient.vCard = card;
                    }
                    ////suresh request vCard
                    if(iq.Type=="result")
                    {
                        if(iq.From!=iq.To)
                        {
                            XMPPClient.FireDelegateVcardHandler(card,iq.From);
                        }
                    }

                    return true;
                }

                foreach (XElement avaelem in iq.InitalXMLElement.Descendants("{jabber:iq:avatar}query"))
                {
                    IQAvatarQuery ava = Utility.ParseObjectFromXMLString(avaelem.ToString(), typeof(IQAvatarQuery)) as IQAvatarQuery;
                    if (ava != null)
                    {
                        /// Found a new avatar using this 3rd method, tell the client
                    }

                    return true;
                }
            }
            
            return base.NewIQ(iq);
        }
    }

    public class RosterItemImageHash
    {
        public RosterItem RosterItem {get; set;}
        public string ImageHash { get; set; }
    }
}
