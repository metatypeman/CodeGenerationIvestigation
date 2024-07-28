namespace TestSandBox.Serialization
{
    public interface ISerializer
    {
        void Serialize(ISerializable serializable);
        ObjectPtr GetSerializedObjectPtr(ISerializable serializable);

        ObjectPtr GetSerializedObjectPtrFromObject(object serializable);
    }
}
