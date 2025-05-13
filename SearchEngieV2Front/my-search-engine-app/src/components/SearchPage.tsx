import { useState } from "react";
import SearchBar from "./Search/SearchBar";
import SearchResults from "./Search/SearchResult";
import { Box, Typography } from "@mui/material";
import axios from "axios";

// ✅ Define your result item structure
interface SearchResultItem {
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
    missing: string[];
    includeTimeStamp: boolean;
    termnet: string[];
  }[];
  ignored: string[];
  timeUsed: string;
}

const SearchPage: React.FC = () => {
  const [results, setResults] = useState<SearchResultItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async (
  query: string,
  maxAmount: number,
  caseSensitive: boolean,
  includeTimeStamp: boolean,
  termnet: string[]
) => {
  if (!query) return;

  setLoading(true);
  setError(null);

  try {
    const params: any = {
      query,
      maxAmount,
      caseSensitive,
      includeTimeStamp,
    };

    // Only add domains if any are selected
    if (termnet.length > 0) {
      params.domains = termnet.join(",");
    }

    const response = await axios.get("/api/search", { params });

    const data = response.data;
    const normalizedResults = Array.isArray(data) ? data : [data];

    console.log("Received data from backend:", normalizedResults);
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

      <SearchBar onSearch={handleSearch} loading={loading} />

      {error && (
        <Typography color="error" sx={{ mt: 2 }}>
          {error}
        </Typography>
      )}

      <SearchResults results={results} loading={loading} />
    </Box>
  );
};

export default SearchPage;
