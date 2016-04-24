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

    /// <summary>
    /// Parses incoming messages and adds them to a RosterItems' conversation list
    /// </summary>
    public class GenericMessageLogic : Logic
    {
        public GenericMessageLogic(XMPPClient client)
            : base(client)
        {
            IsCompleted = false;
        }
        public void SendChatMessage(TextMessage txtmsg)
        {
            txtmsg.Sent = true;
            ChatMessage msg = new ChatMessage(null);
            msg.From = txtmsg.From;
            msg.To = txtmsg.To;
            msg.Type = "chat";
            msg.Body = txtmsg.Message;            
            //suresh added for message receipt
            msg.InnerXML = "<request xmlns='urn:xmpp:receipts'/>";

            if (txtmsg.Thread == null || txtmsg.Thread.Length < 1)
                //txtmsg = ExtractThread(txtmsg);
                ExtractThread(txtmsg);
             
            msg.Thread = txtmsg.Thread;
            
            //msg.InnerXML = string.Format(@"<body>{0}</body>", txtmsg.Message);

            /// Find the roster guy for this message and add it to their conversation
            /// 
            RosterItem item = XMPPClient.FindRosterItem(txtmsg.To);
            if (item != null)
            {
                item.AddSendTextMessage(txtmsg);
                //commented due to newconversation for conatct or delay msg send for not in the our roster item
                // Notify XMPPClient that a new conversation item has been added
               // XMPPClient.FireNewConversationItem(item, false, txtmsg,msg.ID);
            }
            XMPPClient.FireNewConversationItem(item, false, txtmsg, msg.ID);
            XMPPClient.SendXMPP(msg);
        }
        //photon added suresh for Message for non receipt
        public void SendChatMessageWithOutReceipt(TextMessage txtmsg)
        {
            txtmsg.Sent = true;
            ChatMessage msg = new ChatMessage(null);
            msg.From = txtmsg.From;
            msg.To = txtmsg.To;
            msg.Type = "chat";
            msg.Body = txtmsg.Message;
            
            if (txtmsg.Thread == null || txtmsg.Thread.Length < 1)
                //txtmsg = ExtractThread(txtmsg);
                ExtractThread(txtmsg);

            msg.Thread = txtmsg.Thread;

            RosterItem item = XMPPClient.FindRosterItem(txtmsg.To);
            if (item != null)
            {
                item.AddSendTextMessage(txtmsg);
                // Notify XMPPClient that a new conversation item has been added
                XMPPClient.FireNewConversationItem(item, false, txtmsg, msg.ID);
            }

            XMPPClient.SendXMPP(msg);
        }

        public override bool NewMessage(Message iq)
        {
           
            /// See if this is a standard text message
            /// 
            if (iq is ChatMessage)
            {
                ChatMessage chatmsg = iq as ChatMessage;
                RosterItem item = XMPPClient.FindRosterItem(chatmsg.From);
                if (item != null)
                {
                    if (chatmsg.Body != null)
                    {
                        TextMessage txtmsg = new TextMessage();
                        txtmsg.From = chatmsg.From;
                        txtmsg.To = chatmsg.To;
                        txtmsg.Received = DateTime.Now;
                        if (chatmsg.Delivered.HasValue == true)
                            txtmsg.Received = chatmsg.Delivered.Value; /// May have been a server stored message
                        txtmsg.Message = chatmsg.Body;
                        txtmsg = ExtractThread(txtmsg, true);
                        txtmsg.Sent = false;
                        item.AddRecvTextMessage(txtmsg);
                        item.HasNewMessages = true;

                        // Notify XMPPClient that a new conversation item has been added
                        XMPPClient.FireNewConversationItem(item, true, txtmsg,iq.ID);
                    }
                    if (chatmsg.ConversationState != ConversationState.none)// A conversation message
                    {
                        item.Conversation.ConversationState = chatmsg.ConversationState;
                        XMPPClient.FireNewConversationState(item, item.Conversation.ConversationState);
                    }
                    else
                    {
                        XDocument xmldocument = XDocument.Parse(iq.InnerXML.ToString());
                        if (xmldocument.Root.LastAttribute != null)
                        {
                            string id = xmldocument.Root.LastAttribute.Value.ToString();
                            XMPPClient.FireDelegateReceiptHandler(chatmsg.From, id);
                        }
                    }
                }
                if(item==null)
                {
                    XDocument xmldocument = XDocument.Parse(iq.InnerXML.ToString());
                    if (xmldocument.Root.LastAttribute != null)
                    {
                        string id = xmldocument.Root.LastAttribute.Value.ToString();
                        XMPPClient.FireDelegateReceiptHandler(chatmsg.From, id);
                    }
                }

                return true;
            }
            //suresh handle group chat
            else if (iq.Type == "groupchat")
            {
                XDocument xmldoc = XDocument.Parse(iq.InnerXML.ToString());
                string body = xmldoc.Descendants("body").Single().Value.ToString();
                XMPPClient.FireNewGroupConversation(iq, body);
            }

            else if (iq.Type == "normal")
            {
                XDocument xmldocument = XDocument.Parse(iq.MessageXML.ToString());
                XMPPClient.FireNewGroupConversation(iq, xmldocument.ToString());
            }
                //suresh handle receipt 
            else if (iq.Type == "" || iq.Type == null)
            {
                XDocument xmldocument = XDocument.Parse(iq.InnerXML.ToString());
                string id = xmldocument.Root.LastAttribute.Value.ToString();
                JID messageFromJID = iq.From;
                
                XMPPClient.FireDelegateReceiptHandler(messageFromJID, id);
            }
            return false;
        }

        public TextMessage ExtractThread(TextMessage txtmsg)
        {
            return ExtractThread(txtmsg, false);
        }

        public TextMessage ExtractThread(TextMessage txtmsg, bool bUpdateMessageText)
        {
            System.Diagnostics.Debug.WriteLine("Original message: " + txtmsg.Message);

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(strThreadPattern);
            System.Text.RegularExpressions.MatchCollection matchCollection = regex.Matches(txtmsg.Message);

            if (matchCollection.Count > 0)
            {
                foreach (System.Text.RegularExpressions.Match match in matchCollection)
                {
                    if (match.Groups["threadName"] != null)
                    {
                        txtmsg.Thread = match.Groups["threadName"].Value;
                        txtmsg.Thread.Trim();
                        System.Diagnostics.Debug.WriteLine("Thread: " + txtmsg.Thread);

                    }
                    if (match.Groups["messageText"] != null)
                    {
                        if (bUpdateMessageText)
                        {
                            txtmsg.Message = match.Groups["messageText"].Value;
                            txtmsg.Message = txtmsg.Message.Trim();
                            System.Diagnostics.Debug.WriteLine("Message: " + txtmsg.Message);
                        }
                    }

                }
            }
            return txtmsg;
        }

        public static string strThreadPattern =
   //  @"[\s]*\[(?<threadName>[^\]]*)\](?<messageText>.*)";
       @"^[\s]*\[(?<threadName>[^\]]*)\](?<messageText>[^$]*)";
    }
}