using GoldStarr_Trading.Classes;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GoldStarr_Trading
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    /// 

    sealed partial class App : Application
    {

        #region Collections
        BaseNotifier baseNotifier = new BaseNotifier();

        public const string CustomerFileName = "Customer.json";
        public const string StockFileName = "Stock.json";
        public const string IncomingDeliverysFileName = "IncomingDeliverys.json";
        public const string CustomerOrdersFileName = "CustomerOrders.json";
        public const string QueuedOrdersFileName = "QueuedOrders.json";
        public const string SuppliersFileName = "Suppliers.json";
        public const string NewItemsFileName = "NewItems.json";



        private ObservableCollection<CustomerClass> Customer { get; set; } //= new ObservableCollection<CustomerClass>();
        private ObservableCollection<StockClass> Stock { get; set; }  //= new ObservableCollection<StockClass>();
        private ObservableCollection<StockClass> IncomingDeliverys { get; set; } //= new ObservableCollection<StockClass>();
        private ObservableCollection<CustomerOrderClass> CustomerOrders { get; set; }  //= new ObservableCollection<CustomerOrderClass>();  

        

        // ObsColl with private backing
        private ObservableCollection<QueuedOrder> queuedOrders;
        public ObservableCollection<QueuedOrder> QueuedOrders
        {
            get => queuedOrders;
            set
            {
                queuedOrders = value;
                baseNotifier.OnPropertyChanged();
            }
        }


        private ObservableCollection<Supplier> suppliers;
        public ObservableCollection<Supplier> Suppliers 
        {
            get => suppliers;
            set
            {
                suppliers = value;
                baseNotifier.OnPropertyChanged();
            }
        }


        private ObservableCollection<StockClass> newItems;
        public ObservableCollection<StockClass> NewItems
        {
            get => newItems;
            set
            {
                newItems = value;
                baseNotifier.OnPropertyChanged();
            }
        }
        #endregion



        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            #region OLD
            //Customer.Add(new CustomerClass("Lisa Underwood", "Smallhill 7", "215 70", "Malmö", "+46 0707-123-456"));
            //Customer.Add(new CustomerClass("Olle Bull", "Djäknegatan 13", "215 71", "Malmö", "0707-234-567"));
            //Customer.Add(new CustomerClass("Ben Knota", "Stengränd 11", "215 72", "Malmö", "0707-345 678"));
            //Customer.Add(new CustomerClass("Vilma Hypoxia", "Nicolaigatan 5", "215 73", "Malmö", "0707 456 789"));
            //Customer.Add(new CustomerClass("Ken Barbie", "Dockgatan 3", "215 74", "Malmö", "0707- 567  890"));

            //Stock.Add(new StockClass("HydroSpanner", "Acme AB", 1));
            //Stock.Add(new StockClass("Airscoop", "Acme AB", 2));
            //Stock.Add(new StockClass("Hyper-transceiver", "Corelian Inc", 3));
            //Stock.Add(new StockClass("Nanosporoid", "Corelian Inc", 4));
            //Stock.Add(new StockClass("Boarding-spike", "Joruba Consortium", 5));

            //IncomingDeliverys.Add(new StockClass("HydroSpanner", "Acme AB", 5));
            //IncomingDeliverys.Add(new StockClass("Airscoop", "Acme AB", 4));
            //IncomingDeliverys.Add(new StockClass("Hyper-transceiver", "Corelian Inc", 3));
            //IncomingDeliverys.Add(new StockClass("Nanosporoid", "Corelian Inc", 2));
            //IncomingDeliverys.Add(new StockClass("Boarding-spike", "Joruba Consortium", 1));

            //CustomerOrders = new ObservableCollection<CustomerOrderClass>();
            #endregion
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {


            Frame rootFrame = Window.Current.Content as Frame;



            #region CustomerCollectionHandling

            DataHelper CustomerHelper = new DataHelper(CustomerFileName);
            Customer = await CustomerHelper.ReadFromFile<ObservableCollection<CustomerClass>>();
            if (Customer == null)
            {
                Customer = new ObservableCollection<CustomerClass>();

                Customer.Add(new CustomerClass("Lisa Underwood", "Smallhill 7", "215 70", "Malmö", "+46 0707-123-456"));
                Customer.Add(new CustomerClass("Olle Bull", "Djäknegatan 13", "215 71", "Malmö", "0707-234-567"));
                Customer.Add(new CustomerClass("Ben Knota", "Stengränd 11", "215 72", "Malmö", "0707-345 678"));
                Customer.Add(new CustomerClass("Vilma Hypoxia", "Nicolaigatan 5", "215 73", "Malmö", "0707 456 789"));
                Customer.Add(new CustomerClass("Ken Barbie", "Dockgatan 3", "215 74", "Malmö", "0707- 567  890"));

                await WriteToFile(CustomerFileName, Customer);
                Customer.CollectionChanged += Customer_CollectionChanged;

            }
            else
            {
                await WriteToFile(CustomerFileName, Customer);
                Customer.CollectionChanged += Customer_CollectionChanged;
            }
            #endregion


            #region StockCollection Handling

            DataHelper StockHelper = new DataHelper(StockFileName);
            Stock = await StockHelper.ReadFromFile<ObservableCollection<StockClass>>();

            if (Stock == null)
            {
                Stock = new ObservableCollection<StockClass>();

                Stock.Add(new StockClass("HydroSpanner", "Acme AB", 1));
                Stock.Add(new StockClass("Airscoop", "Acme AB", 2));
                Stock.Add(new StockClass("Hyper-transceiver", "Corelian Inc", 3));
                Stock.Add(new StockClass("Nanosporoid", "Corelian Inc", 4));
                Stock.Add(new StockClass("Boarding-spike", "Joruba Consortium", 5));

                await WriteToFile(StockFileName, Stock);
                Stock.CollectionChanged += Stock_CollectionChanged;
            }
            else
            {
                await WriteToFile(StockFileName, Stock);
                Stock.CollectionChanged += Stock_CollectionChanged;
            }
            #endregion


            #region IncomingDeliverys Handling

            DataHelper IncomingDeliverysHelper = new DataHelper(IncomingDeliverysFileName);
            IncomingDeliverys = await IncomingDeliverysHelper.ReadFromFile<ObservableCollection<StockClass>>();

            if (IncomingDeliverys == null)
            {

                IncomingDeliverys = new ObservableCollection<StockClass>();

                IncomingDeliverys.Add(new StockClass("HydroSpanner", "Acme AB", 5));
                IncomingDeliverys.Add(new StockClass("Airscoop", "Acme AB", 4));
                IncomingDeliverys.Add(new StockClass("Hyper-transceiver", "Corelian Inc", 3));
                IncomingDeliverys.Add(new StockClass("Nanosporoid", "Corelian Inc", 2));
                IncomingDeliverys.Add(new StockClass("Boarding-spike", "Joruba Consortium", 1));

                await WriteToFile(IncomingDeliverysFileName, IncomingDeliverys);
                IncomingDeliverys.CollectionChanged += IncomingDeliverys_CollectionChanged;

            }
            else
            {
                await WriteToFile(IncomingDeliverysFileName, IncomingDeliverys);
                IncomingDeliverys.CollectionChanged += IncomingDeliverys_CollectionChanged;
            }
            #endregion


            #region CustomerOrdersCollection Handling

            DataHelper CustomerOrdersHelper = new DataHelper(CustomerOrdersFileName);
            CustomerOrders = await CustomerOrdersHelper.ReadFromFile<ObservableCollection<CustomerOrderClass>>();

            if (CustomerOrders == null)
            {
                CustomerOrders = new ObservableCollection<CustomerOrderClass>();

                await WriteToFile(CustomerOrdersFileName, CustomerOrders);
                CustomerOrders.CollectionChanged += CustomerOrders_CollectionChanged;
            }
            else
            {
                await WriteToFile(CustomerOrdersFileName, CustomerOrders);
                CustomerOrders.CollectionChanged += CustomerOrders_CollectionChanged;
            }
            #endregion


            #region QueuedOrdersCollection Handling

            DataHelper QueuedOrdersHelper = new DataHelper(QueuedOrdersFileName);
            QueuedOrders = await QueuedOrdersHelper.ReadFromFile<ObservableCollection<QueuedOrder>>();

            if (QueuedOrders == null)
            {
                QueuedOrders = new ObservableCollection<QueuedOrder>();

                await WriteToFile(QueuedOrdersFileName, QueuedOrders);
                QueuedOrders.CollectionChanged += QueuedOrders_CollectionChanged;
            }
            else
            {
                await WriteToFile(QueuedOrdersFileName, QueuedOrders);
                QueuedOrders.CollectionChanged += QueuedOrders_CollectionChanged;
            }
            #endregion


            #region SupplierCollectionHandling

            DataHelper SupplierHelper = new DataHelper(SuppliersFileName);
            Suppliers = await SupplierHelper.ReadFromFile<ObservableCollection<Supplier>>();
            if (Suppliers == null)
            {
                Suppliers = new ObservableCollection<Supplier>();

                Suppliers.Add(new Supplier("Acme Ab", "Smallhill 7", "215 70", "Malmö", "+46 0707-123-456"));
                Suppliers.Add(new Supplier("Corelian Inc", "Djäknegatan 13", "215 71", "Malmö", "0707-234-567"));
                Suppliers.Add(new Supplier("Joruba Consortium", "Stengränd 11", "215 72", "Malmö", "0707-345 678"));

                await WriteToFile(SuppliersFileName, Suppliers);
                Customer.CollectionChanged += Customer_CollectionChanged;

            }
            else
            {
                await WriteToFile(SuppliersFileName, Suppliers);
                Customer.CollectionChanged += Customer_CollectionChanged;
            }

            #endregion


            #region NewItemsCollectionHandling

            DataHelper NewItemsHelper = new DataHelper(NewItemsFileName);
            NewItems = await NewItemsHelper.ReadFromFile<ObservableCollection<StockClass>>();
            if (NewItems == null)
            {
                NewItems = new ObservableCollection<StockClass>();

                NewItems.Add(new StockClass("HydroSpanner v2.0", "Acme AB"));
                NewItems.Add(new StockClass("Hyper-transceiver v2.0", "Corelian Inc"));
                NewItems.Add(new StockClass("Boarding-spike v2.0", "Joruba Consortium"));

                await WriteToFile(NewItemsFileName, NewItems);
                NewItems.CollectionChanged += Customer_CollectionChanged;

            }
            else
            {
                await WriteToFile(NewItemsFileName, NewItems);
                NewItems.CollectionChanged += Customer_CollectionChanged;
            }

            #endregion










            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }




        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();


        }


        #region Methods

        #region Getters
        public ObservableCollection<CustomerClass> GetDefaultCustomerList()
        {
            return Customer;
        }

        public ObservableCollection<StockClass> GetDefaultStockList()
        {
            return Stock;
        }

        public ObservableCollection<StockClass> GetDefaultDeliverysList()
        {
            return IncomingDeliverys;
        }

        public ObservableCollection<CustomerOrderClass> GetDefaultCustomerOrdersList()
        {
            return CustomerOrders;
        }
        #endregion


        #region CollectionChanged methods
        private async void Customer_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await WriteToFile(CustomerFileName, Customer);
        }

        private async void Stock_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await WriteToFile(StockFileName, Stock);
        }

        private async void IncomingDeliverys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await WriteToFile(IncomingDeliverysFileName, IncomingDeliverys);
        }

        private async void CustomerOrders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await WriteToFile(CustomerOrdersFileName, CustomerOrders);
        }

        private async void QueuedOrders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await WriteToFile(QueuedOrdersFileName, QueuedOrders);
        }

        private async void Suppliers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await WriteToFile(SuppliersFileName, Suppliers);
        }

        private async void NewItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await WriteToFile(NewItemsFileName, NewItems);
        }

        public async Task WriteToFile<T>(string fileName, T collection)
        {
            DataHelper helper = new DataHelper(fileName);
            await helper.WriteToFile(collection);
        }
        #endregion

        #endregion

    }
}
