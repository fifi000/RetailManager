using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductModel> _products;
        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();
        private ProductModel _selectedProduct;
        private CartItemModel _selectedCartItem;
        private int _itemQuantity = 1;
        private readonly IProductEndpoint _productEndpoint;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IConfigHelper _config;
        private readonly IWindowManager _window;
        private readonly StatusInfoViewModel _msgBox;

        public SalesViewModel(IProductEndpoint productEndpoint,
                              ISaleEndpoint saleEndpoint,
                              IConfigHelper config,
                              StatusInfoViewModel msgBox,
                              IWindowManager window)
        {
            _productEndpoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _config = config;
            _msgBox = msgBox;
            _window = window;
        }
        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemModel>();
            await LoadProducts();
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;

                if (ex.Message == "Unauthorized")
                {
                    _msgBox.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
                    await _window.ShowDialogAsync(_msgBox, settings: settings);

                }
                else
                {
                    _msgBox.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_msgBox, settings: settings);

                }

                await TryCloseAsync();
            }
        }

        private async Task LoadProducts()
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
        public CartItemModel SelectedCartItem 
        {
            get => _selectedCartItem;
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
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

            Products.ResetItem(Products.IndexOf(SelectedProduct));
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }
        public bool CanRemoveFromCart
        {
            get
            {
                return SelectedCartItem != null;
            }
        }
        public void RemoveFromCart()
        {
            CartItemModel removedItem = SelectedCartItem;

            Cart.Remove(removedItem);
            removedItem.Product.QuantityInStock += removedItem.QuantityInCart;

            Products.ResetItem(Products.IndexOf(removedItem.Product));

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanAddToCart);
            NotifyOfPropertyChange(() => CanCheckOut);
        }
        public bool CanCheckOut
        {
            get
            {
                return Cart.Count > 0;
            }
        }
        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);

            await ResetSalesViewModel();
        }

        private decimal GetSubTotal()
        {
            decimal subTotal = 0;

            subTotal = Cart.Sum(x => x.Product.RetailPrice * x.QuantityInCart);
            
            return subTotal;
        }
        private decimal GetTax()
        {
            decimal taxPercent = (decimal)_config.GetTaxRate() / 100;
            decimal tax = 0;

            tax = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxPercent);

            return tax;
        }

    }
}
