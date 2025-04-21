import { useState } from "react";
import SearchBar from "./Search/SearchBar";
import SearchResults from "./Search/SearchResult"; 
import { Box, Typography } from "@mui/material";
import axios from "axios";

interface SearchResultsProps {
    query: string[];
    hits: number;
    documentHits: {
      document: {
        mId: number;
        mUrl: string;
        mIdxTime: string;
        mCreationTime: string;
      };
      noOfHits: number;
      missing: [];
      includeTimeStamp: boolean;
    }[]; 
    ignored: [];
    timeUsed: string;
  loading: boolean;
}

const SearchPage: React.FC = () => {
  const [results, setResults] = useState<SearchResultsProps[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async (query: string, maxAmount: number, caseSensitive: boolean, includeTimeStamp: boolean) => {
    if (!query) return;
    setLoading(true);
    setError(null);

    try {
      const response = await axios.get(
        `/api/search/${encodeURIComponent(query)}`, {
          params: {
            maxAmount, 
            caseSensitive,
            includeTimeStamp
          }
        }
      );

      const data = response.data;
      const normalizedResults = Array.isArray(data) ? data : [data];

      setResults(normalizedResults);
    } catch (error) {
      console.error("Error fetching search results:", error);
      setError("Failed to fetch search results.");
      setResults([]);
    }

    setLoading(false);
  };

  return (
    <Box sx={{ maxWidth: 600, margin: "auto", textAlign: "center", mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        Search API
      </Typography>

      {/* SearchBar Component */}
      <SearchBar onSearch={handleSearch} loading={loading} />

      {/* Display Errors */}
      {error && (
        <Typography color="error" sx={{ mt: 2 }}>
          {error}
        </Typography>
      )}

      {/* SearchResults Component */}
      <SearchResults results={results} loading={loading} />
    </Box>
  );
};

export default SearchPage;
