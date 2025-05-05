using Shared.Model;

namespace TermnetAPI.Interfaces
{
    public interface ITermnetService
    {
        Task<List<SynonymTermer>> GetSynonymsAsync(string word, List<string> domainNames);
    }
}
