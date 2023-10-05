using UnityEngine;

public class Opener : MonoBehaviour
{
    public GameObject inventoryObject;
    public GameObject menuObject;
    public GameObject interfaceObject;
    private MainInputSystem inputSystem;

    private void Start()
    {
        inputSystem = new MainInputSystem();
        inputSystem.Enable();

        inputSystem.Actions.Inventory.performed += context => Toggle(inventoryObject);
        inputSystem.Actions.Menu.performed += context => Toggle(menuObject);
    }
    private void Toggle(GameObject gObject)
    {
        if(gObject.active)
        {
            gObject.SetActive(false);
            interfaceObject.SetActive(true);
        }
        else
        {
            gObject.SetActive(true);
            interfaceObject.SetActive(false);
        }
    }
}
