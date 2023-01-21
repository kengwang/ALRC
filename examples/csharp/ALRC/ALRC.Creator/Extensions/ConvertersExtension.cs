using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace ALRC.Creator.Extensions;

public static class ConvertersExtension
{
    public static void AddConverters(this Application app, Assembly? scanningAssembly = null)
    {
        var assembly = scanningAssembly ?? Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IValueConverter).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract) continue;
            app.Resources[type.Name] = Activator.CreateInstance(type);
        }
    }
}