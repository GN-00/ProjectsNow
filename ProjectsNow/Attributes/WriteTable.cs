using System;

namespace ProjectsNow.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WriteTable : Attribute
    {
        public string Name { get; set; }
        public WriteTable(string name)
        {
            this.Name = name;
        }
    }
}
