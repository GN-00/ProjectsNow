using System;

namespace ProjectsNow.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DontRead : Attribute
    {
    }
}