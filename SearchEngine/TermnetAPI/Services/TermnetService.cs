using Shared.Model;
using TermnetAPI.Interfaces;

namespace TermnetAPI.Services
{
    public class TermnetService : ITermnetService
    {
        public Task<List<SynonymTermer>> GetSynonymsAsync(string word, List<string> domainNames)
        {
            var mockData = new Dictionary<string, List<SynonymTermer>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Advokat"] = new List<SynonymTermer>
                {
                    new SynonymTermer { Term = "Jurist", Weight = 0.8 },
                    new SynonymTermer { Term = "Resthjælp", Weight = 0.5 }
                },
                ["Hjælp"] = new List<SynonymTermer>
                {
                    new SynonymTermer { Term = "Assistance", Weight = 0.7 },
                    new SynonymTermer { Term = "Bistand", Weight = 0.6 }
                }
            };
            if (mockData.TryGetValue(word, out var synonyms))
            {
                return Task.FromResult(synonyms);
            }
            return Task.FromResult(new List<SynonymTermer>());
        }
    }
 }
