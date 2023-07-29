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
}