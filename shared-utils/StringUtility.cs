using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pssg.SharedUtils
{
    public class StringUtility
    {
        public static MemoryStream StringToStream(string data)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string StreamToString(MemoryStream data)
        {
            StreamReader reader = new StreamReader(data);
            string text = reader.ReadToEnd();
            return text;
        }

    }
}
