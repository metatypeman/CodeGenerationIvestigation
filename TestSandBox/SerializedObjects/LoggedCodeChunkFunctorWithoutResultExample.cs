using NLog;
using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public class LoggedCodeChunkFunctorWithoutResultExample
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public void SomeRun()
        {
            IDisposable disposable = null;
            ISerializer serializer = null;

            var globalContext1 = 16;

            LoggedCodeChunkFunctorWithoutResult<int, float>.Run(_logger, "E45B942C-6B8A-4D3E-A842-0F0C82DE7C5E", globalContext1, (ILogger loggerValue, int globalContextValue, float localContextValue) => {
                var a = 17;
            }, disposable, serializer);

            var globalContext2 = "Hi!";

            var functor = new LoggedCodeChunkFunctorWithoutResult<string, decimal>(_logger, "36C1EDC4-6FCE-43C1-B1C1-BD6B579F3C00", globalContext2, (ILogger loggerValue, string globalContextValue, decimal localContextValue) => {
                var a = 18;
            }, disposable, serializer);

            var globalContext3 = 25;

            LoggedCodeChunkFunctorWithoutResult<int, List<float>>.Run(_logger, "A381E840-A6F0-48D0-9009-433C44E2A310", globalContext3, (ILogger loggerValue, int globalContextValue, List<float> localContextValue) => {
                var a = 20;
            }, disposable, serializer);

            var globalContext4 = 30;

            OtherLoggedCodeChunkFunctorWithoutResult<int, float>.Run(_logger, "2442FEE6-F0D7-4F4A-90E9-2B5056AC2843", globalContext4, int (ILogger loggerValue, int globalContextValue, float localContextValue) => {
                var a = 36;

                return a;
            }, disposable, serializer);
        }
    }
}
