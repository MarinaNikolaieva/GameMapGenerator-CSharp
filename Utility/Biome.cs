using System.Drawing;
using Newtonsoft.Json;

namespace MapGeneration.Utility
{
    public class Biome : IEquatable<Biome>
    {
        [JsonProperty]
        private string Name { get; set; }
        [JsonProperty]
        private Color Color { get; set; }
        private List<BasicResource> Resources { get; set; }
        private List<int> HeightLowerLimits { get; set; }  //0 = the effect doesn't matter
        private List<TemperatureObject> Temperatures { get; set; }  //Empty list = doesn't matter
        private List<MoistureObject> Moistures { get; set; }

        private List<Soil> Soils { get; set; }

        public Biome(string name, Color color)
        {
            Name = name;
            Color = color;
            Resources = new List<BasicResource>();
            HeightLowerLimits = new List<int>();
            Temperatures = new List<TemperatureObject>();
            Moistures = new List<MoistureObject>();
            Soils = new List<Soil>();
        }

        public string getName()
        {
            return Name;
        }

        public Color getColor()
        {
            return Color;
        }

        public List<BasicResource> getResources()
        {
            return Resources;
        }

        public void addResource(BasicResource resource)
        {
            if (resource != null && !Resources.Contains(resource))
                Resources.Add(resource);
        }

        public List<int> getHeights()
        {
            return HeightLowerLimits;
        }

        public void addHeight(int height)
        {
            if (!HeightLowerLimits.Contains(height))
                HeightLowerLimits.Add(height);
        }

        public List<TemperatureObject> getTemperatures()
        {
            return Temperatures;
        }

        public void addTemperature(TemperatureObject temper)
        {
            if (!Temperatures.Contains(temper))
                Temperatures.Add(temper);
        }

        public List<MoistureObject> getMoistures()
        {
            return Moistures;
        }

        public void addMoisture(MoistureObject moist)
        {
            if (!Moistures.Contains(moist))
                Moistures.Add(moist);
        }

        public List<Soil> getSoils()
        {
            return Soils;
        }

        public void addSoil(Soil soil)
        {
            if (!Soils.Contains(soil))
                Soils.Add(soil);
        }

        public bool Equals(Biome? other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }
    }
}
