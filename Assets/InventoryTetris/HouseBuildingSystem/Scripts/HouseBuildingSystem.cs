using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class HouseBuildingSystem : MonoBehaviour {

    public static HouseBuildingSystem Instance { get; private set; }

    public event EventHandler OnActiveGridLevelChanged;
    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    public enum PlaceObjectType {
        GridObject,
        EdgeObject,
        LooseObject,
    }

    [SerializeField] private LayerMask placedObjectEdgeColliderLayerMask;

    [SerializeField] private List<FloorEdgeObjectTypeSO> floorEdgeObjectTypeSOList = null;
    private FloorEdgeObjectTypeSO floorEdgeObjectTypeSO;

    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private LooseObjectSO looseObjectSO;
    private float looseObjectEulerY;
    private List<Transform> looseObjectTransformList;

    private List<GridXZ<GridObject>> gridList;
    private GridXZ<GridObject> selectedGrid;

    private PlaceObjectType placeObjectType;
    private PlacedObjectTypeSO.Dir dir;

    private bool isDemolishActive;

    private void Awake() {
        Instance = this;

        Application.targetFrameRate = 100;

        int gridWidth = 50;
        int gridHeight = 50;
        float cellSize = 1f;

        gridList = new List<GridXZ<GridObject>>();
        int gridVerticalCount = 4;
        float gridVerticalSize = 2.5f;
        for (int i = 0; i < gridVerticalCount; i++) {
            GridXZ<GridObject> grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, gridVerticalSize * i, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));
            gridList.Add(grid);
        }

        selectedGrid = gridList[0];

        placedObjectTypeSO = null;

        looseObjectTransformList = new List<Transform>();
    }

    public class GridObject {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject placedObject;

        public GridObject(GridXZ<GridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString() {
            return x + ", " + y + "\n" + placedObject;
        }

        public void TriggerGridObjectChanged() {
            grid.TriggerGridObjectChanged(x, y);
        }

        public void SetPlacedObject(PlacedObject placedObject) {
            this.placedObject = placedObject;
            TriggerGridObjectChanged();
        }

        public void ClearPlacedObject() {
            placedObject = null;
            TriggerGridObjectChanged();
        }

        public PlacedObject GetPlacedObject() {
            return placedObject;
        }

        public bool CanBuild() {
            return placedObject == null;
        }

    }

    private void Update() {
        HandleGridSelect();
        HandleTypeSelect();
        HandleNormalObjectPlacement();
        HandleEdgeObjectPlacement();
        HandleLooseObjectPlacement();
        HandleDirRotation();
        HandleDemolish();

        if (Input.GetMouseButtonDown(1)) {
            DeselectObjectType();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            Load();
        }
    }

    private void HandleNormalObjectPlacement() {
        if (placeObjectType == PlaceObjectType.GridObject) {
            if (Input.GetMouseButton(0) && placedObjectTypeSO != null && !UtilsClass.IsPointerOverUI()) {
                Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                selectedGrid.GetXZ(mousePosition, out int x, out int z);

                Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                if (TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject)) {
                    // Object placed
                } else {
                    // Error!
                    //UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
                }
            }
        }
    }

    private void HandleEdgeObjectPlacement() {
        if (placeObjectType == PlaceObjectType.EdgeObject) {
            if (!UtilsClass.IsPointerOverUI()) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, placedObjectEdgeColliderLayerMask)) {
                    // Raycast Hit Edge Object
                    if (raycastHit.collider.TryGetComponent(out FloorEdgePosition floorEdgePosition)) {
                        if (raycastHit.collider.transform.parent.TryGetComponent(out FloorPlacedObject floorPlacedObject)) {
                            // Found parent FloorPlacedObject
                            if (Input.GetMouseButtonDown(0) && floorEdgeObjectTypeSO != null) {
                                // Place Object on Edge
                                floorPlacedObject.PlaceEdge(floorEdgePosition.edge, floorEdgeObjectTypeSO);
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandleLooseObjectPlacement() {
        if (placeObjectType == PlaceObjectType.LooseObject && looseObjectSO != null) {
            if (!UtilsClass.IsPointerOverUI()) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
                    // Raycast Hit something
                    if (Input.GetMouseButtonDown(0)) {
                        Transform looseObjectTransform = Instantiate(looseObjectSO.prefab, raycastHit.point, Quaternion.Euler(0, looseObjectEulerY, 0));
                        looseObjectTransformList.Add(looseObjectTransform);
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.R)) {
            looseObjectEulerY += Time.deltaTime * 90f;
        }
    }

    private void HandleTypeSelect() {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            placeObjectType = PlaceObjectType.GridObject;
            placedObjectTypeSO = placedObjectTypeSOList[0]; 
            RefreshSelectedObjectType();
        }*/

        if (Input.GetKeyDown(KeyCode.Alpha0)) { SetDemolishActive(); }
    }

    private void HandleDirRotation() {
        if (Input.GetKeyDown(KeyCode.R)) {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }
    }

    private void HandleGridSelect() {
        if (Input.GetKeyDown(KeyCode.F)) {
            int nextSelectedGridIndex = (gridList.IndexOf(selectedGrid) + 1) % gridList.Count;
            selectedGrid = gridList[nextSelectedGridIndex];
            OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HandleDemolish() {
        if (isDemolishActive && Input.GetMouseButtonDown(0) && !UtilsClass.IsPointerOverUI()) {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            PlacedObject placedObject = selectedGrid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null) {
                // Demolish
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in gridPositionList) {
                    selectedGrid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }

    public void SelectPlacedObjectTypeSO(PlacedObjectTypeSO placedObjectTypeSO) {
        placeObjectType = PlaceObjectType.GridObject;
        this.placedObjectTypeSO = placedObjectTypeSO;
        RefreshSelectedObjectType();
    }

    public void SelectFloorEdgeObjectTypeSO(FloorEdgeObjectTypeSO floorEdgeObjectTypeSO) {
        placeObjectType = PlaceObjectType.EdgeObject;
        this.floorEdgeObjectTypeSO = floorEdgeObjectTypeSO;
        RefreshSelectedObjectType();
    }

    public void SelectLooseObjectTypeSO(LooseObjectSO looseObjectSO) {
        placeObjectType = PlaceObjectType.LooseObject;
        this.looseObjectSO = looseObjectSO;
        RefreshSelectedObjectType();
    }

    private void DeselectObjectType() {
        placedObjectTypeSO = null;
        floorEdgeObjectTypeSO = null;
        looseObjectSO = null;

        isDemolishActive = false;

        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        //UpdateCanBuildTilemap();

        if (placedObjectTypeSO == null) {
            //TilemapVisual.Instance.Hide();
        } else {
            //TilemapVisual.Instance.Show();
        }

        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateCanBuildTilemap() {
        /*
        // Not implemented by default
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                // Tilemap
                Tilemap.Instance.SetTilemapSprite(new Vector3(x, y),
                    grid.GetGridObject(x, y).CanBuild() ?
                    Tilemap.TilemapObject.TilemapSprite.CanBuild :
                    Tilemap.TilemapObject.TilemapSprite.CannotBuild);
            }
        }*/
    }

    public bool TryPlaceObject(int x, int y, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir) {
        return TryPlaceObject(new Vector2Int(x, y), placedObjectTypeSO, dir, out PlacedObject placedObject);
    }

    public bool TryPlaceObject(Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir) {
        return TryPlaceObject(placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);
    }

    public bool TryPlaceObject(Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir, out PlacedObject placedObject) {
        return TryPlaceObject(selectedGrid, placedObjectOrigin, placedObjectTypeSO, dir, out placedObject);
    }

    public bool TryPlaceObject(GridXZ<GridObject> grid, Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir) {
        return TryPlaceObject(grid, placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);
    }

    public bool TryPlaceObject(GridXZ<GridObject> grid, Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir, out PlacedObject placedObject) {
        // Test Can Build
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList) {
            //bool isValidPosition = grid.IsValidGridPositionWithPadding(gridPosition);
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
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);

            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            placedObject.GridSetupDone();

            OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);

            return true;
        } else {
            // Cannot build here
            placedObject = null;
            return false;
        }
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        selectedGrid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition) {
        return selectedGrid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }

    public GridObject GetGridObject(Vector2Int gridPosition) {
        return selectedGrid.GetGridObject(gridPosition.x, gridPosition.y);
    }

    public GridObject GetGridObject(Vector3 worldPosition) {
        return selectedGrid.GetGridObject(worldPosition);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        return selectedGrid.IsValidGridPosition(gridPosition);
    }

    public FloorEdgePosition GetMouseFloorEdgePosition() {
        if (!UtilsClass.IsPointerOverUI()) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, placedObjectEdgeColliderLayerMask)) {
                // Raycast Hit Edge Object
                if (raycastHit.collider.TryGetComponent(out FloorEdgePosition floorEdgePosition)) {
                    return floorEdgePosition;
                }
            }
        }

        return null;
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        selectedGrid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placedObjectTypeSO != null) {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        } else {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }

    public void SetSelectedPlacedObject(PlacedObjectTypeSO placedObjectTypeSO) {
        this.placedObjectTypeSO = placedObjectTypeSO;
        isDemolishActive = false;
        RefreshSelectedObjectType();
    }

    public FloorEdgeObjectTypeSO GetFloorEdgeObjectTypeSO() {
        return floorEdgeObjectTypeSO;
    }

    public LooseObjectSO GetLooseObjectSO() {
        return looseObjectSO;
    }

    public float GetLooseObjectEulerY() {
        return looseObjectEulerY;
    }

    public void SetDemolishActive() {
        placedObjectTypeSO = null;
        isDemolishActive = true;
        RefreshSelectedObjectType();
    }

    public bool IsDemolishActive() {
        return isDemolishActive;
    }

    public PlaceObjectType GetPlaceObjectType() {
        return placeObjectType;
    }

    public int GetActiveGridLevel() {
        return gridList.IndexOf(selectedGrid);
    }





    private void Save() {
        List<PlacedObjectSaveObjectArray> placedObjectSaveObjectArrayList = new List<PlacedObjectSaveObjectArray>();

        foreach (GridXZ<GridObject> grid in gridList) {
            List<PlacedObject.SaveObject> saveObjectList = new List<PlacedObject.SaveObject>();
            List<PlacedObject> savedPlacedObjectList = new List<PlacedObject>();

            for (int x = 0; x < grid.GetWidth(); x++) {
                for (int y = 0; y < grid.GetHeight(); y++) {
                    PlacedObject placedObject = grid.GetGridObject(x, y).GetPlacedObject();
                    if (placedObject != null && !savedPlacedObjectList.Contains(placedObject)) {
                        // Save object
                        savedPlacedObjectList.Add(placedObject);
                        saveObjectList.Add(placedObject.GetSaveObject());
                    }
                }
            }

            PlacedObjectSaveObjectArray placedObjectSaveObjectArray = new PlacedObjectSaveObjectArray { placedObjectSaveObjectArray = saveObjectList.ToArray() };
            placedObjectSaveObjectArrayList.Add(placedObjectSaveObjectArray);
        }

        List<LooseSaveObject> looseSaveObjectList = new List<LooseSaveObject>();
        foreach (Transform looseObjectTransform in looseObjectTransformList) {
            if (looseObjectTransform == null) continue;
            looseSaveObjectList.Add(new LooseSaveObject {
                looseObjectSOName = looseObjectTransform.GetComponent<LooseObjectSOHolder>().looseObjectSO.name,
                position = looseObjectTransform.position,
                quaternion = looseObjectTransform.rotation
            });
        }

        SaveObject saveObject = new SaveObject {
            placedObjectSaveObjectArrayArray = placedObjectSaveObjectArrayList.ToArray(),
            looseSaveObjectArray = looseSaveObjectList.ToArray(),
        };

        string json = JsonUtility.ToJson(saveObject);

        PlayerPrefs.SetString("HouseBuildingSystemSave", json);
        SaveSystem.Save("HouseBuildingSystemSave", json, false);

        Debug.Log("Saved!");
    }

    private void Load() {
        if (PlayerPrefs.HasKey("HouseBuildingSystemSave")) {
            string json = PlayerPrefs.GetString("HouseBuildingSystemSave");
            json = SaveSystem.Load("HouseBuildingSystemSave_15");

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

            for (int i=0; i<gridList.Count; i++) {
                GridXZ<GridObject> grid = gridList[i];

                foreach (PlacedObject.SaveObject placedObjectSaveObject in saveObject.placedObjectSaveObjectArrayArray[i].placedObjectSaveObjectArray) {
                    PlacedObjectTypeSO placedObjectTypeSO = HouseBuildingSystemAssets.Instance.GetPlacedObjectTypeSOFromName(placedObjectSaveObject.placedObjectTypeSOName);
                    TryPlaceObject(grid, placedObjectSaveObject.origin, placedObjectTypeSO, placedObjectSaveObject.dir, out PlacedObject placedObject);
                    if (placedObject is FloorPlacedObject) {
                        FloorPlacedObject floorPlacedObject = (FloorPlacedObject)placedObject;
                        floorPlacedObject.Load(placedObjectSaveObject.floorPlacedObjectSave);
                    }
                }
            }

            foreach (LooseSaveObject looseSaveObject in saveObject.looseSaveObjectArray) {
                Transform looseObjectTransform = Instantiate(
                    HouseBuildingSystemAssets.Instance.GetLooseObjectSOFromName(looseSaveObject.looseObjectSOName).prefab,
                    looseSaveObject.position,
                    looseSaveObject.quaternion
                );
                looseObjectTransformList.Add(looseObjectTransform);
            }
        }

        Debug.Log("Load!");
    }


    [Serializable]
    public class SaveObject {

        public PlacedObjectSaveObjectArray[] placedObjectSaveObjectArrayArray;
        public LooseSaveObject[] looseSaveObjectArray;

    }

    [Serializable]
    public class PlacedObjectSaveObjectArray {

        public PlacedObject.SaveObject[] placedObjectSaveObjectArray;

    }

    [Serializable]
    public class LooseSaveObject {

        public string looseObjectSOName;
        public Vector3 position;
        public Quaternion quaternion;

    }

}
