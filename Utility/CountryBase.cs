using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace MapGeneration.Utility
{
    public class CountryBase : IEquatable<CountryBase>
    {
        public string Name;
        [JsonIgnore]
        public Color Color;
        [JsonIgnore]
        public Vector2 Capital;

        public CountryBase(string Name, Color Color)
        {
            this.Name = Name;
            this.Color = Color;
        }

        public bool Equals(CountryBase? other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }

        public string Output()
        {
            StringBuilder build = new StringBuilder();
            build.Append(Name);
            build.Append(";");
            build.Append(ColorTranslator.ToHtml(Color.FromArgb(Color.ToArgb())));
            build.Append(";");
            build.Append("X:");
            build.Append(Capital.X);
            build.Append(";Y:");
            build.Append(Capital.Y);
            return build.ToString();
        }
    }
}
