using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop
{
    public int id;
    public string title;
    public string description;
    public int price;
    public int pro;
    public Prop(int id1, string title1, string description1, int price1, int pro1)
    {
        this.id = id1;
        this.title = title1;
        this.description = description1;
        this.price = price1;
        this.pro = pro1;
    }
    public Prop(Prop p)
    {
        this.id = p.id;
        this.title = p.title;
        this.description = p.description;
        this.price = p.price;
        this.pro = p.pro;
    }
}
