using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace GoldStarr_Trading.Classes
{
    internal class StoreClass : IMessageToUser
    {
        #region Properties

        private App _app { get; set; }

        #endregion Properties

        #region Constructors

        public StoreClass()
        {
            _app = (App)App.Current;
        }

        #endregion Constructors

        #region Methods

        public async void RemoveFromStock(StockClass merchandise, int stockToRemove)
        {
            foreach (var item in _app.GetDefaultStockList())
            {
                if (item.ItemName == merchandise.ItemName)
                {
                    if (item.Qty - stockToRemove < 0)
                    {
                        MessageToUser("Not enough items in stock, order more from supplier");
                        break;
                    }
                    else
                    {
                        item.Qty -= stockToRemove;
                        await _app.WriteToFile(App.StockFileName, _app.GetDefaultStockList());
                    }
                }
            }
        }

        public void CreateOrder(CustomerClass customer, StockClass merch)
        {
            CultureInfo myCultureInfo = new CultureInfo("sv-SV");
            DateTime orderDate = DateTime.UtcNow;

            CustomerOrderClass customerOrder = new CustomerOrderClass(customer, merch, orderDate);
            _app.GetDefaultCustomerOrdersList().Add(customerOrder);
        }

        /// <summary>
        /// Overload to create a queued order.
        /// </summary>
        /// <param name="customer">A customer object, preferably of the customer who placed the order.</param>
        /// <param name="merch">The merchandise to be shipped</param>
        /// <param name="queueID">What place in line to place the order, generate from querying the ObsColl</param>
        public void CreateOrder(CustomerClass customer, StockClass merch, int amount, int queueID)
        {
            // Define CultureInfo
            CultureInfo cultureInfo = new CultureInfo("sv-SV");
            DateTime dateTimeOfOrder = DateTime.UtcNow;

            QueuedOrder order = new QueuedOrder(customer, new StockClass(merch.ItemName, merch.Supplier, amount), dateTimeOfOrder, queueID);
            _app.QueuedOrders.Add(order);
        }

        public async void RemoveFromDeliveryList(StockClass merchandise, int stockToRemove)
        {
            foreach (var item in _app.GetDefaultDeliverysList())
            {
                if (item.ItemName == merchandise.ItemName)
                {
                    item.Qty -= stockToRemove;
                    await _app.WriteToFile(App.IncomingDeliverysFileName, _app.GetDefaultDeliverysList());
                }
            }
        }

        public async void AddToStock(StockClass merchandise, int stockToAdd)
        {
            int stockToRemove = stockToAdd;

            foreach (var item in _app.GetDefaultStockList())
            {
                if (item.ItemName == merchandise.ItemName)
                {
                    item.Qty += stockToAdd;
                    await _app.WriteToFile(App.StockFileName, _app.GetDefaultStockList());
                    RemoveFromDeliveryList(merchandise, stockToRemove);
                }
            }
        }

        /// <summary>
        /// Method to send all Queued orders
        /// </summary>
        public async void TrySendQO()
        {
            int qCount = _app.QueuedOrders.Count - 1;
            if (_app.QueuedOrders.Count == 0)
            {
                await MessageToUser("No pending orders to send");
            }
            else
            {
                for (int i = qCount; i >= 0; --i)
                {
                    await SendOrder(_app.QueuedOrders[i]);
                }
            }
        }

        /// <summary>
        /// Method to send a queued order
        /// </summary>
        /// <param name="queuedOrder">Object of a queued order</param>
        public async Task SendOrder(QueuedOrder queuedOrder)
        {
            var product = FindProduct(queuedOrder.Merchandise.ItemName);
            if (product.Qty - queuedOrder.Merchandise.Qty >= 0)
            {
                RemoveFromStock(product, queuedOrder.Merchandise.Qty);
                // Remove from collection, the collection starts at 0
                // but IDs at 1 so ID - 1 gets you the correct number
                _app.QueuedOrders.RemoveAt(queuedOrder.QueueID - 1);
                // Update the remaining objects with a new qID
                foreach (var item in _app.QueuedOrders)
                {
                    if (item.QueueID > 1) { item.QueueID -= 1; }
                    else { continue; }
                }
                _app.GetDefaultCustomerOrdersList().Add(queuedOrder.ConvertFromQueued());
                await MessageToUser("Order sent!");
            }
            else
            {
                await MessageToUser("Not enough in stock to send order!");
            }
        }

        private StockClass FindProduct(string merchName)
        {
            StockClass stock = null;
            foreach (var item in _app.GetDefaultStockList())
            {
                if (item.ItemName == merchName)
                {
                    stock = item;
                }
            }
            return stock;
        }

        /// <summary>
        /// Locate a queued order
        /// </summary>
        /// <param name="name">Name of customer</param>
        /// <returns></returns>
        public QueuedOrder FindQueued(string name)
        {
            QueuedOrder queuedOrder = null;
            foreach (var item in _app.QueuedOrders)
            {
                if (item.Customer.CustomerName == name)
                {
                    queuedOrder = item;
                }
            }
            return queuedOrder;
        }

        public async Task MessageToUser(string inputMessage)
        {
            var message = new MessageDialog(inputMessage);
            await message.ShowAsync();
        }

        #endregion Methods
    }
}