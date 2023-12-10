using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlacedObject : PlacedObject {

    public enum Edge {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private FloorEdgePosition upFloorEdgePosition;
    [SerializeField] private FloorEdgePosition downFloorEdgePosition;
    [SerializeField] private FloorEdgePosition leftFloorEdgePosition;
    [SerializeField] private FloorEdgePosition rightFloorEdgePosition;

    private FloorEdgePlacedObject upEdgeObject;
    private FloorEdgePlacedObject downEdgeObject;
    private FloorEdgePlacedObject leftEdgeObject;
    private FloorEdgePlacedObject rightEdgeObject;

    public void PlaceEdge(Edge edge, FloorEdgeObjectTypeSO floorEdgeObjectTypeSO) {
        FloorEdgePosition floorEdgePosition = GetFloorEdgePosition(edge);

        Transform floorEdgeObjectTransform = Instantiate(floorEdgeObjectTypeSO.prefab, floorEdgePosition.transform.position, floorEdgePosition.transform.rotation);

        FloorEdgePlacedObject currentFloorEdgePlacedObject = GetFloorEdgePlacedObject(edge);
        if (currentFloorEdgePlacedObject != null) {
            Destroy(currentFloorEdgePlacedObject.gameObject);
        }

        FloorEdgePlacedObject floorEdgePlacedObject = floorEdgeObjectTransform.GetComponent<FloorEdgePlacedObject>();
        SetFloorEdgePlacedObject(edge, floorEdgePlacedObject);
    }

    private FloorEdgePosition GetFloorEdgePosition(Edge edge) {
        switch (edge) {
            default:
            case Edge.Up:       return upFloorEdgePosition;
            case Edge.Down:     return downFloorEdgePosition;
            case Edge.Left:     return leftFloorEdgePosition;
            case Edge.Right:    return rightFloorEdgePosition;
        }
    }

    private void SetFloorEdgePlacedObject(Edge edge, FloorEdgePlacedObject floorEdgePlacedObject) {
        switch (edge) {
            default:
            case Edge.Up:
                upEdgeObject = floorEdgePlacedObject;
                break;
            case Edge.Down:
                downEdgeObject = floorEdgePlacedObject;
                break;
            case Edge.Left:
                leftEdgeObject = floorEdgePlacedObject;
                break;
            case Edge.Right:
                rightEdgeObject = floorEdgePlacedObject;
                break;
        }
    }

    private FloorEdgePlacedObject GetFloorEdgePlacedObject(Edge edge) {
        switch (edge) {
            default:
            case Edge.Up:
                return upEdgeObject;
            case Edge.Down:
                return downEdgeObject;
            case Edge.Left:
                return leftEdgeObject;
            case Edge.Right:
                return rightEdgeObject;
        }
    }


    public override void DestroySelf() {
        if (upEdgeObject != null) Destroy(upEdgeObject.gameObject);
        if (downEdgeObject != null) Destroy(downEdgeObject.gameObject);
        if (leftEdgeObject != null) Destroy(leftEdgeObject.gameObject);
        if (rightEdgeObject != null) Destroy(rightEdgeObject.gameObject);

        base.DestroySelf();
    }

    public string Save() {
        return JsonUtility.ToJson(new FloorSaveObject {
            upEdgeObjectName    = upEdgeObject != null    ? upEdgeObject.GetFloorEdgeObjectTypeSOName()    : "",
            downEdgeObjectName  = downEdgeObject != null  ? downEdgeObject.GetFloorEdgeObjectTypeSOName()  : "",
            leftEdgeObjectName  = leftEdgeObject != null  ? leftEdgeObject.GetFloorEdgeObjectTypeSOName()  : "",
            rightEdgeObjectName = rightEdgeObject != null ? rightEdgeObject.GetFloorEdgeObjectTypeSOName() : "",
        });
    }

    public void Load(string saveString) {
        FloorSaveObject floorSaveObject = JsonUtility.FromJson<FloorSaveObject>(saveString);

        if (floorSaveObject.upEdgeObjectName != "")
            PlaceEdge(Edge.Up, HouseBuildingSystemAssets.Instance.GetFloorEdgeObjectTypeSOFromName(floorSaveObject.upEdgeObjectName));
        if (floorSaveObject.downEdgeObjectName != "")
            PlaceEdge(Edge.Down, HouseBuildingSystemAssets.Instance.GetFloorEdgeObjectTypeSOFromName(floorSaveObject.downEdgeObjectName));
        if (floorSaveObject.leftEdgeObjectName != "")
            PlaceEdge(Edge.Left, HouseBuildingSystemAssets.Instance.GetFloorEdgeObjectTypeSOFromName(floorSaveObject.leftEdgeObjectName));
        if (floorSaveObject.rightEdgeObjectName != "")
            PlaceEdge(Edge.Right, HouseBuildingSystemAssets.Instance.GetFloorEdgeObjectTypeSOFromName(floorSaveObject.rightEdgeObjectName));
    }

    [System.Serializable]
    public class FloorSaveObject {

        public string upEdgeObjectName;
        public string downEdgeObjectName;
        public string leftEdgeObjectName;
        public string rightEdgeObjectName;

    }

}
