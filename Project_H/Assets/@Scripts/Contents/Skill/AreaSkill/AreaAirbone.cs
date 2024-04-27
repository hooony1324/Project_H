using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAirbone : AreaSkill
{
    public override void SetInfo(Creature owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 360;

        if (_indicator != null)
            _indicator.SetInfo(owner, SkillData, Define.EIndicatorType.Cone);
    }
}
