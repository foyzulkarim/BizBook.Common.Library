using System.Security.Claims;
using System.Threading.Tasks;

namespace BizBook.Common.Library.ApiExtensions
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id, string roleId, string shopId);
    }
}
