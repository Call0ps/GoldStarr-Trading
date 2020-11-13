using GoldStarr_Trading.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace GoldStarr_Trading
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page, INotifyPropertyChanged, IMessageToUser
	{
		StoreClass store;
		private App _app;


		public MainPage()
		{
			this.InitializeComponent();
			store = new StoreClass();
			_app = (App)App.Current;
		}


		#region Events

		private void AddOrderContent_Click(object sender, RoutedEventArgs e)
		{
			var parent = (sender as Button).Parent;
			CustomerClass customerOrderer = null;
			StockClass stockOrder = null;
			DateTime orderDate = DateTime.UtcNow;

			string orderQuantity = OrderQuantity.Text;
			int.TryParse(orderQuantity, out int amount);
			customerOrderer = (CustomerClass)CreateOrderTabCustomersComboBox.SelectedValue;
			stockOrder = (StockClass)CreateOrderTabItemComboBox.SelectedValue;

			if (customerOrderer == null || stockOrder == null)
			{
				MessageToUser("You must choose a customer and an item");
			}
			else if (orderQuantity == "" || orderQuantity == "" || amount == 0)
			{
				MessageToUser("You must enter an integer");
			}
			else
			{

				if (orderQuantity != "" && stockOrder.Qty - amount >= 0)
				{
					// if no orders are present, simply add an order to the collection.
					if (_app.GetDefaultCustomerOrdersList().Count == 0)
					{
						store.RemoveFromStock(stockOrder, amount);

						StockClass order = new StockClass(stockOrder.ItemName, stockOrder.Supplier, amount);

						store.CreateOrder(customerOrderer, order);

						MessageToUser($"You have successfully created a new Customer order \n\nCustomer: {customerOrderer.CustomerName} \nItem: {order.ItemName} " +
									  $"\nAmount: {order.Qty} \nOrderdate: {orderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}");

						CreateOrderTabCustomersComboBox.SelectedIndex = -1;
						CreateOrderTabItemComboBox.SelectedIndex = -1;
						OrderQuantity.Text = "";
					}

					// Otherwise create a new order object, prepared for future functionality
					else
					{
						store.RemoveFromStock(stockOrder, amount);

						StockClass order = new StockClass(stockOrder.ItemName, stockOrder.Supplier, amount);

						store.CreateOrder(customerOrderer, order);

						MessageToUser($"You have successfully created a new Customer order \n\nCustomer: {customerOrderer.CustomerName} \nItem: {order.ItemName} " +
									  $"\nAmount: {order.Qty} \nOrderdate: {orderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}");

						CreateOrderTabCustomersComboBox.SelectedIndex = -1;
						CreateOrderTabItemComboBox.SelectedIndex = -1;
						OrderQuantity.Text = "";
					}
				}
				else
				{
					int currQ = _app.QueuedOrders.Count + 1;
					store.CreateOrder(customerOrderer, stockOrder, amount, currQ);
					MessageToUser($"You have successfully created a new Customer order \n\nCustomer: {customerOrderer.CustomerName} \nItem: {stockOrder.ItemName} " +
								  $"\nAmount: {amount} \nOrderdate: {orderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")} \nYour order is placed at number {currQ} in the queue.");
					CreateOrderTabCustomersComboBox.SelectedIndex = -1;
					CreateOrderTabItemComboBox.SelectedIndex = -1;
					OrderQuantity.Text = "";
				}
			}
		}

		private void BtnAddDeliveredMerchandise_Click(object sender, RoutedEventArgs e)
		{
			var parent = (sender as Button).Parent;


			TextBox valueToAdd = parent.GetChildrenOfType<TextBox>().First(x => x.Name == "TxtBoxAddQty");
			TextBlock valueToCheck = parent.GetChildrenOfType<TextBlock>().First(x => x.Name == "QTY");
			TextBlock itemToAdd = parent.GetChildrenOfType<TextBlock>().First(x => x.Name == "ItemName");

			string toConvert = valueToAdd.Text;
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

					foreach (var item in _app.GetDefaultStockList())
					{
						if (item.ItemName == itemToAdd.Text)
						{
							merch = item;
						}
					}

					store.AddToStock(merch, intValueToAdd);

					MessageToUser($"You have added: {valueToAdd.Text} {itemToAdd.Text} to your stock");
					valueToAdd.Text = "";

				}
			}
			else
			{
				MessageToUser("You must enter an integer");
			}
		}

		private void CustomersTabComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string customerName = e.AddedItems[0].ToString();

			CustomerClass newCustomer = _app.GetDefaultCustomerList().First(x => x.CustomerName == customerName);
			CustomerName.Text = newCustomer.CustomerName;
			CustomerPhoneNumber.Text = newCustomer.CustomerPhone;
			CustomerAddress.Text = newCustomer.CustomerAddress;
			CustomerZipCode.Text = newCustomer.CustomerZipCode;
			CustomerCity.Text = newCustomer.CustomerCity;
			CustomerEmail.Text = newCustomer.CustomerEmail;
		}

		private void CustomerAddButton_Click(object sender, RoutedEventArgs e)
		{

			if (AddNewCustomerName.Text == "" || AddNewCustomerName.Text == " " || AddNewCustomerPhoneNumber.Text == "" || AddNewCustomerPhoneNumber.Text == " " || AddNewCustomerAddress.Text == "" || AddNewCustomerAddress.Text == " " || AddNewCustomerZipCode.Text == "" || AddNewCustomerZipCode.Text == "" || AddNewCustomerCity.Text == "" || AddNewCustomerCity.Text == " ")
			{
				MessageToUser("All textboxes must be filled in");

			}
			else
			{

				#region Variables
				string name = AddNewCustomerName.Text;
				string phone = AddNewCustomerPhoneNumber.Text;
				string address = AddNewCustomerAddress.Text;
				string zipCode = AddNewCustomerZipCode.Text;
				string city = AddNewCustomerCity.Text;
				string email = null;
				email = AddNewCustomerEmail.Text;
				#endregion

				#region Regex
				Regex regexToCheckName = new Regex(@"^([A-ZÅÄÖ]\w*[a-zåäö]+\s[A-ZÅÄÖ]\w*[a-zåäö]+)$");                                              //Firstname and Lastname must start with capitol letters
				Regex regexToCheckPhone = new Regex(@"^(\+?\d{2}\-?\s?)?\d{4}\-?\s?\d{3}\-?\s?\d{3}$");                                             //Must be in these formats +46 0707-123 456, +46 0707-123456, +46 0707-123-456, 0707 123 456
				Regex regexToCheckAddress = new Regex(@"^(([A-ZÅÄÖ]\w*[a-zåäö]+|[A-ZÅÄÖ]\w*[a-zåäö]+\s[a-zA-ZåäöÅÄÖ]\w*[a-zåäö]+)+\s?\d{0,3})+$");  //Adress must start with capitol letter with optional second part and digits at end
				Regex regexToCheckZipCode = new Regex(@"^\d{3}\s?\d{2}$");                                                                          //Must be in the format xxx xx
				Regex regexToCheckCity = new Regex(@"^([A-ZÅÄÖ]\w*[a-zåäö]+|[A-ZÅÄÖ]\w*[a-zåäö]+\s[a-zA-ZåäöÅÄÖ]+)$");                              //Must start with capitol letter and can have a optional second part
				Regex regexToCheckEmail = new Regex(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$");
				//Regex to check for valid email formats ex firstname.lastname@domain.com
				#endregion

				#region Input Validation
				if (!regexToCheckName.IsMatch(name))
				{
					MessageToUser("Enter first and last name in the correct format names starting with capitol letters: \n\nEx: Firstname Lastname");
					return;
				}
				if (!regexToCheckPhone.IsMatch(phone))
				{
					MessageToUser("Enter phone number in the correct format: \n\nEx: +46 0707-123 456, +46 0707-123456, +46 0707-123-456, 0707-123 456, 0707 123 456");
					return;
				}
				if (!regexToCheckAddress.IsMatch(address))
				{
					MessageToUser("Enter address in the correct format. Every word must start with a capitol letter: \n\nEx: Street + no, Street, Two Word Street: Capitol Road + no, Two Word Street: Capitol Road");
					return;
				}
				if (!regexToCheckZipCode.IsMatch(zipCode))
				{
					MessageToUser("Enter zipcode in the correct format: \n\nEx: 123 45");
					return;
				}
				if (!regexToCheckCity.IsMatch(city))
				{
					MessageToUser("Enter city name in the correct format. Every word must start with a capitol letter: \n\nEx: City, Two Word City: Capitol City");
					return;
				}

				#endregion



				if (email == "" || email == " ")
				{
					_app.GetDefaultCustomerList().Add(new CustomerClass(name, address, zipCode, city, phone));
				}
				else
				{
					if (!regexToCheckEmail.IsMatch(email))
					{
						MessageToUser("Enter email in the correct format: \n\nEx: example@domain.com, example.example2@domain.net");
						return;
					}
					else
					{
						_app.GetDefaultCustomerList().Add(new CustomerClass(name, address, zipCode, city, phone, email));
					}
				}

				MessageToUser($"You have successfully added a new customer to your customer list \n\nCustomer name: {name}");


				#region Reset TextBoxes
				AddNewCustomerName.Text = "";
				AddNewCustomerPhoneNumber.Text = "";
				AddNewCustomerAddress.Text = "";
				AddNewCustomerZipCode.Text = "";
				AddNewCustomerCity.Text = "";
				AddNewCustomerEmail.Text = "";
				#endregion

			}

		}

		private void CustomerClearFormButton_Click(object sender, RoutedEventArgs e)
		{
			AddNewCustomerName.Text = "";
			AddNewCustomerPhoneNumber.Text = "";
			AddNewCustomerAddress.Text = "";
			AddNewCustomerZipCode.Text = "";
			AddNewCustomerCity.Text = "";
			AddNewCustomerEmail.Text = "";
		}

		private void PendingOrdersBtnSend_Click(object sender, RoutedEventArgs e)
		{
			var parent = (sender as Button).Parent;
			TextBlock cn = parent.GetChildrenOfType<TextBlock>().First(x => x.Name == "PendingOrdersCustomerName");
			QueuedOrder queuedOrder = store.FindQueued(cn.Text);
			store.SendOrder(queuedOrder);
		}

		private void PendingOrdersBtnSendAll_Click(object sender, RoutedEventArgs e)
		{
			store.TrySendQO();
		}

		private void SuppliersTabComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string supplierName = e.AddedItems[0].ToString();

			Supplier showSupplier = _app.Suppliers.First(x => x.SupplierName == supplierName);
			SupplierName.Text = showSupplier.SupplierName;
			SupplierPhoneNumber.Text = showSupplier.SupplierPhone;
			SupplierAddress.Text = showSupplier.SupplierAddress;
			SupplierZipCode.Text = showSupplier.SupplierZipCode;
			SupplierCity.Text = showSupplier.SupplierCity;
		}

		private async void SupplierAddButton_Click(object sender, RoutedEventArgs e)
		{
			if (AddNewSupplierName.Text == "" || AddNewSupplierName.Text == " " || AddNewSupplierPhoneNumber.Text == "" || AddNewSupplierPhoneNumber.Text == " " || AddNewSupplierAddress.Text == "" || AddNewSupplierAddress.Text == " " || AddNewSupplierZipCode.Text == "" || AddNewSupplierZipCode.Text == "" || AddNewSupplierCity.Text == "" || AddNewSupplierCity.Text == " ")
			{
				MessageToUser("All textboxes must be filled in");
			}
			else
			{
				#region Variables
				string name = AddNewSupplierName.Text;
				string phone = AddNewSupplierPhoneNumber.Text;
				string address = AddNewSupplierAddress.Text;
				string zipCode = AddNewSupplierZipCode.Text;
				string city = AddNewSupplierCity.Text;
				#endregion

				#region Regex
				Regex regexToCheckName = new Regex(@"^(([A-ZÅÄÖ]\w*[a-zåäö]+|[A-ZÅÄÖ]\w*[a-zåäö]+\s[a-zA-ZåäöÅÄÖ]\w*[a-zåäö]+)+\s?[A-ZÅÄÖ]\w*[a-zA-ZåäöÅÄÖ])+$");   //Company name one or two words and must end with Inc, AB etc
				Regex regexToCheckPhone = new Regex(@"^(\+?\d{2}\-?\s?)?\d{4}\-?\s?\d{3}\-?\s?\d{3}$");                                                             //Must be in these formats +46 0707-123 456, +46 0707-123456, +46 0707-123-456, 0707 123 456
				Regex regexToCheckAddress = new Regex(@"^(([A-ZÅÄÖ]\w*[a-zåäö]+|[A-ZÅÄÖ]\w*[a-zåäö]+\s[a-zA-ZåäöÅÄÖ]\w*[a-zåäö]+)+\s?\d{0,3})+$");                  //Adress must start with capitol letter with optional second part and digits at end
				Regex regexToCheckZipCode = new Regex(@"^\d{3}\s?\d{2}$");                                                                                          //Must be in the format xxx xx
				Regex regexToCheckCity = new Regex(@"^([A-ZÅÄÖ]\w*[a-zåäö]+|[A-ZÅÄÖ]\w*[a-zåäö]+\s[a-zA-ZåäöÅÄÖ]+)$");                                              //Must start with capitol letter and can have a optional second part
				#endregion

				#region Input Validation
				if (!regexToCheckName.IsMatch(name))
				{
					MessageToUser("Enter company name in the correct format names starting with capitol letters and end with corporate form: \n\nEx: Company Name Inc");
					return;
				}
				if (!regexToCheckPhone.IsMatch(phone))
				{
					MessageToUser("Enter phone number in the correct format: \n\nEx: +46 0707-123 456, +46 0707-123456, +46 0707-123-456, 0707-123 456, 0707 123 456");
					return;
				}
				if (!regexToCheckAddress.IsMatch(address))
				{
					MessageToUser("Enter address in the correct format. Every word must start with a capitol letter: \n\nEx: Street + no, Street, Two Word Street: Capitol Road + no, Two Word Street: Capitol Road");
					return;
				}
				if (!regexToCheckZipCode.IsMatch(zipCode))
				{
					MessageToUser("Enter zipcode in the correct format: \n\nEx: 123 45");
					return;
				}
				if (!regexToCheckCity.IsMatch(city))
				{
					MessageToUser("Enter city name in the correct format. Every word must start with a capitol letter: \n\nEx: City, Two Word City: Capitol City");
					return;
				}
				#endregion


				MessageToUser($"You have successfully added a new supplier to your supplier list \n\nSupplier name: {name}");

				_app.Suppliers.Add(new Supplier(name, address, zipCode, city, phone));
				await _app.WriteToFile(App.SuppliersFileName, _app.Suppliers);

				#region Reset TextBoxes
				AddNewSupplierName.Text = "";
				AddNewSupplierPhoneNumber.Text = "";
				AddNewSupplierAddress.Text = "";
				AddNewSupplierZipCode.Text = "";
				AddNewSupplierCity.Text = "";
				#endregion
			}
		}

		private void SupplierClearFormButton_Click(object sender, RoutedEventArgs e)
		{
			AddNewSupplierName.Text = "";
			AddNewSupplierPhoneNumber.Text = "";
			AddNewSupplierAddress.Text = "";
			AddNewSupplierZipCode.Text = "";
			AddNewSupplierCity.Text = "";
		}


		#endregion



		#region Methods

		public async Task MessageToUser(string inputMessage)
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

		#endregion

	}

	#region Help Class
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
	#endregion

}
