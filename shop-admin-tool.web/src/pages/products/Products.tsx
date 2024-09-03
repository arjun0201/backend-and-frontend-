import { useMemo, useState, useEffect } from 'react';

import {
  Table,

  TableBody,

  TableCell,

  TableContainer,

  TableHead,

  TableRow,

  Paper,

  Box,

  Button,

  IconButton,

  Tooltip,

} from '@mui/material';
import TableSearch from './../../components/TableSearch'
import { ThreeDots } from 'react-loader-spinner'

import { ProductModal } from '../../components/ProductModal'
import { productColumnDefinitions, type ProductColumnDefinition } from './../../components/productColumnDefinitions'

import { Delete, Edit, ArrowDownward, ArrowUpward } from '@mui/icons-material';

import Product from '../../components/Product'

import 'react-notifications/lib/notifications.css';

import { useConfirm } from "material-ui-confirm";

import { getProducts, deleteProduct, updateProduct, createProduct, getProductsState } from './../../slices/productSlice'

import { useAppSelector, useAppDispatch } from '../../app/hooks';

const {NotificationContainer, NotificationManager} = require('react-notifications');

const classes = {
  table: {
    minWidth: 650
  }
};

interface SortSettings {
  sort: boolean,
  columnToSort: keyof Product,
  direction: 'asc' | 'desc'
}

