using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSector
{
    public string HorizontalSector { get; set; }

    public string VerticalSector { get; set; }

    public string Sector { get { return this.HorizontalSector + VerticalSector; } }
}
