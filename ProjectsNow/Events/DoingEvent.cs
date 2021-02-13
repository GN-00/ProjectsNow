using System.Windows;
using System.Threading;

namespace ProjectsNow.Events
{
    class DoingEvent
    {
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
    }
}
