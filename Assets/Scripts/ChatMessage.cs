using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [Header("Links")]
    public Text messageText;
    public Animator anim;
    private void Start() => Invoke(nameof(StartDestroyMessage), 15.0f);
    public void SetText(string message)
    {
        if (message.Length <= 23) GetComponent<RectTransform>().sizeDelta = new Vector2(420.0f, 25.0f);

        messageText.text = message;
    }
    private void StartDestroyMessage() => anim.SetTrigger("Destroy");
    public void DestroyMessage() => Destroy(gameObject);
}
