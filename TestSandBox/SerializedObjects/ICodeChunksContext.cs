namespace TestSandBox.SerializedObjects
{
    public interface ICodeChunksContext
    {
        void CreateCodeChunk(string chunkId, Action action);
        void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action);
    }
}
