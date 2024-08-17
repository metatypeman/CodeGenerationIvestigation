using NLog;
using TestSandBox.Serialization;

namespace TestSandBox.SerializedObjects
{
    public class OtherLoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext>
    {
        public static OtherLoggedCodeChunkFunctorWithoutResult<TGlobalContext, TLocalContext> Run(ILogger logger, string codeChunksContextId, TGlobalContext globalContext,
            Func<ILogger, TGlobalContext, TLocalContext, int> action,
            IDisposable context, ISerializer threadPool)
        {
            throw new NotImplementedException();
        }

        public OtherLoggedCodeChunkFunctorWithoutResult(ILogger logger, string codeChunksContextId, TGlobalContext globalContext,
            Func<ILogger, TGlobalContext, TLocalContext, int> action,
            IDisposable context, ISerializer threadPool)
        {
        }
    }
}
