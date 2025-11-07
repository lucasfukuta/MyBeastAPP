using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels.Community
{
    internal class CommunityFeedViewModel : INotifyPropertyChanged
    {
        private bool _isLoading;
        private ObservableCollection<string> _posts;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Posts
        {
            get => _posts;
            set
            {
                if (_posts != value)
                {
                    _posts = value;
                    OnPropertyChanged();
                }
            }
        }

        public CommunityFeedViewModel()
        {
            _posts = new ObservableCollection<string>();
        }

        public async Task LoadPostsAsync()
        {
            IsLoading = true;

            try
            {
                // Simulate data loading
                await Task.Delay(2000);
                Posts = new ObservableCollection<string>
                    {
                        "Post 1",
                        "Post 2",
                        "Post 3"
                    };
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