const Products = () => {
  const [searched, setSearched] = useState<string>('');

  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [currentEditColumnData, setCurrentEditColumnData] = useState(productColumnDefinitions);
  const [sorting, setSorting] = useState<SortSettings>({ sort: true, columnToSort: 'id', direction: 'asc'})
  const confirm = useConfirm();

  const { 
    isProductsLoading,
    isSuccess,
    isError,
    products,
  } = useAppSelector(getProductsState);
  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(getProducts());
  }, [dispatch])

  const tableData = useMemo(
    () => {
          const searchedProducts = products
            .filter(p => !searched || Object.values(p)
              .some( (v : string) => !!v && v.toString().toLowerCase().includes(searched.toLowerCase())));
          return sorting.sort
            ? searchedProducts.sort((a, b) => {
              if (a[sorting.columnToSort] < b[sorting.columnToSort]) {
                return sorting.direction === 'asc' ? -1 : 1;
              }
              if (a[sorting.columnToSort] > b[sorting.columnToSort]) {
                return sorting.direction === 'asc' ? 1 : -1;
              }
              return 0;
            })
            : searchedProducts;
        },
    [products, searched, sorting] );

  useEffect(() => {
    if(isSuccess)
    {
      NotificationManager.success('Changes saved', 'SaveChanges', 3000);
    }
  }, [isSuccess]);

  useEffect(() => {
    if(isError)
    {
      NotificationManager.error('Error during save operation', 'SaveChanges', 3000);
    }
  }, [isError]);


  const handleCreateNewRow = (product: Product) => {
    dispatch(createProduct(product))
  };

  const handleEditRow = (product: Product) => {
    dispatch(updateProduct(product))
  };

  const handleDeleteRow = (row: Product) => {

    confirm({ description: `Are you sure you want to delete ${row.id}` })
      .then(() => {
        dispatch(deleteProduct(row.id));
      })
      .catch(() => {})
  };

  const handleSort = ( columnKey : keyof Product ) => 
  {
    if(!sorting.sort)
    {
      setSorting({ sort: true, columnToSort: columnKey, direction: 'asc'});
    }
    else if(sorting.columnToSort === columnKey)
    {
      if(sorting.direction === 'asc')
      {
        setSorting({ ...sorting, direction: 'desc'});
      }
      else
      {
        setSorting({ ...sorting, sort: false});
      }
    }
    else
    {
      setSorting({ ...sorting, columnToSort: columnKey});
    }
  }

  return (
    <div style={{textAlign:'center'}}>
      <h1>Shop Admin Tool</h1>
      <NotificationContainer/>
      {isProductsLoading 
        ? <ThreeDots 
            height="80" 
            width="80" 
            radius="9"
            color="#4fa94d" 
            ariaLabel="three-dots-loading"
            visible={true}
            wrapperStyle={{
              display: 'flex',
              justifyContent: 'center'
            }}
            />
        : <Paper>
            <div style={{display: 'flex', justifyContent: 'space-between'}}>
              <Button 
                sx={{
                  'margin': '0.5rem 0rem 1.7rem 0.5rem',
                  'padding': '0rem 1rem 0 1rem'
                }}
                color="secondary"
                variant="contained"
                onClick={() => setCreateModalOpen(true)}
              >Create New Product</Button>
              <TableSearch 
                searchValue={searched}
                onChange={(searchVal : string) => setSearched(searchVal)}
              />
            </div>
            <TableContainer>
              <Table sx={classes.table} aria-label="simple table">
                <TableHead>
                  <TableRow sx={{ fontWeight: 'bold' }}>
                    {productColumnDefinitions
                      .map(c => (<TableCell key={`${c.key}_header`} onClick={() => handleSort(c.key)} style={{ fontWeight: 'bold', cursor: 'pointer' }}>
                        <span>{c.label}</span>
                        {sorting.sort && c.key === sorting.columnToSort && (
                          <>
                            {sorting.direction === 'asc' 
                            ? (<ArrowDownward fontSize='small' sx={{marginBottom: '-0.3rem'}} />) 
                            : (<ArrowUpward fontSize='small' sx={{marginBottom: '-0.3rem'}}/>)}
                          </>
                        )}
                      </TableCell>))
                    }
                    <TableCell key='action_header' style={{ fontWeight: 'bold', textAlign: 'center' }}>Action</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {tableData.map((row) => (
                    <TableRow key={row.id}>
                      {Object.values(row).map((v,i) => ( <TableCell  key={`${row.id}_${i}_body`}>{!searched ? v : getHighlightedText(v.toString(), searched)}</TableCell>))}
                      <TableCell key='action_body' sx={{width: '10%'}}>
                        <Box sx={{display: 'flex', justifyContent: 'space-between'}}>

                          <Tooltip arrow placement="left" title="Edit">

                            <IconButton onClick={() => {
                              const rowMap = new Map<string, string | number>(Object.entries(row));
                              const definitionsWithValues = productColumnDefinitions
                                .map<ProductColumnDefinition>(d => 
                                  ({...d, value: rowMap.has(d.key) 
                                    ? rowMap.get(d.key) as string | number 
                                    : ''}));
                              setCurrentEditColumnData(definitionsWithValues); 
                              setEditModalOpen(true);
                            }}>

                              <Edit />

                            </IconButton>

                          </Tooltip>

                          <Tooltip arrow placement="right" title="Delete">

                            <IconButton color="error" onClick={() => handleDeleteRow(row)}>

                              <Delete />

                            </IconButton>

                          </Tooltip>

                        </Box>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
      }
      <ProductModal

        columns={productColumnDefinitions}

        open={createModalOpen}

        onClose={(cleanModal) => {cleanModal();setCreateModalOpen(false);}}

        onSubmit={handleCreateNewRow}
      />
      <ProductModal

        columns={currentEditColumnData}

        open={editModalOpen}

        onClose={(cleanModal) => {cleanModal();setEditModalOpen(false);setCurrentEditColumnData(productColumnDefinitions)}}

        onSubmit={handleEditRow}

        isEditModal
      />
    </div>
  );
}

const getHighlightedText = (text : string, highlight : string) => {
  const parts = text.split(new RegExp(`(${highlight})`, 'gi'));
  return <span> { parts.map((part, i) => 
      <span key={i} style={part.toLowerCase() === highlight.toLowerCase() ? { 
        backgroundColor: 'rgb(255, 203, 127)',
        borderRadius: '2px',
        color: 'black',
        padding: '2px 1px'
        } : {} }>
          { part }
      </span>)
  } </span>;
}

export default Products