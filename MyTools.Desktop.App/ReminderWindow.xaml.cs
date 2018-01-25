﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyTools.Desktop.App
{
    /// <summary>
    /// Interaction logic for ReminderWindow.xaml
    /// </summary>
    public partial class ReminderWindow : Window
    {
        public ReminderWindow()
        {
            InitializeComponent();
        }

        public void SetReminderText(string text)
        {
            this.ReminderTextBlock.Text = text;
        }
    }
}
