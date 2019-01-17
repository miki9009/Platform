using UnityEngine;

public class Console
{
    public enum LogColor
    {
        White, Red, Green, Blue, Yellow, Magenta, Lime, Gray, Black, Orange
    }
    public static void WriteLine(object message, LogColor color = LogColor.White)
    {
        string col = "white";
        switch(color)
        {
            case LogColor.White:
                col = "white";
                break;
            case LogColor.Red:
                col = "red";
                break;
            case LogColor.Yellow:
                col = "yellow";
                break;
            case LogColor.Green:
                col = "green";
                break;
            case LogColor.Lime:
                col = "lime";
                break;
            case LogColor.Blue:
                col = "blue";
                break;
            case LogColor.Magenta:
                col = "magenta";
                break;
            case LogColor.Gray:
                col = "gray";
                break;
            case LogColor.Black:
                col = "black";
                break;
            case LogColor.Orange:
                col = "orange";
                break;
        }
        Debug.Log("<color="+col+">"+message+"</color>");
    }
}