using UnityEngine;
using Engine;

public class LowPolyWaterSettings : LevelElement
{
    public LPWAsset.LowPolyWaterScript lowPolyWater;

    public override void OnSave()
    {
        base.OnSave();
        //data.Add("Color", col);
        data.Add("xSize", lowPolyWater.sizeX);
        data.Add("ySize", lowPolyWater.sizeZ);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("xSize"))
            lowPolyWater.sizeX = (int)data["xSize"];
        if (data.ContainsKey("ySize"))
            lowPolyWater.sizeZ = (int)data["ySize"];

        lowPolyWater.enabled = false;
        Invoke("EnableWater", 0.1f);
    }

    void EnableWater()
    {
        lowPolyWater.enabled = true;
    }
}