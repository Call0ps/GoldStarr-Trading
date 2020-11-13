using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoldStarr_Trading.Classes
{
    public class Supplier : INotifyPropertyChanged
    {

        #region Properties
        private string _supplierName;
        public string SupplierName
        {
            get => _supplierName;
            set
            {
                if (_supplierName != value)
                {
                    _supplierName = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private string _supplierAddress;
        public string SupplierAddress
        {
            get => _supplierAddress;
            set
            {
                if (_supplierAddress != value)
                {
                    _supplierAddress = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private string _supplierZipCode;
        public string SupplierZipCode
        {
            get => _supplierZipCode;
            set
            {
                if (_supplierZipCode != value)
                {
                    _supplierZipCode = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private string _supplierCity;
        public string SupplierCity
        {
            get => _supplierCity;
            set
            {
                if (_supplierCity != value)
                {
                    _supplierCity = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private string _supplierPhone;
        public string SupplierPhone
        {
            get => _supplierPhone;
            set
            {
                if (_supplierPhone != value)
                {
                    _supplierPhone = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion


        #region Constructor
        public Supplier(string name, string address, string zipCode, string city, string phone)
        {
            SupplierName = name;
            SupplierAddress = address;
            SupplierZipCode = zipCode;
            SupplierCity = city;
            SupplierPhone = phone;
        }
        #endregion


        #region Methods
        public override string ToString()
        {
            return SupplierName;
        }

        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #endregion

    }
}
