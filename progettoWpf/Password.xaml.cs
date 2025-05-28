using classi;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using Ionic.Zip;
using System.Windows.Controls.Primitives;

namespace progettoWpf
{
    /// <summary>
    /// Logica di interazione per Password.xaml
    /// </summary>
    /// 
        
    public partial class Password : Window
    {
        public ZipFile Zip { get; set; }
        public List<Filewithpsw> Files { get; set; }    //ObvservableCollection implementa INotifyPropertyChanged 
        public string Zip_path { get; set; }

        public string Result { get; set; }

        public Password(ZipFile zip, List<Filewithpsw> elm, string zip_path)   
        {
            InitializeComponent();

            Zip = zip;

            Files = elm;

            DataContext = this;

            Zip_path = zip_path;
         
        }

        private void Metti_Password_Click(object sender, RoutedEventArgs e)
        {  
            Class1.Compress(Zip, Files, Zip_path);
            //DialogResult = true;
            Close();            
        }

        

    }

}
