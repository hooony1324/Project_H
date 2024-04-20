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

        Managers.Map.LoadMap("StudyMap");

        HeroCamp camp = Managers.Object.Spawn<HeroCamp>(Vector3.zero, 0);
        Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, HERO_KNIGHT_ID);
        Monster monster = Managers.Object.Spawn<Monster>(Vector3.zero, MONSTER_BEAR_ID);
        //Env env = Managers.Object.Spawn<Env>(Vector3.zero, ENV_TREE1_ID);



        Managers.UI.ShowBaseUI<UI_Joystick>();

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = camp;

        return true;
    }

    public override void Clear()
    {
        
    }
}
