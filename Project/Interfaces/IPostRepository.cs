using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Entities;

namespace Application.Interfaces
{
    public interface IPostRepository
    {
        public Task SaveAsync(Post post);
        public Task InizializeAsync();
    }
}
