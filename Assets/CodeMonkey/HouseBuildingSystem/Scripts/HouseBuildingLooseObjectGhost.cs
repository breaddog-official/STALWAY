using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuildingLooseObjectGhost : MonoBehaviour {

    private Transform visual;
    private LooseObjectSO looseObjectSO;

    private void Start() {
        RefreshVisual();

        HouseBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
            transform.position = Vector3.Lerp(transform.position, raycastHit.point, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, HouseBuildingSystem.Instance.GetLooseObjectEulerY(), 0), Time.deltaTime * 25f);
        }
    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        if (HouseBuildingSystem.Instance.GetPlaceObjectType() == HouseBuildingSystem.PlaceObjectType.LooseObject) {
            LooseObjectSO looseObjectSO = HouseBuildingSystem.Instance.GetLooseObjectSO();

            if (looseObjectSO != null) {
                visual = Instantiate(looseObjectSO.prefab, Vector3.zero, Quaternion.identity);
                visual.parent = transform;
                visual.localPosition = Vector3.zero;
                visual.localEulerAngles = Vector3.zero;
                SetLayerRecursive(visual.gameObject, 14);
            }
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer) {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

}

