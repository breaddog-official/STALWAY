using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTetrisTesting : MonoBehaviour {

    [SerializeField] private Transform inventoryTetrisBackground;
    [SerializeField] private InventoryTetris inventoryTetris;
    [SerializeField] private List<string> addItemTetrisSaveList;

    private int addItemTetrisSaveListIndex;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            inventoryTetris.Load(addItemTetrisSaveList[addItemTetrisSaveListIndex]);

            addItemTetrisSaveListIndex = (addItemTetrisSaveListIndex + 1) % addItemTetrisSaveList.Count;
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log(inventoryTetris.Save());
        }
    }

}
