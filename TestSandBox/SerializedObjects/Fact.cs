using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    //[SocBasePlainObject("Disposable_T")]
    public partial class Fact : /*BaseSomeClass,*/ IDisposable
    {
        public Fact()
        {
        }

        public FirstFactPart FactPart { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
