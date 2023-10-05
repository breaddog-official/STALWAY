/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class InventoryTetris : MonoBehaviour {

    public static InventoryTetris Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;

    private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;
    private Grid<GridObject> grid;
    private RectTransform canvasRectTransform;
    private RectTransform itemContainer;

    private enum State {
        Normal,
        Dragging,
    }

    private State state;
    private PlacedObject dragPlacedObject;


    private void Awake() {
        Instance = this;

        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 50f;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        placedObjectTypeSO = null;

        if (canvas == null) {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas != null) {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();

        state = State.Normal;
    }

    private void Start() {
        TryPlaceItem(placedObjectTypeSOList[0] as ItemTetrisSO, new Vector2Int(0, 0), PlacedObjectTypeSO.Dir.Down);
    }

    public class GridObject {

        private Grid<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject placedObject;

        public GridObject(Grid<GridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString() {
            return x + ", " + y + "\n" + placedObject;
        }

        public void SetPlacedObject(PlacedObject placedObject) {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject() {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject GetPlacedObject() {
            return placedObject;
        }

        public bool CanBuild() {
            return placedObject == null;
        }

    }

    private void Update() {
        return;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 testAnchoredPosition);
        //testTransform.anchoredPosition = testAnchoredPosition;

        //grid.GetXY(anchoredPosition, out int anchoredX, out int anchoredY);
        //Debug.Log(anchoredPosition + ": " + anchoredX + ", " + anchoredY);


        switch (state) {
            case State.Normal:
                // Dragging
                if (placedObjectTypeSO == null) {
                    if (Input.GetMouseButtonDown(0)) {
                        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

                        PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                        if (placedObject != null) {
                            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                            foreach (Vector2Int gridPosition in gridPositionList) {
                                //grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                            }

                            dragPlacedObject = placedObject;
                            state = State.Dragging;
                            Debug.Log("State: Dragging " + dragPlacedObject);
                        }

                    }
                }

                // Try to place
                if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null) {
                    //Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
                    //grid.GetXY(mousePosition, out int x, out int y);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);
                    grid.GetXY(anchoredPosition, out int x, out int y);

                    Vector2Int placedObjectOrigin = new Vector2Int(x, y);

                    // Test Can Build
                    List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
                    bool canBuild = true;
                    foreach (Vector2Int gridPosition in gridPositionList) {
                        bool isValidPosition = grid.IsValidGridPosition(gridPosition);
                        if (!isValidPosition) {
                            // Not valid
                            canBuild = false;
                            break;
                        }
                        if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                            canBuild = false;
                            break;
                        }
                    }

                    if (canBuild) {
                        foreach (Vector2Int gridPosition in gridPositionList) {
                            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                                canBuild = false;
                                break;
                            }
                        }
                    }

                    if (canBuild) {
                        Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

                        PlacedObject placedObject = PlacedObject.CreateCanvas(itemContainer, placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);
                        placedObject.transform.rotation = Quaternion.Euler(0, 0, -placedObjectTypeSO.GetRotationAngle(dir));

                        foreach (Vector2Int gridPosition in gridPositionList) {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                        }

                        OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                    } else {
                        // Cannot build here
                        TooltipCanvas.ShowTooltip_Static("Cannot Build Here!");
                        FunctionTimer.Create(() => { TooltipCanvas.HideTooltip_Static(); }, 2f, "HideTooltip", true, true);
                    }
                }

                if (Input.GetKeyDown(KeyCode.R)) {
                    dir = PlacedObjectTypeSO.GetNextDir(dir);
                }

                if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
                if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
                if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
                if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
                if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
                if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }

                if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }

                // Demolish
                /*
                if (Input.GetMouseButtonDown(1)) {
                    Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
                    PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                    if (placedObject != null) {
                        // Demolish
                        placedObject.DestroySelf();

                        List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                        foreach (Vector2Int gridPosition in gridPositionList) {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                        }
                    }
                }
                /
                break;
            case State.Dragging:
                if (Input.GetMouseButtonDown(0)) {
                    state = State.Normal;
                    Debug.Log("State: Normal");

                    // Validate if can drop item in new position

                    Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
                    grid.GetXY(mousePosition, out int x, out int y);

                    Vector2Int placedObjectOrigin = new Vector2Int(x, y);
                    List<Vector2Int> gridPositionList = dragPlacedObject.GetPlacedObjectTypeSO().GetGridPositionList(placedObjectOrigin, dir);

                    bool canBuild = true;
                    foreach (Vector2Int gridPosition in gridPositionList) {
                        bool isValidPosition = grid.IsValidGridPosition(gridPosition);
                        if (!isValidPosition) {
                            // Not valid
                            canBuild = false;
                            break;
                        }
                        if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                            canBuild = false;
                            break;
                        }
                    }

                    if (canBuild) {
                        foreach (Vector2Int gridPosition in gridPositionList) {
                            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                                canBuild = false;
                                break;
                            }
                        }
                    }

                    if (canBuild) {
                        // Clear item from previous position
                        gridPositionList = dragPlacedObject.GetGridPositionList();
                        foreach (Vector2Int gridPosition in gridPositionList) {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                        }

                        Vector3 worldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y);
                        dragPlacedObject.SetOrigin(placedObjectOrigin);
                        dragPlacedObject.transform.position = worldPosition;
                        dragPlacedObject.transform.rotation = Quaternion.Euler(0, 0, -dragPlacedObject.GetPlacedObjectTypeSO().GetRotationAngle(dir));

                        gridPositionList = dragPlacedObject.GetPlacedObjectTypeSO().GetGridPositionList(placedObjectOrigin, dir);
                        foreach (Vector2Int gridPosition in gridPositionList) {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(dragPlacedObject);
                        }

                        OnObjectPlaced?.Invoke(this, EventArgs.Empty);

                    }
                }
                break;
        }
    }

    private void DeselectObjectType() {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }


    public Grid<GridObject> GetGrid() {
        return grid;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector2 GetCanvasSnappedPosition() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);
        grid.GetXY(anchoredPosition, out int x, out int y);

        if (placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector2 placedObjectCanvas = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
            return placedObjectCanvas;
        } else {
            return anchoredPosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placedObjectTypeSO != null) {
            return Quaternion.Euler(0, 0, -placedObjectTypeSO.GetRotationAngle(dir));
        } else {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }


    public bool TryPlaceItem(ItemTetrisSO itemTetrisSO, Vector2Int placedObjectOrigin, PlacedObjectTypeSO.Dir dir) {
        // Test Can Build
        List<Vector2Int> gridPositionList = itemTetrisSO.GetGridPositionList(placedObjectOrigin, dir);
        bool canPlace = true;
        foreach (Vector2Int gridPosition in gridPositionList) {
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition) {
                // Not valid
                canPlace = false;
                break;
            }
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                canPlace = false;
                break;
            }
        }

        if (canPlace) {
            foreach (Vector2Int gridPosition in gridPositionList) {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                    canPlace = false;
                    break;
                }
            }
        }

        if (canPlace) {
            Vector2Int rotationOffset = itemTetrisSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

            PlacedObject placedObject = PlacedObject.CreateCanvas(itemContainer, placedObjectWorldPosition, placedObjectOrigin, dir, itemTetrisSO);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -itemTetrisSO.GetRotationAngle(dir));

            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            OnObjectPlaced?.Invoke(this, EventArgs.Empty);

            // Object Placed!
            return true;
        } else {
            // Object CANNOT be placed!
            return false;
        }
    }

}

*/