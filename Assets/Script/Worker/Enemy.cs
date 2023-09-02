using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Unit
{
    [SerializeField] private LayerMask buildingLayerMask;
    [SerializeField] private LayerMask unitLayerMask;

    [SerializeField] float checkForEnemyRate = 1f;

    // Start is called before the first frame update
    void Start()
    {
        buildingLayerMask = LayerMask.GetMask("Building");
        unitLayerMask = LayerMask.GetMask("Unit");

        InvokeRepeating("CheckForAttack", 0f, checkForEnemyRate);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != targetStructure)
            return;

        Structure s = other.gameObject.GetComponent<Structure>();
        if ((s != null) && (s.HP > 0))
            state = UnitState.AttackBuilding;
    }

    // checks for nearest enemy building with a sphere cast

    protected Building CheckForNearestEnemyBuilding()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position,
                                                    detectRange,
                                                    Vector3.up,
                                                    buildingLayerMask);

        GameObject closest = null;
        float closestDist = 0f;

        for (int x = 0; x < hits.Length; x++)
        {
            //Debug.Log("Test - " + hits[x].collider.gameObject.ToString());
            Building target = hits[x].collider.GetComponent<Building>();
            float dist = Vector3.Distance(transform.position, hits[x].transform.position);

            // skip if this is not a building
            if (target == null)
                continue;

            // skip if it is any destroyed building
            if (target.HP <= 0)
                continue;
            // if it is not the closest enemy or the distance is less than the closest distance it currently has
            else if (!closest || (dist < closestDist))
            {
                closest = hits[x].collider.gameObject;
                closestDist = dist;
            }
        }

        if (closest != null)
        {
            //Debug.Log(closest.gameObject.ToString() + ", " + closestDist.ToString());
            return closest.GetComponent<Building>();
        }
        else
            return null;

    }

    private void CheckForAttack()
    {
        Building enemyBuilding = FindingTarget.CheckForNearestEnemyBuilding(transform.position,
                                                                            detectRange,
                                                                            buildingLayerMask,
                                                                            "Building");
        Unit enemyUnit = FindingTarget.CheckForNearestEnemyUnit(transform.position, 
                                                  detectRange,
                                                  buildingLayerMask,
                                                  "Unit");

        if (enemyBuilding != null)
        {
            targetStructure = enemyBuilding.gameObject;
            state = UnitState.MoveToAttackBuilding;
        }
        else
        {
            targetStructure = null;
            state = UnitState.Idle;   

        if (enemyUnit != null)
        {
            targetUnit = enemyUnit.gameObject;
            state = UnitState.MoveToAttackUnit;
        }
        else
        {
            targetUnit = null;
            state = UnitState.Idle;            
            }
        }
    }
    protected Unit CheckForNearestEnemyUnit()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position,
                                                    detectRange,
                                                    Vector3.up,
                                                    unitLayerMask);

        GameObject closest = null;
        float closestDist = 0f;

        for (int x = 0; x < hits.Length; x++)
        {
            // skip if this is not a player's unit
            if (hits[x].collider.tag != "Unit")
                continue;

            Unit target = hits[x].collider.GetComponent<Unit>();
            float dist = Vector3.Distance(transform.position, hits[x].transform.position);

            // skip if this is not a unit
            if (target == null)
                continue;
            // skip if it is any dead unit
            if (target.HP <= 0)
                continue;
            // if the closest is null or the distance is less than the closest distance it currently has
            else if ((closest == null) || (dist < closestDist))
            {
                closest = hits[x].collider.gameObject;
                closestDist = dist;
            }
        }

        if (closest != null)
            return closest.GetComponent<Unit>();
        else
            return null;
    }

}

