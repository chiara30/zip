using classi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Collections;
using System.Runtime.CompilerServices;

namespace progettoWpf
{

    public partial class Pass_aggiorna : Window
    {
        public List<Filewithpsw> Lista { get; set; }

        public Pass_aggiorna(List<Filewithpsw> elm)
        {

            InitializeComponent();
            Lista = elm;

            DataContext = this;

        }

        //private void Aggiorna_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();

        //}

    }

        
}
