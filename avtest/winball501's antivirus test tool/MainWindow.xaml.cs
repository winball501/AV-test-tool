using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using MS.WindowsAPICodePack.Internal;

namespace winball501_s_antivirus_test_tool
{
    public partial class MainWindow : Window
    {
        private List<string> filePaths = new List<string>();
        private string selectedPath;
        public MainWindow()
        {
            InitializeComponent();
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
                filesExecutedLabel.Text = $"Executed Successfully: {0} / {filePaths.Count}";
            }
        }

        private async void StartExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (filePaths.Count == 0)
            {
                MessageBox.Show("Please add a path before starting the test.", "No Path Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            startExecuteButton.IsEnabled = false;


            await ExecuteFilesAsync(filePaths);

            startExecuteButton.IsEnabled = true;
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

        private async Task ExecuteFilesAsync(List<string> files)
        {
            int totalFiles = files.Count;
            int executed = 0;
            int notexecuted = 0;
             

            for (int i = 0; i < totalFiles; i++)
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
                            executed++;
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                fileListBox.Items.Add($"Error executing: {file}");
                            });
                            notexecuted++;
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
                    notexecuted++;
                }

                finally
                {
                 

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        filesExecutedLabel.Text = $"Executed Successfully: {executed} / {totalFiles}";
                        accessDeniedLabel.Text = $"Not Executed May Blocked By Antivirus: {notexecuted}";
                        double progress = ((double)notexecuted / totalFiles) * 100;
                        progressBar.Value = progress;
                        percentageText.Text = $"{progress:0}%";
                        ScrollListBoxToBottomIfNeeded();
                    }));
                    if(slowModeCheckBox.IsChecked == true)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(700));
                    }
                  
                }
              
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                fileListBox.Items.Add("Test Completed!");
            }));
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
                 
                    string newFilePath = Path.ChangeExtension(file, ".exe");

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
                } catch (Exception ex)
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
            filesExecutedLabel.Text = "Executed Successfully: 0 / 0";
            accessDeniedLabel.Text = "Not Executed May Blocked By Antivirus: 0";
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
            if(filePaths.Count > 0)
            {
                fileListBox.Items.Clear();
                makeExecutableButton.IsEnabled = false;
                await Task.Run(() => ProcessFiles(filePaths));
                filePaths.Clear();
                filePaths = GetFilesFromDirectory(selectedPath);
                makeExecutableButton.IsEnabled = true;
                MessageBox.Show("Operation Done!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            } else
            {
                MessageBox.Show("Directory not selected!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
        }
    }
}