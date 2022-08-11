using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;

namespace SpeechRecognition
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine(); // Recognizes voice
        SpeechSynthesizer Alex = new SpeechSynthesizer();
        SpeechRecognitionEngine startlistening = new SpeechRecognitionEngine(); // starts listening for commands in speech
        Random rnd = new Random();
        int RecTimeOut = 0; // Counter between 1-10 secs for when it listens
        DateTime TimeNow = DateTime.Now;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _recognizer.SetInputToDefaultAudioDevice(); // Sets audio source (ie. mic, headphones)
            _recognizer.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("DefaultCommands.txt")))));
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            _recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            startlistening.SetInputToDefaultAudioDevice();
            startlistening.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines("DefaultCommands.txt")))));
            startlistening.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(startlistening_SpeechRecognized);
        }

        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            int ranNum;
            string speech = e.Result.Text;

            if (speech == "Hello")
            {
                Alex.SpeakAsync("Hello, I am here");
            }
            if (speech == "How are you")
            {
                Alex.SpeakAsync("I am working normally");
            }
            if (speech == "What time is it")
            {
                Alex.SpeakAsync(DateTime.Now.ToString("h mm tt"));
            }
            if (speech == "What day is it")
            {
                Alex.SpeakAsync(DateTime.Now.ToString("D"));
            }
                if (speech == "Stop talking")
            {
                Alex.SpeakAsyncCancelAll();
                ranNum = rnd.Next(1, 2);
                if (ranNum == 1)
                {
                    Alex.SpeakAsync("Ok");
                }

                if (ranNum == 2)
                {
                    Alex.SpeakAsync("My apologies, I will be quiet");
                }
            }
            if (speech == "Stop listening")
            {
                Alex.SpeakAsync("if you need me please ask");
                _recognizer.RecognizeAsyncCancel();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
            }

            if (speech == "Show commands")
            {
                string[] commands = (File.ReadAllLines("DefaultCommands.txt"));
                LstCommands.Items.Clear();
                LstCommands.SelectionMode = SelectionMode.None;
                LstCommands.Visible = true;
                foreach (string command in commands)
                {
                    LstCommands.Items.Add(command);
                }
            }

            if (speech == "Hide commands")
            {
                LstCommands.Visible = false;
            }

        }

        private void _recognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void startlistening_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            if (speech == "Wake up")
            {
                startlistening.RecognizeAsyncCancel();
                Alex.SpeakAsync("Yes, I am here");
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void TmrSpeaking_Tick_1(object sender, EventArgs e)
        {
            if (RecTimeOut == 10)
            {
                _recognizer.RecognizeAsyncCancel();
            }
            else if (RecTimeOut == 11)
            {
                TmrSpeaking.Stop();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
                RecTimeOut = 0; // start timer over if Alex hears something
            }

        }
    }
}
