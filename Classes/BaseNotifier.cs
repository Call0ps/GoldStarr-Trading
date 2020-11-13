using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace GoldStarr_Trading.Classes
{
    /// <summary>
    /// Class that implements INotifyPropertyChanged, to be used when there is no other inheritance and one needs to implement this event.
    /// </summary>
    public class BaseNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
           this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
