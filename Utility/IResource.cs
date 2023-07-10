using Newtonsoft.Json;

namespace MapGeneration.Utility
{
    public interface IResource : IEquatable<IResource>
    {
        [JsonProperty]  //This is here to make the private property outputtable to json
        string Name { get; }  //This is a PROPERTY, not a field
        [JsonProperty]
        string Category { get; }  //Same here
        public bool SameType(IResource other)
        {
            if (this.GetType() == other.GetType())
            {
                return true;
            }
            return false;
        }

        public bool SameName(IResource other)
        {
            if (this.Name.Equals(other.Name))
                return true;
            return false;
        }

        public bool Equals(IResource other)
        {
            return SameType(other) && SameName(other);
        }
    }
}
