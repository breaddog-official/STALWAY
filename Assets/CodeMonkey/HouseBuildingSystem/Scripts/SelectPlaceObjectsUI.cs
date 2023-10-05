using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class SelectPlaceObjectsUI : MonoBehaviour {

    private Dictionary<PlacedObjectTypeSO, Transform> placedObjectTypeTransformDic;
    private Dictionary<FloorEdgeObjectTypeSO, Transform> floorEdgeObjectTypeTransformDic;

    private void Awake() {
        placedObjectTypeTransformDic = new Dictionary<PlacedObjectTypeSO, Transform>();
        floorEdgeObjectTypeTransformDic = new Dictionary<FloorEdgeObjectTypeSO, Transform>();

        placedObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.floor] = transform.Find("FloorBtn");
        placedObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.floor2] = transform.Find("Floor2Btn");

        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.wall] = transform.Find("WallBtn");
        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.door] = transform.Find("DoorBtn");
        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.stairs] = transform.Find("StairsBtn");

        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.wall2] = transform.Find("Wall2Btn");
        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.door2] = transform.Find("Door2Btn");

        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.glass] = transform.Find("GlassBtn");
        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.wallWindow] = transform.Find("WallWindowBtn");
        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.wallWindow2] = transform.Find("WallWindow2Btn");

        placedObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.roof] = transform.Find("RoofBtn");
        floorEdgeObjectTypeTransformDic[HouseBuildingSystemAssets.Instance.railing] = transform.Find("RailingBtn");

        transform.Find("ArrowBtn").GetComponent<Button_UI>().ClickFunc = () => { };


        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeTransformDic.Keys) {
            placedObjectTypeTransformDic[placedObjectTypeSO].GetComponent<Button_UI>().ClickFunc = () => {
                HouseBuildingSystem.Instance.SelectPlacedObjectTypeSO(placedObjectTypeSO);
            };
        }

        foreach (FloorEdgeObjectTypeSO floorEdgeObjectTypeSO in floorEdgeObjectTypeTransformDic.Keys) {
            floorEdgeObjectTypeTransformDic[floorEdgeObjectTypeSO].GetComponent<Button_UI>().ClickFunc = () => {
                HouseBuildingSystem.Instance.SelectFloorEdgeObjectTypeSO(floorEdgeObjectTypeSO);
            };
        }

        transform.Find("LooseObjectBtn").GetComponent<Button_UI>().ClickFunc = () => {
            LooseObjectSO looseObjectSO = HouseBuildingSystem.Instance.GetLooseObjectSO();
            if (looseObjectSO == null) {
                HouseBuildingSystem.Instance.SelectLooseObjectTypeSO(HouseBuildingSystemAssets.Instance.looseObjectArray[0]);
            } else {
                int nextIndex = System.Array.IndexOf(HouseBuildingSystemAssets.Instance.looseObjectArray, looseObjectSO);
                nextIndex = (nextIndex + 1) % HouseBuildingSystemAssets.Instance.looseObjectArray.Length;
                HouseBuildingSystem.Instance.SelectLooseObjectTypeSO(HouseBuildingSystemAssets.Instance.looseObjectArray[nextIndex]);
            }
        };
    }

    private void Start() {
        HouseBuildingSystem.Instance.OnSelectedChanged += HouseBuildingSystem_OnSelectedChanged;

        RefreshSelectedVisual();
    }

    private void HouseBuildingSystem_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshSelectedVisual();
    }

    private void RefreshSelectedVisual() {
        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeTransformDic.Keys) {
            placedObjectTypeTransformDic[placedObjectTypeSO].Find("Selected").gameObject.SetActive(false);

            if (HouseBuildingSystem.Instance.GetPlaceObjectType() == HouseBuildingSystem.PlaceObjectType.GridObject) {
                placedObjectTypeTransformDic[placedObjectTypeSO].Find("Selected").gameObject.SetActive(
                    HouseBuildingSystem.Instance.GetPlacedObjectTypeSO() == placedObjectTypeSO
                );
            }
        }

        foreach (FloorEdgeObjectTypeSO floorEdgeObjectTypeSO in floorEdgeObjectTypeTransformDic.Keys) {
            floorEdgeObjectTypeTransformDic[floorEdgeObjectTypeSO].Find("Selected").gameObject.SetActive(false);

            if (HouseBuildingSystem.Instance.GetPlaceObjectType() == HouseBuildingSystem.PlaceObjectType.EdgeObject) {
                floorEdgeObjectTypeTransformDic[floorEdgeObjectTypeSO].Find("Selected").gameObject.SetActive(
                    HouseBuildingSystem.Instance.GetFloorEdgeObjectTypeSO() == floorEdgeObjectTypeSO
                );
            }
        }

        transform.Find("ArrowBtn").Find("Selected").gameObject.SetActive(
            HouseBuildingSystem.Instance.GetPlacedObjectTypeSO() == null &&
            HouseBuildingSystem.Instance.GetFloorEdgeObjectTypeSO() == null &&
            HouseBuildingSystem.Instance.GetLooseObjectSO() == null
        );

        transform.Find("LooseObjectBtn").Find("Selected").gameObject.SetActive(HouseBuildingSystem.Instance.GetLooseObjectSO() != null);
    }

}
