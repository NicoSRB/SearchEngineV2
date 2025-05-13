namespace SeachEngineAPI.Interfaces
{
    public interface ITermnetClient
    {
        Task<Dictionary<string, List<string>>> ExpandQueryAsync(string query, string[] domains);
    }
}
