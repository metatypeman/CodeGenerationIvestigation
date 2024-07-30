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
            plainObject.IntProp = IntProp;
            plainObject.StringProp = StringProp;
            plainObject.ObjectProp = serializer.GetSerializedObjectPtr(ObjectProp);
            plainObject.ListStrProp = serializer.GetSerializedObjectPtr(ListStrProp);
            plainObject._parent = serializer.GetSerializedObjectPtr(_parent);
            plainObject.IntField = IntField;
            plainObject.StringField = StringField;
            plainObject.ObjectField = serializer.GetSerializedObjectPtr(ObjectField);
            plainObject.ListStrField = serializer.GetSerializedObjectPtr(ListStrField);
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
