using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherVehicleSetting : IVehicleSetting
{
    public float BaseThrust
    {
        get
        {
            return 6f;
        }
    }

    public float StrafePercent
    {
        get
        {
            return 0.9f;
        }
    }

    public float ThrustMultiplier
    {
        get
        {
            return 0.5f;
        }
    }

    public float JumpForce
    {
        get
        {
            return 17.6f;
        }
    }

    public int FuelPerJump
    {
        get
        {
            return 30;
        }
    }

    public float TimeBetweenJumps
    {
        get
        {
            return 2.2f;
        }
    }

    public float MaxVelocityX
    {
        get
        {
            return 16f;
        }
    }

    public float MaxVelocityZ
    {
        get
        {
            return 24f;
        }
    }

    public float BoostMultiplier
    {
        get
        {
            return 5.5f;
        }
    }

    public int MinPropulsionFuel
    {
        get
        {
            return 0;
        }
    }

    public float DefaultHeight
    {
        get
        {
            return 1.2f;
        }
    }

    public float MaximumHeight
    {
        get
        {
            return 9.2f;
        }
    }

    public float RiseSpeed
    {
        get
        {
            return 0.07f;
        }
    }

    public float LowerSpeed
    {
        get
        {
            return 0.04f;
        }
    }

    public List<WeaponTypes> AvailableWeapons
    {
        get
        {
            return new List<WeaponTypes>
            {
                WeaponTypes.Autocannon,
                WeaponTypes.Pulse
            };
        }
    }
}
