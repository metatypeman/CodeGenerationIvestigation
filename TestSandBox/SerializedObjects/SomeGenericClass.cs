namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class SomeGenericClass<T>
    {
        T Value;
    }

    [SocSerialization]
    public partial class SomeGenericClass<T, U>
    {
        T Value;

        U OtherValue;
    }
}
