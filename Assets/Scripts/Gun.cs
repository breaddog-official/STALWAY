using UnityEngine;

public class Gun : MonoBehaviour
{
    private Transform cursor;
    [SerializeField] private Character character;
    [HideInInspector] public float offset;
    private Character.PlayerTypes playerType;

    private void Awake()
    {
        cursor = GameObject.Find("Cursor").transform;
    }
    void Update()
    {
        if(playerType == Character.PlayerTypes.Player && character.isLocal)
        {
            RotateGun();
        }
    }
    private void RotateGun()
    {
        Vector3 difference = cursor.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotZ + offset);
    }
}
