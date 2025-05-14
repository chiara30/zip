using System.Windows;

namespace progettoWpf
{
    /// <summary>
    /// Logica di interazione per Pass_ZipFile.xaml
    /// </summary>
    public partial class Pass_ZipFile : Window
    {
        public string Result { get; private set; }

        public Pass_ZipFile()
        {
            InitializeComponent();           
        }

        //chiedo la password se le entries sono protette (in questa finestra non serve una checkbox)

        private void Enter_pass_click(object sender, RoutedEventArgs e)
        {
            if (pass_box.Password.Length == 0)
            {
                //non inserisce password
                pass_text.Text = "Enter a password";
            }
            else 
            {
                //gestione password errata nella finestra Picker
                Result = pass_box.Password;

                DialogResult = true;
                Close();
            }
        }
    }
}
