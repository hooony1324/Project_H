using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string DescriptionTextID;
        public string PrefabLabel;
        public float ColliderOffsetX;
        public float ColliderOffsetY;
        public float ColliderRadius;
        public float MaxHp;
        public float UpMaxHpBonus;
        public float Atk;
        public float AtkRange;
        public float AtkBonus;
        public float MoveSpeed;
        public float CriRate;
        public float CriDamage;
        public string IconImage;
        public string SkeletonDataID;
        public int DefaultSkillId;
        public int EnvSkillId;
        public int SkillAId;
        public int SkillBId;
    }
    #endregion

    #region HeroData
    [Serializable]
    public class HeroData : CreatureData
    {
    }

    [Serializable]
    public class HeroDataLoader : ILoader<int, HeroData>
    {
        public List<HeroData> heros = new List<HeroData>();
        public Dictionary<int, HeroData> MakeDict()
        {
            Dictionary<int, HeroData> dict = new Dictionary<int, HeroData>();
            foreach (HeroData hero in heros)
            {
                dict.Add(hero.DataId, hero);
            }
            return dict;
        }
    }
    #endregion

    #region MonsterData
    [Serializable]
    public class MonsterData : CreatureData
    {
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
            {
                dict.Add(monster.DataId, monster);
            }
            return dict;
        }
    }

    #endregion


    #region Env
    [Serializable]
    public class EnvData
    {
        public int DataId;
        public string DescriptionTextID;
        public string PrefabLabel;
        public float MaxHp;
        public int ResourceAmount;
        public float RegenTime;
        public List<String> SkeletonDataIDs = new List<String>();
        public int DropItemId;
    }

    [Serializable]
    public class EnvDataLoader : ILoader<int, EnvData>
    {
        public List<EnvData> envs = new List<EnvData>();
        public Dictionary<int, EnvData> MakeDict()
        {
            Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
            foreach (EnvData env in envs)
                dict.Add(env.DataId, env);
            return dict;
        }
    }
    #endregion
}
