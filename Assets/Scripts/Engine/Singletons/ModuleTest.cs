using Engine.Singletons;
using UnityEngine;

public class ModuleTest : Module
{
    public override void Initialize()
    {
        Debug.Log("Hello from Singleton test");
    }
}