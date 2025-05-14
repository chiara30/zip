using System.Windows;

namespace progettoWpf
{
    /// <summary>
    /// Logica di interazione per password.xaml
    /// </summary>
    public partial class Pass_ZipStream : Window
    {
        public string Result { get; private set; }
    
        public Pass_ZipStream()
        {
            InitializeComponent();          
        }
        
        private void Enter_pass_click(object sender, RoutedEventArgs e)
        {            
            if(pass_check.IsChecked == false && pass_box.Password.Length == 0)
            {
                pass_text.Text = "Enter a password";                
            }
            else if(pass_check.IsChecked == false)
            {     
                Result = pass_box.Password;

                DialogResult = true;
                this.Close();
            }else
            {
                //no password             
                DialogResult = false;
                Close();
            }            
        }
    }
}
