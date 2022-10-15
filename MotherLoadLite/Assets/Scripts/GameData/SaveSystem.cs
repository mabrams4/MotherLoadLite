using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/* This script saves and loads game data using a binary formatter */
public static class SaveSystem
{

    public static bool isNewGame;   // global variable to keep track of whether or not a new or saved
                                    // game is being laoded

    // saves the game
    public static void SaveGame(Inventory inventory, PlayerInfo playerInfo,
                                MissionControl missionControl, RogueColony rogueColony,
                                BabyWorm babyWorm)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.data";
        Debug.Log(path);

        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(inventory, playerInfo, missionControl, rogueColony, babyWorm);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    // loads the game
    public static GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/game.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
