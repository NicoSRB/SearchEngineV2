using Shared.Model;
using TermnetAPI.Interfaces;

namespace TermnetAPI.Services
{
    public class TermnetService : ITermnetService
    {
        public Task<List<SynonymTermer>> GetSynonymsAsync(string word, List<string> domainNames)
        {
            var synonyms = new List<SynonymTermer>();

            foreach (var domain in domainNames)
            {
                if (DomainDictionaries.TryGetValue(domain, out var dictionary))
                {
                    if (dictionary.TryGetValue(word, out var domainSynonyms))
                    {
                        synonyms.AddRange(domainSynonyms);
                    }
                }
            }

            return Task.FromResult(synonyms);
        }


        private static readonly Dictionary<string, Dictionary<string, List<SynonymTermer>>> DomainDictionaries = new(StringComparer.OrdinalIgnoreCase)
        {   
            ["Time"] = new Dictionary<string, List<SynonymTermer>> (StringComparer.OrdinalIgnoreCase)
            {
                ["Hour"] = new List<SynonymTermer>
                    {
                        new SynonymTermer { Term = "Time", Weight = 0.8 },
                        new SynonymTermer { Term = "TimeZone", Weight = 0.5 }
                    }
            },
            ["Help"] = new Dictionary<string, List<SynonymTermer>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assistance"] = new List<SynonymTermer>
                    {
                        new SynonymTermer { Term = "Something", Weight = 0.7 },
                        new SynonymTermer { Term = "Bistand", Weight = 0.6 }
                    }
            }
        };

    }
 }

