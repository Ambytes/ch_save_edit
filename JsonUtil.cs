using System;
using System.IO;
using System.Windows.Forms;
using Ionic.Zlib;
using Newtonsoft.Json;

namespace ch_save_edit
{
    internal class JsonUtil
    {
        private const string Hash = "7a990d405d2c6fb93aa8fbb0ec1a3b23";

        public static string PrettifyJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                //creating jsonreader by reading json from stringreader
                var jsonReader = new JsonTextReader(stringReader);
                //creating jsonwriter by using the stringwriter and the intended formatting method
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                //creating right format for json data
                jsonWriter.WriteToken(jsonReader);
                
                return stringWriter.ToString();
            }
        }

        public static void EncodeJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;
            //compress string with zlib inflation, convert inflated string to base-64 string
            //append it to the constant hash
            //copy to clipboard
            Clipboard.SetText($"{Hash}{Convert.ToBase64String(ZlibStream.CompressString(json))}");

            //inform user about me fecking his clipboard up :)
            MessageBox.Show(@"Encoded save copied to your clipboard.", @"Encoded and copied", MessageBoxButtons.OK, MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification, false);
        }

        public static string DecodeJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            //remove first 32 chars (constant hash), convert base-64 string to standard string, uncompress string with zlib deflation.        
            string decodedJson = ZlibStream.UncompressString(Convert.FromBase64String(json.Substring(32)));
            //return prettified json
            return PrettifyJson(decodedJson);
        }

        public static bool IsEncoded(string data)
        {
            if (data.Length < 32) return false;
            if (data.Substring(0, 32).Equals(Hash))
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
