using Mirror;
using UnityEngine;

public class CharacterChat : NetworkBehaviour
{
    [HideInInspector] public Chat chat;
    [Command]
    public void CmdStartSpawnMessage(string messageText) => StartSpawnMessage(messageText);
    [ClientRpc]
    private void StartSpawnMessage(string messageText) => chat.SpawnMessage(messageText);
}
