namespace TestSandBox.SerializedObjects
{
    [SocSerialization]
    public partial class Fact : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
