namespace BotCore.Types
{
    public class Aisling : MapObject
    {
        public bool IsHidden;
        public string Name;

        public Aisling(string name) : base()
        {
            this.Name = name;
            this.Type = MapObjectType.Aisling;
        }
    }
}
