﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CLIPSNET;


namespace ClipsFormsExample
{
    public partial class ClipsFormsExample : Form
    {
        private CLIPSNET.Environment clips = new CLIPSNET.Environment();
        /// <summary>
        /// Распознаватель речи
        /// </summary>
        //private Microsoft.Speech.Synthesis.SpeechSynthesizer synth;
        
        /// <summary>
        /// Распознавалка
        /// </summary>

        public ClipsFormsExample()
        {
            InitializeComponent();
            //synth = new Microsoft.Speech.Synthesis.SpeechSynthesizer();
            //synth.SetOutputToDefaultAudioDevice();

            //var voices = synth.GetInstalledVoices(System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("ru-RU"));
            //foreach (var v in voices)
            //    voicesBox.Items.Add(v.VoiceInfo.Name);
            //if (voicesBox.Items.Count > 0)
            //{
            //    voicesBox.SelectedIndex = 0;
            //    synth.SelectVoice(voices[0].VoiceInfo.Name);
            //}

            //var RecognizerInfo = Microsoft.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers().Where(ri => ri.Culture.Name == "ru-RU").FirstOrDefault();
            //recogn = new Microsoft.Speech.Recognition.SpeechRecognitionEngine(RecognizerInfo);
            //recogn.SpeechRecognized += Recogn_SpeechRecognized;
            //recogn.SetInputToDefaultAudioDevice();
        }

        private void NewRecognPhrases(List<string> phrases)
        {
            //outputBox.Text += "Стартуем распознавание" + System.Environment.NewLine;
            //var Choises = new Microsoft.Speech.Recognition.Choices();
            //Choises.Add(phrases.ToArray());

            //var gb = new Microsoft.Speech.Recognition.GrammarBuilder();
            //var RecognizerInfo = Microsoft.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers().Where(ri => ri.Culture.Name == "ru-RU").FirstOrDefault();
            //gb.Culture = RecognizerInfo.Culture;
            //gb.Append(Choises);

            //var gr = new Microsoft.Speech.Recognition.Grammar(gb);
            //recogn.LoadGrammar(gr);
            //recogn.RequestRecognizerUpdate();
            //recogn.RecognizeAsync(Microsoft.Speech.Recognition.RecognizeMode.Multiple);
        }

        private void Recogn_SpeechRecognized(object sender, Microsoft.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            //recogn.RecognizeAsyncStop();
            //recogn.RecognizeAsyncCancel();
            //outputBox.Text += "Ваш голос распознан!" + System.Environment.NewLine;
            //clips.Eval("(assert (answer " + e.Result.Text + "))");
            //clips.Eval("(assert (clearmessage))");
            //outputBox.Text += "Продолжаю выполнение!" + System.Environment.NewLine;
            clips.Run();
            HandleResponse();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void HandleResponse()
        {
            //  Вытаскиаваем факт из ЭС
            String evalStr = "(find-fact ((?f ioproxy)) TRUE)";
            FactAddressValue fv = (FactAddressValue)((MultifieldValue)clips.Eval(evalStr))[0];


            /*
             
             (deffacts team
                (entity (name pudge))
                )
             
             */

            MultifieldValue damf = (MultifieldValue)fv["messages"];
            MultifieldValue vamf = (MultifieldValue)fv["answers"];
            
            outputBox.Text += "Новая итерация : " + System.Environment.NewLine;
            for (int i = 0; i < damf.Count; i++)
            {
                LexemeValue da = (LexemeValue)damf[i];
                byte[] bytes = Encoding.Default.GetBytes(da.Value);

                string message = Encoding.UTF8.GetString(bytes);
                outputBox.Text += message + System.Environment.NewLine;

                string[] messages = message.Split();
                string fact = messages.Last().ToString();

                if (fact == checkedListBox2.CheckedItems[0].ToString())
                {
                    outputBox.Text += "Искомый персонаж найден, поздравляем!" + System.Environment.NewLine;
                    break;
                }

                //synth.SpeakAsync(message);
            }

            var phrases = new List<string>();
            if (vamf.Count > 0)
            {
                outputBox.Text += "----------------------------------------------------" + System.Environment.NewLine;
                for (int i = 0; i < vamf.Count; i++)
                {
                    //  Варианты !!!!!
                    LexemeValue va = (LexemeValue)vamf[i];
                    byte[] bytes = Encoding.Default.GetBytes(va.Value);
                    string message = Encoding.UTF8.GetString(bytes);
                    phrases.Add(message);
                    outputBox.Text += "Добавлен вариант для распознавания " + message + System.Environment.NewLine;
                }
            }
            
            if(vamf.Count == 0)
                clips.Eval("(assert (clearmessage))");
            else
                NewRecognPhrases(phrases);
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            clips.Run();
            HandleResponse();
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            outputBox.Text = "Выполнены команды Clear и Reset." + System.Environment.NewLine;
            //  Здесь сохранение в файл, и потом инициализация через него
            clips.Clear();

            /*string stroka = codeBox.Text;
            System.IO.File.WriteAllText("tmp.clp", codeBox.Text);
            clips.Load("tmp.clp");*/

            //  Так тоже можно - без промежуточного вывода в файл
            clips.LoadFromString(codeBox.Text);

            clips.Reset();
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            if (clipsOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                codeBox.Text = System.IO.File.ReadAllText(clipsOpenFileDialog.FileName);
                //Text = "Экспертная система" + clipsOpenFileDialog.FileName;
            }
        }

        private void fontSelect_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                codeBox.Font = fontDialog1.Font;
                outputBox.Font = fontDialog1.Font;
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            clipsSaveFileDialog.FileName = clipsOpenFileDialog.FileName;
            if (clipsSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(clipsSaveFileDialog.FileName, codeBox.Text);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sbnew = new StringBuilder();
            string s = "\r\n(deffacts team \r\n";
            foreach (var a in checkedListBox1.CheckedItems)
            {
                s+= "    (entity (name " + a.ToString() + "))\r\n";
            }
            s += ")";

            sbnew.Append(s);
            codeBox.Text += sbnew.ToString();
            clips.LoadFromString(codeBox.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
