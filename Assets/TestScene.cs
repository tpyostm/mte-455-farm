using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    public Worker worker;

    public void WorkerIdle()
    {
        worker.State = UnitState.Idle;
    }

    public void WorkerWalk()
    {
        worker.State = UnitState.Walk;
    }

    public void WorkerPlow()
    {
        worker.State = UnitState.Plow;
    }

    public void WorkerSow()
    {
        worker.State = UnitState.Sow;
    }

    public void WorkerWater()
    {
        worker.State = UnitState.Water;
    }

    public void WorkerHarvest()
    {
        worker.State = UnitState.Harvest;
    }
}
