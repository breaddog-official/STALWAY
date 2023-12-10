using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuildingSystemAssets : MonoBehaviour {

    public static HouseBuildingSystemAssets Instance { get; private set; }


    private void Awake() {
        Instance = this;
    }


    public PlacedObjectTypeSO[] placedObjectTypeSOArray;

    public PlacedObjectTypeSO floor;
    public PlacedObjectTypeSO floor2;
    public PlacedObjectTypeSO roof;


    public FloorEdgeObjectTypeSO[] floorEdgeObjectTypeSOArray;

    public FloorEdgeObjectTypeSO wall;
    public FloorEdgeObjectTypeSO wall2;
    public FloorEdgeObjectTypeSO door;
    public FloorEdgeObjectTypeSO door2;
    public FloorEdgeObjectTypeSO stairs;
    public FloorEdgeObjectTypeSO glass;
    public FloorEdgeObjectTypeSO wallWindow;
    public FloorEdgeObjectTypeSO wallWindow2;
    public FloorEdgeObjectTypeSO railing;


    public LooseObjectSO[] looseObjectArray;



    public PlacedObjectTypeSO GetPlacedObjectTypeSOFromName(string placedObjectTypeSOName) {
        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeSOArray) {
            if (placedObjectTypeSO.name == placedObjectTypeSOName) {
                return placedObjectTypeSO;
            }
        }
        return null;
    }

    public FloorEdgeObjectTypeSO GetFloorEdgeObjectTypeSOFromName(string floorEdgeObjectTypeSOName) {
        foreach (FloorEdgeObjectTypeSO floorEdgeObjectTypeSO in floorEdgeObjectTypeSOArray) {
            if (floorEdgeObjectTypeSO.name == floorEdgeObjectTypeSOName) {
                return floorEdgeObjectTypeSO;
            }
        }
        return null;
    }

    public LooseObjectSO GetLooseObjectSOFromName(string looseObjectSOName) {
        foreach (LooseObjectSO looseObjectSO in looseObjectArray) {
            if (looseObjectSO.name == looseObjectSOName) {
                return looseObjectSO;
            }
        }
        return null;
    }

}
