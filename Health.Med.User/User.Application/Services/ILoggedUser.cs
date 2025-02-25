using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Application.Services;

public interface ILoggedUser
{
    Task<Domain.Entities.User> GetLoggedUserAsync();
}
