using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.PopupsAI
{
    internal class AIAssistantPopupViewModel : INotifyPropertyChanged
    {
        private string _assistantResponse;
        private string _userInput;
        private bool _isProcessing;

        public string AssistantResponse
        {
            get => _assistantResponse;
            set
            {
                _assistantResponse = value;
                OnPropertyChanged();
            }
        }

        public string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                OnPropertyChanged();
            }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task ProcessUserInputAsync()
        {
            if (string.IsNullOrWhiteSpace(UserInput))
                return;

            IsProcessing = true;

            try
            {
                // Simulate processing logic
                await Task.Delay(1000);
                AssistantResponse = $"Processed: {UserInput}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
