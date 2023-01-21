using System.Windows;
using System.Windows.Controls;
using ALRC.Creator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ALRC.Creator.Extensions;

public static class MvvmDependencyInjectionExtension
{
    public static void AddModels(this IServiceCollection services)
    {
        services.AddAllImplementationsOf<IModelBase>();
    }
    
    public static void AddViewModels(this IServiceCollection services)
    {
        services.AddAllImplementationsOf<IViewModel>();
    }

    public static void AddViews(this IServiceCollection services)
    {
        services.AddAllImplementationsOf<Window>();
        services.AddAllImplementationsOf<Page>();
        services.AddAllImplementationsOf<UserControl>();
    }

    public static void AddMvvm(this IServiceCollection services)
    {
        services.AddModels();
        services.AddViewModels();
        services.AddViews();
    }
}