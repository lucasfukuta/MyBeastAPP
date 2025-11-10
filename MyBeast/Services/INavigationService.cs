using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBeast.Domain.Entities;

namespace MyBeast.Services
{
    internal interface INavigationService
    {
        Task NavigateToAsync(string route);
        Task NavigateToAsync(string route, IDictionary<string, object> parameters);
        Task GoBackAsync();
        Task NavigateToRootAsync();
    }
}
