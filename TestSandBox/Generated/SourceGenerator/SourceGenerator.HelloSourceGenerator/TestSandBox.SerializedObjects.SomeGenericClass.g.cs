using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public partial class SomeGenericClass<T>: ISerializable
    {
        Type ISerializable.GetPlainObjectType() => typeof(SomeGenericClassPo);

        void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)
        {
            OnWritePlainObject((SomeGenericClassPo)plainObject, serializer);
        }

        private void OnWritePlainObject(SomeGenericClassPo plainObject, ISerializer serializer)
        {
            plainObject.Value = serializer.GetSerializedObjectPtr(Value);
        }

        void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)
        {
            OnReadPlainObject((SomeGenericClassPo)plainObject, deserializer);
        }

        private void OnReadPlainObject(SomeGenericClassPo plainObject, IDeserializer deserializer)
        {
            Value = deserializer.GetDeserializedObject<T>(plainObject.Value);
        }

    }
}
