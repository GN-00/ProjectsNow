using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Windows.MessageWindows
{
    public enum CMessageBoxType
    {
        ConfirmationWithYesNo = 0,
        ConfirmationWithYesNoCancel,
        Information,
        Error,
        Warning
    }

    public enum CMessageBoxImage
    {
        Warning = 0,
        Question,
        Information,
        Error,
        None
    }

    public enum CMessageBoxButton
    {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4
    }
    public partial class CMessageBox : Window
    {
        public CMessageBox()
        {
            InitializeComponent();
        }

        static CMessageBox _messageBox;
        static MessageBoxResult _result = MessageBoxResult.No;

        public static MessageBoxResult Show
        (string caption, string msg, CMessageBoxType type)
        {
            switch (type)
            {
                case CMessageBoxType.ConfirmationWithYesNo:
                    return Show(caption, msg, CMessageBoxButton.YesNo,
                    CMessageBoxImage.Question);
                case CMessageBoxType.ConfirmationWithYesNoCancel:
                    return Show(caption, msg, CMessageBoxButton.YesNoCancel,
                    CMessageBoxImage.Question);
                case CMessageBoxType.Information:
                    return Show(caption, msg, CMessageBoxButton.OK,
                    CMessageBoxImage.Information);
                case CMessageBoxType.Error:
                    return Show(caption, msg, CMessageBoxButton.OK,
                    CMessageBoxImage.Error);
                case CMessageBoxType.Warning:
                    return Show(caption, msg, CMessageBoxButton.OK,
                    CMessageBoxImage.Warning);
                default:
                    return MessageBoxResult.No;
            }
        }

        public static MessageBoxResult Show(string msg, CMessageBoxType type)
        {
            return Show(string.Empty, msg, type);
        }

        public static MessageBoxResult Show(string msg)
        {
            return Show(string.Empty, msg,
            CMessageBoxButton.OK, CMessageBoxImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text)
        {
            return Show(caption, text,
            CMessageBoxButton.OK, CMessageBoxImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text, CMessageBoxButton button)
        {
            return Show(caption, text, button,
            CMessageBoxImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text,
        CMessageBoxButton button, CMessageBoxImage image)
        {
            _messageBox = new CMessageBox
            { txtMsg = { Text = text }, MessageTitle = { Text = caption } };
            SetVisibilityOfButtons(button);
            SetImageOfMessageBox(image);
            _messageBox.ShowDialog();
            return _result;
        }

        private static void SetVisibilityOfButtons(CMessageBoxButton button)
        {
            switch (button)
            {
                case CMessageBoxButton.OK:
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnOk.Focus();
                    break;
                case CMessageBoxButton.OKCancel:
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;
                case CMessageBoxButton.YesNo:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Focus();
                    break;
                case CMessageBoxButton.YesNoCancel:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;
                default:
                    break;
            }
        }
        private static void SetImageOfMessageBox(CMessageBoxImage image)
        {
            switch (image)
            {
                case CMessageBoxImage.Warning:
                    _messageBox.SetImage("Warning.png");
                    break;
                case CMessageBoxImage.Question:
                    _messageBox.SetImage("Question.png");
                    break;
                case CMessageBoxImage.Information:
                    _messageBox.SetImage("Information.png");
                    break;
                case CMessageBoxImage.Error:
                    _messageBox.SetImage("Error.png");
                    break;
                default:
                    _messageBox.img.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnOk)
                _result = MessageBoxResult.OK;
            else if (sender == btnYes)
                _result = MessageBoxResult.Yes;
            else if (sender == btnNo)
                _result = MessageBoxResult.No;
            else if (sender == btnCancel)
                _result = MessageBoxResult.Cancel;
            else
                _result = MessageBoxResult.None;
            _messageBox.Close();
            _messageBox = null;
        }

        private void SetImage(string imageName)
        {
            string uri = string.Format("/Images/Icons/{0}", imageName);
            var uriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            img.Source = new BitmapImage(uriSource);
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
