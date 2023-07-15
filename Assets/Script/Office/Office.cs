using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Office : MonoBehaviour
{
    [SerializeField] private int money;
    public int Money { get { return money; } set { money = value; } }

    [SerializeField] private List<Worker> workers = new List<Worker>();
    public List<Worker> Workers { get { return workers; } }

    [SerializeField] private int wheat;
    public int Wheat { get { return wheat; } set { wheat = value; } }
    [SerializeField] private int melon;
    public int Melon { get { return melon; } set { melon = value; } }
    [SerializeField] private int corn;
    public int Corn { get { return corn; } set { corn = value; } }
    [SerializeField] private int milk;
    public int Milk { get { return milk; } set { milk = value; } }
    [SerializeField] private int apple;

    public int Apple { get { return apple; } set { apple = value; } }

    [SerializeField] private int dailyCostWages;

    [SerializeField] private List<Structure> structures = new List<Structure>();
    public List<Structure> Structures { get { return structures; } }


    [SerializeField] private int availStaff;
    public int AvailStaff { get { return availStaff; } set { availStaff = value; } }

    [SerializeField] private GameObject staffParent;

    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private GameObject rallyPosition;

    public static Office instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddBuilding(Structure s)
    {
        structures.Add(s);
    }

    public void RemoveBuilding(Structure s)
    {
        structures.Remove(s);
        Destroy(s.gameObject);
    }

    public bool ToHireStaff(GameObject workerObj)
    {
        if (money <= 0)
            return false;

        workerObj.transform.parent = staffParent.transform;

        Worker w = workerObj.GetComponent<Worker>();

        w.Hired = true; //Hire this worker
        w.ChangeCharSkin(); //Show 3D model
        w.SetToWalk(rallyPosition.transform.position);

        money -= w.DailyWage;
        AddStaff(w);

        //Update UI
        MainUI.instance.UpdateResourceUI();

        return true;
    }

    public void AddStaff(Worker w)
    {
        workers.Add(w);
        dailyCostWages += w.DailyWage;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
