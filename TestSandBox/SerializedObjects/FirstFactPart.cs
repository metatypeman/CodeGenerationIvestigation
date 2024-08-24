using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class FirstFactPart
    {
        public FirstFactPart(int someParam) 
        {
        }

        private void SomeMethod()
        {
        }

        [SocPostDeserializationMethod]
        private void AfterDeserializationMethod()
        {
        }

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

        public List<string> ListStrPropWithInit { get; set; } = new List<string>();
        public List<string> ListStrFieldWithInit = new List<string>();

        [SocNoSerializable]
        public int NoSerializedIntProp { get; set; }

        [SocNoSerializable]
        public int NoSerializedIntField;

        [SocSerializableActionKey]
        public string _id;

        public Action _action;
        public Func<int, int> _func;
    }
}
