using Engine.Config;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = PATH + FILENAME)]
public class TestConfig : Config
{
    public new const string FILENAME = "Test";

    public string[] weekDays;
}