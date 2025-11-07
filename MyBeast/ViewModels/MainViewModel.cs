using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _title;
        private ObservableCollection<string> _items;

        public MainViewModel()
        {
            Title = "Bem-vindo ao MyBeast";
            Items = new ObservableCollection<string>();
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Items
        {
            get => _items;
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddItem(string item)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                Items.Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
