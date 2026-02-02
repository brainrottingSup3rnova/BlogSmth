using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public record PostReadDto(
        Guid Id,
        string Title,
        string Content,
        DateTime CreatedAt
        );
}
