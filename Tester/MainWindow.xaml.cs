using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DictionaryHelper;

namespace Tester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ReadOnlyObservableDictionary<string, string> RODictionary { get; }
        public ObservableDictionary<string, string> Dictionary { get; } = new ObservableDictionary<string, string>();
        string[] keys = { "Key1", "Key2", "Key3" };
        string[] values = { "Val1", "Val2", "Val3" };
        public MainWindow()
        {
            RODictionary = new ReadOnlyObservableDictionary<string, string>(Dictionary);
            InitializeComponent();
        }

        void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary.Add(keys[Dictionary.Count], values[Dictionary.Count]);
        }

        void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary.Remove(Dictionary.Keys[Dictionary.Count-1]);
        }

        void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary[Dictionary.Last().Key] = values[new Random().Next(Dictionary.Count)];
        }
    }
}
