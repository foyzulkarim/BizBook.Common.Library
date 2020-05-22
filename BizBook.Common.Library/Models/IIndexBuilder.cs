using Microsoft.EntityFrameworkCore;

namespace BizBook.Common.Library.Models
{
    public interface IIndexBuilder<T> where T : class
    {
        void BuildIndices(ModelBuilder builder);
    }

    public interface IIndexBuilder2
    {
        void BuildIndices<T>(ModelBuilder builder) where T : class;
    }
}
