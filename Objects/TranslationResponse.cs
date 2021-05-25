using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Objects
{
    public class TranslationResponse
    {
        private List<Translation> translations;
        [JsonProperty( "translations" )]
        public List<Translation> Translations { get => translations; set => translations = value; }

    }
    public class Translation
    {
        List<string> texts;
        [JsonProperty( "text" )]
        public string Text
        {
            get => string.Join( "\n", texts );
            set
            {
                var processed = value.Replace( "\\ ", "\\" )
                    .Replace( "\r\n", "\n" )
                    .Replace( @"\\", @"\" )
                    .Replace( "\\\"", "\"" )
                    .Trim( '"' );
                Retry:
                try
                {
                    texts = JsonConvert.DeserializeObject<List<string>>( processed );
                }catch (JsonReaderException jre )
                {
                    var msg = jre.Message;
                    var posStr = GetStringBetween( msg, "position ", "." );
                    int pos = int.Parse( posStr );
                    string badChar = new string(processed[ pos -1 ], 1 );
                    if ( string.IsNullOrWhiteSpace( badChar ) )
                        badChar = "  ";
                    processed = processed.Substring( 0, pos - 1 ) + @"\ " + badChar[ 0 ] + processed.Substring( pos + 1 );
                    //texts = JsonConvert.DeserializeObject<List<string>>( processed );
                    goto Retry;
                }
            }
        }

        [JsonProperty( "detectedLanguageCode" )]
        public string LangCode { get; set; }
        private static string GetStringBetween( string line, string firstBound, string secondBound )
        {
            var startIndex = line.IndexOf( firstBound ) + firstBound.Length;
            var length = line.IndexOf( secondBound, startIndex + 1 ) - startIndex;
            if ( length < 0 )
                return string.Empty;
            try
            {
                return line.Substring( startIndex, length );
            }
            catch ( IndexOutOfRangeException )
            {
                return string.Empty;
            }
        }
    }

    public class TokenResponse
    {
        [JsonProperty( "iamToken" )]
        public string Token { get; set; }

        [JsonProperty( "expiresAt" )]
        public DateTime ExpiresAt;
    }


}
