using System;
using System.Xml.Serialization;

namespace P3D.Legacy.MapEditor.BMFont
{
    public class XmlFontKerning
    {
        [XmlAttribute("first")]
        public Int32 First { get; set; }

        [XmlAttribute("second")]
        public Int32 Second { get; set; }

        [XmlAttribute("amount")]
        public Int32 Amount { get; set; }
    }
}
