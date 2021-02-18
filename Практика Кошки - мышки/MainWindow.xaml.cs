namespace Практика_Кошки___мышки
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Pole();
            DataGrid.MouseLeftButtonUp += ((Pole)DataContext).Click;
        }
    }
}