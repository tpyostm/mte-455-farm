using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    [SerializeField] private int _curPriceWheat;
    [SerializeField] private int _curPriceMelon;

    public void SellWheat()
    {
        Office.instance.Money += Office.instance.Wheat * _curPriceWheat;
        Office.instance.Wheat = 0;

        MainUI.instance.UpdateResourceUI();
    }

    public void SellMelon()
    {
        Office.instance.Money += Office.instance.Melon * _curPriceMelon;
        Office.instance.Melon = 0;

        MainUI.instance.UpdateResourceUI();
    }

    public void SellAll()
    {
        SellWheat();
        SellMelon();
    }
}

