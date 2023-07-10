using System.Drawing;

namespace MapGeneration.Utility
{
    public class MoistureObject : IEquatable<MoistureObject>
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public MoistureObject(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public bool Equals(MoistureObject? other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }
    }
}
