using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public partial class Fact: ISerializable
    {
        Type ISerializable.GetPlainObjectType() => typeof(FactPo);

        void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)
        {
        OnWritePlainObject((FactPo)plainObject, serializer);
        }

        void OnWritePlainObject(FactPo plainObject, ISerializer serializer)
        {
        }

        void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)
        {
        OnReadPlainObject((FactPo)plainObject, deserializer);
        }

        void OnReadPlainObject(FactPo plainObject, IDeserializer deserializer)
        {
        }

    }
}
