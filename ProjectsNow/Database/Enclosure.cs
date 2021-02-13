using System;
using System.Reflection;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    public class Enclosure 
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Installation { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        public string IP { get; set; }

        public string Location { get; set; }
        public string Color { get; set; }
        public string Metal { get; set; }
        public string Form { get; set; }
        public string Door { get; set; }
        public string Functional { get; set; }

    }

}
