using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridLevelUI : MonoBehaviour {

    private TextMeshProUGUI textMeshPro;

    private void Awake() {
        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "Grid Level " + 1;
    }

    private void Start() {
        HouseBuildingSystem.Instance.OnActiveGridLevelChanged += HouseBuildingSystem_OnActiveGridLevelChanged;
    }

    private void HouseBuildingSystem_OnActiveGridLevelChanged(object sender, System.EventArgs e) {
        textMeshPro.text = "Grid Level " + (HouseBuildingSystem.Instance.GetActiveGridLevel() + 1);
    }

}
