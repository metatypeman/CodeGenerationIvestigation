namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class FirstFactPart
    {
        public Fact Parent { get => _parent; set => _parent = value; }

        private Fact _parent;

        public int IntProp { get; set; }

        public int IntField;

        public string StringProp { get; set; }
        public object ObjectProp { get; set; }
        public List<string> ListStrProp { get; set; }
    }
}
