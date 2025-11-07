using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Services
{
    internal class NavigationService : INavigationService
    {
        private readonly Stack<string> _navigationStack = new Stack<string>();

        public void NavigateTo(string page)
        {
            if (string.IsNullOrWhiteSpace(page))
                throw new ArgumentException("Page cannot be null or empty.", nameof(page));

            _navigationStack.Push(page);
        }

        public string GoBack()
        {
            if (_navigationStack.Count <= 1)
                throw new InvalidOperationException("No pages to navigate back to.");

            // Pop the current page
            _navigationStack.Pop();

            // Return the previous page
            return _navigationStack.Peek();
        }

        public string GetCurrentPage()
        {
            if (_navigationStack.Count == 0)
                throw new InvalidOperationException("No pages in the navigation stack.");

            return _navigationStack.Peek();
        }

        public Task NavigateToAsync(string route)
        {
            throw new NotImplementedException();
        }

        public Task GoBackAsync()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToRootAsync()
        {
            throw new NotImplementedException();
        }
    }
}
