namespace TestSandBox.SerializedObjects
{
    public partial class SomeGenericClassWithConditions<T>
        where T : class
    {
        T Value;
    }
}
