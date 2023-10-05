using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEdgePlacedObject : MonoBehaviour {

    [SerializeField] private FloorEdgeObjectTypeSO floorEdgeObjectTypeSO;

    public string GetFloorEdgeObjectTypeSOName() {
        return floorEdgeObjectTypeSO.name;
    }

}
