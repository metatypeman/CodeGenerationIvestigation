using System.Text.Json.Serialization;

namespace TestSandBox
{
    [CustomSerialization]
    [Obsolete("Hi!"), JsonSourceGenerationOptions(WriteIndented = true)]
    public partial class SomeClass
    {
        public OtherClass OtherClassProp { get; set; }
        public int IntProp { get; set; }
        public string StrProp { get; set; }
        private List<object> ListField;
    }
}
