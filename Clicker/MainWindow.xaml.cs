using System;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

namespace Clicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        public MainWindow()
        {
            InitializeComponent();
        }

        private Key[] modifiers = {
          Key.LeftCtrl, Key.RightCtrl,
          Key.LeftAlt, Key.RightAlt,
          Key.LeftShift, Key.RightShift,
          Key.LWin, Key.RWin, Key.System
        };

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            shortcut.Text = Properties.Settings.Default["Text"].ToString();

            registerShortcut();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == KeyInterop.VirtualKeyFromKey((Key)Properties.Settings.Default.Key))
                            {
                                MouseOperations.ToggleButton();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.modifiers.Contains(e.Key)) {
                Properties.Settings.Default["Modifiers"] = e.KeyboardDevice.Modifiers.GetHashCode();
                Properties.Settings.Default["Key"] = e.Key.GetHashCode();
                Properties.Settings.Default["Text"] = keysToText(e);
                Properties.Settings.Default.Save();

                shortcut.Text = keysToText(e);

                registerShortcut();

                shortcut.Background = Brushes.White;
            }
            
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.modifiers.Contains(e.Key))
            {
                shortcut.Text = keysToText(e);
            }
        }

        private String keysToText(KeyEventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(e.KeyboardDevice.Modifiers.ToString());
            builder.Append(" + ");
            builder.Append(e.Key.ToString());

            return builder.ToString();
        }

        private void registerShortcut()
        {
            uint modifiers = (uint)Properties.Settings.Default.Modifiers;
            uint key = (uint)KeyInterop.VirtualKeyFromKey((Key)Properties.Settings.Default.Key);

            RegisterHotKey(_windowHandle, HOTKEY_ID, modifiers, key);
        }

        private void Shorcut_GotFocus(object sender, RoutedEventArgs e)
        {
            shortcut.Background = Brushes.LightYellow;
        }

        private void Shorcut_LostFocus(object sender, RoutedEventArgs e)
        {
            shortcut.Background = Brushes.White;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.ShowDialog();
        }
    }
}
