namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class Fact : IDisposable
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
