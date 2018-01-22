using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Wulfram3.Scripts.InternalApis.Classes
{
    public class BaseSetup 
    {
        public string mapName { get; set; }

        public int loadNumber { get; set; }

        public List<Base> bases { get; set; }
    }

    public class Base
    {
        public string baseName { get; set; }

        public PunTeams.Team team { get; set; }

        public List<BaseUnit> units { get; set; }
    }

    public class BaseUnit
    {
        public UnitType unitType { get; set; }

        public int posX { get; set; }

        public int posY { get; set; }

        public int posZ { get; set; }

        public string GetPrefabNameFromUnitType(PunTeams.Team team)
        {
            switch (team)
            {
                case PunTeams.Team.none:
                    switch (unitType)
                    {
                        case UnitType.None:
                            return "";
                        case UnitType.Tank:
                            return "";
                        case UnitType.Scout:
                            return "";
                        case UnitType.Cargo:
                            return "";
                        case UnitType.PowerCell:
                            return "";
                        case UnitType.RepairPad:
                            return "";
                        case UnitType.RefuelPad:
                            return "";
                        case UnitType.FlakTurret:
                            return "";
                        case UnitType.GunTurret:
                            return "";
                        case UnitType.MissleLauncher:
                            return "";
                        case UnitType.Skypump:
                            return "";
                        case UnitType.Darklight:
                            return "";
                        case UnitType.Uplink:
                            return "";
                        default:
                            return "";
                    }
                case PunTeams.Team.Red:
                    switch (unitType)
                    {
                        case UnitType.None:
                            return "";
                        case UnitType.Tank:
                            return "RedTank";
                        case UnitType.Scout:
                            return "RedScout";
                        case UnitType.Cargo:
                            return "";
                        case UnitType.PowerCell:
                            return "RedPowerCell";
                        case UnitType.RepairPad:
                            return "RepairPad";
                        case UnitType.RefuelPad:
                            return "";
                        case UnitType.FlakTurret:
                            return "RedFlakTurret";
                        case UnitType.GunTurret:
                            return "";
                        case UnitType.MissleLauncher:
                            return "";
                        case UnitType.Skypump:
                            return "";
                        case UnitType.Darklight:
                            return "";
                        case UnitType.Uplink:
                            return "";
                        default:
                            return "";
                    }
                case PunTeams.Team.Blue:
                    switch (unitType)
                    {
                        case UnitType.None:
                            return "";
                        case UnitType.Tank:
                            return "PlayerTank";
                        case UnitType.Scout:
                            return "BlueScout";
                        case UnitType.Cargo:
                            return "Cargo";
                        case UnitType.PowerCell:
                            return "";
                        case UnitType.RepairPad:
                            return "RepairPad";
                        case UnitType.RefuelPad:
                            return "";
                        case UnitType.FlakTurret:
                            return "BlueFlakTurret";
                        case UnitType.GunTurret:
                            return "BlueGunTurret";
                        case UnitType.MissleLauncher:
                            return "";
                        case UnitType.Skypump:
                            return "";
                        case UnitType.Darklight:
                            return "";
                        case UnitType.Uplink:
                            return "";
                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }
    }
}