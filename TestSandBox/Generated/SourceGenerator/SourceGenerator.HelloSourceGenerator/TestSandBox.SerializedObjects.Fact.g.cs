namespace TestSandBox.SerializedObjects
{
    public partial class Fact: ISerializable
    {
    8Type ISerializable.GetPlainObjectType() => typeof(FactPo);
    }
}
