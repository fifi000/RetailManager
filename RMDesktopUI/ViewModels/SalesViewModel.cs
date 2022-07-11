using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductModel> _products;
        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();
        private ProductModel _selectedProduct;     
        private int _itemQuantity = 1;
        private readonly IProductEndpoint _productEndpoint;
        private readonly IConfigHelper _config;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper config)
        {
            _productEndpoint = productEndpoint;
            _config = config;
        }

        protected override async void OnViewLoaded(object view)
        {
            var products = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(products);
        }

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }
        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }
        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set 
            { 
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value; 
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }
        public string SubTotal 
        {
            get
            {
                return GetSubTotal().ToString("C");
            }
        }
        public string Tax
        {
            get
            {
                return GetTax().ToString("C");
            }
        }
        public string Total
        {
            get
            {
                return (GetSubTotal() + GetTax()).ToString("C");
            }
        }

        public bool CanAddToCart 
        {
            get
            {
                bool output = false;

                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }
        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                Cart.ResetItem(Cart.IndexOf(existingItem));
            }
            else
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }

            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }
        public bool CanRemoveFormCart
        {
            get
            {
                bool output = false;

                // Make sure something is selected

                return output;
            }
        }
        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }
        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                // Make sure something is in the cart

                return output;
            }
        }
        public void CheckOut()
        {

        }

        private decimal GetSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += item.Product.RetailPrice * item.QuantityInCart;
            }

            return subTotal;
        }
        private decimal GetTax()
        {
            decimal taxPercent = (decimal)_config.GetTaxRate() / 100;
            decimal tax = 0;

            foreach (var item in Cart)
            {
                if (item.Product.IsTaxable)
                {
                    tax += item.Product.RetailPrice * item.QuantityInCart * taxPercent;
                }
            }

            return tax;
        }

    }
}
