using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Entities;

namespace Application.Interfaces
{
    public interface IBlogRepository
    {
        Task SaveAsync(Post post);
        Task<Post?> GetByIdAsync(string id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task UpdateAsync(Post post);
        Task DeleteAsync(string id);
    }
}
