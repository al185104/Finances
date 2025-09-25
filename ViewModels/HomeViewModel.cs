using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Finances.Interfaces;
using Finances.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finances.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly ICategoryService _categoryService;
        private readonly ISummaryService _summaryService;
        private readonly ITransactionService _transactionService;

        [ObservableProperty]
        bool _isBusy = false;

        [ObservableProperty]
        ObservableCollection<CategoryDto> _categories = new();

        [ObservableProperty]
        SummaryResponseDto _summary = new();

        [ObservableProperty]
        ObservableCollection<TransactionDto> _transactions = new();

        public HomeViewModel(
            ICategoryService categoryService, 
            ISummaryService summaryService,
            ITransactionService transactionService)
        {
            _categoryService = categoryService;
            _summaryService = summaryService;
            _transactionService = transactionService;
        }

        [RelayCommand]
        async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;

                var categories = await _categoryService.GetAllAsync();
                if(categories is not null && categories.Any())
                {
                    Categories = new ObservableCollection<CategoryDto>(categories);
                }

                var summary = await _summaryService.GetAsync(); 
                if(summary is not null)
                {
                    Summary = summary;
                }

                var transactions = await _transactionService.ListAsync();
                if(transactions is not null && transactions.Any())
                {
                    Transactions = new ObservableCollection<TransactionDto>(transactions);
                }   
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task AddTransactionAsync()
        {
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(Views.AddTransactionPage));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
