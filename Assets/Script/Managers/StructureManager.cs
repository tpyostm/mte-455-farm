using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StructureManager : MonoBehaviour
{
    [SerializeField] private bool isConstructing;
    [SerializeField] private bool isDemolishing;

    [SerializeField] private GameObject curBuildingPrefab;
    [SerializeField] private GameObject buildingParent;

    [SerializeField] private Vector3 curCursorPos;

    public GameObject buildingCursor;
    public GameObject gridPlane;
    public GameObject demolishCursor;

    private GameObject ghostBuilding;

    [SerializeField] private GameObject _curStructure; //Currently selected structure
    public GameObject CurStructure { get { return _curStructure; } set { _curStructure = value; } }

    [SerializeField] private GameObject[] structurePrefab;

    private Camera cam;

    //Road
    [Header("Road")]
    [SerializeField] private GameObject roadGreenTilePrefab;
    [SerializeField] private GameObject roadGreenTileParent;
    [SerializeField] private Vector3 startRoadPos;
    [SerializeField] private Vector3 endRoadPos;
    [SerializeField] private List<GameObject> roadGreenTilesList = new List<GameObject>(); //list contains green tiles

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            CancelStructureMode();

        curCursorPos = Formula.instance.GetCurTilePosition();

        if (isConstructing) //Mode Construct
        {
            buildingCursor.transform.position = curCursorPos;
            gridPlane.SetActive(true);
        }
        else if (isDemolishing) //Mode Demolish
        {
            demolishCursor.transform.position = curCursorPos;
            gridPlane.SetActive(true);
        }
        else //Mode Play
        {
            gridPlane.SetActive(false);
        }
        CheckLeftClick();

        CheckRoadMode();

    }

    public void BeginNewBuildingPlacement(GameObject prefab)
    {
        if (CheckMoney(prefab) == false)
            return;

        isDemolishing = false;
        isConstructing = true;

        curBuildingPrefab = prefab;

        //Instantiage Ghost Building
        ghostBuilding = Instantiate(curBuildingPrefab, curCursorPos, Quaternion.identity);
        ghostBuilding.GetComponent<FindBuildingSite>().Plane.SetActive(true);

        buildingCursor = ghostBuilding;
        buildingCursor.SetActive(true);
    }
    private void PlaceBuilding()
    {
        if (buildingCursor.GetComponent<FindBuildingSite>().CanBuild == false)
            return;

        GameObject structureObj = Instantiate(curBuildingPrefab,
                                               curCursorPos,
                                               Quaternion.identity,
                                               buildingParent.transform);

        Structure s = structureObj.GetComponent<Structure>();

        //Add building in Office
        Office.instance.AddBuilding(s);
        //Deduct Money
        DeductMoney(s.CostToBuild);
        //Cacel it no Money
        if (CheckMoney(structureObj) == false)
            CancelStructureMode();
    }

    private void CheckLeftClick()
    {
        if (buildingCursor != null && buildingCursor.tag == "Road")
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isConstructing)
                PlaceBuilding(); //Real Construction
            else if (isDemolishing)
                Demolish();
            else
                CheckOpenPanel(); //Normal Mode
        }
    }
    private void CancelStructureMode()
    {
        isConstructing = false;

        if (buildingCursor != null)
            buildingCursor.SetActive(false);

        if (ghostBuilding != null)
            Destroy(ghostBuilding);
    }
    private bool CheckMoney(GameObject obj)
    {
        int cost = obj.GetComponent<Structure>().CostToBuild;

        if (cost <= Office.instance.Money)
            return true;
        else
            return false;
    }

    private void DeductMoney(int cost)
    {
        Office.instance.Money -= cost;
        MainUI.instance.UpdateResourceUI();
    }

    public void OpenFarmPanel()
    {
        string name = CurStructure.GetComponent<Farm>().StructureName;

        MainUI.instance.FarmNameText.text = name;
        MainUI.instance.ToggleFarmPanel();
    }

    private void CheckOpenPanel()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //if we left click something
        if (Physics.Raycast(ray, out hit, 1000))
        {
            //Mouse over UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            CurStructure = hit.collider.gameObject;

            switch (hit.collider.tag)
            {
                case "Farm": // if we click Object with Farm tag 
                    OpenFarmPanel();
                    break;
            }
        }
    }

    public void CallStaff()
    {
        Office.instance.SendStaff(CurStructure);
        MainUI.instance.UpdateResourceUI();
    }
    private void Demolish()
    {
        Structure s = Office.instance.Structures.Find(x => x.transform.position == curCursorPos);

        if (s != null)
        {
            Office.instance.RemoveBuilding(s);
        }

        MainUI.instance.UpdateResourceUI();
    }
    public void ToggleDemolish() //Map with Demolish Btn
    {
        isConstructing = false;
        isDemolishing = !isDemolishing;

        gridPlane.SetActive(isDemolishing);
        demolishCursor.SetActive(isDemolishing);
    }

    private void GenGreenTile(Vector3 pos)
    {
        GameObject greenTileObj = Instantiate(roadGreenTilePrefab,
                                                pos,
                                                Quaternion.identity,
                                                roadGreenTileParent.transform);

        roadGreenTilesList.Add(greenTileObj);

    }

    private void ClearGreenTileList()
    {
        foreach (GameObject tileObj in roadGreenTilesList)
            Destroy(tileObj);

        roadGreenTilesList.Clear();
    }

    private List<Vector3> FindNewPath(Vector3 startPos, Vector3 endPos)
    {
        List<Vector3> path = new List<Vector3>();

        int xTileNum = ((int)endRoadPos.x - (int)startRoadPos.x) / 5;
        int zTileNum = ((int)endRoadPos.z - (int)startRoadPos.z) / 5;

        int xModifier = xTileNum >= 0 ? 1 : -1;
        int zModifier = zTileNum >= 0 ? 1 : -1;

        path.Add(startPos);

        for (int i = 1; i <= Mathf.Abs(xTileNum); i++)
            path.Add(startPos + new Vector3(5f * i * xModifier, 0f, 0f));

        Vector3 corner = path.Count > 1 ? path[path.Count - 1] :

            startRoadPos;

        for (int i = 1; i <= Mathf.Abs(zTileNum); i++)
            path.Add(corner + new Vector3(0f, 0f, 5f * i * zModifier));

        return path;
    }

    private void PlanningRoad()
    {
        curCursorPos = Formula.instance.GetCurTilePosition();

        if (endRoadPos == curCursorPos)
            return;

        endRoadPos = curCursorPos;

        //Finding Path
        List<Vector3> newPath = FindNewPath(startRoadPos, endRoadPos);

        ClearGreenTileList();

        //Gen Green Tile
        foreach (Vector3 pos in newPath)
            GenGreenTile(pos);
    }

    private void ConstructRoad()
    {
        foreach (GameObject tileObj in roadGreenTilesList)
        {
            if (CheckMoney(curBuildingPrefab) == false) //Cancel if there is not enough money
                CancelStructureMode();
            else //enough money to construct road
            {
                GameObject roadObj = Instantiate(curBuildingPrefab, tileObj.transform.position,
                                                       Quaternion.identity,
                                                       buildingParent.transform);

                Structure s = roadObj.GetComponent<Structure>();

                //Add building in Office
                Office.instance.AddBuilding(s);
                //Deduct Money
                DeductMoney(s.CostToBuild);
            }
        }
        ClearGreenTileList();
        CancelStructureMode();
    }

    private void CheckRoadMode()
    {
        if (buildingCursor == null || buildingCursor.tag != "Road")
            return;

        if (buildingCursor.GetComponent<FindBuildingSite>().CanBuild == false)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0)) //Start left click for road
        {
            isConstructing = true;
            startRoadPos = curCursorPos;

            roadGreenTilesList.Clear();

            GameObject greenTileObj = Instantiate(roadGreenTilePrefab,
                                                    startRoadPos,
                                                    Quaternion.identity,
                                                    roadGreenTileParent.transform);
            roadGreenTilesList.Add(greenTileObj);
        }

        if (Input.GetMouseButton(0)) //keep holding left click
            PlanningRoad();

        if (Input.GetMouseButtonUp(0)) //release left mouse
            ConstructRoad();
    }
}


