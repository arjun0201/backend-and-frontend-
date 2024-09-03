import { useAppSelector } from './../app/hooks'
import { Formik, FormikHelpers } from 'formik'
import * as yup from 'yup'

import {

  Button,

  Dialog,

  DialogActions,

  DialogContent,

  DialogTitle,

  Stack,

  TextField,

} from '@mui/material';

import Product from './Product'

import { type ProductColumnDefinition } from './productColumnDefinitions'
import { getProductIds } from './../slices/productSlice'

export interface ProductModalProps {

  columns: ProductColumnDefinition[];

  onClose: (cleandData: Function) => void;

  onSubmit: (values: Product) => void;

  open: boolean;

  isEditModal?: boolean;
  
}

export const ProductModal = ({

    open,
  
    columns,
  
    onClose,
  
    onSubmit,

    isEditModal
  
  }: ProductModalProps) => {

    const initialValues = columns.reduce((acc, column) => {
  
      acc[column.key] = column.value;
  
      return acc;

    }, {} as any);

    const getAllProductIds = useAppSelector(getProductIds);
    
    const schema = yup.object().shape({
      id: yup.string().required('Id is mandatory field')
      .test(
        'checkUniqueId',
        'Too many bags for selected class',
        function (id) {
          const { path, createError } = this
          return (
            isEditModal || !getAllProductIds.some(i => i === id)) ||
            createError({
              path,
              message: 'Id must be unique'
            })
        }
      ),
      name: yup.string().trim().required('Name is mandatory field'),
      brand: yup.string().required('Brand is mandatory field'),
      price: yup
        .number()
        .typeError('Price must be a number')
        .moreThan(0, 'Price must be greater than zero')
        .required('Price is mandatory field'),
      description: yup.string().required('Description is mandatory field'),
      stock: yup
        .number()
        .typeError('Stock must be a number')
        .min(0, 'Stock cannot be positive value')
        .max(Number.MAX_SAFE_INTEGER, 'Stock cannot be bigger than 1000000000000000')
        .required('Stock is mandatory field'),
    })

    const onFormSubmit = async (product : Product, { resetForm } : FormikHelpers<Product>) => {
      
      onSubmit(product)
  
      onClose(resetForm);
    }
  
    return (
  
      <Dialog open={open}>
  
        <DialogTitle textAlign="center">{`${isEditModal ? 'Edit' : 'Create'} Product`}</DialogTitle>
  
        

        <Formik
          initialValues={initialValues}
          validationSchema={schema}
          onSubmit={onFormSubmit}
        >
        {({
          values,
          errors,
          touched,
          handleChange,
          handleSubmit,
          handleBlur,
          resetForm
        }) => (
          <>
            <form onSubmit={handleSubmit}>
              <DialogContent>
      
                <Stack
      
                  sx={{
      
                    width: '100%',
      
                    minWidth: { xs: '300px', sm: '360px', md: '400px' },
      
                    gap: '1.5rem',
      
                  }}
      
                >
      
                  {columns.map((column) => (
      
                    <TextField

                      disabled={isEditModal && column.isId != null && column.isId}

                      key={column.key}
      
                      label={column.label}
      
                      name={column.key}
      
                      error={errors &&  touched[column.key] && !!errors[column.key]}
      
                      value={values[column.key]}
                      
                      onChange={handleChange}
                      
                      onBlur={handleBlur}
                      helperText={
                        errors && touched[column.key] && errors[column.key]
                          ? errors[column.key]
                          : ''
                      }
      
                    />
      
                  ))}
      
                </Stack>
      
    
              </DialogContent>
        
              <DialogActions sx={{ p: '1.25rem' }}>
        
                <Button onClick={() => onClose(resetForm)}>Cancel</Button>
        
                <Button color={`${isEditModal ? 'primary' : 'secondary'}`} type='submit' variant='contained'>
        
                  {`${isEditModal ? 'Save' : 'Create'}`}
        
                </Button>
        
              </DialogActions>
            </form>
          </>
        )}
        </Formik>
  
      </Dialog>
  
    );
  };
