import { createSlice, createAsyncThunk, createSelector  } from "@reduxjs/toolkit";
import productsApi from './../api/productsApi';
import Product from './../components/Product'
import { RootState } from './../store';

type ProductsState = 
{
    products: Product[],
    isProductsLoading: boolean,
    isProductsSuccess: boolean,
    isSuccess: boolean,
    isError: boolean,
}

const initialState : ProductsState = {
    products : [],
    isProductsLoading: false,
    isProductsSuccess: false,
    isSuccess: false,
    isError: false,
};

export const createProduct = createAsyncThunk(
  'products/create',
  async ( product : Product ) => {
    await productsApi.create(product);
    return product;
  }
);

export const getProducts = createAsyncThunk(
    'products/getAll',
  async () => {
    const res = await productsApi.getAll();
    return res.data;
  }
);

export const updateProduct = createAsyncThunk(
    'products/update',
  async ( product : Product ) => {
    await productsApi.update(product);
    return product;
  }
);

export const deleteProduct = createAsyncThunk(
    'products/delete',
  async ( id : string ) => {
    await productsApi.remove(id);
    return id;
  }
);

const productSlice = createSlice({
  name: "product",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
    .addCase(createProduct.fulfilled, (state, action) => {
      state.products.push(action.payload);
      state.isSuccess = true;
    })
    .addCase(createProduct.pending, (state) => {
      state.isSuccess = false;
      state.isError = false;
    })
    .addCase(createProduct.rejected, (state) => {
      state.isError = true;
    })
    .addCase(getProducts.fulfilled, (state, action) => {
      state.products = action.payload;
      state.isProductsLoading = false;
      state.isProductsSuccess = true;
    })
    .addCase(getProducts.pending, (state) => {
      state.isProductsLoading = true;
      state.isProductsSuccess = false;
      state.isError = false;
    })
    .addCase(getProducts.rejected, (state) => {
      state.isError = true;
    })
    .addCase(updateProduct.fulfilled, (state, action) => {
      const index = state.products.findIndex(tutorial => tutorial.id === action.payload.id);
      state.products[index] = action.payload
      state.isSuccess = true;
    })
    .addCase(updateProduct.pending, (state) => {
      state.isSuccess = false;
      state.isError = false;
    })
    .addCase(updateProduct.rejected, (state) => {
      state.isError = true;
    })
    .addCase(deleteProduct.fulfilled, (state, action) => {
      let index = state.products.findIndex(({ id }) => id === action.payload);
      state.products.splice(index, 1);
      state.isSuccess = true;
    })
    .addCase(deleteProduct.pending, (state) => {
      state.isSuccess = false;
      state.isError = false;
    })
    .addCase(deleteProduct.rejected, (state) => {
      state.isError = true;
    });
  },
});

export const getProductsState = (state : RootState) => state.products;
export const getProductIds = createSelector(getProductsState, (products : ProductsState) => products.products.map(p => p.id));

const { reducer } = productSlice;
export default reducer;