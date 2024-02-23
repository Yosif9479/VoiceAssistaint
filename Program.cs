using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;


namespace VoiceAssistaint
{
    internal class Program
    {
        //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 You can use mine if you want :) but i dont have much money on it
        private static OpenAIAPI api = new OpenAIAPI("YOUR OPEN AI API TOKEN");
        private static Conversation chat;
        private static SpeechSynthesizer speechSynthesizer;
        private static SpeechRecognitionEngine speechRecognizer;
        static void Main()
        {
            speechSynthesizer = new SpeechSynthesizer();

            chat = api.Chat.CreateConversation();
            chat.RequestParameters.Temperature = 0;

            speechSynthesizer.SpeakCompleted += (sender, args) =>
            {
                speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            };
            
            using (speechRecognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US")))
            {
                speechRecognizer.LoadGrammar(new DictationGrammar());
                speechRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs> (OnSpeechRecognized);
                speechRecognizer.SetInputToDefaultAudioDevice();
                speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                Console.ReadLine(); 
            }
            
        }

        private static void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs arguments)
        {
            if (arguments.Result.Text.ToLower().StartsWith("submit"))
            {
                Console.WriteLine(arguments.Result.Text);
                Talk(arguments.Result.Text.Remove(0, 5));
            }
            else
            {
                Console.WriteLine("... " + arguments.Result.Text);
            }
        }

        private static async void Talk(string question)
        {
            //speechRecognizer.RecognizeAsyncCancel();
            chat.AppendUserInput(question);
            var response = chat.GetResponseFromChatbotAsync();
            await response;
            Console.WriteLine(response.Result);

            speechSynthesizer.SelectVoice("Microsoft David Desktop");
            speechSynthesizer.Rate = 0;
            speechSynthesizer.Volume = 100;
            speechSynthesizer.SpeakAsync(response.Result);
        }
    }
}
