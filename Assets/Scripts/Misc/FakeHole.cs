using UnityEngine;
using Engine;

public class FakeHole : Scenery
{
    public SpriteRenderer spriteRenderer;

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("RenderSize"))
            spriteRenderer.size = (Float2)data["RenderSize"];
    }

    public override void OnSave()
    {
        base.OnSave();
        data["RenderSize"] = (Float2)spriteRenderer.size;
    }
}