
namespace Assets.Wulfram3.Scripts.InternalApis.Classes
{
    public enum UnitType
    {
        None,
        Tank,
        Scout,
        Cargo,
        PowerCell,
        RepairPad,
        RefuelPad,
        FlakTurret, 
        GunTurret, 
        MissleLauncher, 
        Skypump, 
        Darklight,
        Uplink,
        Other,
    }

    public enum MapType
    {
        Mini, 
        Large, 
        Spawn,
    }

    public enum SpawnStatus
    {
        IsSpawning,
        IsReady,
        IsAlive,
    }
}