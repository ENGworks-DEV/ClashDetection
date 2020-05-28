using Autodesk.Revit.DB;

namespace ClashDetection
{
    public class RevitElement
    {
        public Element element {get;set;}
        public string Name {
            get { return element.Name + " " + element.Id.ToString(); }
            }
        public string ToString()
        {
            return element.Name;
        }

    }
}