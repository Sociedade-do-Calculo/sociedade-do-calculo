﻿// Buffs are like Skills, but for the Buffs list.
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;

[Serializable]
public partial struct Buff
{
    // hashcode used to reference the real ScriptableSkill (can't link to data
    // directly because synclist only supports simple types). and syncing a
    // string's hashcode instead of the string takes WAY less bandwidth.
    public int hash;

    // dynamic stats (cooldowns etc.)
    public int level;
    public double buffTimeEnd; // server time. double for long term precision.

    // constructors
    public Buff(BuffSkill data, int level)
    {
        hash = data.name.GetStableHashCode();
        Data = data;
        this.level = level;
        buffTimeEnd = NetworkTime.time + data.buffTime.GetType(level); // start buff immediately
    }

    // wrappers for easier access
    public BuffSkill data
    {
        get
        {
            // show a useful error message if the key can't be found
            // note: ScriptableSkill.OnValidate 'is in resource folder' check
            //       causes Unity SendMessage warnings and false positives.
            //       this solution is a lot better.
            if (!ScriptableSkill.dict.ContainsKey(hash))
                throw new KeyNotFoundException("There is no ScriptableSkill with hash=" + hash + ". Make sure that all ScriptableSkills are in the Resources folder so they are loaded properly.");
            return (BuffSkill)ScriptableSkill.dict[hash];
        }
    }
    public string Name => data.name;
    public Sprite Image => data.image;
    public float buffTime => data.buffTime.Get(level);
    public int bonusHealthMax => data.bonusHealthMax.Get(level);
    public int bonusManaMax => data.bonusManaMax.Get(level);
    public int bonusDamage => data.bonusDamage.Get(level);
    public int bonusDefense => data.bonusDefense.Get(level);
    public float bonusBlockChance => data.bonusBlockChance.Get(level);
    public float bonusCriticalChance => data.bonusCriticalChance.Get(level);
    public float bonusHealthPercentPerSecond => data.bonusHealthPercentPerSecond.Get(level);
    public float bonusManaPercentPerSecond => data.bonusManaPercentPerSecond.Get(level);
    public float bonusSpeed => data.bonusSpeed.Get(level);
    public int maxLevel => data.maxLevel;

    public BuffSkill Data { get; }

    // tooltip - runtime part
    public string ToolTip()
    {
        // we use a StringBuilder so that addons can modify tooltips later too
        // ('string' itself can't be passed as a mutable object)
        StringBuilder tip = new StringBuilder(data.ToolTip(level));

        // addon system hooks
        Utils.InvokeMany(typeof(Buff), this, "ToolTip_", tip);

        return tip.ToString();
    }

    public float BuffTimeRemaining()
    {
        // how much time remaining until the buff ends? (using server time)
        return NetworkTime.time >= buffTimeEnd ? 0 : (float)(buffTimeEnd - NetworkTime.time);
    }
}

public class BuffSkill
{
    internal object name;
    internal object buffTime;

    public static explicit operator BuffSkill(ScriptableSkill v)
    {
        throw new NotImplementedException();
    }
}

#pragma warning disable CS0618 // O tipo ou membro é obsoleto
public class SyncListBuff : SyncList<Buff>
{
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    protected override Buff DeserializeItem(UnityEngine.Networking.NetworkReader reader)
    {
        throw new NotImplementedException();
    }

    protected override void SerializeItem(UnityEngine.Networking.NetworkWriter writer, Buff item)
    {
        throw new NotImplementedException();
    }
}
#pragma warning restore CS0618 // O tipo ou membro é obsoleto