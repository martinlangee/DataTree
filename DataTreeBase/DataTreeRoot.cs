using System.Xml;

namespace DataTreeBase
{
    public class DataTreeRoot: DataTreeContainer
    {
        protected DataTreeRoot()
            : base(null, "Root", "Root")
        {
        }

        public void LoadFromFile(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);
            LoadFromXml(doc.DocumentElement);
            ResetModified();
        }

        public void SaveToFile(string fileName)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<Config/>");
            SaveToXml(doc.DocumentElement);
            doc.Save(fileName);
            ResetModified();
        }
    }
}
