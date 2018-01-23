using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVehicleSetting

{ 
    float BaseThrust { get; }

    float StrafePercent { get; }

    float ThrustMultiplier { get; }

    float JumpForce { get; }

    int FuelPerJump { get; }

    float TimeBetweenJumps { get; }

    float MaxVelocityX { get; }

    float MaxVelocityZ { get; }

    float BoostMultiplier { get; }

    int MinPropulsionFuel { get; }

    float DefaultHeight { get; }

    float MaximumHeight { get; }

    float RiseSpeed { get; }

    float LowerSpeed { get; }

    List<WeaponTypes> AvailableWeapons { get; }

}

public class VehicleSettingFactory
{
    public static IVehicleSetting GetVehicleSetting(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Tank:
                return new TankVehicleSetting();
            case UnitType.Scout:
                return new ScoutVehicleSettings();
            case UnitType.Other:
                return new OtherVehicleSetting();
            default:
                return null;
        }
    }
}
