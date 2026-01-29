using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace winball501_s_antivirus_test_tool
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public string settingspath = Environment.CurrentDirectory + "\\settings.txt";
        public SettingsWindow()
        {
            InitializeComponent();
        }
        public void savesettings()
        {
            try
            {
                string[] lines = File.ReadAllLines(settingspath);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith("delay=") && delayTextBox.Text.Length > 0)
                        {

                            lines[i] = "delay=" + delayTextBox.Text;
                        }

                        if (saveStatusCheckBox.IsChecked == true)
                        {
                            if (lines[i].Contains("savestat=0"))
                            {
                                lines[i] = "savestat=1";
                            }
                        }
                        if (saveStatusCheckBox.IsChecked == true)
                        {
                            if (lines[i].Contains("savestat=0"))
                            {
                                lines[i] = "savestat=1";
                            }

                        }
                        else
                        {
                            if (lines[i].Contains("savestat=1"))
                            {
                                lines[i] = "savestaat=0";
                            }
                        }
                        if (noLogRadio.IsChecked == true)
                        {

                            if (lines[i].Contains("nolog=0"))
                            {
                                lines[i] = "nolog=1";
                            }

                        }
                        else
                        {
                            if (lines[i].Contains("nolog=1"))
                            {
                                lines[i] = "nolog=0";
                            }
                        }

                        if (notExecutedOnlyRadio.IsChecked == true)
                        {
                            if (lines[i].Contains("notexecuted=0"))
                            {
                                lines[i] = "notexecuted=1";
                            }
                        }
                        else
                        {
                            if (lines[i].Contains("notexecuted=1"))
                            {
                                lines[i] = "notexecuted=0";
                            }
                        }
                        if (executedOnlyRadio.IsChecked == true)
                        {
                            if (lines[i].Contains("executed=0"))
                            {
                                lines[i] = "executed=1";
                            }
                        }
                        else
                        {
                            if (lines[i].Contains("executed=1"))
                            {
                                lines[i] = "executed=0";
                            }
                        }
                        if (allFilesRadio.IsChecked == true)
                        {
                            if (lines[i].Contains("all=0"))
                            {
                                lines[i] = "all=1";
                            }
                        }
                        else
                        {
                            if (lines[i].Contains("all=1"))
                            {
                                lines[i] = "all=0";
                            }
                        }
                     


                    }
                    File.Delete(settingspath);
                    using (StreamWriter writer = new StreamWriter(settingspath, append: true))
                    {
                        foreach (string line in lines)
                    {
                  
                            writer.WriteLine(line);
                      
                    }
                    }
                }
                else
                {
                    StreamWriter writer = new StreamWriter(settingspath, append: true);
                    if (delayTextBox.Text.Length > 0)
                    {
                         writer.WriteLine("delay=" +  delayTextBox.Text);
                    } else
                    {
                         writer.WriteLine("delay=" + 0);
                    }
                    if (saveStatusCheckBox.IsChecked == true)
                    {

                        writer.WriteLine("savestat=1");

                    }
                    else
                    {

                        writer.WriteLine("savestat=0");

                    }
                    if (noLogRadio.IsChecked == true)
                    {

                       
                            writer.WriteLine("nolog=1");
                       

                    }
                    else
                    {
                       
                            writer.WriteLine("nolog=0");
                        
                    }

                    if (notExecutedOnlyRadio.IsChecked == true)
                    {
                      
                            writer.WriteLine("notexecuted=1");
                        
                    }
                    else
                    {
                        
                            writer.WriteLine("notexecuted=0");
                        
                    }
                    if (executedOnlyRadio.IsChecked == true)
                    {
                       
                            writer.WriteLine("executed=1");
                        
                    }
                    else
                    {
                     
                            writer.WriteLine("executed=0");
                        
                    }
                    if (allFilesRadio.IsChecked == true)
                    {
                       
                            writer.WriteLine("all=1");
                       
                    }
                    else
                    {
                        
                            writer.WriteLine("all=0");
                       
                    }
                    writer.Close();

                }
                MessageBox.Show("Settings saved successfully.");
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         

        }
        protected override void OnClosing(CancelEventArgs e)
        {
        
            e.Cancel = true;

            
            this.Hide();
 
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            savesettings();
        }
        private void Settings_Closing(object sender, CancelEventArgs e)
        {

            e.Cancel = true;


            this.Hide();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private static readonly Regex _numericRegex = new Regex("^[0-9]+$");

        private void DelayTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_numericRegex.IsMatch(e.Text);
        }

        private void DelayTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!_numericRegex.IsMatch(text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }
        private void delayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
