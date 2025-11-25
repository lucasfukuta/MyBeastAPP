namespace MyBeast.Services
{
    public interface INavigationService
    {
        // Navega para uma rota (ex: "WorkoutDetailPage")
        Task NavigateToAsync(string route);

        // Navega passando parâmetros (ex: passar o treino clicado)
        Task NavigateToAsync(string route, IDictionary<string, object> parameters);

        // Volta para a tela anterior
        Task GoBackAsync();
    }
}