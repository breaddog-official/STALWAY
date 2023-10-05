using UnityEngine;
using Mirror;
using System;

public class Character : NetworkBehaviour
{
    public enum PlayerTypes
    {
        Player,
        NPC,
    }

    [Header("Main")]
    public PlayerTypes playerType;
    [SyncVar(hook = nameof(SetName))] public string PlayerName;

    [Header("Floats")]
    [SerializeField] private float speed = 3.5f;

    [Header("Links")]
    [SerializeField] private Transform playerSprite;
    [SerializeField] private Gun gun;
    [SerializeField] private Animator anim;

    [HideInInspector] public Cloud cloud;
    [HideInInspector] public bool isLocal = false;

    private Transform cursor;
    private float speedMultiplier = 1.0f;
    private MainInputSystem inputSystem;
    private bool isRunning = false;
    private bool isMoving = false;
    private bool isRight = true;
    void Start()
    {
        if (isLocalPlayer) isLocal = true;

        cloud = GetComponent<Cloud>();

        CharacterChat characterChat = GetComponent<CharacterChat>();

        Chat chat = FindFirstObjectByType<Chat>();
        characterChat.chat = chat;

        if (playerType == PlayerTypes.Player && isLocal)
        {
            chat.localCharacterChat = characterChat;
            cursor = GameObject.Find("Cursor").transform;

            CmdServerSetName(PlayerPrefs.GetString("Nick", "default"));
            Character[] charactersOnScene = FindObjectsByType<Character>(FindObjectsSortMode.None);
            for (int i = 0; i < charactersOnScene.Length; i++) charactersOnScene[i].cloud.nickText.text = charactersOnScene[i].PlayerName;

            cloud.CmdSetMessageStart("Я родился", 2.0f);

            StopGame stopGame = FindFirstObjectByType<StopGame>();
            stopGame.characterChat = characterChat;

            characterChat.CmdStartSpawnMessage("<color=yellow>Игрок " + PlayerPrefs.GetString("Nick", "default") + " подключился</color>");

            inputSystem = new MainInputSystem();
            inputSystem.Enable();

            inputSystem.Movement.Run.performed += context => Run();
        }
    }
    void Update()
    {
        if (playerType == PlayerTypes.Player && isLocal)
        {
            Move(inputSystem.Movement.Move.ReadValue<Vector2>());
        }
    }
    private void Move(Vector2 direction)
    {
        float scaledSpeed = speed * speedMultiplier * Time.deltaTime;
        Vector3 moveDirection = new Vector3(direction.x * scaledSpeed, direction.y * scaledSpeed, 0);
        transform.position += moveDirection;

        if (isRight && transform.position.x > cursor.position.x) CmdFlipServerSync(180.0f);
        else if (!isRight && transform.position.x < cursor.position.x) CmdFlipServerSync(0.0f);

        if (direction.x != 0 || direction.y != 0)
        {
            if (!isMoving)
            {
                CmdAnimationServerSync(true);
                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                CmdAnimationServerSync(false);
                isMoving = false;
            }
        }
    }

    private void Run()
    {
        if (isRunning)
        {
            speedMultiplier = 1.0f;
            isRunning = false;
        }
        else
        {
            speedMultiplier = 1.5f;
            isRunning = true;
        }
    }
    public void SetName(string oldName, string newName) => cloud.nickText.text = newName;
    [Command]
    private void CmdServerSetName(string name) => PlayerName = name;
    [Command]
    private void CmdFlipServerSync(float newOffset) => RpcFlip(newOffset);
    [Command]
    private void CmdAnimationServerSync(bool value) => RpcAnimation(value);

    [ClientRpc]
    private void RpcFlip(float newOffset)
    {
        gun.offset = newOffset;
        isRight = !isRight;

        Vector3 scaler = playerSprite.localScale;
        if (newOffset == 0.0f) scaler.x = 1;
        else scaler.x = -1;

        playerSprite.localScale = scaler;
    }
    [ClientRpc]
    private void RpcAnimation(bool value)
    {
        anim.SetBool("isMove", value);
    }
}
