namespace TestSandBox.SerializedObjects
{
    public interface ISocSerializableActionFactory
    {
        string Id { get; }
        object GetAction();
    }
}
