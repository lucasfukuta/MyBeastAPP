using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyBeast.Domain.Entities;
using MyBeast.Models.DTOs.Diet;
using MyBeast.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Diet
{
    public partial class DietViewModel : ObservableObject
    {
        private readonly IDietApiService _dietApiService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<MealLogDto> _mealLogs = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalProtein), nameof(TotalCarbs), nameof(TotalFat))]
        private decimal _totalCalories;

        [ObservableProperty]
        private bool _isLoading;

        public decimal TotalProtein => MealLogs.Sum(m => m.TotalProtein);
        public decimal TotalCarbs => MealLogs.Sum(m => m.TotalCarbs);
        public decimal TotalFat => MealLogs.Sum(m => m.TotalFat);

        public DietViewModel(IDietApiService dietApiService, INavigationService navigationService)
        {
            _dietApiService = dietApiService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task LoadLogsAsync()
        {
            IsLoading = true;
            MealLogs.Clear();
            try
            {
                var logs = await _dietApiService.GetMealLogsByDateAsync(SelectedDate);
                foreach (var log in logs)
                {
                    MealLogs.Add(log);
                }
                CalculateTotals();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar o diário: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CalculateTotals()
        {
            TotalCalories = MealLogs.Sum(m => m.TotalCalories);
            // Notifica manualmente as propriedades dependentes
            OnPropertyChanged(nameof(TotalProtein));
            OnPropertyChanged(nameof(TotalCarbs));
            OnPropertyChanged(nameof(TotalFat));
        }

        // Este método é chamado automaticamente quando a propriedade _selectedDate muda
        partial void OnSelectedDateChanged(DateTime value)
        {
            // Executa o comando para carregar os logs da nova data
            if (LoadLogsCommand.CanExecute(null))
            {
                LoadLogsCommand.Execute(null);
            }
        }

        // Comando para navegar para a página de adicionar refeição
        [RelayCommand]
        private async Task GoToAddMealAsync(string mealType)
        {
            var navigationParams = new Dictionary<string, object>
            {
                { "SelectedDate", SelectedDate },
                { "MealType", mealType }
            };
            // Usaremos "AddMealPage" como o nome da rota
            await _navigationService.NavigateToAsync("AddMealPage", navigationParams);
        }
    }
}
