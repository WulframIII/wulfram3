using Medallion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

public interface IUnitModule
{
    System.Guid Id { get; set; }

    string ModuleName { get; set; }

    ModuleRarity Rarity { get; set; }

    ModuleSlot Slot { get; set; }

    List<KeyValuePair<ModuleEnchanment, int>> Stats { get; set; }

}

public class UnitModule : IUnitModule
{
    public UnitModule(ModuleRarity rarity, ModuleSlot moduleSlot)
    {
        this.Rarity = rarity;
        this.Slot = moduleSlot;
        this.Id = Guid.NewGuid();
        this.Stats = GenerateRandomStats();
    }

    public Guid Id { get; set; }

    public string ModuleName { get; set; }

    public ModuleRarity Rarity { get; set; }

    public ModuleSlot Slot { get; set; }

    public List<KeyValuePair<ModuleEnchanment, int>> Stats { get; set; }

    private List<KeyValuePair<ModuleEnchanment, int>> GenerateRandomStats()
    {
        var statCount = GetStatsCountByRarity();
        var enchanmentList = GetListOfEnchanmentsBySlot();
        var results = new List<KeyValuePair<ModuleEnchanment, int>>();
        foreach (var item in enchanmentList)
        {
            results.Add(GetStat(item));
        }

        return results;
    }

    private int GetStatsCountByRarity()
    {
        switch (this.Rarity)
        {
            case ModuleRarity.Gray:
                return 1;
            case ModuleRarity.White:
                return 1;
            case ModuleRarity.Green:
                return 2;
            case ModuleRarity.Blue:
                return 2;
            case ModuleRarity.Purple:
                return 3;
            default:
                return 1;
        }
    }

    private int GetStatsMaxBoundsByRarity()
    {
        switch (this.Rarity)
        {
            case ModuleRarity.Gray:
                return RandomNumber(1, 10);
            case ModuleRarity.White:
                return RandomNumber(11, 20);
            case ModuleRarity.Green:
                return RandomNumber(21, 30);
            case ModuleRarity.Blue:
                return RandomNumber(31, 40);
            case ModuleRarity.Purple:
                return RandomNumber(45, 65);
            default:
                return RandomNumber(1, 10);
        }
    }

    private List<ModuleEnchanment> GetListOfEnchanmentsBySlot()
    {
        var count = GetStatsCountByRarity();
        var results = new List<ModuleEnchanment>();
        ModuleEnchanment[] enums;
        switch (this.Slot)
        {
            case ModuleSlot.Hull:
                enums = EnumExt.FilterEnumWithAttributeOf<ModuleEnchanment, HullAttribute>().ToList().ToArray();
                break;
            case ModuleSlot.Engines:
                enums = EnumExt.FilterEnumWithAttributeOf<ModuleEnchanment, EnginesAttribute>().ToList().ToArray();
                break;
            case ModuleSlot.Weapons:
                enums = EnumExt.FilterEnumWithAttributeOf<ModuleEnchanment, WeaponsAttribute>().ToList().ToArray();
                break;
            case ModuleSlot.AuxiliaryOne:
                enums = EnumExt.FilterEnumWithAttributeOf<ModuleEnchanment, AuxiliaryOneAttribute>().ToList().ToArray();
                break;
            case ModuleSlot.AuxiliaryTwo:
                enums = EnumExt.FilterEnumWithAttributeOf<ModuleEnchanment, AuxiliaryTwoAttribute>().ToList().ToArray();
                break;
            default:
                enums = EnumExt.FilterEnumWithAttributeOf<ModuleEnchanment, AuxiliaryOneAttribute>().ToList().ToArray();
                break;
        }
        for (int i = 1; i <= count; i++)
        {
            results.Add((ModuleEnchanment)enums.GetValue(RandomNumber(enums.Length)));
        }

        return results;
    }

    private KeyValuePair<ModuleEnchanment, int> GetStat(ModuleEnchanment moduleEnchanment)
    {      
        //All values are % boosters
        switch (moduleEnchanment)
        {
            case ModuleEnchanment.Health:            
                return new KeyValuePair<ModuleEnchanment, int>(moduleEnchanment, RandomNumber(1,GetStatsMaxBoundsByRarity()));
                
            case ModuleEnchanment.Fuel:
                return new KeyValuePair<ModuleEnchanment, int>(moduleEnchanment, RandomNumber(GetStatsMaxBoundsByRarity()));
                
            case ModuleEnchanment.WeaponDamage:
                return new KeyValuePair<ModuleEnchanment, int>(moduleEnchanment, RandomNumber(GetStatsMaxBoundsByRarity()));
                
            case ModuleEnchanment.Shield:
                return new KeyValuePair<ModuleEnchanment, int>(moduleEnchanment, RandomNumber(GetStatsMaxBoundsByRarity()));
                
            case ModuleEnchanment.Speed:
                return new KeyValuePair<ModuleEnchanment, int>(moduleEnchanment, RandomNumber(GetStatsMaxBoundsByRarity()));
                
            case ModuleEnchanment.JumpBoost:
                return new KeyValuePair<ModuleEnchanment, int>(moduleEnchanment, RandomNumber(GetStatsMaxBoundsByRarity()));
                
            default:
                return new KeyValuePair<ModuleEnchanment, int>(moduleEnchanment, RandomNumber(GetStatsMaxBoundsByRarity()));
                
        }
    }

    public override string ToString()
    {
        var stats = "";
        foreach (var item in Stats)
        {
            stats += "[" + item.Key + "|" + item.Value + "]";
        }

        return Id.ToString() + " " + ModuleName + " " + Rarity + " " + Slot + " " + stats;
    }


    private readonly System.Random random = Rand.Create();
    private readonly object syncLock = new object();
    public int RandomNumber(int min, int max)
    {
        lock (syncLock)
        { // synchronize
            return random.Next(min, max);
        }
    }

    public int RandomNumber(int max)
    {
        lock (syncLock)
        { // synchronize
            return random.Next(max);
        }
    }
}


public class UnitModuleGenerator
{
    public void GenerateNewModuleTest()
    {
        Array Raritys = Enum.GetValues(typeof(ModuleRarity));
        Array Slots = Enum.GetValues(typeof(ModuleSlot));

        var random = Rand.Create();
        var modCount = 10;
        for (int i = 1; i <= modCount; i++)
        {
            var newItem = new UnitModule((ModuleRarity)Raritys.GetValue(random.Next(Raritys.Length)), (ModuleSlot)Slots.GetValue(random.Next(Slots.Length)));
            Debug.Log(newItem.ToString());
        }

        
    }
}


public static class EnumExt
{
    public static T RandomEnumValue<T>()
    {
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(new System.Random().Next(v.Length));
    }

    public static IEnumerable<TEnum> FilterEnumWithAttributeOf<TEnum, TAttribute>()

    where TEnum : struct

    where TAttribute : class

    {

        foreach (var field in

            typeof(TEnum).GetFields(BindingFlags.GetField |

                                     BindingFlags.Public |

                                     BindingFlags.Static))

        {



            if (field.GetCustomAttributes(typeof(TAttribute), false).Length > 0)

                yield return (TEnum)field.GetValue(null);

        }

    }
}


