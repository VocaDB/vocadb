using System.Xml.Serialization;

namespace VocaDb.NicoApi {

    [XmlRoot(ElementName = "nicovideo_thumb_response", Namespace = "")]
    public class NicoResponse {
		
        [XmlElement("error")]
        public NicoResponseError Error { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlElement("thumb")]
        public NicoResponseThumb Thumb { get; set; }

    }

    public class NicoResponseError {
		
        [XmlElement("code")]
        public string Code { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

    }

    public class NicoResponseThumb {
		
        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("first_retrieve")]
        public string First_Retrieve { get; set; }

        [XmlElement("length")]
        public string Length { get; set; }

        [XmlArray("tags")]
        [XmlArrayItem("tag")]
        public Tag[] Tags { get; set; }

        [XmlElement("thumbnail_url")]
        public string Thumbnail_Url { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("user_icon_url")]
        public string User_Icon_Url { get; set; }

        [XmlElement("user_id")]
        public string User_Id { get; set; }

        [XmlElement("user_nickname")]
        public string User_Nickname { get; set; }

        [XmlElement("video_id")]
        public string Video_Id { get; set; }

        [XmlElement("view_counter")]
        public int ViewCount { get; set; }

    }

    public class Tag
    {
        [XmlAttribute("lock")]
        public bool Lock { get; set; }

        [XmlText]
        public string Name { get; set; }
    }

}
