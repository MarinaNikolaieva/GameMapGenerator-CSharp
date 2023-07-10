using System.Drawing;

namespace MapGeneration.Utility
{
    public class TemperatureObject : IEquatable<TemperatureObject>
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public TemperatureObject(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public bool Equals(TemperatureObject? other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }
    }
}
