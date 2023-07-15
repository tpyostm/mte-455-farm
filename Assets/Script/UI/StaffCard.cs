using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaffCard : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private Image profilePic;
    [SerializeField] private TMP_Text candidateName;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private TMP_Text wage;

    [SerializeField] private Button HireButton;
    [SerializeField] private Button FireButton;

    public void UpdateID(int i)
    {
        id = i;
        UpdateIDText(id.ToString());
    }

    private void UpdateIDText(string s)
    {
        idText.text = "ID: " + s;
    }

    public void UpdateProfilePic(Sprite s)
    {
        profilePic.sprite = s;
    }

    public void UpdateWage(int n)
    {
        wage.text = n.ToString();
    }

    public void UpdateProfileName(string s)
    {
        candidateName.text = s;
    }

   public void Hire()
    {
        bool hired = Office.instance.ToHireStaff(LaborMarket.instance.LaborInMarket[id]);

        if (hired)
            gameObject.SetActive(false);
    }
}
