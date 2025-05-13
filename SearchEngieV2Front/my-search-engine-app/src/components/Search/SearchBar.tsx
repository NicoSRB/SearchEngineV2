import { useState } from "react";
import {
  TextField,
  Button,
  Slider,
  Box,
  CircularProgress,
  Typography,
  Checkbox,
  FormControlLabel,
  List,
  ListItem,
} from "@mui/material";

interface SearchBarProps {
  onSearch: (
    query: string,
    maxWords: number,
    caseSensitive: boolean,
    includeTimeStamp: boolean,
    domains: string[]
  ) => void;
  loading: boolean;
}

const termNets = ["Time", "Help"];

const SearchBar: React.FC<SearchBarProps> = ({ onSearch, loading }) => {
  const [query, setQuery] = useState<string>("");
  const [maxWords, setMaxWords] = useState<number>(10);
  const [caseSensitive, setCaseSensitive] = useState<boolean>(false);
  const [includeTimeStamp, setIncludeTimeStamp] = useState<boolean>(false);
  const [selectedTermnets, setSelectedTermnets] = useState<string[]>([]);

  const handleSubmit = () => {
    onSearch(
      query,
      maxWords,
      caseSensitive,
      includeTimeStamp,
      selectedTermnets
    );
  };

  const handleTermnetToggle = (termnet: string) => {
    setSelectedTermnets((prev) =>
      prev.includes(termnet)
        ? prev.filter((t) => t !== termnet)
        : [...prev, termnet]
    );
  };

  return (
    <Box sx={{ textAlign: "center", mt: 2 }}>
      <TextField
        label="Search Query"
        variant="outlined"
        fullWidth
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        sx={{ mb: 2 }}
      />
      <Typography gutterBottom>Max Words: {maxWords}</Typography>
      <Slider
        value={maxWords}
        onChange={(_, newValue) => setMaxWords(newValue as number)}
        step={1}
        marks
        min={1}
        max={20}
        sx={{ mb: 2 }}
      />

      <FormControlLabel
        control={
          <Checkbox
            checked={caseSensitive}
            onChange={(e) => setCaseSensitive(e.target.checked)}
          />
        }
        label="Case Sensitive"
      />
      <FormControlLabel
        control={
          <Checkbox
            checked={includeTimeStamp}
            onChange={(e) => setIncludeTimeStamp(e.target.checked)}
          />
        }
        label="Include Timestamps"
      />
      <Typography sx={{ mt: 2 }}>Select TermNets:</Typography>
      <List dense>
        {termNets.map((termnet) => (
          <ListItem key={termnet} disablePadding>
            <FormControlLabel
              control={
                <Checkbox
                  checked={selectedTermnets.includes(termnet)}
                  onChange={() => handleTermnetToggle(termnet)}
                />
              }
              label={termnet}
            />
          </ListItem>
        ))}
      </List>

      <br></br>
      <Button
        variant="contained"
        color="primary"
        onClick={handleSubmit}
        disabled={loading}
      >
        {loading ? <CircularProgress size={24} /> : "Search"}
      </Button>
    </Box>
  );
};

export default SearchBar;
