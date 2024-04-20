using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static Unity.Burst.Intrinsics.X86.Avx;
public class ObjectManager
{
    public HashSet<Hero> Heroes { get; } = new HashSet<Hero>();
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
    public HashSet<Env> Envs { get; } = new HashSet<Env>();
    public HeroCamp Camp { get; private set; }
    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }

    public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        if (obj.ObjectType == EObjectType.Hero)
        {
            obj.transform.parent = HeroRoot;
            Hero hero = go.GetComponent<Hero>();
            Heroes.Add(hero);
            hero.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            obj.transform.parent = MonsterRoot;
            Monster monster = go.GetComponent<Monster>();
            Monsters.Add(monster);
            monster.SetInfo(templateID);
        }
        //else if (obj.ObjectType == EObjectType.Projectile)
        //{
        //    obj.transform.parent = ProjectileRoot;

        //    Projectile projectile = go.GetComponent<Projectile>();
        //    Projectiles.Add(projectile);

        //    projectile.SetInfo(templateID);
        //}
        else if (obj.ObjectType == EObjectType.Env)
        {
            obj.transform.parent = EnvRoot;

            Env env = go.GetComponent<Env>();
            Envs.Add(env);

            env.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.HeroCamp)
        {
            Camp = go.GetComponent<HeroCamp>();
        }
        //else if (obj.ObjectType == EObjectType.Npc)
        //{
        //    obj.transform.parent = NpcRoot;

        //    Npc npc = go.GetOrAddComponent<Npc>();
        //    Npcs.Add(npc);

        //    npc.SetInfo(templateID);
        //}
        //else if (obj.ObjectType == EObjectType.ItemHolder)
        //{
        //    obj.transform.parent = ItemHolderRoot;

        //    ItemHolder itemHolder = go.GetOrAddComponent<ItemHolder>();
        //    ItemHolders.Add(itemHolder);
        //}

        return obj as T;
    }
    #endregion
}
