namespace MyBeast.Services
{
    public class NavigationService : INavigationService
    {
        public Task NavigateToAsync(string route)
        {
            // Usa o Shell nativo do MAUI para navegar
            return Shell.Current.GoToAsync(route);
        }

        public Task NavigateToAsync(string route, IDictionary<string, object> parameters)
        {
            // Usa o Shell para navegar levando dados (ex: ID do treino)
            return Shell.Current.GoToAsync(route, parameters);
        }

        public Task GoBackAsync()
        {
            // ".." é o código do Shell para "Voltar 1 nível"
            return Shell.Current.GoToAsync("..");
        }
    }
}