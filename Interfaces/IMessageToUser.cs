using System.Threading.Tasks;

namespace GoldStarr_Trading.Classes
{
    internal interface IMessageToUser
    {
        Task MessageToUser(string inputMessage);
    }
}