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
    public partial class AddTransactionViewModel : ObservableObject
    {
        private readonly ICategoryService _categoryService;
        private readonly ITransactionService _transactionService;

        [ObservableProperty]
        bool _isBusy = false;

        [ObservableProperty]
        ObservableCollection<CategoryDto> _categories = new();

        [ObservableProperty]
        CategoryDto? _selectedCategory;

        [ObservableProperty]
        CreateTransactionRequest _request = new();

        public IReadOnlyList<TxnType> TxnTypes { get; } = Enum.GetValues<TxnType>().ToList();

        [ObservableProperty]
        TxnType _selectedTxnType = TxnType.expense;

        public IReadOnlyList<RecurrencePeriod> RecurrencePeriods { get; } = Enum.GetValues<RecurrencePeriod>().ToList();

        [ObservableProperty]
        RecurrencePeriod _selectedRecurrencePeriod = RecurrencePeriod.monthly;

        public AddTransactionViewModel(ICategoryService categoryService, ITransactionService transactionService)
        {
            _categoryService = categoryService;
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
                var ret = await _transactionService.CreateAsync(new CreateTransactionRequest
                {
                    Amount = Request.Amount,
                    CategoryId = SelectedCategory == null ? Categories.First().Id : SelectedCategory.Id,
                    TxnDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Description = Request.Description,
                    RecurrenceInterval = Request.RecurrenceInterval,
                    RecurrencePeriod = SelectedRecurrencePeriod,
                    TxnType = SelectedTxnType,
                });

                if(ret != null)
                {
                    await Shell.Current.DisplayAlert("Success", "Transaction added successfully.", "OK");
                    await Shell.Current.GoToAsync("..");
                }

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
