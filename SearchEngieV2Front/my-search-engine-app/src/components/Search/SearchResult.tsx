  import { Box, Typography, CircularProgress } from "@mui/material";

  interface SearchResultsProps {
    results: {
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
    }[];
    loading: boolean;
  }
  

  const SearchResult: React.FC<SearchResultsProps> =  ({ results, loading }) => {
    if (loading) return <CircularProgress sx={{ mt: 3 }} />;
    if (!results.length) return <Typography sx={{ mt: 3 }}>No results found.</Typography>;

    return (
      <Box sx={{ mt: 4, textAlign: "left" }}>
        <Typography variant="h6">Results:</Typography>
        <ul>
          {results.map((result, resultIndex) => (
          result.documentHits?.map((hit, hitIndex) => (
            <li key={`${resultIndex}-${hitIndex}`}>
          <strong>Hits:</strong> {result.hits} <br />
          <strong>Id</strong> {hit.document.mId} <br />
          <strong>Document URL:</strong> {hit.document.mUrl} <br />
          <strong>IdxTime:</strong> {hit.document.mIdxTime} <br />
          <strong>Creation Time:</strong> {hit.document.mCreationTime} <br />
          <strong>TimeStamp:</strong> {hit.includeTimeStamp}
      </li>
    ))
  ))}
</ul>
      </Box>
    );
  };

  export default SearchResult;
