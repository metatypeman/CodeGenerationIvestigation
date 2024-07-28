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
        public string StringField;

        public object ObjectProp { get; set; }
        public object ObjectField;
        public List<string> ListStrProp { get; set; }
        public List<string> ListStrField;

    }
}
