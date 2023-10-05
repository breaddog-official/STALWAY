using Structs;
using UnityEngine;
using System;
using System.IO;
using static TopDownShooter.BattleSystem_Done;

namespace Core
{
    public class PlayerJSON
    {
        private string savePath;
        private string saveFileName;

        public void SaveToFile(PlayerStruct save)
        {
            string json = JsonUtility.ToJson(save, true);

            try
            {
                saveFileName = "player_" + save.name + ".json";
                #if UNITY_ANDROID && !UNITY_EDITOR
                savePath = Path.Combine(Application.persistentDataPath, saveFileName);
                #else
                savePath = Path.Combine(Application.streamingAssetsPath, saveFileName);
                #endif
                File.WriteAllText(savePath, json);
            }
            catch (Exception e)
            {
                Debug.Log("{GameLog} => [GameCore] - (<color=red>Error</color>) - SaveToFile -> " + e.Message);                                                        
            }
        }

        public PlayerStruct LoadFromFile(string name)
        {
            if (!File.Exists(savePath))
            {
                Debug.Log("{GameLog} => [GameCore] - LoadFromFile -> File Not Found!");

                PlayerStruct saveloadContainer = new PlayerStruct();
                return saveloadContainer;
            }

            try
            {
                saveFileName = "player_" + name + ".json";
                savePath = Path.Combine(Application.streamingAssetsPath, saveFileName);

                string json = File.ReadAllText(savePath);

                PlayerStruct containerFromJson = JsonUtility.FromJson<PlayerStruct>(json);
                return containerFromJson;
            }
            catch (Exception e)
            {
                Debug.Log("{GameLog} - [GameCore] - (<color=red>Error</color>) - LoadFromFile -> " + e.Message);

                PlayerStruct saveloadContainer = new PlayerStruct();
                return saveloadContainer;
            }
            
        }
    }
}
