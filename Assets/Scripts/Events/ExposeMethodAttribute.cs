using System;

// Place this file in any folder that is or is a descendant of a folder named "Scripts"
namespace Engine.Events
{
    // Restrict to methods only
    [AttributeUsage(AttributeTargets.Class)]
    public class ExposeMethodAttribute : Attribute
    {
    }
}