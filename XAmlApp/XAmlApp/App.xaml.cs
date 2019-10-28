using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace XAmlApp
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            string mutName = "Приложение";
            mutex = new System.Threading.Mutex(true, mutName, out createdNew);
            if (!createdNew)
            {
                this.Shutdown();
            }
        }
    }
}
