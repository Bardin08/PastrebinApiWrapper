namespace PastebinApiWrapper.Models.SerializeModels
{
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class pastes
    {
        [System.Xml.Serialization.XmlElement("paste")]
        public pastesPaste[] paste { get; set; }
    }

    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class pastesPaste
    {
        public string paste_key { get; set; }
        public uint paste_date { get; set; }
        public string paste_title { get; set; }
        public ushort paste_size { get; set; }
        public uint paste_expire_date { get; set; }
        public byte paste_private { get; set; }
        public string paste_format_long { get; set; }
        public string paste_format_short { get; set; }
        public string paste_url { get; set; }
        public byte paste_hits { get; set; }
    }
}
