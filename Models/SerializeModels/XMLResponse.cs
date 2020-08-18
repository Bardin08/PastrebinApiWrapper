namespace PastebinApiWrapper.Models.SerializeModels
{
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class XMLResponse
    {
        public string paste_key { get; set; }

        public uint paste_date { get; set; }

        public string paste_title { get; set; }

        public int paste_size { get; set; }

        public uint paste_expire_date { get; set; }

        public byte paste_private { get; set; }

        public string paste_format_long { get; set; }

        public string paste_format_short { get; set; }

        public string paste_url { get; set; }

        public uint paste_hits { get; set; }
    }
}
