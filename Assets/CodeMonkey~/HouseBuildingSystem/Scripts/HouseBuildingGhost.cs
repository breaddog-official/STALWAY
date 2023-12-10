using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuildingGhost : MonoBehaviour {

    private Transform visual;

    private void Start() {
        RefreshVisual();

        HouseBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Vector3 targetPosition = HouseBuildingSystem.Instance.GetMouseWorldSnappedPosition();
        //targetPosition.y += .1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, HouseBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 25f);
    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        if (HouseBuildingSystem.Instance.GetPlaceObjectType() == HouseBuildingSystem.PlaceObjectType.GridObject) {
            PlacedObjectTypeSO placedObjectTypeSO = HouseBuildingSystem.Instance.GetPlacedObjectTypeSO();

            if (placedObjectTypeSO != null) {
                visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
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

