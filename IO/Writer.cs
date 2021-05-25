using Newtonsoft.Json;
using Objects;
using System;
using System.IO;
using CSharpMath.SkiaSharp;


namespace IO
{
    public class Writer
    {
        static string pathToLog = string.Empty;
        public static void Log( string content )
        {
            using (var sw = new StreamWriter( pathToLog, append: true ))
            {
                sw.WriteLine( DateTime.Now.ToString( "hh/mm/ss" ) + "\t\t" + content );
            }
        }

        public static void Init()
        {
            if (!Directory.Exists( "../../../Logs" ))
                Directory.CreateDirectory( "../../../Logs" );

            pathToLog = Path.Combine( "../../../Logs", DateTime.Now.ToString( "yyyy-MM-dd" ) + "_appLog.txt" );
            var stream = File.OpenWrite( pathToLog );
            stream.Close();
            stream.Dispose();
            Log( "Logging started" );
        }

        public static void WriteText(string text, string filePath )
        {
            if ( File.Exists( filePath ) )
            {
                File.Delete( filePath );
                File.Create( filePath );
            }
            using (var sw = new StreamWriter(filePath, false) )
            {
                sw.Write( text );
            }
            Log( $"written text to file [{filePath}]" );
        }

        public static void UpdateConfig( Config conf, string path )
        {
            using (var sw = new StreamWriter( path, false ))
            {
                sw.Write( JsonConvert.SerializeObject( conf, Formatting.Indented ) );
            }
        }


        public static void RenderLaTeX(string latex, string filePath = "../../../render.png" )
        {
            var painter = new MathPainter() {LaTeX = latex };
            var pngStream = painter.DrawAsStream( format: SkiaSharp.SKEncodedImageFormat.Png );

            using ( var fileStream = File.Create( filePath ) )
            {
                pngStream.Seek( 0, SeekOrigin.Begin );
                pngStream.CopyTo( fileStream );
            }
        }
    }
}
