using Microsoft.WindowsAPICodePack.Dialogs;
using MS.WindowsAPICodePack.Internal;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace winball501_s_antivirus_test_tool
{
    public partial class MainWindow : Window
    {
        private List<string> filePaths = new List<string>();
        private string selectedPath;
 
        public MainWindow()
        {
            InitializeComponent();
            if(!File.Exists(Environment.CurrentDirectory + "\\settings.txt"))
            {
                File.Create(Environment.CurrentDirectory + "\\settings.txt").Close();
            }
        
            loadsettings();
       
        }
        

        public string stringtokenizer(string input, string token, int index)
        {

            string[] tokens = input.Split(token);
            if (index >= 0 && index < tokens.Length)
            {
                return (tokens[index]);
            }
            else
            {
                return (null);
            }
        }
        public void loadsettings()
        {
            settings = new SettingsWindow();
            string[] lines = File.ReadAllLines(settings.settingspath);
            settings.Show();
            foreach (string line in lines)
            {
                if(line.Contains("delay="))
                {
                   string input = stringtokenizer(line, "=", 1);
                   settings.delayTextBox.Text = input;
                }

                if (line.Contains("savestat=0"))
                {
                    settings.saveStatusCheckBox.IsChecked = false;
                }
                else if (line.Contains("savestat=1"))
                {
                    settings.saveStatusCheckBox.IsChecked = true;
                }




                if (line.Contains("nolog=1"))
                {
                    settings.noLogRadio.IsChecked = true;
                }




                if (line.Contains("notexecuted=1"))
                {
                    settings.notExecutedOnlyRadio.IsChecked = true;
                }


                if (line.Contains("executed=1"))
                {
                    settings.executedOnlyRadio.IsChecked = true;
                }


                if (line.Contains("all=1"))
                {
                    settings.allFilesRadio.IsChecked = true;
                }



            }
            settings.Hide();
        }
        private void AddPathButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Folder Containing Files to Test"
            };

            if (folderPicker.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPath = folderPicker.FileName;
                filePaths = GetFilesFromDirectory(selectedPath);

                directoryLabel.Text = selectedPath;
                filesExecutedLabel.Text = $"{0} / {filePaths.Count}";
            }
        }

        private async void StartExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (filePaths.Count == 0)
            {
                MessageBox.Show("Please add a path before starting the test.", "No Path Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                startExecuteButton.IsEnabled = false;
                if (settings.saveStatusCheckBox.IsChecked == true)
                {
                    if (File.Exists(Environment.CurrentDirectory + "\\notexecuted.txt") && File.Exists(Environment.CurrentDirectory + "\\executed.txt"))
                    {
                        string executed = File.ReadAllText(Environment.CurrentDirectory + "\\executed.txt");
                        string notexecuted = File.ReadAllText(Environment.CurrentDirectory + "\\notexecuted.txt");

                        Thread t = new Thread(() => ExecuteFilesAsync(filePaths, int.Parse(executed), int.Parse(notexecuted)));
                        t.Start();
                    } else
                    {
                        Thread t = new Thread(() => ExecuteFilesAsync(filePaths, 0, 0));
                        t.Start();
                    }
                }
                else
                {
                    Thread t = new Thread(() => ExecuteFilesAsync(filePaths, 0, 0));
                    t.Start();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private List<string> GetFilesFromDirectory(string path)
        {
            List<string> files = new List<string>();

            if (Directory.Exists(path))
            {
                files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
            }

            return files;
        }

        private async Task ExecuteFilesAsync(List<string> files, int executed = 0, int notexecuted = 0)
        {
            int totalFiles = files.Count;

            string outputpath = Environment.CurrentDirectory + "\\output.txt";
            string executedpath = Environment.CurrentDirectory + "\\executed.txt";
            string notexecutedpath = Environment.CurrentDirectory + "\\notexecuted.txt";
            int total = executed + notexecuted;
            for (int i = total; i < totalFiles; i++)
            {
                string file = files[i];
                Process checkp = null;
                byte checkptwo = 0;

                try
                {
                    checkp = Process.Start(file);
                    checkptwo = 1;
                    if (checkptwo == 1)
                    {
                        if (checkp != null)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                fileListBox.Items.Add($"Executed: {file}");
                               
                            });
                            this.Dispatcher.Invoke(() =>
                            {
                                if (settings.executedOnlyRadio.IsChecked == true || settings.allFilesRadio.IsChecked == true)
                                {
                                    using (StreamWriter writer = new StreamWriter(outputpath, append: true))
                                    {
                                        writer.WriteLine($"Executed: {file}");
                                    }
                                }
                            });
                             
                            executed++;
                            this.Dispatcher.Invoke(() =>
                            {
                                if (settings.saveStatusCheckBox.IsChecked == true)
                                {
                                    File.WriteAllText(executedpath, executed.ToString());
                                }
                            });
                        
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                fileListBox.Items.Add($"Error executing: {file}");

                            });
                            this.Dispatcher.Invoke(() =>
                            {
                                if (settings.notExecutedOnlyRadio.IsChecked == true || settings.allFilesRadio.IsChecked == true)
                                {
                                    using (StreamWriter writer = new StreamWriter(outputpath, append: true))
                                    {
                                        writer.WriteLine($"Error executing: {file}");
                                    }
                                }
                            });
                        
                            notexecuted++;
                            this.Dispatcher.Invoke(() =>
                            {
                                if (settings.saveStatusCheckBox.IsChecked == true)
                                {
                                    File.WriteAllText(notexecutedpath, notexecuted.ToString());
                                }
                            });
                           
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        fileListBox.Items.Add($"Error executing {file}: {ex.Message}");
                    });
                    this.Dispatcher.Invoke(() =>
                    {
                        if (settings.notExecutedOnlyRadio.IsChecked == true || settings.allFilesRadio.IsChecked == true)
                        {
                            using (StreamWriter writer = new StreamWriter(outputpath, append: true))
                            {
                                writer.WriteLine($"Error executing {file}: {ex.Message}");
                            }
                        }
                    });
                
                    notexecuted++;
                    this.Dispatcher.Invoke(() =>
                    {
                        if (settings.saveStatusCheckBox.IsChecked == true)
                        {
                            File.WriteAllText(notexecutedpath, notexecuted.ToString());
                        }
                    });
                
                }

                finally
                {


                    await this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        filesExecutedLabel.Text = $"{executed} / {totalFiles}";
                        accessDeniedLabel.Text = $"{notexecuted}";
                        double progress = ((double)notexecuted / totalFiles) * 100;
                        progressBar.Value = progress;
                        percentageText.Text = $"{progress:0.00}%";
                        ScrollListBoxToBottomIfNeeded();
                    }));
                    await this.Dispatcher.Invoke(async () =>
                    {
                        if (slowModeCheckBox.IsChecked == true)
                        {
                            await Task.Delay(700);
                        }
                        if (settings.delayTextBox.Text.Length > 0)
                        {
                            await Task.Delay(int.Parse(settings.delayTextBox.Text));
                           
                        }
                    });


                }

            }
            this.Dispatcher.Invoke(() =>
            {
                startExecuteButton.IsEnabled = true;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    fileListBox.Items.Add("Test Completed!");
                }));
                File.Delete(notexecutedpath);
                File.Delete(executedpath);
            });

        }
        protected override void OnClosing(CancelEventArgs e)
        {

            Environment.Exit(0);

        }
        private void ProcessFiles(List<string> filePaths)
        {
            int processed = 0;
            foreach (var file in filePaths)
            {
                processed++;
                double progress = ((double)processed / filePaths.Count) * 100;


                Dispatcher.Invoke(() =>
                {
                    progressBar.Value = progress;
                    percentageText.Text = $"{(int)progress}%";

                    fileListBox.Items.Add("Processing file: " + file);


                    ScrollListBoxToBottomIfNeeded();
                });
                try
                {
                    Dispatcher.Invoke(() => fileListBox.Items.Add("Processing file: " + file));

                    string newFilePath = System.IO.Path.ChangeExtension(file, ".exe");

                    if (!File.Exists(newFilePath))
                    {
                        File.Copy(file, newFilePath);
                        File.Delete(file);


                        Dispatcher.Invoke(() => fileListBox.Items.Add($"File copied as: {newFilePath}"));
                    }
                    else
                    {
                        Dispatcher.Invoke(() => fileListBox.Items.Add($"File with name {newFilePath} already exists."));
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => fileListBox.Items.Add($"Cannot change extension for " + file));
                }

            }
        }

        private void ScrollListBoxToBottomIfNeeded()
        {
            var scrollViewer = GetScrollViewer(fileListBox);
            if (scrollViewer != null)
            {

                double scrollableHeight = scrollViewer.ScrollableHeight;
                double verticalOffset = scrollViewer.VerticalOffset;
                double threshold = 10;


                if (scrollableHeight - verticalOffset <= threshold)
                {

                    fileListBox.ScrollIntoView(fileListBox.Items[fileListBox.Items.Count - 1]);
                }
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject obj)
        {

            if (obj is ScrollViewer)
                return obj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

            progressBar.Value = 0;
            percentageText.Text = "0%";
            filesExecutedLabel.Text = "0 / 0";
            accessDeniedLabel.Text = "0";
            directoryLabel.Text = "No directory selected";
            fileListBox.Items.Clear();
            filePaths.Clear();
        }

        private void ClearListButton_Click(object sender, RoutedEventArgs e)
        {

            fileListBox.Items.Clear();
        }

        private async void MakeExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (filePaths.Count > 0)
            {
                fileListBox.Items.Clear();
                makeExecutableButton.IsEnabled = false;
                await Task.Run(() => ProcessFiles(filePaths));
                filePaths.Clear();
                filePaths = GetFilesFromDirectory(selectedPath);
                makeExecutableButton.IsEnabled = true;
                MessageBox.Show("Operation Done!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Directory not selected!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        SettingsWindow settings;
        private void SettingsClick(object sender, RoutedEventArgs e)
        {
           
            settings.Show();
        }


    }
}
