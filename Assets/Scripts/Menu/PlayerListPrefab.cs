using UnityEngine;
using UnityEngine.UI;

public class PlayerListPrefab : MonoBehaviour
{
    public Text name;
    public Text time;

    [HideInInspector] public PlayerList playerList;
    public void Install(string newName, string newTime, PlayerList newPlayerList)
    {
        name.text = newName;
        time.text = newTime;
        playerList = newPlayerList;
    }
    public void NewCelectedCharacter()
    {
        string nick = PlayerPrefs.GetString("Nick", "");

        if (nick == "") playerList.SelectCharacter(name.text);
        else if (nick == name.text) playerList.SelectCharacter("");
        else playerList.SelectCharacter(name.text);
    }
}
