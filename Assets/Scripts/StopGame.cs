using UnityEngine;
using Mirror;

public class StopGame : MonoBehaviour
{
    [HideInInspector] public CharacterChat characterChat;
    public void Stop()
    {
        if (NetworkServer.active && NetworkClient.isConnected) NetworkManager.singleton.StopHost();
        else if (NetworkClient.isConnected)
        {
            Invoke(nameof(StopClient), 0.5f);
            characterChat.CmdStartSpawnMessage("<color=yellow>����� " + PlayerPrefs.GetString("Nick", "default") + " ����������</color>");
        }
        else if (NetworkServer.active) NetworkManager.singleton.StopServer();
    }
    private void StopClient() => NetworkManager.singleton.StopClient();
}
