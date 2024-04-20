using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : InitBase
{
    private BaseObject _target;

    public BaseObject Target
    {
        get { return _target; }
        set { _target = value; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Camera.main.orthographicSize = 15.0f;

        return true;
    }

    void LateUpdate()
    {
        if (Target == null)
            return;

        Vector3 targetPosition = new Vector3(Target.CentorPosition.x, Target.CentorPosition.y, -10f);
        transform.position = targetPosition;
    }
}
