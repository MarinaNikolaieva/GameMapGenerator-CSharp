using Newtonsoft.Json;

namespace MapGeneration.Utility
{
    public class MapPart : IEquatable<MapPart>
    {
        public int ID = -1;  //These will be UNIQUE! The other parameters may be the same
        public int X = -1;
        public int Y = -1;
        public int Height = 0;
        //public bool isLand = true;
        [JsonIgnore]
        public TemperatureObject TemperatureFinal = null;
        [JsonIgnore]
        public TemperatureObject TemperatureBasic = null;
        [JsonIgnore]
        public MoistureObject Moisture = null;
        [JsonIgnore]
        public Biome Biome = null;
        [JsonIgnore]
        public bool NoHeightSpecific = false;
        [JsonIgnore]
        public Soil Soil = null;
        public List<IResource> Resources = new List<IResource>();
        public CountryBase Country = null;
        [JsonIgnore]
        public List<MapPart> neighbors = new List<MapPart>();
        public List<int> neighborIDs = new List<int>();

        public MapPart(int ID)
        {
            this.ID = ID;
        }

        public bool Equals(MapPart? other)
        {
            if (other == null) return false;
            return ID == other.ID;
        }
    }
}
