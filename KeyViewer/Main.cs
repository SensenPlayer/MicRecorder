using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;
using UnityEngine;
using NAudio.Wave;
using NAudio.Lame;
using System;



namespace MicRecorder
{
    public static class Main
    {

        public static TextBehaviour textObject;
        public static TextBehaviour recording;

        public static UnityModManager.ModEntry ModEntry;

        public static Harmony harmony;

        public static Setting setting;

        public static WaveInEvent waveIn;
        public static LameMP3FileWriter writer;
        public static Boolean isRecording = false;

        public static List<string> microphones = GetMicrophones();

        public static void Setup(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            modEntry.OnToggle = OnToggle;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnGUI = OnGUI;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool isModEnabled)
        {
            if(isModEnabled)
            {
                setting = new Setting();
                setting = UnityModManager.ModSettings.Load<Setting>(modEntry);

                recording = new GameObject().AddComponent<TextBehaviour>();
                recording.TextObject.SetActive(false);
                recording.setText(setting.text);
                UnityEngine.Object.DontDestroyOnLoad(recording);

                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

            }
            else
            {
                harmony.UnpatchAll();
            }
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        { 
            //Microphone
            GUILayout.BeginHorizontal();

            GUILayout.Label("Please select your microphone device:");
            GUILayout.Label("Currently selected: " + setting.selectedMicrophone);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            int n = 0;
            foreach (string microphone in microphones)
            {        
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (GUILayout.Button(n + ": " + microphone))
                {
                    setting.selectedMicrophone = n;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                n++;
            }

            //Path
            GUILayout.BeginHorizontal();

            GUILayout.Label("File save path");
            setting.outputPath = GUILayout.TextField(setting.outputPath);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Text
            GUILayout.BeginHorizontal();
            GUILayout.Label("Text:");
            setting.text = GUILayout.TextField(setting.text);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Size
            GUILayout.BeginHorizontal();
            GUILayout.Label("Record Text Size:");
            string recordSizeStr = GUILayout.TextField(setting.voiceSize + "");
            try
            {
                setting.voiceSize = int.Parse(recordSizeStr);
            }
            catch
            {
                setting.voiceSize = 80;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //X
            GUILayout.BeginHorizontal();

            GUILayout.Label("Record Text X:");
            setting.voiceX = GUILayout.HorizontalSlider(setting.voiceX, 0, 1, GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Y
            GUILayout.BeginHorizontal();

            GUILayout.Label("Record Text Y:");
            setting.voiceY = GUILayout.HorizontalSlider(setting.voiceY, 0, 1, GUILayout.Width(100));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            Main.recording.setSize((int)setting.voiceSize);
            Main.recording.setPosition(setting.voiceX, setting.voiceY);

        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            setting.Save(modEntry);
        }

        private static List<string> GetMicrophones()
        {
            List<string> microphones = new List<string>();

            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                var capabilities = WaveIn.GetCapabilities(n);
                microphones.Add(capabilities.ProductName);
            }

            return microphones;
        }
    }
}