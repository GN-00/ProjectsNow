using System;

namespace ProjectsNow.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JoinID : Attribute
    {
        public string ToTable { get; set; }
        public string To2ndTable { get; set; }
        public string To3rdTable { get; set; }
        public string To4thTable { get; set; }
        public string To5thTable { get; set; }
        public string To6thTable { get; set; }
        public string To7thTable { get; set; }
        public string To8thTable { get; set; }

        public JoinID(string toTable)
        {
            this.ToTable = toTable;
        }

        public JoinID(string toTable1, string toTable2)
        {
            this.ToTable = toTable1;
            this.To2ndTable = toTable2;
        }

        public JoinID(string toTable1, string toTable2, string toTable3)
        {
            this.ToTable = toTable1;
            this.To2ndTable = toTable2;
            this.To3rdTable = toTable3;
        }

        public JoinID(string toTable1, string toTable2, string toTable3, string toTable4)
        {
            this.ToTable = toTable1;
            this.To2ndTable = toTable2;
            this.To3rdTable = toTable3;
            this.To4thTable = toTable4;
        }

        public JoinID(string toTable1, string toTable2, string toTable3, string toTable4, string toTable5)
        {
            this.ToTable = toTable1;
            this.To2ndTable = toTable2;
            this.To3rdTable = toTable3;
            this.To4thTable = toTable4;
            this.To5thTable = toTable5;
        }

        public JoinID(string toTable1, string toTable2, string toTable3, string toTable4, string toTable5, string toTable6)
        {
            this.ToTable = toTable1;
            this.To2ndTable = toTable2;
            this.To3rdTable = toTable3;
            this.To4thTable = toTable4;
            this.To5thTable = toTable5;
            this.To6thTable = toTable6;
        }

        public JoinID(string toTable1, string toTable2, string toTable3, string toTable4, string toTable5, string toTable6, string toTable7)
        {
            this.ToTable = toTable1;
            this.To2ndTable = toTable2;
            this.To3rdTable = toTable3;
            this.To4thTable = toTable4;
            this.To5thTable = toTable5;
            this.To6thTable = toTable6;
            this.To7thTable = toTable7;
        }

        public JoinID(string toTable1, string toTable2, string toTable3, string toTable4, string toTable5, string toTable6, string toTable7, string toTable8)
        {
            this.ToTable = toTable1;
            this.To2ndTable = toTable2;
            this.To3rdTable = toTable3;
            this.To4thTable = toTable4;
            this.To5thTable = toTable5;
            this.To6thTable = toTable6;
            this.To7thTable = toTable7;
            this.To8thTable = toTable8;
        }
    }
}
