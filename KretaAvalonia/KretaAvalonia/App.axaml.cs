using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using KretaAvalonia.ViewModels;
using KretaAvalonia.Views;
using KretaAvalonia.Model;

namespace KretaAvalonia;

public partial class App : Application
{
    private LoginPage LoginPage;
    private LoginPageViewModel LoginPageViewModel;
    private MainViewModel MainViewModel;
    private MenuPage MenuPage;
    private MenuPageViewModel MenuPageViewModel;
    private ApiSession _session;
    private AuthModel _auth;
    private OrarendPage orarendPage;
    private OrarendViewModel orarendViewModel;
    private JegyekModel jegyekModel;
    private JegyekPage jegyekPage;
    private JegyekViewModel jegyekViewModel;
    private OrarendModel orarendModel;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }



    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _session = new ApiSession("https://localhost:7273/");
            _auth = new AuthModel(_session);
            LoginPage = new LoginPage();
            LoginPageViewModel = new LoginPageViewModel(_auth);
            MainViewModel = new MainViewModel();
            MenuPage = new MenuPage();
            MenuPageViewModel = new MenuPageViewModel(_session);
            orarendPage = new OrarendPage();
            orarendModel = new OrarendModel(_session);
            orarendViewModel = new OrarendViewModel(_session, orarendModel);
            jegyekPage = new JegyekPage();
            jegyekModel = new JegyekModel(_session);
            
            jegyekViewModel = new JegyekViewModel(jegyekModel);
            
            
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = MainViewModel

            };
            desktop.MainWindow.Content = LoginPage;
            desktop.MainWindow.DataContext = LoginPageViewModel;
            LoginPageViewModel.SikeresBelepes += (s, e) =>
            {
                desktop.MainWindow.Content = MenuPage;
                desktop.MainWindow.DataContext = MenuPageViewModel;
            };
                // SikeresBelepes-en KÍVÜLRE tedd ezeket:
                MenuPageViewModel.KijelentkezesEvent += (s, e) =>
                {
                    desktop.MainWindow.Content = LoginPage;
                    desktop.MainWindow.DataContext = LoginPageViewModel;
                };

                MenuPageViewModel.OrarendEvent += async (s, e) =>
                {
                    desktop.MainWindow.Content = orarendPage;
                    desktop.MainWindow.DataContext = orarendViewModel;
                    await orarendViewModel.BetoltOrarend();
                };

                MenuPageViewModel.JegyekEvent += async (s, e) =>
                {
                    desktop.MainWindow.Content = jegyekPage;
                    desktop.MainWindow.DataContext = jegyekViewModel;
                    await jegyekModel.GetJegyek(_session.Userid);
                    jegyekViewModel.SetUpTantargyak();
                };

                orarendViewModel.VisszaEvent += (s, e) =>
                {
                    desktop.MainWindow.Content = MenuPage;
                    desktop.MainWindow.DataContext = MenuPageViewModel;
                };

                
                

                orarendViewModel.VisszaEvent += (s, e) =>
                {
                    desktop.MainWindow.Content = MenuPage;
                    desktop.MainWindow.DataContext = MenuPageViewModel;
                };
            
            jegyekViewModel.VisszaEvent += (s, e) =>
            {
                desktop.MainWindow.Content = MenuPage;
                desktop.MainWindow.DataContext = MenuPageViewModel;
            };
        }
        






    

        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
{
    // Get an array of plugins to remove
    var dataValidationPluginsToRemove =
        BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

    // remove each entry found
    foreach (var plugin in dataValidationPluginsToRemove)
    {
        BindingPlugins.DataValidators.Remove(plugin);
    }
}
}