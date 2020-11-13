using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoldStarr_Trading.Classes
{
    public class CustomerOrderClass : INotifyPropertyChanged
    {

        #region Properties
        public CustomerClass Customer { get; set; }

        public StockClass Merchandise { get; set; }

        public string OrderDate { get; set; }
        #endregion


        #region Constructors
        public CustomerOrderClass(CustomerClass customerClass, StockClass merchandise, DateTime orderDate)
        {
            this.Customer = customerClass;
            this.Merchandise = merchandise;
            this.OrderDate = orderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion


        #region Methods
        public string GetOrder()
        {
            string customerName = Customer.CustomerName;
            string billingAddress = $"{ Customer.CustomerAddress}\n{Customer.CustomerZipCode}\n{Customer.CustomerCity}";
            string contactCustomer = Customer.CustomerPhone;

            return $"{customerName}\n{billingAddress}\n{contactCustomer}";
        }

        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #endregion

    }
}
