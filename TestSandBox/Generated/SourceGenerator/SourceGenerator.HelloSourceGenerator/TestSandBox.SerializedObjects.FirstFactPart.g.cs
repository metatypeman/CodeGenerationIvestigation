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
            plainObject.ObjectProp = serializer.GetSerializedObjectPtrFromObject(ObjectProp);
            plainObject.ListStrProp = serializer.GetSerializedObjectPtrFromObject(ListStrProp);
            plainObject._parent = serializer.GetSerializedObjectPtr(_parent);
            plainObject.IntField = IntField;
            plainObject.StringField = StringField;
            plainObject.ObjectField = serializer.GetSerializedObjectPtrFromObject(ObjectField);
            plainObject.ListStrField = serializer.GetSerializedObjectPtrFromObject(ListStrField);
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
