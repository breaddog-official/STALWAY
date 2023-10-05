using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Structs;
using Core;
using System.Linq;

public class PlayerList : MonoBehaviour
{
    [SerializeField] private PlayerListPrefab playerListPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Text selectedCharacter;
    [SerializeField] private Button[] buttonsHostClient;
    private GameObject[] playersInList;
    void Start()
    {
        Refresh();
    }
    public void Refresh()
    {
        string[] pathSaves = Directory.GetFiles(Application.streamingAssetsPath);
        if (playersInList == null) playersInList = new GameObject[pathSaves.Length];

        for (int i = 0; i < pathSaves.Length; i++)
        {
            if (!Path.GetFileName(pathSaves[i]).StartsWith("player_")) continue;

            PlayerStruct playerStruct = JsonUtility.FromJson<PlayerStruct>(File.ReadAllText(pathSaves[i]));
            if (playersInList.Count() >= i) Destroy(playersInList[i]);

            PlayerListPrefab prefab = Instantiate(playerListPrefab, content);
            playersInList[i] = prefab.gameObject;
            prefab.Install(playerStruct.name, null, this);
        }
    }
    public void NewPlayer()
    {
        if (nameInputField.text == "") return;

        PlayerJSON json = new PlayerJSON();

        PlayerListPrefab prefab = Instantiate(playerListPrefab, content);
        prefab.Install(nameInputField.text, null, this);

        PlayerStruct structP = new PlayerStruct();
        structP.name = nameInputField.text;

        SelectCharacter(structP.name);
        json.SaveToFile(structP);
    }
    public void SelectCharacter(string name)
    {
        selectedCharacter.text = name;
        PlayerPrefs.SetString("Nick", name);

        bool value;
        if (name == "") value = false;
        else value = true;

        for (int i = 0; i < buttonsHostClient.Length; i++) buttonsHostClient[i].interactable = value;
    }
}
