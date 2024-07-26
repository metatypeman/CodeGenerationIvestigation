namespace TestSandBox.Serialization
{
    public interface ISerializable
    {
        Type GetPlainObjectType();
        void OnWritePlainObject(object plainObject, ISerializer serializer);
    }
}
