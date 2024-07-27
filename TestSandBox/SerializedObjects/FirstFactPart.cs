namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class FirstFactPart
    {
        public Fact Parent { get => _parent; set => _parent = value; }

        private Fact _parent;
    }
}
