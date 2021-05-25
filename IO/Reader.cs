using System;
using System.IO;
using System.Text;
using static System.Text.Encoding;
namespace IO
{
    public class Reader
    {
        public static string ReadFile( string path, bool detectEncoding = false )
        {
            if (!File.Exists( path ))
                throw new Exception( "No file found!" );

            var sr = new StreamReader( path );
            var content = sr.ReadToEnd();

            if (detectEncoding)
            {
                var encoding = DetectEncoding( content );
                if (sr.CurrentEncoding != encoding)
                {
                    sr = new StreamReader( path, encoding );
                    content = sr.ReadToEnd();
                }
            }
            sr.Close();
            sr.Dispose();
            return content;
        }

        private static Encoding DetectEncoding( string text )
        {
            int encodingStart = text.IndexOf( "charset=" ) + "charset=".Length;
            int encodingEnd = text.IndexOf( Environment.NewLine, encodingStart );
            string encoding = text.Substring( encodingStart, encodingEnd - encodingStart ).Trim();

            RegisterProvider( CodePagesEncodingProvider.Instance );
            try
            {
                switch ( encoding )
                {
                    case "UTF8":
                        return Encoding.UTF8;
                    default:
                        return Encoding.GetEncoding( encoding );
                }
            }catch (Exception e )
            {
                Writer.Log( e.Message );
                return Encoding.UTF8;
            }
        }
    }
}
