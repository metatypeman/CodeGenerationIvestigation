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

        private void OnWritePlainObject(FactPo plainObject, ISerializer serializer)
        {
            plainObject.FactPart = serializer.GetSerializedObjectPtr(FactPart);
        }

        void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)
        {
            OnReadPlainObject((FactPo)plainObject, deserializer);
        }

        private void OnReadPlainObject(FactPo plainObject, IDeserializer deserializer)
        {
            
        }

    }
}
