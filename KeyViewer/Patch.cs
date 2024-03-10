using HarmonyLib;
using NAudio.Lame;
using NAudio.Wave;
using System;


namespace MicRecorder
{
    public static class Patch
    {


        [HarmonyPatch(typeof(scnGame), "Play")]
        public static class scnGame_Play
        {
            private static void Prefix()
            {
                if (Main.isRecording == true)
                {
                    Main.isRecording = false;
                    stopRecording();
                }
                Main.isRecording = true;
                startRecord();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
        public static class scnEditor_SwitchToEditMode
        {
            private static void Prefix()
            {
                Main.isRecording = false;
                stopRecording();
            }
        }

        [HarmonyPatch(typeof(scrController), "FailAction")]
        public static class scrController_FailAction
        {
            private static void Prefix()
            {
                if (scrController.instance.noFail)
                {
                    return;
                }

                Main.isRecording = false;
                stopRecording();
            }
        }

        public static void startRecord()
        {
            Main.waveIn = new WaveInEvent();
            Main.waveIn.DeviceNumber = Main.setting.selectedMicrophone;
            Main.waveIn.WaveFormat = new WaveFormat(44100, 1); // 44.1kHz sampling rate, mono
            Main.waveIn.DataAvailable += WaveIn_DataAvailable;
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("yyyy-MM-dd_HH-mm-ss.fff");
            Main.writer = new LameMP3FileWriter(Main.setting.outputPath + "\\" + formattedTime + ".mp3", Main.waveIn.WaveFormat, 128);

            Main.waveIn.StartRecording();

            Main.recording.TextObject.SetActive(true);

        }

        public static void stopRecording()
        {
            Main.waveIn.StopRecording();
            Main.writer.Close();
            Main.recording.TextObject.SetActive(false);

        }
        private static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Main.writer.Write(e.Buffer, 0, e.BytesRecorded);
        }
    }
}
