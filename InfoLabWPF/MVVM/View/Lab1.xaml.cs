using System.Windows.Controls;
using System.Windows.Input;

namespace InfoLabWPF.MVVM.View;

public partial class Lab1 : UserControl
{
    public Lab1()
    {
        InitializeComponent();
    }

    private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true; 
    }
    
}