import { FC } from 'react';

import TextField from '@mui/material/TextField';
import IconButton from '@mui/material/IconButton';

import SearchIcon from '@mui/icons-material/Search';

const classes = {
  root: {
    display: "flex",
    alignItems: "center",
    marginRight: "0.5rem"
    },
    input: {
    padding: "0.5rem 0.25rem"
    }
};

const TableSearch: FC<{ onChange: (value: string) => void, 
  searchValue: string,
} > = 
({
  onChange,
  searchValue
}) => {

  return (
    <div style={classes.root}>
      <TextField
        sx = { classes.input }
        onChange={event => onChange(event.target.value)}
        placeholder="Search..."
        value = { searchValue }
      />
      <IconButton>
        <SearchIcon />
      </IconButton>
    </div>
  );
};

export default TableSearch;