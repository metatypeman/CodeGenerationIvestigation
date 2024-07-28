using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public partial class FirstFactPartPo
    {
        public int IntProp { get; set; }
        public string StringProp { get; set; }
        public ObjectPtr ObjectProp { get; set; }
        public ObjectPtr ListStrProp { get; set; }
        public ObjectPtr _parent;
        public int IntField;
        public string StringField;
        public ObjectPtr ObjectField;
        public ObjectPtr ListStrField;
    }
}
