using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using IO;
using Newtonsoft.Json;
using Objects;
using Processing;

namespace CorrectTranslation
{
    public partial class Form1 : Form
    {
        Config config;
        private const string config_path = "../../../../config.json";
        public Form1()
        {
            InitializeComponent();
            OrigRTB.AllowDrop = true;
            OrigRTB.DragEnter += OrigRTB_DragEnter;
            OrigRTB.DragDrop += OrigRTB_DragDrop;
            Writer.Init();

            var json = Reader.ReadFile( config_path );
            try
            {
                config = JsonConvert.DeserializeObject<Config>( json );
            }
            catch ( Exception e )
            {
                Writer.Log( $"Error while reading config, exception: [{e.Message}]" );
                throw;
            }
            if ( config.token_date <= DateTime.Now.AddHours( -12 ).AddMinutes( 5 ) )
            {
                UpdateIamToken();
            }
        }

        private void OrigRTB_DragEnter( object sender, DragEventArgs e )
        {
            if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void OrigRTB_DragDrop( object sender, DragEventArgs e )
        {
            var files = e.Data.GetData( DataFormats.FileDrop ) as string[];

            foreach ( string filePath in files )
            {
                Writer.Log( "Reading file: " + filePath );
                var file = Reader.ReadFile( filePath, true );
                Writer.Log( $"Read file: {filePath} successfully, length: {file.Length}" );
                OrigRTB.Text = file;
            }

            var paragraphs = OrigRTB.Text.GetParagraphs();

            paragraphsCB.Items.Clear();
            paragraphsCB.Items.AddRange( paragraphs.Select(x => x.Length > 50 ? x.Substring(0,50) : x ).ToArray() );
        }


        private void translateBtn_Click( object sender, EventArgs e )
        {
            string text = OrigRTB.Text;
            text = text.PreprocessText();
            var titles = text.GetTitles();

            List<Replacement> replacements;
            text = text.ReplaceFormulas( config, out replacements );


            text = text.ReplaceSymbols( config, out var charReplacements );

            //replacements.AddRange( charReplacements );

            var toTranslate = text.GetSentences();
            var translations = SendSeveralRequests( toTranslate );

            if ( translations is null )
            {
                Writer.Log( "Remote server returned empty response" );
                MessageBox.Show( "Response is empty, pls try again!", "Remote server error" );
                return;
            }

            string translatedText = string.Join( ". ", translations.Select( x => string.Join( "\n", x.Text ) ).ToArray() );

            translatedText = translatedText.SubstituteReplacements( replacements );
            translatedText = translatedText.SubstituteReplacements( charReplacements );

            

            Writer.WriteText( translatedText, "../../../../translation.tex" );

            translateRTB.Text = translatedText;
        }

        private List<Translation> SendSeveralRequests( string[] to_translate )
        {
            int totalLength = to_translate.Select( x => x.Length ).Sum();
            if ( totalLength < config.SingleRequestSize ) //все поместится в один запрос
            {
                var content = new Dictionary<string, string>()
                {
                    [ "folder_id" ] = config.folder_id,
                    [ "texts" ] = JsonConvert.SerializeObject( to_translate ),
                    [ "targetLanguageCode" ] = "en"
                };

                var request = (HttpWebRequest) WebRequest.Create( config.translate_url );
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add( "Authorization", "Bearer " + config.iam_token );

                using ( var streamWriter = new StreamWriter( request.GetRequestStream() ) )
                {
                    streamWriter.Write( JsonConvert.SerializeObject( content ) );
                }
                var response = request.GetResponse();
                var yResp = JsonConvert.DeserializeObject<TranslationResponse>(
                    new StreamReader( response.GetResponseStream() ).ReadToEnd() );
                return yResp.Translations;
            }
            else
            {
                int partLength = 0;
                List<string> partToTranslate = new List<string>();
                List<Translation> results = new List<Translation>();
                for ( int i = 0; i < to_translate.Length; i++ )
                {
                    if ( partToTranslate.Count == 0 )
                        partLength = 0;
                    else
                        partLength = partToTranslate.Select( x => x.Length ).Sum();

                    if ( partLength + to_translate[ i ].Length < config.SingleRequestSize )
                        partToTranslate.Add( to_translate[ i ] );
                    else
                    {
                        partToTranslate.RemoveAll( x => x == "" );
                        var response = SendRequest( JsonConvert.SerializeObject( partToTranslate ) );
                        results.AddRange( response?.Translations ?? new List<Translation>());
                        i--;
                        partToTranslate.Clear();
                    }
                }
                return results;
            }
        }

        private TranslationResponse SendRequest( string content )
        {
            if ( content == "[]" )
                return null;
            var payload = new Dictionary<string, string>()
            {
                [ "folder_id" ] = config.folder_id,
                [ "texts" ] = JsonConvert.SerializeObject( content ),
                [ "targetLanguageCode" ] = "en"
            };

            var request = (HttpWebRequest) WebRequest.Create( config.translate_url );
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers.Add( "Authorization", "Bearer " + config.iam_token );

            using ( var streamWriter = new StreamWriter( request.GetRequestStream() ) )
            {
                streamWriter.Write( JsonConvert.SerializeObject( payload ) );
            }
            TranslationResponse resp = new TranslationResponse();
            try
            {
                Writer.Log( $"SendRequest for translation: [{payload[ "texts" ].Substring( 0, 50 )}]" );
                var response = request.GetResponse();
                resp = JsonConvert.DeserializeObject<TranslationResponse>(
                    new StreamReader( response.GetResponseStream() ).ReadToEnd() );
                Writer.Log( $"Got response for translation: [{resp.Translations[ 0 ].Text.Substring( 0, 50 )}]" );
            }
            catch ( WebException we )
            {
                Writer.Log( $"Error while getting response for translation: [{we.Message}]" );
            }
            catch ( Exception e )
            {
                Writer.Log( $"Error during translation request: [{e.Message}]" );
            }
            return resp;
        }

        private void UpdateIamToken()
        {
            Writer.Log( "Starting IAM token update" );
            var request = (HttpWebRequest) WebRequest.Create( config.token_url );
            request.Method = "POST";
            using ( var streamWriter = new StreamWriter( request.GetRequestStream() ) )
            {
                streamWriter.Write( JsonConvert.SerializeObject( new Dictionary<string, string>()
                {
                    [ "yandexPassportOauthToken" ] = config.oauth_token
                } ) );
            }
            var response = request.GetResponse();

            TokenResponse tokenResp = JsonConvert.DeserializeObject<TokenResponse>(
                new StreamReader( response.GetResponseStream() ).ReadToEnd() );
            config.iam_token = tokenResp.Token;
            config.token_date = tokenResp.ExpiresAt;
            Writer.UpdateConfig( config, config_path );
            Writer.Log( "IAM token updated successfully" );
        }

        public string JoinText( string[] texts, string[] titles )
        {
            var sb = new StringBuilder();
            for ( int i = 0; i < Math.Min( texts.Length, titles.Length ); i++ )
            {
                sb.Append( texts[ i ] );
                sb.Append( titles[ i ] );
            }
            return sb.ToString();
        }

        private void renderBtn_Click( object sender, EventArgs e )
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK )
            {
                var filepath = ofd.FileName;
                var stream = ofd.OpenFile();
                string content;
                using (var sr = new StreamReader( stream ) )
                {
                    content = sr.ReadToEnd();
                }
                string directory = Path.GetDirectoryName( filepath );
                Writer.RenderLaTeX( content, directory + "/render.png" );
            }
        }
    }
}
