using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Cloud : NetworkBehaviour
{
    [SerializeField] public Text nickText;
    [SerializeField] private Text messageText;

    [Command]
    public void CmdSetNickStart(string newNick) => RpcSetNick(newNick);
    [Command]
    public void CmdSetMessageStart(string newMessage, float time) => RpcSetMessage(newMessage, time);
    [Command]
    public void CmdSetMessageStart(string newMessage) => RpcSetMessage(newMessage);
    [Command]
    public void CmdToggleMessageObjectStart(bool value) => RpcToggleMessageObject(value);
    [Command]
    public void CmdToggleMessageObjectStart() => RpcToggleMessageObject();


    [ClientRpc]
    private void RpcSetNick(string newNick)
    {
        nickText.text = newNick;
    }
    [ClientRpc]
    private void RpcSetMessage(string newMessage, float time)
    {
        messageText.text = newMessage;
        CmdToggleMessageObjectStart(true);

        Invoke(nameof(CmdToggleMessageObjectStart), time);
    }
    [ClientRpc]
    private void RpcSetMessage(string newMessage)
    {
        messageText.text = newMessage;
        CmdToggleMessageObjectStart(true);
    }
    [ClientRpc]
    private void RpcToggleMessageObject(bool value)
    {
        messageText.gameObject.SetActive(value);

        if (value) nickText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0.25f, 0);
        else nickText.GetComponent<RectTransform>().anchoredPosition = messageText.GetComponent<RectTransform>().anchoredPosition;
    }
    [ClientRpc]
    private void RpcToggleMessageObject()
    {
        messageText.gameObject.SetActive(!messageText.gameObject.activeSelf);

        if (messageText.gameObject.activeSelf) nickText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0.25f, 0);
        else nickText.GetComponent<RectTransform>().anchoredPosition = messageText.GetComponent<RectTransform>().anchoredPosition;
    }
}
