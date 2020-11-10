﻿using System;
using System.Globalization;
using Windows.UI.Popups;

namespace GoldStarr_Trading.Classes
{
    class StoreClass
    {
        #region Properties
        private App _app { get; set; }
        #endregion

        #region Constructors
        public StoreClass()
        {
            _app = (App)App.Current;
        }
        #endregion

        #region Methods

        public void RemoveFromStock(StockClass merchandise, int stockToRemove)
        {

            foreach (var item in _app.GetDefaultStockList())
            {

                if (item.ItemName == merchandise.ItemName)
                {
                    if (item.Qty - stockToRemove < 0)
                    {
                        ShowMessage("Not enough items in stock, order more from supplier");
                        break;
                    }
                    else
                    {
                        item.Qty -= stockToRemove;
                        _app.GetDefaultStockList().CollectionChanged += _app.Stock_CollectionChanged;
                    }
                }
            }

        }

        public void CreateOrder(CustomerClass customer, StockClass merch)
        {
            CultureInfo myCultureInfo = new CultureInfo("sv-SV");
            DateTime orderDate = DateTime.Now;

            CustomerOrderClass customerOrder = new CustomerOrderClass(customer, merch, orderDate);
            _app.GetDefaultCustomerOrdersList().Add(customerOrder);
            _app.GetDefaultCustomerOrdersList().CollectionChanged += _app.CustomerOrders_CollectionChanged;
        }
        /// <summary>
        /// Overload to create a queued order.
        /// </summary>
        /// <param name="customer">A customer object, preferably of the customer who placed the order.</param>
        /// <param name="merch">The merchandise to be shipped</param>
        /// <param name="queueID">What place in line to place the order, generate from querying the ObsColl</param>
        public void CreateOrder(CustomerClass customer, StockClass merch, int queueID) 
        {
        }

        public void RemoveFromDeliveryList(StockClass merchandise, int stockToRemove)
        {
            foreach (var item in _app.GetDefaultDeliverysList())
            {

                if (item.ItemName == merchandise.ItemName)
                {
                    item.Qty -= stockToRemove;
                }
            }

        }

        public void AddToStock(StockClass merchandise, int stockToAdd)
        {
            int stockToRemove = stockToAdd;

            foreach (var item in _app.GetDefaultStockList())
            {
                if (item.ItemName == merchandise.ItemName)
                {
                    item.Qty += stockToAdd;
                    RemoveFromDeliveryList(merchandise, stockToRemove);
                }
            }

        }

        public static async void ShowMessage(string inputMessage)
        {
            var message = new MessageDialog(inputMessage);
            await message.ShowAsync();
        }

        #endregion

    }
}
