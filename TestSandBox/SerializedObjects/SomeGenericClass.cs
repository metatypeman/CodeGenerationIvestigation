namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class SomeGenericClass<T>: object
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
