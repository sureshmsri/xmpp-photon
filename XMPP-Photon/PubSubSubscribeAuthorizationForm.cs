using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.negociosit.XMPP.XMPPphoton
{
    public class PubSubSubscribeAuthorizationForm
    {
        public PubSubSubscribeAuthorizationForm()
        {
        }

        private bool m_bAllow = true;

        public bool Allow
        {
            get { return m_bAllow; }
            set { m_bAllow = value; }
        }

    }
}
