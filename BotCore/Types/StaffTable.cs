namespace BotCore.Types
{
    public class StaffTable 
    {
        public string Name { get; set; }
        public byte Level { get; set; }
        public string @Class { get; set; }
        public byte Id { get; set; }
        public SpellModifiers Modifer = new SpellModifiers();
    }

    public enum ActionModifier
    {
        Set,
        Decrease,
        Increase
    }

    public enum SpellScope
    {
        Single,
        Group,
        All,
        AllExcept
    }

    public class SpellModifiers
    {
        public ActionModifier Action { get; set; }
        public SpellScope Scope { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
    }

    public interface ICastableTarget
    {
        int Caster { get; set; }
        int Target { get; set; }

        Spell SpellInfo { get; set; }
    }
}
