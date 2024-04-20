using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Monster;

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = ECreatureState.Idle;
    }
    void Start()
    {
        _initPos = transform.position;
    }

    #region AI
    Vector3 _destPos;
    Vector3 _initPos;
    protected override void UpdateIdle()
    {
        // Patrol
        {
            int patrolPercent = 10;
            int rand = Random.Range(0, 100);
            if (rand <= patrolPercent)
            {
                _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                CreatureState = ECreatureState.Move;
            }
        }

    }
    protected override void UpdateMove()
    {
        if (Target.IsValid() == false)
        {

        }    
    }
    #endregion

}
