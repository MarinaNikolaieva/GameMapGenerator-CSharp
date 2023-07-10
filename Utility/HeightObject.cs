using System.Drawing;

namespace MapGeneration.Utility
{
    public class HeightObject
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public float TemperatureCoeffitient { get; set; }
        public float MoistureCoeffitient { get; set; }

        public HeightObject(string name, Color color, float temperatureCoeffitient, float moistureCoeffitient)
        {
            Name = name;
            Color = color;
            TemperatureCoeffitient = temperatureCoeffitient;
            MoistureCoeffitient = moistureCoeffitient;
        }
    }
}
