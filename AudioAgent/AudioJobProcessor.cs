﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioAgent
{
    class AudioJobProcessor
    {
        private bool isReady = true;
        public AudioJobProcessor()
        {

        }
        public byte[] SubmitWorkItem(byte[] FileContent, string inputPropriety, int inputLanguage, int voiceSpeed, int format)
        {
            string[] inProp = inputPropriety.Split(':');
            VoicePropriety[] voiceProprieties = inProp.Where(x => Enum.IsDefined(typeof(VoicePropriety), x))
                .Select(x => (VoicePropriety)Enum.Parse(typeof(VoicePropriety), x))
                .ToArray();
            Language language = (Language)inputLanguage;
            VoiceGender gender = VoiceGender.Female;
            CultureInfo ci = CultureInfo.GetCultureInfo("en-US");
            Encoding enc = Processor.GetEncoding(FileContent, language);

            string voiceName = String.Empty;

            #region LanguageSelection
            switch (language)
            {
                case Language.daDK:
                    ci = CultureInfo.GetCultureInfo("da-DK");
                    if (voiceProprieties.Contains(VoicePropriety.Male))
                    {
                        gender = VoiceGender.Male;
                        voiceName = "Carsten";
                    }
                    else
                    {
                        gender = VoiceGender.Female;
                        if (voiceProprieties.Contains(VoicePropriety.Anne))
                            voiceName = "Anne";
                        else
                            voiceName = "Sara";
                    }
                    break;
                case Language.ltLT:
                    ci = CultureInfo.GetCultureInfo("lt-LT");
                    if (voiceProprieties.Contains(VoicePropriety.Male))
                    {
                        gender = VoiceGender.Male;
                        if (voiceProprieties.Contains(VoicePropriety.Older))
                            voiceName = "Vladas";
                        else voiceName = "Edvardas";
                    }
                    else
                    {
                        gender = VoiceGender.Female;
                        if (voiceProprieties.Contains(VoicePropriety.Older))
                            voiceName = "Regina";
                        else voiceName = "Aiste";
                    }
                    break;
                //case Language.arEG:
                //    ci = CultureInfo.GetCultureInfo("ar-EG");
                //    if (voiceProprieties.Contains(VoicePropriety.Male))
                //    {
                //        gender = VoiceGender.Male;
                //        voiceName = "Sakhr Voice One";
                //    }
                //    else
                //    {
                //        gender = VoiceGender.Female;
                //        voiceName = "Sakhr Voice Two"; //Three, Four, Five, Six
                //    }
                //    break;
                case Language.huHU:
                    ci = CultureInfo.GetCultureInfo("hu-HU");
                    string huPreamble = "A ProfiVox beszél. . .";
                    FileContent = enc.GetBytes(huPreamble + enc.GetString(FileContent));
                    if (voiceProprieties.Contains(VoicePropriety.Male))
                    {
                        gender = VoiceGender.Male;
                        voiceName = "Gabor";
                    }
                    else
                    {
                        gender = VoiceGender.Female;
                        voiceName = "Eszter";
                    }
                    break;
                case Language.isIS:
                    ci = CultureInfo.GetCultureInfo("is-IS");
                    if (voiceProprieties.Contains(VoicePropriety.Male))
                    {
                        gender = VoiceGender.Male;
                        voiceName = "IVONA 2 Karl";
                    }
                    else
                    {
                        gender = VoiceGender.Female;
                        voiceName = "IVONA 2 Dóra";
                    }
                    break;
                case Language.nlNL:
                    ci = CultureInfo.GetCultureInfo("nl-NL");
                    if (voiceProprieties.Contains(VoicePropriety.Male))
                    {
                        gender = VoiceGender.Male;
                        voiceName = "Arthur";
                    }
                    else
                    {
                        gender = VoiceGender.Female;
                        voiceName = "Janneke";
                    }
                    break;
                case Language.cyGB:
                    ci = CultureInfo.GetCultureInfo("cy-GB");
                    if (voiceProprieties.Contains(VoicePropriety.Male))
                    {
                        gender = VoiceGender.Male;
                        voiceName = "IVONA 2 Geraint";

                    }
                    else
                    {
                        gender = VoiceGender.Female;
                        voiceName = "IVONA 2 Gwyneth";
                    }
                    break;
                case Language.enUS: ci = CultureInfo.GetCultureInfo("en-US"); gender = VoiceGender.Female; voiceName = "IVONA 2 Jennifer"; break;
                case Language.enGB: ci = CultureInfo.GetCultureInfo("en-GB"); gender = VoiceGender.Female; voiceName = "Kate"; break;
                case Language.frFR: ci = CultureInfo.GetCultureInfo("fr-FR"); gender = VoiceGender.Female; voiceName = "ScanSoft Virginie_Full_22kHz"; break;
                case Language.deDE: ci = CultureInfo.GetCultureInfo("de-DE"); gender = VoiceGender.Male; voiceName = "Stefan"; break;
                case Language.esES: ci = CultureInfo.GetCultureInfo("es-ES"); gender = VoiceGender.Male; voiceName = "Jorge"; break;
                case Language.esCO: ci = CultureInfo.GetCultureInfo("es-CO"); gender = VoiceGender.Female; voiceName = "Ximena"; break;
                case Language.bgBG: ci = CultureInfo.GetCultureInfo("bg-BG"); gender = VoiceGender.Female; voiceName = "Gergana"; break;
                case Language.itIT: ci = CultureInfo.GetCultureInfo("it-IT"); gender = VoiceGender.Female; voiceName = "Paola"; break;
                case Language.nbNO: ci = CultureInfo.GetCultureInfo("nb-NO"); break;
                case Language.roRO: ci = CultureInfo.GetCultureInfo("ro-RO"); gender = VoiceGender.Female; voiceName = "IVONA 2 Carmen"; break;
                case Language.svSE: ci = CultureInfo.GetCultureInfo("sv-SE"); break;
                case Language.plPL: ci = CultureInfo.GetCultureInfo("pl-PL"); gender = VoiceGender.Male; voiceName = "Krzysztof"; break;
                case Language.ptBR: ci = CultureInfo.GetCultureInfo("pt-BR"); break;
                case Language.enAU: ci = CultureInfo.GetCultureInfo("en-AU"); break;
                case Language.frCA: ci = CultureInfo.GetCultureInfo("fr-CA"); break;
                case Language.ptPT: ci = CultureInfo.GetCultureInfo("pt-PT"); gender = VoiceGender.Female; voiceName = "Amalia"; break;
                case Language.klGL: ci = CultureInfo.GetCultureInfo("kl-GL"); gender = VoiceGender.Female; voiceName = "Martha"; break;
                case Language.elGR: ci = CultureInfo.GetCultureInfo("el-GR"); gender = VoiceGender.Female; voiceName = "Maria"; break;
                case Language.slSI: ci = CultureInfo.GetCultureInfo("sl-SI"); gender = VoiceGender.Male; voiceName = "Renato Govorec"; break;
                case Language.jaJP: ci = CultureInfo.GetCultureInfo("ja-JP"); break;
                case Language.koKR: ci = CultureInfo.GetCultureInfo("ko-KR"); break;
                case Language.zhCN: ci = CultureInfo.GetCultureInfo("zh-CN"); voiceName = "Ekho Mandarin"; break;
                case Language.zhHK: ci = CultureInfo.GetCultureInfo("zh-HK"); voiceName = "Ekho Cantonese"; break;
                case Language.zhTW: ci = CultureInfo.GetCultureInfo("zh-TW"); voiceName = "Ekho Hakka"; break;
                case Language.fiFI: ci = CultureInfo.GetCultureInfo("fi-FI"); break;
                case Language.esMX: ci = CultureInfo.GetCultureInfo("es-MX"); break;
                case Language.caES: ci = CultureInfo.GetCultureInfo("ca-ES"); break;
                case Language.ruRU: ci = CultureInfo.GetCultureInfo("ru-RU"); gender = VoiceGender.Female; voiceName = "IVONA 2 Tatyana"; break;
                case Language.czCZ: ci = CultureInfo.GetCultureInfo("cz-CZ"); gender = VoiceGender.Female; voiceName = "Zuzana"; break;
                case Language.skSK: ci = CultureInfo.GetCultureInfo("sk-SK"); gender = VoiceGender.Female; voiceName = "Laura"; break;
                default: ci = CultureInfo.GetCultureInfo("en-US"); gender = VoiceGender.Female; voiceName = "IVONA 2 Jennifer"; break;
            }
            #endregion

            string tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
            File.Create(tempfile).Close();
            int rate = 0;// between -10 to 10
            if (voiceSpeed >= -10 && voiceSpeed <= 10)
                rate = voiceSpeed;

            try
            {
                Console.WriteLine("Preparing string for audio conversion.");
                var preparedString = PrepareTextForAudio(FileContent, enc); //enc.GetString(FileContent)
                Console.WriteLine("Preparation work done!");

                Console.WriteLine("Search for: " + ci.DisplayName + " " + gender + " " + voiceName);
                if (isVoiceInstalled(ci, gender, voiceName))
                {
                    Console.WriteLine("Voice installed! " + voiceName);
                    //main processing
                    if (ci.Equals(CultureInfo.GetCultureInfo("bg-BG"))
                        )
                    {
                        //may only work on 32bit configuration
                        ConvertWithSpeechLib(preparedString, voiceName, rate, tempfile);
                        return ConvertToAudioFormat(format, tempfile);
                    }
                    else
                    {
                        //may only work on 64bit configuration
                        ConvertWithSystemSpeech(preparedString, voiceName, gender, ci, rate, tempfile);
                        while (!isReady)
                        {
                            Console.Write(".");
                        }
                        Console.WriteLine();
                        return ConvertToAudioFormat(format, tempfile);
                    }
                    //Thread.Sleep(500);
                }
                //may only work on 64bit configuration
                else if (ConvertWithMicrosoftSpeech(preparedString, rate, ci, tempfile))
                {
                    //Thread.Sleep(500);
                    return ConvertToAudioFormat(format, tempfile);
                }
                else return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source);
                return null;
            }
        }

        public string PrepareTextForAudio(byte[] fileContent, Encoding enc)
        {
            string[] textBufArray = enc.GetString(fileContent).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < textBufArray.Length; i++)
            {
                if (!
                    (textBufArray[i].Trim().EndsWith(".")
                    || textBufArray[i].Trim().EndsWith("?")
                    || textBufArray[i].Trim().EndsWith("!")
                    || textBufArray[i].Trim().EndsWith(",")
                    || textBufArray[i].Trim().EndsWith(";")))
                {
                    if ((textBufArray[i].Trim().Length > 0))
                    {
                        textBufArray[i] += "..";
                    }
                }
            }
            return String.Join(Environment.NewLine, textBufArray);
        }

        /// <summary>
        /// Processes the wave file into the destination output format, deletes the temporary files and returns the result as a byte array
        /// </summary>
        /// <param name="format"></param>
        /// <param name="tempfile"></param>
        /// <returns></returns>
        private byte[] ConvertToAudioFormat(int format, string tempfile)
        {
            try
            {
                //output file to the correct format
                OutputEncodingProcessor oep = new OutputEncodingProcessor();
                string outputFile = "", fileName = Guid.NewGuid().ToString() + ".wav";
                switch (format)
                {
                    case 1:
                        fileName = Guid.NewGuid().ToString() + ".mp3";
                        outputFile = Path.Combine(Path.GetTempPath(), fileName);
                        File.Create(outputFile).Close();
                        oep.WaveToMP3(tempfile, outputFile, 32);
                        break;
                    case 2:
                        //tempfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
                        outputFile = tempfile;
                        tempfile = "do not delete";
                        break;
                    case 4:
                        fileName = Guid.NewGuid().ToString() + ".wma";
                        outputFile = Path.Combine(Path.GetTempPath(), fileName);
                        File.Create(outputFile).Close();
                        oep.WaveToWMA(tempfile, outputFile, 44100);
                        break;
                    case 8:
                        fileName = Guid.NewGuid().ToString() + ".aac";
                        outputFile = Path.Combine(Path.GetTempPath(), fileName);
                        File.Create(outputFile).Close();
                        if (oep.IsWinVistaOrHigher())
                            oep.WaveToAAC(tempfile, outputFile, 44100);
                        else
                            File.Copy(tempfile, outputFile, true);
                        break;
                }
                byte[] result = File.ReadAllBytes(outputFile);
                if (File.Exists(tempfile))
                    File.Delete(tempfile);
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
                //start put result on WEBSERVER2 file system
                string toSend = @"\\WEBSERVER2\Temp\" + fileName;
                File.WriteAllBytes(toSend, result);
                byte[] pathResult = Encoding.UTF8.GetBytes(toSend);
                //stop
                Console.WriteLine("Job Done.");
                return pathResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private bool ConvertWithSystemSpeech(string FileContent, string voiceName, VoiceGender gender, CultureInfo ci, int rate, string tempfile)
        {
            Console.WriteLine("Converting with: System Speech");
            try
            {
                EncodingFormat eformat = EncodingFormat.Pcm;
                using (var speech = new SpeechSynthesizer())
                {
                    var installedVoices = speech.GetInstalledVoices();
                    VoiceInfo selectedVoice = null;
                    foreach (var voice in installedVoices)
                    {
                        if (voice.VoiceInfo.Name == voiceName)
                        {
                            selectedVoice = voice.VoiceInfo;
                            break;
                        }
                        else if (voice.VoiceInfo.Culture == ci)
                        {
                            if (voice.VoiceInfo.Gender == gender)
                            {
                                selectedVoice = voice.VoiceInfo;
                                break;
                            }
                        }
                    }
                    speech.Rate = rate;
                    speech.SelectVoice(selectedVoice.Name);
                    var safi = new SpeechAudioFormatInfo(eformat, 22000, 16, 1, 22000 * 4, 4, null);
                    speech.SetOutputToWaveFile(tempfile, safi);
                    speech.SpeakCompleted += Speech_SpeakCompleted;
                    speech.Speak(FileContent);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private void Speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            isReady = e.Prompt.IsCompleted;
            //Console.WriteLine("Speech completed! "+e.Prompt.IsCompleted);
        }

        private unsafe bool ConvertWithSpeechLib(string FileContent, string voiceName, int rate, string tempfile)
        {
            Console.WriteLine("Converting with: SpeechLib");
            try
            {
                //process with Interop.Speech
                var SpVoice = new SpeechLib.SpVoice();
                foreach (SpeechLib.SpObjectToken t in SpVoice.GetVoices("", ""))
                {
                    string a = t.GetDescription(0);
                    if (a.Contains(voiceName))
                    {
                        SpVoice.Voice = t;
                        break;
                    }
                }
                SpVoice.Rate = rate;
                var cpFileStream = new SpeechLib.SpFileStream();
                cpFileStream.Format.Type = SpeechLib.SpeechAudioFormatType.SAFT22kHz16BitMono;
                cpFileStream.Open(tempfile, SpeechLib.SpeechStreamFileMode.SSFMCreateForWrite, false);
                SpVoice.AudioOutputStream = cpFileStream;
                SpVoice.Speak(FileContent, SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                SpVoice = null;
                cpFileStream.Close();
                cpFileStream = null;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool ConvertWithMicrosoftSpeech(string FileContent, int rate, CultureInfo ci, string tempPath)
        {
            Console.WriteLine("Converting with: Microsoft Speech");
            try
            {
                using (Microsoft.Speech.Synthesis.SpeechSynthesizer ss = new Microsoft.Speech.Synthesis.SpeechSynthesizer())
                {
                    var prompt = new Microsoft.Speech.Synthesis.PromptBuilder(ci);
                    prompt.AppendText(FileContent);
                    ss.Rate = rate;
                    ss.SetOutputToWaveFile(tempPath);
                    ss.Speak(prompt);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private bool isVoiceInstalled(CultureInfo ci, VoiceGender gender, string voiceName)
        {
            bool result = false;
            try
            {
                var speech = new SpeechSynthesizer();
                var installedVoices = speech.GetInstalledVoices();
                VoiceInfo selectedVoice = null;
                foreach (var voice in installedVoices)
                {
                    if (voice.VoiceInfo.Name == voiceName)
                    {
                        result = true;
                        selectedVoice = voice.VoiceInfo;
                        break;
                    }
                    else if (voice.VoiceInfo.Culture == ci)
                    {
                        if (voice.VoiceInfo.Gender == gender)
                        {
                            result = true;
                            selectedVoice = voice.VoiceInfo;
                            break;
                        }
                    }
                }
                //comment the below condition to include default Microsoft installed voices
                if (selectedVoice != null)
                    if (result && selectedVoice.Name.StartsWith("Microsoft", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = false;
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return true;
            }
            return result;
        }

        public static void WriteVoiceNames()
        {
            var speech = new SpeechSynthesizer();

            var voices = speech.GetInstalledVoices();
            foreach (var voice in voices)
            {
                char sex = 'F';
                if (voice.VoiceInfo.Gender.Equals(VoiceGender.Male))
                    sex = 'M';
                Console.WriteLine("Lang: " + voice.VoiceInfo.Culture + " Sex: " + sex + " Name: " + voice.VoiceInfo.Name);
            }
        }

        /// <summary>
        /// Used only to check for installed voices on your machine and also find necessary information about the voices
        /// </summary>
        public static void WriteAllInstalledVoicesToFile()
        {
            var speech = new SpeechSynthesizer();
            var voices = speech.GetInstalledVoices();
            Console.WriteLine("Installed voices: " + voices.Count);
            string voiceinfo = "";
            foreach (var voice in voices)
            {
                string addInfo = "", supAudio = "";
                foreach (var add in voice.VoiceInfo.AdditionalInfo)
                {
                    addInfo += "Key: " + add.Key + "- Value: " + add.Value + Environment.NewLine;
                }
                foreach (var sup in voice.VoiceInfo.SupportedAudioFormats)
                {
                    supAudio += " " + sup.AverageBytesPerSecond + " " + sup.BitsPerSample + " " + sup.BlockAlign + " " + sup.ChannelCount + " " + sup.EncodingFormat + " " + sup.SamplesPerSecond + Environment.NewLine;
                }
                voiceinfo += "Voice: ID=" + voice.VoiceInfo.Id + Environment.NewLine +
                    " Culture: " + voice.VoiceInfo.Culture + Environment.NewLine +
                    " Name: " + voice.VoiceInfo.Name + Environment.NewLine +
                    " Gender: " + voice.VoiceInfo.Gender + Environment.NewLine +
                    " Age: " + voice.VoiceInfo.Age + Environment.NewLine +
                    " Description: " + voice.VoiceInfo.Description + Environment.NewLine +
                    " AdditionalInfo: " + addInfo + Environment.NewLine +
                    " SupportedAudioFormats: " + supAudio + Environment.NewLine + Environment.NewLine;
            }
            Console.WriteLine(voiceinfo);
            File.Create(Environment.CurrentDirectory + "\\installedvoices.txt").Close();
            File.WriteAllText(Environment.CurrentDirectory + "\\installedvoices.txt", voiceinfo);
        }
    }
}