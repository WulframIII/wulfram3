using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankVehicleSetting : IVehicleSetting
{
    public float BaseThrust
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float StrafePercent
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float ThrustMultiplier
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float JumpForce
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public int FuelPerJump
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float TimeBetweenJumps
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float MaxVelocityX
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float MaxVelocityZ
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float BoostMultiplier
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public int MinPropulsionFuel
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float DefaultHeight
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float DaximumHeight
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float RiseSpeed
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public float LowerSpeed
    {
        get
        {
            throw new System.NotImplementedException();
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
