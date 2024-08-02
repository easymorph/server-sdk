using System;
using System.Xml.Linq;

namespace Morph.Server.Sdk.Model
{
    public static class ApiSessionSerializationFactory
    {
        public static string Serialize(ApiSession apiSession)
        {
            if (apiSession is null)
            {
                throw new ArgumentNullException(nameof(apiSession));
            }

            switch (apiSession)
            {
                case PersitableApiSession persitableApiSession:
                    var xdoc =
                    new XDocument(new XElement("ApiSession",
                                new XAttribute("Version", 1),
                                new XAttribute("Type", "PersitableApiSession"),
                                new XElement("AuthToken", persitableApiSession.AuthToken),
                                new XElement("HeaderName", PersitableApiSession.AuthHeaderName)
                                ));
                    return xdoc.ToString();
                default:
                    throw new NotSupportedException("This kind of ApiSession can't be serialized");
            }            

        }

        public static ApiSession Deserialize(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException($"'{nameof(str)}' cannot be null or empty.", nameof(str));
            }


            XDocument xdoc = null;

            try
            {
                xdoc = XDocument.Parse(str);
            }
            catch (Exception e)
            {
                throw new Exception("Wrong api session format", e);
            }

            var xApiSession = xdoc.Element("ApiSession");
            if (xApiSession is null)
            {
                throw new Exception("Wrong api session format");
            }

            if (xApiSession.Attribute("Version")?.Value != "1")
            {
                throw new Exception("Wrong api session version");
            }

            switch (xApiSession.Attribute("Type")?.Value)
            {
                case "PersitableApiSession":
                    var token = xApiSession.Element("AuthToken")?.Value ?? throw new Exception("Persisable api session is empty");
                    return ApiSessionFactory.CreatePersitableApiSession(token);
                default:
                    throw new Exception("Not supported persisted api session type");
            };

        }
    }


}


