using Ravi.App.ViewModels;

namespace Ravi.App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new HomeViewModel();
    }
}
