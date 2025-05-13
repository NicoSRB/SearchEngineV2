import { Box, Typography, CircularProgress } from "@mui/material";
import { SearchResultItem } from "../../types/SearchTypes";

interface SearchResultsProps {
  results: SearchResultItem[];
  loading: boolean;
}


const SearchResult: React.FC<SearchResultsProps> = ({ results, loading }) => {
  // Håndtering af loading tilstand
  if (loading) return <CircularProgress sx={{ mt: 3 }} />;
  
  // Håndtering af tilfælde, hvor der ikke er fundet resultater
  if (!results.length)
    return <Typography sx={{ mt: 3 }}>No results found.</Typography>;

  return (
    <Box sx={{ mt: 4, textAlign: "left" }}>
      <Typography variant="h6">Search Results:</Typography>

      {/* Gennemgå resultater */}
      {results.map((result, resultIndex) => (
        <div key={resultIndex}>
          <Typography variant="body1" sx={{ fontWeight: "bold" }}>
            Query terms used:
          </Typography>
          <Typography variant="body2">{result.query.join(", ")}</Typography>
          
          {/* Vis eventuelle ignorerede søgeord */}
          {result.ignored.length > 0 && (
            <Typography variant="body2" color="textSecondary">
              Ignored terms: {result.ignored.join(", ")}
            </Typography>
          )}

          <Typography variant="body2" sx={{ mt: 2 }}>
            <strong>Hits:</strong> {result.hits}
          </Typography>

          {/* Tid brugt på søgningen */}
          <Typography variant="body2" color="textSecondary">
            <strong>Time Used:</strong> {result.timeUsed}
          </Typography>

          <ul>
            {/* Vis dokument hits */}
            {result.documentHits.map((hit, hitIndex) => (
              <li key={`${resultIndex}-${hitIndex}`}>
                <strong>Document ID:</strong> {hit.document.mId} <br />
                <strong>Document URL:</strong> <a href={hit.document.mUrl} target="_blank" rel="noopener noreferrer">{hit.document.mUrl}</a> <br />
                <strong>IdxTime:</strong> {hit.document.mIdxTime} <br />
                <strong>Creation Time:</strong> {hit.document.mCreationTime} <br />
                <strong>Number of Hits in Document:</strong> {hit.noOfHits} <br />
              </li>
            ))}
          </ul>
        </div>
      ))}
    </Box>
  );
};

export default SearchResult;
