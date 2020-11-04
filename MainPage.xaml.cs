﻿using GalaSoft.MvvmLight.Command;
using GoldStarr_Trading.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps.Guidance;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GoldStarr_Trading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region Properties
        public ICommand AddButtonCommand { get; set; }
        #endregion

        #region Collections
        ObservableCollection<CustomerClass> CustomerList { get; set; }
        ObservableCollection<StockClass> StockList { get; set; }
        ObservableCollection<CustomerOrderClass> CustomerOrders = new ObservableCollection<CustomerOrderClass>();

        #endregion

        public MainPage()
        {

            this.InitializeComponent();

            DataContext = this;

            StoreClass store = new StoreClass();

            InStockList.ItemsSource = store.GetCurrentStockList();
            StockToAddList.ItemsSource = store.GetCurrentDeliverysList();
            CustomerList = new ObservableCollection<CustomerClass>(store.GetCurrentCustomerList());
            StockList = new ObservableCollection<StockClass>(store.GetCurrentStockList());

            #region OLD            
            //this.CreateOrderTabCustomersComboBox.ItemsSource = store.GetCurrentStockList();
            //InStockList.ItemsSource = dataSets.GetDefaultStockList();
            //StockToAddList.ItemsSource = dataSets.GetDefaultDeliverysList();

            //CustomerList = store.GetCurrentCustomerList();
            //CustomerList = dataSets.GetDefaultCustomerList();
            #endregion

            #region OLD
            //PopulateCustomerComboBox(store);
            //PopulateCreateOrderComboBox(store);
            #endregion


        }



        #region OLD Methods
        //private void PopulateCustomerComboBox(StoreClass store)
        //{
        //    List<string> customers = new List<string>();


        //    foreach (var item in store.Customer)
        //    {
        //        customers.Add(item.Name);
        //    }

        //    //DataContext = customers;

        //    this.CustomersTabComboBox.ItemsSource = customers;
        //    this.CreateOrderTabCustomersComboBox.ItemsSource = customers;
        //}

        //private void PopulateCreateOrderComboBox(StoreClass store)
        //{
        //    List<string> merchandise = new List<string>();


        //    foreach (var item in store.Stock)
        //    {
        //        merchandise.Add(item.ItemName);
        //    }

        //    this.CreateOrderTabItemComboBox.ItemsSource = merchandise;
        //}
        #endregion


        #region Events
        private void AddOrderContent_Click(object sender, RoutedEventArgs e)
        {
            var parent = (sender as Button).Parent;
            CustomerClass customerOrderer = null;
            StockClass stockOrder = null;
            List<StockClass> stockClass = new List<StockClass>();



            string orderQuantity = OrderQuantity.Text;

            customerOrderer = (CustomerClass)CreateOrderTabCustomersComboBox.SelectedValue;

            stockOrder = (StockClass)CreateOrderTabItemComboBox.SelectedValue;
            // Create copy of stockOrder, this will be the object added to order.
            StockClass copyStock = stockOrder;



            if (customerOrderer == null || stockOrder == null || orderQuantity == "")
            {
                if (customerOrderer == null || stockOrder == null)
                {

                    MessageToUser("You must choose a customer and an item");
                }
                else
                {
                    MessageToUser("You must enter an integer");

                }
            }

            else if (int.TryParse(orderQuantity, out int amount) || orderQuantity == "")
            {
                if (CustomerOrders.Count == 0)
                {
                    stockOrder.Qty = stockOrder.Qty - amount;
                    copyStock.Qty = amount;
                    stockClass.Add(copyStock);

                    CustomerOrders.Add(new CustomerOrderClass(customerOrderer, stockClass));

                    //MessageToUser($"You have successfully created a new Customer order for: \n{customerOrderer.CustomerName} with {amount} {stockOrder.ItemName} in it");
                    MessageToUser($"You have successfully created a new Customer order \n\nCustomer:{customerOrderer.CustomerName} \nItem: {stockOrder.ItemName} \nAmount: {amount}");

                    OrderQuantity.Text = "";
                }
                else
                {
                    foreach (var order in CustomerOrders)
                    {
                        if (order.Customer == customerOrderer)
                        {

                            stockOrder.Qty = stockOrder.Qty - amount;
                            copyStock.Qty = amount;
                            stockClass.Add(copyStock);

                            order.Merchandise.Add(copyStock);

                            //MessageToUser($"You have successfully created a new Customer order for: \n{customerOrderer.CustomerName} with {amount} {stockOrder.ItemName} in it");
                            MessageToUser($"You have successfully created a new Customer order \n\nCustomer:{customerOrderer.CustomerName} \nItem: {stockOrder.ItemName} \nAmount: {amount}");

                            OrderQuantity.Text = "";
                        }
                    }
                }
            }
            else
            {
                MessageToUser("You must enter an integer");

                OrderQuantity.Text = "";
            }



            #region For Debug
            //Debug.WriteLine(customerOrderer.CustomerName);
            //Debug.WriteLine(stockOrder.ItemName);
            //Debug.WriteLine(amount);

            //Debug.WriteLine(customerCombo);
            //Debug.WriteLine(merchCombo);
            #endregion

        }
        private void BtnAddDeliveredMerchandise_Click(object sender, RoutedEventArgs e)
        {
            var parent = (sender as Button).Parent;

            TextBox valueToAdd = parent.GetChildrenOfType<TextBox>().First(x => x.Name == "TxtBoxAddQty");

            TextBlock valueToCheck = parent.GetChildrenOfType<TextBlock>().First(x => x.Name == "QTY");
            TextBlock itemToAdd = parent.GetChildrenOfType<TextBlock>().First(x => x.Name == "ItemName");

            string toConvert = valueToAdd.Text;

            //int intValueToAdd = Convert.ToInt32(valueToAdd.Text);
            int intValueToAdd = 0;
            int intValueToCheck = Convert.ToInt32(valueToCheck.Text);

            if (int.TryParse(toConvert, out intValueToAdd))
            {
                if (intValueToAdd > intValueToCheck)
                {
                    MessageToUser($"Enter the correct number of stock to submit, maximum number to submit is: {intValueToCheck} ");
                    valueToAdd.Text = "";
                }
                else
                {
                    StockClass merch = null;

                    foreach (var item in StockList)
                    {
                        if (item.ItemName == itemToAdd.Text)
                        {
                            merch = item;
                        }
                    }


                    StoreClass.AddToStock(merch, intValueToAdd);

                    MessageToUser($"You have added: {valueToAdd.Text} {itemToAdd.Text} to your stock");
                    valueToAdd.Text = "";
                }
            }
            else
            {
                MessageToUser("You must enter an integer");
            }



            Debug.WriteLine(valueToAdd.Text);
            Debug.WriteLine(valueToCheck.Text);



        }
        private void CustomersTabComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string customerName = e.AddedItems[0].ToString();

            CustomerClass newCustomer = CustomerList.First(x => x.CustomerName == customerName);
            CustomerName.Text = newCustomer.CustomerName;
            CustomerPhoneNumber.Text = newCustomer.CustomerPhone;
            CustomerAddress.Text = newCustomer.CustomerAddress;
            CustomerZipCode.Text = newCustomer.CustomerZipCode;
            CustomerCity.Text = newCustomer.CustomerCity;



            #region OLD
            //switch (customerName)
            //{
            //    case "Lisa Underwood":
            //        //var message = new MessageDialog(DataContextProperty.ToString());
            //        var message = new MessageDialog("CustomersTab ComboBox Changed");
            //        await message.ShowAsync();
            //        break;
            //}

            #endregion
        }

        private void CreateOrderTabCustomersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string customerName = e.AddedItems[0].ToString();

            #region OLD
            //switch (customerName)
            //{
            //    case "Lisa Underwood":
            //        //var message = new MessageDialog(DataContextProperty.ToString());
            //        var message = new MessageDialog("CreateOrders Tab ComboBox Changed");
            //        await message.ShowAsync();
            //        break;
            //   }
            #endregion
        }

        private void CreateOrderTabItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion

        public static async void MessageToUser(string inputMessage)
        {
            var message = new MessageDialog(inputMessage);
            await message.ShowAsync();
        }

        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public static class Extensions
    {
        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject start) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                var realItem = item as T;
                if (realItem != null)
                {
                    yield return realItem;
                }

                int count = VisualTreeHelper.GetChildrenCount(item);
                for (int i = 0; i < count; i++)
                {
                    queue.Enqueue(VisualTreeHelper.GetChild(item, i));
                }
            }
        }
    }
}
