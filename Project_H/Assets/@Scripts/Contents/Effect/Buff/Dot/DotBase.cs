using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBase : EffectBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    protected override void ProcessDot()
    {
        Owner.HandleDotDamage(this);
    }
}
