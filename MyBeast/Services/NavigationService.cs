using MyBeast.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// MyBeast/Services/NavigationService.cs
namespace MyBeast.Services
{
    // Troque 'internal' por 'public' para garantir que a Injeção de Dependência funcione
    public class NavigationService : INavigationService
    {
        // Implementação correta usando o Shell do MAUI
        public Task NavigateToAsync(string route) =>
            Shell.Current.GoToAsync(route);

        // Implementação correta para o método com parâmetros
        public Task NavigateToAsync(string route, IDictionary<string, object> parameters)
        {
            return Shell.Current.GoToAsync(route, parameters);
        }

        // Implementação correta para voltar
        public Task GoBackAsync() =>
            Shell.Current.GoToAsync("..");

        // Implementação correta para ir para a raiz
        public Task NavigateToRootAsync() =>
            Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }
}