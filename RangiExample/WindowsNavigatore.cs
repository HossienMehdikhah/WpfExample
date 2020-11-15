using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfExample.WpfClient.DialogForm;

namespace WpfExample.WpfClient
{
    public class WindowsNavigatore
    {
        readonly IServiceProvider _serviceProvider;

        public WindowsNavigatore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Window CreateMainWindow()
        {
            return _serviceProvider.GetRequiredService<MainWindow>();
        }

        public Window Create_OrderCreate_DialogForm(Window parent)
        {
            var temp = _serviceProvider.GetRequiredService<OrderCreate_DialogForm>();
            temp.Owner = Window.GetWindow(parent);
            return temp;
        }

        public Window Create_OrderDetailCreate_DialogForm(Window parent)
        {
            var temp = _serviceProvider.GetRequiredService<OrderDetailCreate_DialogForm>();
            temp.Owner = Window.GetWindow(parent);
            return temp;
        }
    }
}
