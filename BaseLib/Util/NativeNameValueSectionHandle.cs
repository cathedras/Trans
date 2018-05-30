using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace myzy.Util
{
    public class NativeNameValueSectionHandle : IConfigurationSectionHandler
    {
        private List<NativeDllInfo> _nameValueCollection = new List<NativeDllInfo>();
        public object Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                foreach (XmlNode xmlNode in section.SelectNodes("//NativeDlls//Native"))
                {
                    if (xmlNode.Attributes != null)
                    {
                        var isCopy = false;
                        bool.TryParse(xmlNode.Attributes["copy"]?.Value, out isCopy);
                        var ndi = new NativeDllInfo
                        {
                            Name = xmlNode.Attributes["name"]?.Value,
                            Path = xmlNode.Attributes["path"]?.Value,
                            CopyOrLoad = isCopy,
                        };
                        _nameValueCollection.Add(ndi);
                    }
                }
            }
            catch 
            {
                
            }

            return _nameValueCollection;
        }
    }

    public class NativeDllInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool CopyOrLoad { get; set; }
    }
}