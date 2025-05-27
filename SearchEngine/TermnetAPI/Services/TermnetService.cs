using Shared.Model;
using TermnetAPI.Interfaces;

namespace TermnetAPI.Services
{
    public class TermnetService : ITermnetService
    {

        /// <summary> Gets synonyms on multi dimentional dictionary </summary>
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

            return Task.FromResult(synonyms
                .OrderByDescending(s => s.Weight)
                .Take(2) // Limit to top 2 synonyms
                .ToList());
        }


        private static readonly Dictionary<string, Dictionary<string, List<SynonymTermer>>> DomainDictionaries = new(StringComparer.OrdinalIgnoreCase)
        {   
            ["Time"] = new Dictionary<string, List<SynonymTermer>> (StringComparer.OrdinalIgnoreCase)
            {
                ["Hour"] = new List<SynonymTermer>
                    {
                        new SynonymTermer { Term = "Time", Weight = 0.2 },
                        new SynonymTermer { Term = "TimeZone", Weight = 0.8 },
                        new SynonymTermer { Term = "Country", Weight = 0.1 }
                    }
            },
            ["Help"] = new Dictionary<string, List<SynonymTermer>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Assistance"] = new List<SynonymTermer>
                    {
                        new SynonymTermer { Term = "Something", Weight = 0.1 },
                        new SynonymTermer { Term = "Bistand", Weight = 0.6 }
                    }
            },
            ["Technology"] = new Dictionary<string, List<SynonymTermer>>(StringComparer.OrdinalIgnoreCase)
            {
                ["AI"] = new List<SynonymTermer>
                    {
                        new SynonymTermer { Term = "Artificial Intelligence", Weight = 1.0 },
                        new SynonymTermer { Term = "Machine Learning", Weight = 0.9 }
                    },
               ["Cloud"] = new List<SynonymTermer>
                    {
                        new SynonymTermer { Term = "AWS", Weight = 0.8 },
                        new SynonymTermer { Term = "Azure", Weight = 0.8 },
                        new SynonymTermer { Term = "GCP", Weight = 0.8 }
                    }
            }

        };

    }
 }

