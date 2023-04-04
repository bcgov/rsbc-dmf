using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rsbc.Dmf.LegacyAdapter
{
    public static class DebugUtils
    {

        static public void SaveDebug(string suffix, string data)
        {

            string filename = Path.GetTempPath() + "debug-" + DateTime.Now.Ticks+"-"+suffix;
            System.IO.File.WriteAllText(filename, data);
        }
    }
}
