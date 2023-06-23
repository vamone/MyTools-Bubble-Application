using MyTools.Desktop.App;
using MyTools.Desktop.App.Helpers;
using MyTools.Desktop.App2.Models;
using MyTools.Desktop.App2.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace MyTools.Desktop.App2;

public partial class LoggerWindow : Window
{
    readonly IFileReaderService<ICollection<TaskLogger>> _taskService;

    public LoggerWindow(IFileReaderService<ICollection<TaskLogger>> taskService)
    {
        this._taskService = taskService;

        InitializeComponent();

        this.richTextBoxDescription.Document.Blocks.Clear();
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void buttonSaveStart_Click(object sender, RoutedEventArgs e)
    {
        this.Save();

        var window = WindowHelper.GetWindowByClassName<MainWindow>();
        window.StartFocusTimer(sender, e);

        this.Close();
    }

    private void buttonStartOnly_Click(object sender, RoutedEventArgs e)
    {
        var window = WindowHelper.GetWindowByClassName<MainWindow>();
        window.StartFocusTimer(sender, e);

        this.Close();
    }

    private void buttonLogOnly_Click(object sender, RoutedEventArgs e)
    {
        this.Save();
        this.Close();
    }

    private void Save()
    {
        try
        {
            var tasks = this._taskService.Get() ?? new List<TaskLogger>();

            tasks.Add(new TaskLogger
            {
                TaskId = this.textBoxTaskId.Text,
                TaskDescription = new TextRange(this.richTextBoxDescription.Document.ContentStart, this.richTextBoxDescription.Document.ContentEnd).Text
            });

            this._taskService.Set(tasks);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }
}