using System.Threading.Tasks;

namespace GoldStarr_Trading.Classes
{
    interface IMessageToUser
    {
        Task MessageToUser(string inputMessage);
    }
}
