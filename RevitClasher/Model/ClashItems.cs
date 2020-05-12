using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitClasher.Model
{
    /// <summary>
    /// Future implementation, group by clashing elements
    /// </summary>
    public class ClashItems
    {
        //public static RevitElement ElementA { get; set; }

        //public static RevitElement ElementB { get; set; }

        public string Name
        {
            get { return Number + "-Group: " + ElementA.Name + "-" + ElementB.Name; }

        }

        public string Number { get; set; }
        public Element ElementA { get; internal set; }

        public Element ElementB { get; internal set; }
        public string ToString()
        {
            return Name;
        }

    }
}
