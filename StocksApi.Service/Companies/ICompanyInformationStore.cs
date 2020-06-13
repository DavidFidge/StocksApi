using System.Threading.Tasks;

namespace StocksApi.Service.Companies
{
    public interface ICompanyInformationStore
    {
        Task<string> GetFromStore();
    }
}