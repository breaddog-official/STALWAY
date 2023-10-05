using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    public Texture2D[] cursorTextures;
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;

        transform.position = mousePosition;
    }
    public void ChangeCursorTexture(int typeNumber)
    {
        Cursor.SetCursor(cursorTextures[typeNumber], Vector2.zero, CursorMode.Auto);
    }
}
