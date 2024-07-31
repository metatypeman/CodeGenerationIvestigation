using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public partial class SomeGenericClassWithConditions<T>: ISerializable
    {
        Type ISerializable.GetPlainObjectType() => typeof(SomeGenericClassWithConditionsPo);

        void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)
        {
            OnWritePlainObject((SomeGenericClassWithConditionsPo)plainObject, serializer);
        }

        private void OnWritePlainObject(SomeGenericClassWithConditionsPo plainObject, ISerializer serializer)
        {
            plainObject.Value = serializer.GetSerializedObjectPtr(Value);
        }

        void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)
        {
            OnReadPlainObject((SomeGenericClassWithConditionsPo)plainObject, deserializer);
        }

        private void OnReadPlainObject(SomeGenericClassWithConditionsPo plainObject, IDeserializer deserializer)
        {
            Value = deserializer.GetDeserializedObject<T>(plainObject.Value);
        }

    }
}
