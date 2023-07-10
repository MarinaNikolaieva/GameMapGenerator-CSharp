using Newtonsoft.Json;
using System.Drawing;

namespace MapGeneration.Utility
{
    public class BasicResource : IResource, IEquatable<IResource>
    {
        private int ID { get; }
        public string Name { get; }  //Implementing the property
        [JsonIgnore]
        public string Category { get; }  //Same here
        private Color color { get; }

        public BasicResource(int ID, string Name, Color color, string Category)
        {
            this.ID = ID;
            this.Name = Name;
            this.color = color;
            this.Category = Category;
        }

        public BasicResource(BasicResource other)  //Sort of a copy constructor
        {
            this.ID = other.getID();
            this.Name = other.getName();
            this.color = other.getColor();
        }

        public int getID()
        {
            return ID;
        }

        public string getName()
        {
            return Name;
        }

        public Color getColor()
        {
            return color;
        }

        public bool Equals(IResource other)
        {
            if (this is IResource && other is IResource)
                return (this as IResource).SameType(other) && (this as IResource).SameName(other);
            return false;
        }
    }
}
