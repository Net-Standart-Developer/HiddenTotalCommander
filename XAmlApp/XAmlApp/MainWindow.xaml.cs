using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace XAmlApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public StringBuilder LeftPath = new StringBuilder(150);
        internal StringBuilder RightPath = new StringBuilder(150);
        public static bool IsExistValue = false;
        public MainWindow()
        {
            InitializeComponent();
            Thread thread = new Thread(() =>
            {
                UpdateFiles();
                Thread.Sleep(2000);
            });
            thread.Start();
            AddDrives();
            //TimerCallback timerCallback = OnUppdateFiles;
            //Timer timer = new Timer(timerCallback, null, 200, 300); //проблема с обновлениями файлов
            List<string> styles = new List<string>() { "Чёрный", "Белый" };
            string[] extensions = new string[] { "txt", "doc", "docx", "bmp", "contact", "jnt", "pptx", "rar", "xlsx", "zip" };
            //List<string> accesext = new List<string>(20);
            RegistryKey key = Registry.ClassesRoot;
            var subkeys = key.GetSubKeyNames();
            MenuItem menu1 = new MenuItem();
            menu1.Header = "новый каталог";
            menu1.Click += Create;
            CreateMenuLeft.Items.Add(menu1);
            MenuItem menu2 = new MenuItem();
            menu2.Header = "новый каталог";
            menu2.Click += Create;
            CreateMenuRight.Items.Add(menu2);
            foreach (string name in subkeys)
            {
                foreach (string ext in extensions)
                {
                    if (name == "." + ext)
                    {
                        //accesext.Add(name);
                        MenuItem menu3 = new MenuItem();
                        MenuItem menu4 = new MenuItem();
                        menu3.Header = name;
                        menu4.Header = name;
                        //CommandBinding commandBinding = new CommandBinding();
                        //commandBinding.Command = ApplicationCommands.New;
                        //commandBinding.Executed += Create;
                        //menu.CommandBindings.Add(commandBinding);
                        menu3.Click += Create;
                        menu4.Click += Create;
                        CreateMenuLeft.Items.Add(menu3);
                        CreateMenuRight.Items.Add(menu4);
                    }
                }
            }
            
            
        }
        
        private void RightList_MouseDoubleClick(object sender, MouseButtonEventArgs e)//иногда выделенный элемент почему-то равен null
        {   
            
            if(RightList.SelectedItem != null)
            {
                Element element = (Element)RightList.SelectedItem;
                if (element.isElement == IsElement.IsDirectory || element.isElement == IsElement.IsDrive)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(RightPath.ToString() + element.Name + "\\");
                    if (directoryInfo.Exists)
                    {
                        try
                        {
                            DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();
                            FileInfo[] files = directoryInfo.GetFiles();

                            var nameofdirectories = from dir in subdirectories
                                                    select new Element { Name = dir.Name, isElement = IsElement.IsDirectory,Date = dir.CreationTime.ToShortDateString().ToString() };
                            var nameoffiles = from file in files
                                              select new Element { Name = file.Name, isElement = IsElement.IsFile, Date=file.CreationTime.ToShortDateString().ToString() };

                            var names = nameofdirectories.Union(nameoffiles);
                            RightList.ItemsSource = names;
                            RightPath.Append(element.Name + "\\");
                        }
                        catch (UnauthorizedAccessException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }
                    else
                    {
                        if (element.isElement == IsElement.IsDrive)
                            MessageBox.Show("Диск не подключен");
                        else MessageBox.Show("Каталог недоступен для использования");
                        RightPath.Clear();
                    }
                }
                else
                {
                    Process.Start(RightPath + element.Name);
                }
            }
            
        }

        private void LeftList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {   
            if(LeftList.SelectedItem != null)
            {
                Element element = (Element)LeftList.SelectedItem;
                if (element.isElement == IsElement.IsDirectory || element.isElement == IsElement.IsDrive)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(LeftPath.ToString() + ((Element)element).Name + "\\");
                    if (directoryInfo.Exists)
                    {
                        try
                        {
                            DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();

                            FileInfo[] files = directoryInfo.GetFiles();

                            var nameofdirectories = from dir in subdirectories
                                                    select new Element { Name = dir.Name, isElement = IsElement.IsDirectory, Date = dir.CreationTime.ToShortDateString().ToString() };
                            var nameoffiles = from file in files
                                              select new Element { Name = file.Name, isElement = IsElement.IsFile,Date = file.CreationTime.ToShortDateString().ToString() };

                            var names = nameofdirectories.Union(nameoffiles);
                            LeftList.ItemsSource = names;
                            LeftPath.Append(((Element)element).Name + "\\");
                        }
                        catch (UnauthorizedAccessException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }
                    else
                    {   
                        if(element.isElement == IsElement.IsDrive)
                        MessageBox.Show("Диск не подключен");
                        else MessageBox.Show("Каталог недоступен для использования");
                        LeftPath.Clear();
                    }

                }
                else
                {
                    Process.Start(LeftPath + element.Name);
                }
            }
        }

        private void BackRight_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RightPath.Length > 4)
            {
                for (int count = RightPath.Length - 1; count > 2; count--)
                {
                    if (RightPath[count] == '\\' && count != RightPath.Length - 1)
                    {
                        RightPath.Remove(count + 1, RightPath.Length - count - 1);
                        break;
                    }
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(RightPath.ToString());
                DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();
                FileInfo[] files = directoryInfo.GetFiles();

                var nameofdirectories = from dir in subdirectories
                                        select new Element { Name = dir.Name, isElement = IsElement.IsDirectory,Date = dir.CreationTime.ToShortDateString() };
                var nameoffiles = from file in files
                                  select new Element { Name = file.Name, isElement = IsElement.IsFile,Date = file.CreationTime.ToShortDateString() };

                var names = nameofdirectories.Union(nameoffiles);
                RightList.ItemsSource = names;

            }
            else if (RightPath.Length > 0)
            {
                AddDrives(2);
            }
            
        }

        private void BackLeft_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {   
            if(LeftPath.Length > 4)
            {
                for (int count = LeftPath.Length - 1; count > 2; count--)
                {
                    if (LeftPath[count] == '\\' && count != LeftPath.Length - 1)
                    {
                        LeftPath.Remove(count + 1, LeftPath.Length - count - 1);
                        break;
                    }
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(LeftPath.ToString());
                DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();
                FileInfo[] files = directoryInfo.GetFiles();

                var nameofdirectories = from dir in subdirectories
                                        select new Element { Name = dir.Name, isElement = IsElement.IsDirectory,Date = dir.CreationTime.ToShortDateString() };
                var nameoffiles = from file in files
                                  select new Element { Name = file.Name, isElement = IsElement.IsFile,Date = file.CreationTime.ToShortDateString() };

                var names = nameofdirectories.Union(nameoffiles);
                LeftList.ItemsSource = names;
            }
            else if(LeftPath.Length>0)
            {
                AddDrives(1);
            }
            
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string style = ((RadioButton)sender).Content.ToString();
            Uri uri;
            if (style == "Чёрный") uri = new Uri("DarkStyle.xaml", UriKind.Relative);
            else  uri = new Uri("WhiteStyle.xaml", UriKind.Relative);
            ResourceDictionary resource = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resource);
        }
        private void RightList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LeftList.SelectedIndex = -1;
        }

        private void LeftList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightList.SelectedIndex = -1;
        }

        private void SliderForText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
        }
    }
 
}
