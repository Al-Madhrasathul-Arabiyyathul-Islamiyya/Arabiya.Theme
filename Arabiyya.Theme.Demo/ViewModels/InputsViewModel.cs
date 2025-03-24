using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Arabiyya.Theme.Demo.ViewModels;
public partial class InputsViewModel : ViewModelBase
{
    public InputsViewModel() 
    {
    }

    public static async Task OnLoaded(Control view)
    {
        await Task.Delay(100);

        if (view.Parent?.Parent is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToHome();
        }
    }
}
