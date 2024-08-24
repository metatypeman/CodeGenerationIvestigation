using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class SomeGenericClassWithConditions<T>
        where T : class
    {
        T Value;
    }
}
