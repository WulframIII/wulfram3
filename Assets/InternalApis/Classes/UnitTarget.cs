using Assets.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTarget
{

	public GameObject Object { get; set; }

    public double LastDistance { get; set; }

    public UnitType ObjectType { get; set; }

    public PunTeams.Team ObjectTeam { get; set; }
}
