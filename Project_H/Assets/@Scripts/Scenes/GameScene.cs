using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        Managers.Map.LoadMap("BaseMap");
        Managers.Map.StageTransition.SetInfo();

        // -100, -66
        // 22, 10
        var cellPos = Managers.Map.World2Cell(new Vector3(-100, -66));

        HeroCamp camp = Managers.Object.Spawn<HeroCamp>(Vector3.zero, 0);
        camp.SetCellPos(cellPos, true);

        Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, HERO_KNIGHT_ID);
        Managers.Map.MoveTo(hero, cellPos, true);
        //Monster monster = Managers.Object.Spawn<Monster>(Vector3.zero, MONSTER_BEAR_ID);

        Managers.UI.ShowBaseUI<UI_Joystick>();

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = camp;

        return true;
    }

    public override void Clear()
    {
        
    }
}
