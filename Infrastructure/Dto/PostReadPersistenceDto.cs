using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dto
{
    public record PostReadPersistenceDto(
        string Id,
        string Title,
        string Content,
        DateTime CreatedAt
        );
}
