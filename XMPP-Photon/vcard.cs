using System;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Security;
using System.Security.Cryptography;

namespace com.negociosit.XMPP.XMPPphoton
{

    [XmlRoot(ElementName = "N")]
    public class Name
    {
        public Name()
        {
        }

        private string m_strGivenName = null;
        [XmlElement(ElementName = "GIVEN")]
        public string GivenName
        {
            get { return m_strGivenName; }
            set { m_strGivenName = value; }
        }

        private string m_strFamilyName = null;
        [XmlElement(ElementName = "FAMILY")]
        public string FamilyName
        {
            get { return m_strFamilyName; }
            set { m_strFamilyName = value; }
        }
        //suresh
        private string m_strMiddleName = null;
        [XmlElement(ElementName = "MIDDLE")]
        public string MiddleName
        {
            get { return m_strMiddleName; }
            set { m_strMiddleName = value; }
        }

    }
    //suresh v card
    [XmlRoot(ElementName = "ORG")]
    public class OrgName
    {
        public OrgName()
        { }
        private string m_OrgName = null;
        [XmlElement(ElementName = "ORGNAME")]
        public string Organization
        {
            get { return m_OrgName; }
            set { m_OrgName = value; }
        }
        private string m_OrgUnit = null;
        [XmlElement(ElementName = "ORGUNIT")]
        public string OrgUnit
        {
            get { return m_OrgUnit; }
            set { m_OrgUnit = value; }
        }
    }
    //<EMAIL><INTERNET/><PREF/><USERID>stpeter@jabber.org</USERID></EMAIL>
    [XmlRoot(ElementName = "EMAIL")]
    public class Email
    {
        public Email()
        { }
        private string m_INTERNET = null;
        [XmlElement(ElementName = "INTERNET")]
        public string InterNet
        {
            get { return m_INTERNET; }
            set { m_INTERNET = value; }
        }
        private string m_Pref = null;
        [XmlElement(ElementName = "PREF")]
        public string Pref
        {
            get { return m_Pref; }
            set { m_Pref = value; }
        }
        private string m_USERID = null;
        [XmlElement(ElementName = "USERID")]
        public string UserID
        {
            get { return m_USERID; }
            set { m_USERID = value; }
        }
    }

    [XmlRoot(ElementName = "ADR")]
    public class Address
    {
        public Address()
        {
        }
        private string m_strCountry = null;
        [XmlElement(ElementName = "CTRY")]
        public string Country
        {
            get { return m_strCountry; }
            set { m_strCountry = value; }
        }

        private string m_strLocality = null;
        [XmlElement(ElementName = "LOCALITY")]
        public string Locality
        {
            get { return m_strLocality; }
            set { m_strLocality = value; }
        }

        private string m_strHome = null;
        [XmlElement(ElementName = "HOME")]
        public string Home
        {
            get { return m_strHome; }
            set { m_strHome = value; }
        }
    }

    [XmlRoot(ElementName = "PHOTO")]
    public class Photo
    {
        public Photo()
        {
        }

        private string m_strType = "image/png";
        [XmlElement(ElementName = "TYPE")]
        public string Type
        {
            get { return m_strType; }
            set { m_strType = value; }
        }
        //Photon
        private byte[] m_bBytes = null;

        // private string  m_bBytes = null;
        [XmlElement(ElementName = "BINVAL", DataType = "base64Binary")]
        public byte[] Bytes
        {
            get { return m_bBytes; }
            set
            {
                if (m_bBytes != value)
                {
                    //string values = Convert.ToBase64String(m_bBytes, 0, m_bBytes.Length);
                    m_bBytes = value;

                    SHA1Managed sha = new SHA1Managed();
                    Hash = xmedianet.socketserver.TLS.ByteHelper.HexStringFromByte(sha.ComputeHash(m_bBytes), false, int.MaxValue);
                }
            }
        }

        [XmlIgnore()]
        public string Hash = null;
    }

    [XmlRoot(ElementName = "vCard", Namespace = "vcard-temp")]
    public class vcard
    {
        public vcard()
        {
        }

        private Name m_objName = null;
        [XmlElement(ElementName = "N")]
        public Name Name
        {
            get { return m_objName; }
            set { m_objName = value; }
        }

        private string m_strNickName = null;
        [XmlElement(ElementName = "NICKNAME")]
        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

        private Photo m_objPhoto = null;
        [XmlElement(ElementName = "PHOTO")]
        public Photo Photo
        {
            get { return m_objPhoto; }
            set { m_objPhoto = value; }
        }

        private Address m_objAddress = null;
        [XmlElement(ElementName = "ADR")]
        public Address Address
        {
            get { return m_objAddress; }
            set { m_objAddress = value; }
        }
        private OrgName m_orgName = null;
        [XmlElement(ElementName = "ORG")]
        public OrgName OrgName
        {
            get { return m_orgName; }
            set { m_orgName = value; }
        }
        private Email m_Email = null;
        [XmlElement(ElementName = "EMAIL")]
        public Email Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }
    }
}
