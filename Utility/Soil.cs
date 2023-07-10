using Newtonsoft.Json;
using System.Drawing;

namespace MapGeneration.Utility
{
    public class Soil : IEquatable<Soil>
    {
        [JsonProperty]
        private string Name { get; set; }
        [JsonProperty]
        private Color Color { get; set; }

        public Soil(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public string getName()
        {
            return Name;
        }

        public Color getColor()
        {
            return Color;
        }

        public bool Equals(Soil? other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }
    }
}
