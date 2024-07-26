namespace TestSandBox.Serialization
{
    public readonly struct ObjectPtr
    {
        public ObjectPtr(string id, string typeName)
        {
            Id = id;
            TypeName = typeName;
        }

        public string Id { get; init; }
        public string TypeName { get; init; }

        public override string ToString() => $"({nameof(Id)}: '{Id}', {nameof(TypeName)}: '{TypeName}')";
    }
}
