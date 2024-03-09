using System;
using System.IO;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace MicRecorder
{
    public class Setting : UnityModManager.ModSettings
    {

        public string text = "Recording Microphone...";
        public float voiceSize = 80f, voiceX = 0.5f, voiceY = 0.5f;
        public int selectedMicrophone = 0;
        public string outputPath = "H:\\SensenMod";







        public override void Save(UnityModManager.ModEntry modEntry) {
            var filepath = GetPath(modEntry);
            try {
                using (var writer = new StreamWriter(filepath)) {
                    var serializer = new XmlSerializer(GetType());
                    serializer.Serialize(writer, this);
                }
            } catch (Exception e) {
                modEntry.Logger.Error($"Can't save {filepath}.");
                modEntry.Logger.LogException(e);
            }
        }
        
        public override string GetPath(UnityModManager.ModEntry modEntry) {
            return Path.Combine(modEntry.Path, GetType().Name + ".xml");
        }

    }
}
