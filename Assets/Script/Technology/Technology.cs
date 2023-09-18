using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public struct TechCost
{
    public int money;
    public int stone;
    public int wood;
}

public enum TechState
{
    Locked,
    Unlocked,
    InProgress,
    Completed
}

    // Start is called before the first frame update
    [System.Serializable]
    public class Technology
    {
        [SerializeField] private int id;
        public int ID { get { return id; } }

        [SerializeField] private string techName;
        public string TechName { get { return techName; } }

        [SerializeField] private Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        private List<int> requiredTechID = new List<int>();
        public List<int> RequiredTechID { get { return requiredTechID; } }

        [SerializeField]
        private TechCost cost;
        public TechCost Cost { get { return cost; } set { cost = value; } }
        [SerializeField]
        private int daysRequired;
        public int DaysRequired { get { return daysRequired; } set { daysRequired = value; } }

        [SerializeField]
        private string description;
        public string Description { get { return description; } }

        [SerializeField]
        private TechState state;
        public TechState State { get { return state; } set { state = value; } }

        public void InitData(TechSO SO)
        {
            id = SO.id;
            techName = SO.techName;
            icon = SO.icon;
            requiredTechID.AddRange(SO.requiredTechID);
            cost = SO.cost;
            daysRequired = SO.daysRequired;
            state = TechState.Locked;
        }

        public void ReduceResearchDay()
        {
            daysRequired--;

            if (daysRequired <= 0)
                state = TechState.Completed;
        }

        public bool CheckResourceCost()
        {
            if (Office.instance.Money < cost.money)
                return false;
            if (Office.instance.Stone < cost.stone)
                return false;
            if (Office.instance.Wood < cost.wood)
                return false;
            return true;
        }

        public void CheckRequiredTech(TechManager techM)
        {
            if (requiredTechID.Count == 0)
            {
                state = TechState.Unlocked;
                return;
            }

            foreach (int i in requiredTechID)
            {
                if (techM.TechSet[i].state != TechState.Completed)
                    return;
            }
            state = TechState.Unlocked;
        }
    }

