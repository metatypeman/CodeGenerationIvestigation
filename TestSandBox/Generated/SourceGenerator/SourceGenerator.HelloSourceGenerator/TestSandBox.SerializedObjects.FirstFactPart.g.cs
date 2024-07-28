using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public partial class FirstFactPart: ISerializable
    {
        Type ISerializable.GetPlainObjectType() => typeof(FirstFactPartPo);

        void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)
        {
            OnWritePlainObject((FirstFactPartPo)plainObject, serializer);
        }

        private void OnWritePlainObject(FirstFactPartPo plainObject, ISerializer serializer)
        {
        }

        void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)
        {
            OnReadPlainObject((FirstFactPartPo)plainObject, deserializer);
        }

        private void OnReadPlainObject(FirstFactPartPo plainObject, IDeserializer deserializer)
        {
        }

    }
}
