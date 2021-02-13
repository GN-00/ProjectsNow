using System;

namespace ProjectsNow.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Join : Attribute
    {
        public string Name { get; set; }
        public Join(string name)
        {
            this.Name = name;
        }
    }
}