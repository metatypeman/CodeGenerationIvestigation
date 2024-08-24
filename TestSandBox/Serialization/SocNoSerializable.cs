namespace TestSandBox.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class SocNoSerializable : Attribute
    {
    }
}
