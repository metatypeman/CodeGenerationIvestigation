using TestSandBox.SerializedObjects.PlainObjects;

namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    //[SocBasePlainObject(nameof(SomeGenericClassPo_T))]
    public partial class BaseSomeClass: SomeGenericClass<int>
    {
    }
}
