using System;

namespace GoldStarr_Trading.Classes
{
    /// <summary>
    /// Class to create a queued (IDK how to spell it, could be right, could be kewewewd as well) order.
    /// </summary>
    public class QueuedOrder : CustomerOrderClass
    {

        #region Properties

        private int qID;
        public int QueueID { get { return this.qID; } set { qID = value; OnPropertyChanged(); } }

        #endregion


        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerClass">Customer to send to</param>
        /// <param name="merchandise">Merchandise to send</param>
        /// <param name="orderDate">The date the order was placed.</param>
        /// <param name="qID">Place in queue, starts at 1</param>
        public QueuedOrder(CustomerClass customerClass, StockClass merchandise, DateTime orderDate, int qID) : base(customerClass, merchandise, orderDate)
        {
            base.Customer = customerClass;
            base.Merchandise = merchandise;
            base.OrderDate = orderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            this.qID = qID;
        }
        /// <summary>
        /// A function essientially stripping the QueueID from the object.
        /// </summary>
        /// <returns>Returns a CustomerOrderClass object</returns>
        public CustomerOrderClass ConvertFromQueued()
        {
            // Create a new copy of the order to return
            var convertTo = new CustomerOrderClass(base.Customer, base.Merchandise, DateTime.Parse(base.OrderDate));
            return convertTo;
        }
        #endregion

    }
}
