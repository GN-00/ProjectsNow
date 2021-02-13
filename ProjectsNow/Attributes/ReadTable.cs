using System;

namespace ProjectsNow.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReadTable : Attribute
    {
        public string Name { get; set; }
        public ReadTable(string name)
        {
            this.Name = name;
        }
    }
}