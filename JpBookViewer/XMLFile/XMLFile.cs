using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace XMLFile
{
    static class XMLFile
    {
        public static void SaveXML(this object O, string FileName)
        {
            XmlSerializer Writer = new XmlSerializer(O.GetType());

            string Dir = Path.GetDirectoryName(FileName);
            if ((Dir.Length > 0) && !Directory.Exists(Dir)) Directory.CreateDirectory(Dir);

            StreamWriter file = new StreamWriter(FileName);
            Writer.Serialize(file, O);
            file.Close();
        }
        
        public static T LoadXML<T>(string FileName)
        {
            XmlSerializer Writer = new XmlSerializer(typeof(T));
            T O = default(T);

            StreamReader file = new StreamReader(FileName);
            file.ReadToEnd();
            file.BaseStream.Position = 0;
            if (file.BaseStream.ReadByte() == 0xEF)
                file.BaseStream.Position = 3;
            else
                file.BaseStream.Position = 0;

            O = (T)Writer.Deserialize(file);
            file.Close();

            return O;
        }

    }
}
