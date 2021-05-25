using Objects;
using IO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Processing
{
    public static class TextProcessing
    {
        public static List<string> GetLines( this string text, string title )
        {
            if ( !title.StartsWith( "\\" ) )
                title = "\\" + title;
            List<string> result = new List<string>();

            //берем все загловки строк
            List<int> title_indices = new List<int>();
            for ( int index = 0; ; index += title.Length )
            {
                index = text.IndexOf( title, index );
                if ( index == -1 )
                    break;
                title_indices.Add( index );
            }

            //и окончания тех строк
            List<int> end_indices = new List<int>();
            foreach ( int index in title_indices )
                end_indices.Add( text.IndexOf( "\n", index ) );

            for ( int i = 0; i < end_indices.Count; i++ )
            {
                var substring = text.Substring( title_indices[ i ] + title.Length, end_indices[ i ] - title_indices[ i ] - title.Length );
                result.Add( substring );
            }
            return result;
        }

        public static string[] GetTitles( this string text )
        {
            var words = text.Split( " \n".ToCharArray() );
            var titles = new List<string>();
            for ( int i = 0; i < words.Length; i++ )
            {
                var word = words[ i ];
                if ( word.StartsWith( "\\" ) ) // это заголовок
                {
                    if ( word.Contains( "{" ) ) // там есть аргумент
                    {
                        int argStart = word.IndexOf( "{" );
                        if ( Titles.TitlesWithArgument.Contains( word.Substring( 1, argStart - 1 ) ) ) // надо отдельно перевести аргумент
                            titles.Add( word.Substring( 1, argStart - 1 ) );
                    }
                    else
                        titles.Add( word.Substring( 1 ) );
                }
            }
            return titles.ToArray();
        }

        public static string[] GetParagraphs( this string text ) =>
            text.Split( new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries );

        public static string[] GetSplittedText( this string text, string[] titles )
        {
            var partsToTranslate = new List<string>();

            int curIndex = 0, nextIndex = text.IndexOf( "\\" + titles[ 0 ] );
            int partLen;
            for ( int i = 1; i < titles.Length; i++ )
            {
                partLen = nextIndex - curIndex;
                string nextPart = text.Substring( curIndex, partLen );
                curIndex = nextIndex;
                nextIndex = text.IndexOf( "\\" + titles[ i ], curIndex );
                partsToTranslate.Add( nextPart );
            }
            Writer.Log( $"splitted text in {partsToTranslate.Count} parts, longest of {partsToTranslate.Max( x => x.Length )} chars" );
            return partsToTranslate.ToArray();
        }

        public static string[] GetSentences( this string text )
        {
            return text.Split( new string[] { ". " }, StringSplitOptions.RemoveEmptyEntries );
        }

        public static string ReplaceFormulas( this string text, Config c, out List<Replacement> replacements )
        {
            replacements = new List<Replacement>();
            Regex reg = new Regex( @"\${1,2}[a-zA-Z\s\\ \(\)\+\-\*\.\,\/\&]+\${1,2}|\\\[[a-zA-Z\s\\ \(\)\+\-\*\.\,\/\&]+\\\]|\\\([a-zA-Z\s\\ \(\)\+\-\*\.\,\/\&]+\\\)", // $ or $$, then any char, then $ or $$
                RegexOptions.Singleline | RegexOptions.Compiled );

            MatchCollection matches = reg.Matches( text );

            foreach ( Match match in matches )
            {
                Replacement rep;
                // пустая формула == новый объект
                if ( ( rep = replacements.Find( x => x.Original == match.Value ) ) is null )
                {
                    rep = new Replacement()
                    {
                        Original = match.Value,
                        Substitute = ( replacements.Count + 1 ).ToString() + c.FormulaReplacement
                    };
                    replacements.Add( rep );
                }
                text = text.Replace( match.Value, rep.Substitute );
            }
            Writer.Log( "formulas replaced with [" + c.FormulaReplacement + $"], found {replacements.Count} new values" );
            return text;
        }

        public static string ReplaceSymbols( this string text, Config c, out List<Replacement> replacements )
        {
            replacements = new List<Replacement>();
            Regex reg = new Regex( @"\\[^\\][^\\\s]*", // \someshit 
                RegexOptions.Singleline | RegexOptions.Compiled );

            MatchCollection matches = reg.Matches( text );

            foreach ( Match match in matches )
            {
                Replacement rep;
                if ( ( rep = replacements.Find( x => x.Original == match.Value ) ) is null )
                {
                    rep = new Replacement()
                    {
                        Original = match.Value,
                        Substitute = ( replacements.Count + 1 ).ToString() + c.CharReplacement
                    };
                    replacements.Add( rep );
                }
                text = text.Replace( match.Value, rep.Substitute );
            }
            Writer.Log( "special characters replaced with [" + c.CharReplacement + $"], found {replacements.Count} new values" );
            return text;
        }

        private static string GetStringBetween( string line, string firstBound, string secondBound )
        {
            var startIndex = line.IndexOf( firstBound ) + firstBound.Length;
            var length = line.IndexOf( secondBound, startIndex + 1 ) - startIndex;
            try
            {
                return line.Substring( startIndex, length );
            }
            catch ( IndexOutOfRangeException iore )
            {
                Writer.Log( $"[GetStringBetween] Error: {iore.Message}, source: [{line}], start: [{firstBound}], end: [{secondBound}]" );
                return string.Empty;
            }
        }

        public static string ReplaceQuotes( this string text ) =>
            text.Replace( "«", "\"" ).Replace( "»", "\"" )
            .Replace("<<", "\"").Replace(">>", "\"")
            .Replace("''", "\"").Replace("``", "\"");

        public static string SetSpaces( this string text ) =>
            text.Replace( "\n", " \n " ).Replace( "\\", "\\ " );

        public static string ReEncodeToUTF8( this string text, out bool Ok, string startEncoding = null )
        {
            Encoding startEnc;
            try
            {
                if ( startEncoding is null )
                {
                    var firstPart = text.Substring( 0, 1000 );
                    startEncoding = GetStringBetween( firstPart, "charset=", "\n" );
                }
                startEnc = Encoding.GetEncoding( startEncoding );
                var bytes = startEnc.GetBytes( text );
                var newBytes = Encoding.Convert( startEnc, Encoding.UTF8, bytes );
                text = Encoding.UTF8.GetString( newBytes );
                Ok = true;
                return text;
            }
            catch ( Exception e )
            {
                Writer.Log( e.Message );
                Ok = false;
                return text;
            }
        }

        public static string PreprocessText( this string text, out bool encodingOk )
        {
            return text.ReEncodeToUTF8(out encodingOk).ReplaceQuotes().SetSpaces();
        }

        public static string PostprocessText(this string text, List<Replacement> formulas, List<Replacement> chars )
        {
            text = text.SubstituteReplacements( formulas ).SubstituteReplacements( chars ).Replace( "\\ ", "\\" );
            return text;
        }

        public static string SubstituteReplacements( this string text, List<Replacement> replacements )
        {
            var inversed = replacements.
                OrderByDescending( x => ( x.Translation ?? x.Substitute ).Length ).ThenByDescending( x => x.Translation ?? x.Substitute );
            foreach ( Replacement rep in inversed )
                text = text.Replace( rep.Translation ?? rep.Substitute, rep.Original );
            return text;
        }
    }
}
