using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC
{
    public string name;
    public Type type;
    public List<Plot> plots = new List<Plot>();
    public bool isAlive=true;//活着不
    public bool isKnown=false;//主角认识不
    public NPC(string name, Type type)
    {
        this.name =name;
        this.type=type;
    }
}
