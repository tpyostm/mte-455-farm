using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaborMarket : MonoBehaviour
{
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject workerParent;

    [SerializeField] private GameObject staffCardPrefab;
    [SerializeField] private GameObject staffCardParent;

    [SerializeField] private List<GameObject> laborInMarket;
    public List<GameObject> LaborInMarket { get { return laborInMarket; } }
    [SerializeField] private List<GameObject> laborCardInMarket;

    [SerializeField] private int maxStaffInMarket = 20;

    private string[] maleName = { "Ben",  "John", "Sam", "Frank",
                                "Joey", "Brandon", "Dan", "Peter",
                                "Gary", "George", "Donald", "Richard",
                                "Edward", "Brian", "Jason", "Anthony",
                                "Cody", "Darren", "Adam", "Bruce"};

    private string[] femaleName = { "Ann", "Mary", "Jane", "Clara",
                                    "Tara", "Christina", "Angie", "Laurel",
                                    "Lara", "Penny", "Rosa", "Anya",
                                    "Ramona", "Katelyn", "Tiana", "Sunny",
                                    "Lillie", "Greta", "Kelly", "Hana"};

    public static LaborMarket instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GenerateCandidate();
    }

    public void GenerateCandidate()
    {
        for (int i = 0; i < maxStaffInMarket; i++)
        {
            GameObject staffObj = Instantiate(workerPrefab, workerParent.transform);

            Worker w = staffObj.GetComponent<Worker>();

            w.ID = i;
            w.InitiateCharID(Random.Range(0, 5));
            w.SetGender();

            w.StaffName = SetName(w);
            w.DailyWage = Random.Range(80, 120);

            laborInMarket.Add(staffObj);

            GameObject cardObj = InitializeLaborCard(w);

            laborCardInMarket.Add(cardObj);
        }
    }

    private string SetName(Worker w)
    {
        int r = Random.Range(0, 20);
        string name;

        if (w.StaffGender == Gender.male)
            name = maleName[r];
        else
            name = femaleName[r];

        return name;
    }

    private GameObject InitializeLaborCard(Worker w)
    {
        GameObject staffCardObj = Instantiate(staffCardPrefab, staffCardParent.transform);

        StaffCard card = staffCardObj.GetComponent<StaffCard>();
        card.UpdateID(w.ID);
        card.UpdateProfilePic(w.charFacePic[w.CharFaceID]);
        card.UpdateProfileName(w.StaffName);
        card.UpdateWage(w.DailyWage);

        return staffCardObj;
    }
}
