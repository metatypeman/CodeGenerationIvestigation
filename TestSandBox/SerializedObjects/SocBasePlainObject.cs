namespace TestSandBox.SerializedObjects
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SocBasePlainObject : Attribute
    {
        public SocBasePlainObject(string basePlainObjectName)
        {
        }
    }
}
