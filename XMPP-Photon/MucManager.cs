using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.negociosit.XMPP.XMPPphoton
{
    public class MucManager : xmedianet.socketserver.SocketClient
    {
        public MucManager(XMPPClient client)
            : base()
        {
            XMPPClient = client;
        }
        public MucManager(XMPPClient client, xmedianet.socketserver.ILogInterface loginterface)
            : base(loginterface, "")
        {
            XMPPClient = client;
        }
        XMPPClient XMPPClient = null;
        //join the room
        public void EnterRoom(JID room, string joinerName)
        {
            try
            {
                //         <presence from='juliet@example.com/balcony'
                //          to='verona@chat.example.org/JuliC'>
                //  <x xmlns='http='http://jabber.org/protocol/muc'/>
                //</presence>
                // string roomCreation = "<presence from='"+XMPPClient.JID.BareJID+"/"+" to='" + room + "/" + joinerName + "'>" + "<x xmlns='http://jabber.org/protocol/muc'/> " + "<history maxstanzas='0'/></presence>";                                       

                ////<presence to="1660|test_july@conference.ap.negociosit.com/sureshmsri" xmlns="jabber:client">
  //<x xmlns="http://jabber.org/protocol/muc" />
//</presence>

               
                string _to =room+"/"+joinerName;
               
                StringBuilder roomConfig=new StringBuilder();
                roomConfig.Append("<presence to=");
                roomConfig.Append("\"");
                roomConfig.Append(_to);
                roomConfig.Append("\"");
                roomConfig.Append(" xmlns=");
                roomConfig.Append("\"");
                roomConfig.Append("jabber:client");
                roomConfig.Append("\"");
                roomConfig.Append(">");
                roomConfig.Append("<x xmlns=").Append("\"").Append("http://jabber.org/protocol/muc").Append("\"").Append(">");
                roomConfig.Append("<history maxchars=").Append("\"").Append("0").Append("\"").Append("/>").Append("</x>");
                roomConfig.Append("</presence>");

              //  string roomCreation = "<presence to="+"\" +"_to\""" +  + "<x xmlns='http://jabber.org/protocol/muc'/> " + "<history seconds='0'/></presence>";
               
               // XMPPClient.SendXMPP(SendIQ);
                XMPPClient.SendRawXML(roomConfig.ToString());
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        //get the room configuration
        public IQ RequestRoomConfiguration(JID roomJID)
        {
            try
            {
                IQ roomConfigiq = new IQ();
                roomConfigiq.Type = IQType.get.ToString();
                roomConfigiq.From = XMPPClient.JID;
                roomConfigiq.To = roomJID;
                roomConfigiq.InnerXML = "<query xmlns='http://jabber.org/protocol/muc#owner'/>";
                roomConfigiq.ID = "roomConfigID";
                IQ response = XMPPClient.SendRecieveIQ(roomConfigiq, 10000);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
                    
        }
        //set room as persistent
        public IQ SubmitRoomConfiguration(JID roomJid)
        {
            try
            {
                IQ persistentIQ = new IQ();
                persistentIQ.Type = IQType.set.ToString();
                persistentIQ.From = XMPPClient.JID;
                persistentIQ.To = roomJid;
                persistentIQ.ID = "roomConfigID";
                persistentIQ.InnerXML = "<query xmlns='http://jabber.org/protocol/muc#owner'><x xmlns='jabber:x:data' type='submit'><field type='hidden' var='FORM_TYPE'><value>http://jabber.org/protocol/muc#roomconfig</value></field><field var='muc#roomconfig_persistentroom'><value>true</value></field></x></query>";
                IQ response = XMPPClient.SendRecieveIQ(persistentIQ, 10000);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //invite user to join the room
        public void DirectInvite(JID to, JID roomJid)
        {
            try
            {
                //string roomCreation = "<presence to='" + to + "'>" + "<x xmlns='http://jabber.org/protocol/muc'/></presence>";
                string inviteXML = "<message type=\"normal\" to='" + to + "'>" + "<x xmlns='jabber:x:conference' jid='" + roomJid + "'" + " reason='Join this room!'/>" + "</message>";

                XMPPClient.SendRawXML(inviteXML);
            }
            catch  (Exception ex)
            {
                throw ex;
            }
        }
        //remove the user from room
        public IQ BanUser(JID roomJID,JID banUserJID)
        {
            try
            {
                IQ banuserIQ = new IQ();
                banuserIQ.Type = IQType.set.ToString();
                banuserIQ.From = XMPPClient.JID;
                banuserIQ.To = roomJID;
                banuserIQ.ID = "banUserTAPID";
                banuserIQ.InnerXML = "<query xmlns='http://jabber.org/protocol/muc#admin'><item affiliation='outcast' jid='" + banUserJID + "'/>" + "</query>";
                IQ response = XMPPClient.SendRecieveIQ(banuserIQ, 10000);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //request banned user list
        public IQ RequestMembersList(JID roomJID)
        {
            try
            {
                IQ memberList = new IQ();
                memberList.Type = IQType.get.ToString();
                memberList.From = XMPPClient.JID;
                memberList.To = roomJID;
                memberList.ID = "memberlistID";
                memberList.InnerXML = "<query xmlns='http://jabber.org/protocol/muc#admin'><item affiliation='outcast'/></query>";
                IQ response = XMPPClient.SendRecieveIQ(memberList, 10000);
                return response;
            }
            catch(Exception  ex)
            {
                throw ex;
            }
        }
        //rejoin the banned user 
        public IQ ModifyRoomList(JID roomJID,JID memberJID)
        {
            try
            {
                IQ modifyList = new IQ();
                modifyList.Type = IQType.set.ToString();
                modifyList.From = XMPPClient.JID;
                modifyList.To = roomJID;
                modifyList.ID = "modifyRoomListID";
                modifyList.InnerXML = "<query xmlns='http://jabber.org/protocol/muc#admin'><item affiliation='none' jid='" + memberJID + "'><reason>rejoin</reason></item></query>";
                IQ response = XMPPClient.SendRecieveIQ(modifyList, 10000);
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        //send group chat message
        public void SendGroupChat(JID roomJID,string messagebody)
        {
            try
            {
                ChatMessage msg = new ChatMessage(null);
                msg.From = XMPPClient.JID;
                msg.To = roomJID;
                msg.Type = "groupchat";
                msg.Body = messagebody;
                XMPPClient.SendXMPP(msg);               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //exit from group
        public IQ DestroyRoom(JID roomJID)
        {
            try
            {
                IQ destroyIQ = new IQ();
                destroyIQ.Type = IQType.set.ToString();
                destroyIQ.From = XMPPClient.JID;
                destroyIQ.To = roomJID;
                destroyIQ.ID = "modifyRoomListID";
                destroyIQ.InnerXML = "<query xmlns='http://jabber.org/protocol/muc#owner'><destroy jid='" + roomJID + "'><reason>delete</reason></destroy></query>";
                IQ response = XMPPClient.SendRecieveIQ(destroyIQ, 10000);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //join
      
        public void JoinRoom(JID room, string nickname)
        {
           
            //join room and request no history
                 //  <presence to="1660|test_july@conference.ap.negociosit.com/sureshmsri" xmlns="jabber:client">
                //  <x xmlns="http://jabber.org/protocol/muc">
                    //    <history maxchars="0" />
                        //  </x>
                    //</presence>
            string _to = room + "/" + nickname;

            StringBuilder roomConfig = new StringBuilder();
            roomConfig.Append("<presence to=");
            roomConfig.Append("\"");
            roomConfig.Append(_to);
            roomConfig.Append("\"");
            roomConfig.Append(" xmlns=");
            roomConfig.Append("\"");
            roomConfig.Append("jabber:client");
            roomConfig.Append("\"");
            roomConfig.Append(">");
            roomConfig.Append("<x xmlns=").Append("\"").Append("http://jabber.org/protocol/muc").Append("\"").Append(">");
            roomConfig.Append("<history maxchars=").Append("\"").Append("0").Append("\"").Append("/>").Append("</x>");
            roomConfig.Append("</presence>");
            XMPPClient.SendRawXML(roomConfig.ToString());           
        }
        
        public void JoinRoom(JID room, string nickname,DateTime historySince)
        {
            string _to = room + "/" + nickname;
            string historyFrom = Time.Iso8601DateString(historySince);
            StringBuilder roomConfig = new StringBuilder();
            roomConfig.Append("<presence to=");
            roomConfig.Append("\"");
            roomConfig.Append(_to);
            roomConfig.Append("\"");
            roomConfig.Append(" xmlns=");
            roomConfig.Append("\"");
            roomConfig.Append("jabber:client");
            roomConfig.Append("\"");
            roomConfig.Append(">");
            roomConfig.Append("<x xmlns=").Append("\"").Append("http://jabber.org/protocol/muc").Append("\"").Append(">");
            roomConfig.Append("<history since=").Append("\"").Append(historyFrom).Append("\"").Append("/>").Append("</x>");
            roomConfig.Append("</presence>");
            XMPPClient.SendRawXML(roomConfig.ToString());    
        }
    }
}
