using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using KretaAvalonia_SB.Model;
using KretaAvalonia_SB.ViewModels;
using KretaAvalonia_SB.Views;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace KretaAvalonia_SB;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            const string baseUrl = "https://localhost:7273/";

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                // SSL bypass fejlesztéshez:
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var sharedClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            var authModel = new AuthModel(sharedClient);
            var gradeModel = new GradeModel(sharedClient);
            var messageModel = new MessageModel(sharedClient);
            var timeTableModel = new TimeTableModel(sharedClient);
            var dataModel = new DataModel(sharedClient);

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(authModel, gradeModel, messageModel, timeTableModel, dataModel)
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            const string baseUrl = "https://localhost:7273/";

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                // SSL bypass fejlesztéshez:
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var sharedClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            var authModel = new AuthModel(sharedClient);
            var gradeModel = new GradeModel(sharedClient);
            var messageModel = new MessageModel(sharedClient);
            var timeTableModel = new TimeTableModel(sharedClient);
            var dataModel = new DataModel(sharedClient);

            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel(authModel, gradeModel, messageModel, timeTableModel, dataModel)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}