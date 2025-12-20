using MealMind.ViewModels;

namespace MealMind.Views.Pages
{



    // ShoppingListPage
    // Responsible for wiring the ViewModel, refreshing data on appearing,
    // and routing UI events to ViewModel actions.
    public partial class ShoppingListPage : ContentPage
    {
        // Initializes the page and assigns its ViewModel.
        public ShoppingListPage()
        {
            InitializeComponent();
            BindingContext = new ShoppingListPageViewModel();
        }


        // Refresh the shopping list whenever the page becomes visible
        // so changes made elsewhere are reflected.
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ShoppingListPageViewModel vm)
                await vm.RefreshAsync();
        }


        // Checkbox handler: updates checked state in both UI and persisted storage.
        private async void ItemCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (BindingContext is not ShoppingListPageViewModel vm) return;
            if ((sender as CheckBox)?.BindingContext is not ShoppingListItemRow row) return;

            await vm.SetCheckedAsync(row, e.Value);
        }

        // Deletes a single shopping list item.
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is not ShoppingListPageViewModel vm) return;
            if ((sender as Button)?.BindingContext is not ShoppingListItemRow row) return;

            await vm.DeleteAsync(row);
        }

        // Removes all checked items from the shopping list.
        private async void ClearChecked_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is ShoppingListPageViewModel vm)
                await vm.ClearCheckedAsync();
        }

        // Clears the entire shopping list.
        private async void ClearAll_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is ShoppingListPageViewModel vm)
                await vm.ClearAllAsync();
        }
    }
}

