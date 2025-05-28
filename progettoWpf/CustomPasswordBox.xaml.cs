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

namespace progettoWpf
{
    /// <summary>
    /// Logica di interazione per CustomPasswordBox.xaml
    /// </summary>
    public partial class CustomPasswordBox : UserControl
    {
        public CustomPasswordBox()
        {
            InitializeComponent();
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(CustomPasswordBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) =>
                {
                    if (s is not CustomPasswordBox a) return;

                    if (a.psw_box.Password != (string)e.NewValue)
                    {
                        a.psw_box.Password = (string)e.NewValue;
                    }
                   
                }));



        private void psw_box_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = psw_box.Password;
        }
    }
}
