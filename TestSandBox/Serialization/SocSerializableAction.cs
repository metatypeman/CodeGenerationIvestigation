namespace TestSandBox.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SocSerializableAction : Attribute
    {
        public string Id { get; set; }
    }
}
