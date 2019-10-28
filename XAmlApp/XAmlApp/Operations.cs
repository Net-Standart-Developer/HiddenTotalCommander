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

namespace XAmlApp
{
    public partial class MainWindow : Window
    {
        private bool IsWorked = false;
        private async void MenuItem_ClickCopy(object sender, RoutedEventArgs e) //удаляет содержимое при копировании
        {
            await Task.Run(() => ExtForCopy());
        }
        private void ExtForCopy()
        {
            Action action = () =>
            {
                Thread thread = new Thread(() =>
                {
                    int leftCount = 0;
                    int rightCount = 0;
                    IsWorked = true;
                    Element[] elements;
                    this.Dispatcher.Invoke((ThreadStart)delegate()
                    {
                        leftCount = LeftList.SelectedItems.Count;
                        rightCount = RightList.SelectedItems.Count;
                    });
                    if (leftCount > 0)
                    {
                        GetNewCollection(out elements, true);
                        foreach (Element element in elements)
                        {
                            ChangePlace(false, element, true);
                        }
                    }
                    else if (rightCount > 0)
                    {
                        GetNewCollection(out elements, false);
                        foreach (Element element in elements)
                        {
                            ChangePlace(false, element, false);
                        }
                    }
                    IsWorked = false;
                });
                thread.Start();
            };
            Dispatcher.Invoke(action);
        }
        private void ChangePlace(bool delete, Element element, bool IsLeft)
        {
            try
            {
                string newpath, oldpath;
                string name = element.Name;
                if (IsLeft)
                {
                    if (RightPath.ToString() == "") throw new Exception("Неверный путь");
                    oldpath = LeftPath.ToString();
                    newpath = RightPath.ToString() + name;
                }
                else
                {
                    if (LeftPath.ToString() == "") throw new Exception("Неверный путь");
                    oldpath = RightPath.ToString();
                    newpath = LeftPath.ToString() + name;
                }
                if (element.isElement == IsElement.IsDirectory)
                {
                    if (!new DirectoryInfo(newpath).Exists)
                    {
                        DirectoryInfo newdirect = new DirectoryInfo(newpath);
                        newdirect.Create();

                        GetSubDir(new DirectoryInfo(oldpath + name), newpath, delete);
                        try
                        {
                            if (delete) Directory.Delete(oldpath + name, true);
                        }
                        catch (IOException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }
                }
                else if (element.isElement == IsElement.IsFile)
                {
                    if (delete)
                    {
                        try
                        {
                            File.Move(oldpath + name, newpath);
                        }
                        catch (IOException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            File.Copy(oldpath + name, newpath, false);
                        }
                        catch (IOException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                        catch (UnauthorizedAccessException exc)
                        {
                            MessageBox.Show(exc.Message);
                        }

                    }
                }
                UpdateFiles();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            
        }
        private void GetSubDir(DirectoryInfo directory, string newpath, bool delete)
        {

            FileInfo[] files = directory.GetFiles();
            for (int countfile = 0; countfile < files.Length; countfile++)
            {
                if (delete)
                {
                    try
                    {
                        files[countfile].MoveTo(newpath + "\\" + files[countfile].Name);
                    }
                    catch (IOException exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }

                else files[countfile].CopyTo(newpath + "\\" + files[countfile].Name);
            }
            DirectoryInfo[] subdirect = directory.GetDirectories();
            for (int count = 0; count < subdirect.Length; count++)
            {
                DirectoryInfo newdir = new DirectoryInfo(newpath + "\\" + subdirect[count].Name);
                newdir.Create();
                GetSubDir(subdirect[count], newpath + "\\" + newdir.Name, delete);
            }

        }

        private void UpdateFiles()
        {
            try
            {
                if (LeftPath.ToString() != "")
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(LeftPath.ToString());
                    DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();

                    FileInfo[] files = directoryInfo.GetFiles();

                    var nameofdirectories = from dir in subdirectories
                                            select new Element { Name = dir.Name, isElement = IsElement.IsDirectory, Date = dir.CreationTime.ToShortDateString() };
                    var nameoffiles = from file in files
                                      select new Element { Name = file.Name, isElement = IsElement.IsFile, Date = file.CreationTime.ToShortDateString() };

                    var names = nameofdirectories.Union(nameoffiles);
                    Dispatcher.BeginInvoke((ThreadStart)delegate ()
                    {
                        LeftList.ItemsSource = names;
                    });
                }

                if (RightPath.ToString() != "")
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(RightPath.ToString());
                    DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();
                    FileInfo[] files = directoryInfo.GetFiles();

                    var nameofdirectories = from dir in subdirectories
                                            select new Element { Name = dir.Name, isElement = IsElement.IsDirectory, Date = dir.CreationTime.ToShortDateString() };
                    var nameoffiles = from file in files
                                      select new Element { Name = file.Name, isElement = IsElement.IsFile, Date = file.CreationTime.ToShortDateString() };

                    var names = nameofdirectories.Union(nameoffiles);
                    Dispatcher.BeginInvoke((ThreadStart)delegate ()
                    {
                        RightList.ItemsSource = names;
                    });
                }
            }
            catch(System.UnauthorizedAccessException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private async void Move_Click(object sender, RoutedEventArgs e)
        {
            IsWorked = true;
            if (LeftList.SelectedItems.Count > 0)
            {
                await Task.Run(() =>
                {
                    Element[] elements;
                    GetNewCollection(out elements, true);
                    foreach (Element element in elements)
                    {
                        ChangePlace(true, element, true);
                    }
                });
            }
            else if (RightList.SelectedItems.Count > 0)
            {
                await Task.Run(() =>
                {
                    Element[] elements;
                    GetNewCollection(out elements, false);
                    foreach (Element element in elements)
                    {
                        ChangePlace(true, element, false);
                    }
                });
            }
            IsWorked = false;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            IsWorked = true;
            string path;
            if (LeftList.SelectedItems.Count > 0 && LeftPath.Length > 0)
            {
                await Task.Run(() =>
                {
                    Element[] elements;
                    GetNewCollection(out elements, true);
                    path = LeftPath.ToString();
                    foreach (Element element in elements)
                    {
                        Delete(element, path);
                    }
                });
            }
            else if (RightList.SelectedItems.Count > 0 && RightPath.Length > 0)
            {
                await Task.Run(() =>
                {
                    path = RightPath.ToString();
                    Element[] elements;
                    GetNewCollection(out elements, false);
                    foreach (Element element in elements)
                    {
                        Delete(element, path);
                    }
                });
            }
            else
            {
                if (LeftPath.Length == 0 && RightPath.Length == 0 && (LeftList.SelectedItems.Count > 0 || RightList.SelectedItems.Count > 0))
                    MessageBox.Show("Диск не подлежит удалению");
                else MessageBox.Show("Не выбран элемент для удаления");
                IsWorked = false;
                return;
            }
            void Delete(Element elementfordel, String pathfordel)
            {
                if (elementfordel.isElement == IsElement.IsDirectory)
                {
                    try
                    {
                        Directory.Delete(pathfordel + elementfordel.Name, true);
                    }
                    catch (IOException exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                    catch(UnauthorizedAccessException exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(pathfordel + elementfordel.Name);
                    }
                    catch(Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }
                UpdateFiles();
            }
            
            LeftList.SelectedIndex = -1;
            RightList.SelectedIndex = -1;
            IsWorked = false;
        }
        private void AddDrives(byte a = 0)
        {
            var drives = from drive in DriveInfo.GetDrives()
                         select new Element { Name = drive.Name, isElement = IsElement.IsDrive};
            switch (a)
            {
                case 0:
                    LeftPath.Clear();
                    RightPath.Clear();
                    LeftList.ItemsSource = drives;
                    RightList.ItemsSource = drives;
                    break;
                case 1:
                    LeftPath.Clear();
                    LeftList.ItemsSource = drives;
                    break;
                case 2:
                    RightPath.Clear();
                    RightList.ItemsSource = drives;
                    break;
            }
        }
        private void GetNewCollection(out Element[] elements, bool IsLeftList)
        {
            int leftCount = 0;
            int rightCount = 0;
            Element[] secondElements = null;
            this.Dispatcher.Invoke((ThreadStart)delegate ()
            {
                leftCount = LeftList.SelectedItems.Count;
                rightCount = RightList.SelectedItems.Count;
            });

            if (IsLeftList)
            {
                secondElements = new Element[leftCount];
                elements = new Element[leftCount];
                for (int count = 0; count < elements.Length; count++)
                {
                    Dispatcher.Invoke((ThreadStart)delegate ()
                    {
                        secondElements[count] = (Element)LeftList.SelectedItems[count];
                    });
                }
                secondElements.CopyTo(elements, 0);
            }
            else
            {
                secondElements = new Element[rightCount];
                elements = new Element[rightCount];
                for (int count = 0; count < elements.Length; count++)
                {
                    Dispatcher.Invoke((ThreadStart)delegate ()
                    {
                        secondElements[count] = (Element)RightList.SelectedItems[count];
                    });
                }
                secondElements.CopyTo(elements, 0);
            }
        }
    }
    

}
