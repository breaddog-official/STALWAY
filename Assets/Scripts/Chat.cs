using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private ChatMessage messagePrefab;
    [SerializeField] private Transform content;
    [SerializeField] private InputField inputField;
    [HideInInspector] public CharacterChat localCharacterChat;
    public void SpawnLocalMessage(string messageText)
    {
        if (inputField.text.Length == 0) return;
        inputField.text = "";

        localCharacterChat.CmdStartSpawnMessage("[" + PlayerPrefs.GetString("Nick", "default") + "]" + messageText);
    }
    public void SpawnMessage(string messageText)
    {
        ChatMessage spawnedMessage = Instantiate(messagePrefab, content);
        spawnedMessage.SetText(messageText);
    }
}
