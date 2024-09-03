import { act, render, screen, cleanup, fireEvent, within } from '@testing-library/react';
import { Provider } from 'react-redux';
import { store } from './../../store';
import Products from './Products';
import config from './../../api/config'
import { ConfirmProvider } from "material-ui-confirm";

import { rest } from 'msw'
import { setupServer } from 'msw/node'

export const handlers = [
  rest.get(`${config.API_URL}/products`, (req : any, res : any, ctx : any) => {
    return res(ctx.json(useGetProductsQueryDataMock.data), ctx.delay(5))
  }),
  rest.options(`${config.API_URL}/products`, (req : any, res : any, ctx : any) => {
    return res(ctx.status(204), ctx.delay(5))
  }),
  rest.post(`${config.API_URL}/products`, (req : any, res : any, ctx : any) => {
    return res(ctx.status(201), ctx.delay(5))
  }),
  rest.patch(`${config.API_URL}/products/*`, (req : any, res : any, ctx : any) => {
    return res(ctx.status(200), ctx.delay(5))
  }),
  rest.delete(`${config.API_URL}/products/*`, (req : any, res : any, ctx : any) => {
    return res(ctx.status(200), ctx.delay(5))
  }),
]

const server = setupServer(...handlers)

beforeAll(() => server.listen())

afterEach(() => { cleanup(); server.resetHandlers()})

afterAll(() => {cleanup(); server.close()})

const useGetProductsQueryDataMock = {
  isLoading: false,
  data: [
    { 
      id: 'id1',
      name: 'name1',
      brand: 'brand1',
      price: 222,
      description: 'description1',
      stock: 321,
    },
    { 
      id: 'id2',
      name: 'name2',
      brand: 'brand2',
      price: 111,
      description: 'description2',
      stock: 321,
    }
  ]
}

test('<Products/> renders header and loadingContainer', () => {
  render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );

  expect(screen.getByText(/Shop Admin Tool/i)).toBeInTheDocument();
  expect(screen.queryByRole('row')).not.toBeInTheDocument();
  expect(screen.getByRole('status')).toBeInTheDocument();
});

test('<Products/> renders products', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));
  expect(screen.getAllByRole('row')).toHaveLength(3);
});

test('<Products/> delete product', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));

  await act(async () => await fireEvent.click(screen.getAllByRole('button', { name: 'Delete' })[0]));
  await act(async () => await fireEvent.click(screen.getByRole('button', { name: 'Ok' })));
  await act(async () => await new Promise((r) => setTimeout(r, 300)));
  
  expect(screen.getAllByRole('row')).toHaveLength(2);
});

test('<Products/> edit product', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));

  await act(async () => await fireEvent.click(screen.getAllByRole('button', { name: 'Edit' })[0]));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Description' })[0], 
    {target: {value: 'testEditDescription'}}));
  await act(async () => await fireEvent.click(screen.getByRole('button', { name: 'Save' })));
  await act(async () => await new Promise((r) => setTimeout(r, 300)));
  
  expect(screen.getByRole('cell', { name: 'testEditDescription' })).toBeInTheDocument();
});

test('<Products/> create product', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));

  await act(async () => await fireEvent.click(screen.getAllByRole('button', { name: 'Create New Product' })[0]));

  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'ID' })[0], 
    {target: {value: 'testId25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Name' })[0], 
    {target: {value: 'testName25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Brand' })[0], 
    {target: {value: 'testBrand25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Price(EUR)' })[0], 
    {target: {value: '25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Description' })[0], 
    {target: {value: 'testDescription25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Stock' })[0], 
    {target: {value: '25'}}));
  await act(async () => await fireEvent.click(screen.getByRole('button', { name: 'Create' })));
  await act(async () => await new Promise((r) => setTimeout(r, 300)));
  
  expect(screen.getByRole('cell', { name: 'testDescription25' })).toBeInTheDocument();
  expect(screen.getAllByRole('row')).toHaveLength(4);
});

test('<Products/> create product check uniqueId validation', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));

  await act(async () => await fireEvent.click(screen.getAllByRole('button', { name: 'Create New Product' })[0]));
  expect(screen.queryByText('Id must be unique')).not.toBeInTheDocument();

  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'ID' })[0], 
    {target: {value: 'id2'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Name' })[0], 
    {target: {value: 'testName25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Brand' })[0], 
    {target: {value: 'testBrand25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Price(EUR)' })[0], 
    {target: {value: '25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Description' })[0], 
    {target: {value: 'testDescription25'}}));
  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Stock' })[0], 
    {target: {value: '25'}}));
  await act(async () => await fireEvent.click(screen.getByRole('button', { name: 'Create' })));
  expect(screen.getByText('Id must be unique')).toBeInTheDocument();
});

test('<Products/> search products', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));
  
  await act(async () => await fireEvent.change(screen.getByRole('textbox'), 
    {target: {value: 'me1'}}));
  await act(async () => await new Promise((r) => setTimeout(r, 300)));
  expect(screen.getAllByRole('row')).toHaveLength(2);
  
  await act(async () => await fireEvent.change(screen.getByRole('textbox'), 
    {target: {value: 'me1123123'}}));
  await act(async () => await new Promise((r) => setTimeout(r, 300)));
  expect(screen.getAllByRole('row')).toHaveLength(1);
});

test('<Products/> check sorting', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));
  
  await act(async () => await fireEvent.click(screen.getByText('Price(EUR)')));
  await act(async () => await fireEvent.click(screen.getByText('Price(EUR)')));
  await act(async () => await new Promise((r) => setTimeout(r, 300)));

  const row = screen.getAllByRole('row')[1]
  expect(within(row).getByText('222')).toBeInTheDocument();
});

test('<Products/> edit product check validation', async () => {
  await render(
    <Provider store={store}>
      <ConfirmProvider>
        <Products />
      </ConfirmProvider>
    </Provider>
  );
  await act(async () => await new Promise((r) => setTimeout(r, 100)));

  await act(async () => await fireEvent.click(screen.getAllByRole('button', { name: 'Edit' })[0]));
  expect(screen.queryByText('Price is mandatory field')).not.toBeInTheDocument();


  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Price(EUR)' })[0], 
    {target: {value: ''}}));
  await act(async () => await fireEvent.click(screen.getByRole('button', { name: 'Save' })));
  expect(screen.getByText('Price is mandatory field')).toBeInTheDocument();


  await act(async () => await fireEvent.change(screen.getAllByRole('textbox', { name: 'Price(EUR)' })[0], 
    {target: {value: '333'}}));
  await act(async () => await fireEvent.click(screen.getByRole('button', { name: 'Save' })));
  await act(async () => await new Promise((r) => setTimeout(r, 300)));
  
  expect(screen.queryByText('Price is mandatory field')).not.toBeInTheDocument();
});
