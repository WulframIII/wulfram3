using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleSlot
{
    Hull,
    Engines,
    Weapons,
    AuxiliaryOne,
    AuxiliaryTwo,
}

public enum ModuleEnchanment
{
    [Hull]
    [Engines]
    [AuxiliaryOne]
    [AuxiliaryTwo]
    Health,
    [Engines]
    [Weapons]
    [AuxiliaryOne]
    [AuxiliaryTwo]
    Fuel,
    [Weapons]
    [AuxiliaryOne]
    [AuxiliaryTwo]
    WeaponDamage,
    [Hull]
    [Engines]
    [Weapons]
    [AuxiliaryOne]
    [AuxiliaryTwo]
    Shield,
    [Hull]
    [Engines]
    [AuxiliaryOne]
    [AuxiliaryTwo]
    Speed,
    [Engines]
    [AuxiliaryOne]
    [AuxiliaryTwo]
    JumpBoost
}

public enum ModuleRarity
{
    Gray,
    White,
    Green,
    Blue,
    Purple,
}


[AttributeUsage(AttributeTargets.Field)]
public class HullAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Field)]
public class EnginesAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Field)]
public class WeaponsAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Field)]
public class AuxiliaryOneAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Field)]
public class AuxiliaryTwoAttribute : Attribute
{

}