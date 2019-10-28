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
    public partial class MainWindow : Window
    {
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsWorked)
            {
                MessageBox.Show("Приложение не может быть закрыто, так как выполняет задачу!");
            }
            else
            this.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://documentation.help/TC-WCMD-ru/m0nqwe.html");
        }
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            //OpenFileWindow window = new OpenFileWindow();
            //window.Show();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Process.Start(openFileDialog.FileName);
                }
                catch (IOException exc)
                {
                    MessageBox.Show(exc.Message);
                }
                catch (UnauthorizedAccessException exc)
                {
                    MessageBox.Show(exc.Message);
                }
                catch(Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        private void ButtonForText_Click(object sender, RoutedEventArgs e)
        {
            SliderForText.Value = 12;
        }
        private void Rename(object sender, RoutedEventArgs e)
        {
            Element element;
            string path;
            if (LeftList.SelectedItem != null && LeftList.SelectedItems.Count == 1)
            {
                element = (Element)LeftList.SelectedItem;
                path = LeftPath.ToString();
            }
            else if (RightList.SelectedItem != null && RightList.SelectedItems.Count == 1)
            {
                element = (Element)RightList.SelectedItem;
                path = RightPath.ToString();
            }
            else
            {
                MessageBox.Show("Для переименования должен быть выбран 1 элемент");
                return;
            }
            if (element.isElement == IsElement.IsDrive)
            {
                MessageBox.Show("Разделы диска или носители нельзя переименовывать");
            }
            if (element.isElement == IsElement.IsDirectory)
            {
                DirectoryInfo directory = new DirectoryInfo(path + element.Name);
                RenameWindow renameWindow = new RenameWindow();
                renameWindow.ShowDialog();
                if (IsExistValue)
                {
                    if (renameWindow.TextBoxForName.Text != "")
                    {
                        try
                        {
                            directory.MoveTo(path + "\\" + renameWindow.TextBoxForName.Text);
                        }
                        catch(IOException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Значение не введено");
                    }
                    IsExistValue = false;
                }
            }
            if (element.isElement == IsElement.IsFile)
            {
                FileInfo file = new FileInfo(path + element.Name);
                RenameWindow renameWindow = new RenameWindow();
                renameWindow.TextBoxForName.Text = file.Extension;
                renameWindow.ShowDialog();
                if (IsExistValue)
                {
                    
                    if (renameWindow.TextBoxForName.Text != file.Extension)
                    {
                        try
                        {
                            file.MoveTo(path + "\\" + renameWindow.TextBoxForName.Text);
                        }
                        catch (IOException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }  
                    else
                    {
                        MessageBox.Show("Значение не введено");
                    }                   
                }
            }
            UpdateFiles();
        }
        //private void Encode(object sender, RoutedEventArgs e)
        //{
        //    Element element;
        //    string path;
        //    if (LeftList.SelectedItem != null && LeftList.SelectedItems.Count == 1)
        //    {
        //        element = (Element)LeftList.SelectedItem;
        //        path = LeftPath.ToString();
        //    }
        //    else if (RightList.SelectedItem != null && RightList.SelectedItems.Count == 1)
        //    {
        //        element = (Element)RightList.SelectedItem;
        //        path = RightPath.ToString();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Закодировать можно только 1 текстовый документ");
        //        return;
        //    }
        //    if(element.isElement == IsElement.IsFile)
        //    {
        //        FileInfo file = new FileInfo(path + "//" + element.Name);
        //        if(file.Extension == ".txt" || file.Extension == ".docx" || file.Extension == ".doc")
        //        {
        //            try
        //            {
        //                using (StreamReader reader = new StreamReader(File.Open(path + "//" + element.Name, FileMode.Open),Encoding.Default))
        //                {
        //                    using (StreamWriter writer = new StreamWriter(File.Open(path + "//" + "EncodingFile.docx", FileMode.CreateNew), Encoding.Default))
        //                    {
        //                        string temp;
        //                        StringBuilder stringBuilder = new StringBuilder(100);
        //                        while ((temp = reader.ReadLine()) != null)
        //                        {

        //                            for (int count = 0; count < temp.Length; count++)
        //                            {
        //                                stringBuilder.Append((char)((char)(temp[count]) + 5));
        //                            }
        //                            writer.WriteLine(stringBuilder.ToString());
        //                            stringBuilder.Clear();
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message);
        //            }
        //            UpdateFiles();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Документе не имеет требуемое расширение txt, docx или doc");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Документе должен быть файлом с расширением txt, docx или doc");
        //    }
        //}
        //private void Decode(object sender, RoutedEventArgs e)
        //{
        //    Element element;
        //    string path;
        //    if (LeftList.SelectedItem != null && LeftList.SelectedItems.Count == 1)
        //    {
        //        element = (Element)LeftList.SelectedItem;
        //        path = LeftPath.ToString();
        //    }
        //    else if (RightList.SelectedItem != null && RightList.SelectedItems.Count == 1)
        //    {
        //        element = (Element)RightList.SelectedItem;
        //        path = RightPath.ToString();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Раскодировать можно только 1 текстовый документ");
        //        return;
        //    }
        //    if (element.isElement == IsElement.IsFile)
        //    {
        //        FileInfo file = new FileInfo(path + "//" + element.Name);
        //        if (file.Extension == ".txt" || file.Extension == ".docx" || file.Extension == ".doc")
        //        {
        //            try
        //            {
        //                using (StreamReader reader = new StreamReader(File.Open(path + "//" + element.Name, FileMode.Open), Encoding.Default))
        //                {
        //                    using (StreamWriter writer = new StreamWriter(File.Open(path + "//" + "DecodingFile.docx", FileMode.CreateNew), Encoding.Default))
        //                    {
        //                        string temp;
        //                        StringBuilder stringBuilder = new StringBuilder(100);
        //                        while ((temp = reader.ReadLine()) != null)
        //                        {

        //                            for (int count = 0; count < temp.Length; count++)
        //                            {
        //                                stringBuilder.Append((char)((char)(temp[count]) - 5));
        //                            }
        //                            writer.WriteLine(stringBuilder.ToString());
        //                            stringBuilder.Clear();
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message);
        //            }

        //            UpdateFiles();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Документе не имеет требуемое расширение txt, docx или doc");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Документе должен быть файлом с расширением txt, docx или doc");
        //    }
        //}
        
        private void Create(object sender, RoutedEventArgs e)
        {
            string path = "";
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.Parent.GetValue(NameProperty).ToString() == "CreateMenuLeft")
            {   
                path = LeftPath.ToString();
            }
            else if(menuItem.Parent.GetValue(NameProperty).ToString() == "CreateMenuRight")
            {
                path = RightPath.ToString();
            }
            
            if (path != "")
            {
                RenameWindow window = new RenameWindow();
                string name = ((MenuItem)sender).Header.ToString();
                window.TextBoxForName.Text = name;
                window.button.Content = "Создать";
                window.ShowDialog();
                if (IsExistValue)
                {
                    if(name == "новый каталог")
                    {
                        DirectoryInfo directory = new DirectoryInfo(path + window.TextBoxForName.Text);
                        if (!directory.Exists)
                        {
                            try
                            {
                                directory.Create();
                            }
                            catch (System.UnauthorizedAccessException exc)
                            {
                                MessageBox.Show(exc.Message);
                            }
                        }
                        else MessageBox.Show("Каталог с таким именем уже существует");
                    }
                    else
                    {
                        FileInfo file = new FileInfo(path + window.TextBoxForName.Text);
                        
                        if (!file.Exists)
                        {
                            try
                            {
                                file.Create();
                                
                            }
                            catch (System.UnauthorizedAccessException exc)
                            {
                                MessageBox.Show(exc.Message);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Данный элемент уже существует");
                        }
                    }
                    IsExistValue = false;
                    UpdateFiles();
                }
            }
            else
            {
                MessageBox.Show("Елемент должен быть создан в разделе диска либо в запоминающем устройстве");
            }
        }
    }
    public class ExitCommand
    {
        static ExitCommand()
        {
            Exit = new RoutedCommand("Exit", typeof(ExitCommand));
        }
        public static RoutedCommand Exit { get; set; }
    }
}
