using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace WPF_Arayuz_Deneme
{
    public partial class MainWindow : Window
    {
        String[] regs = { "$40", "$48", "$50", "$58", "$60", "$68", "$70", "$78" };
        bool[,] states = new bool[8, 5];
        Button[,] buttons = new Button[8, 5];
        ComboBox comboBox;
        int[] numbers = new int[8];
        Label[] labels = new Label[8];

        public MainWindow()
        {
            InitializeComponent();
            CreateButtons();
            kodYazdir();
        }

        private void CreateButtons()
        {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(32);
            ButtonsGrid.RowDefinitions.Add(rowDefinition);
            for (int x = 0; x < 8; x++)
            {
                ButtonsGrid.RowDefinitions.Add(new RowDefinition());
            }

            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(25);
            ButtonsGrid.ColumnDefinitions.Add(columnDefinition1);
            for (int y = 0; y < 5; y++)
            {
                ButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition2.Width = new GridLength(35);
            ButtonsGrid.ColumnDefinitions.Add(columnDefinition2);

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    buttons[x, y] = new Button
                    {
                        Background = Brushes.White,
                        Margin = new Thickness(2),
                        Tag = y * 10 + x
                    };
                    buttons[x, y].Style = (Style)FindResource("btnStyle");
                    buttons[x, y].Click += ButtonClicked;
                    Grid.SetRow(buttons[x, y], x + 1);
                    Grid.SetColumn(buttons[x, y], y + 1);
                    ButtonsGrid.Children.Add(buttons[x, y]);
                }
            }

            for (int x = 0; x < 5 ; x++)
            {
                Label label = new Label
                {
                    Content = Math.Pow(2, x),
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, 5 - x);
                ButtonsGrid.Children.Add(label);
            }

            for (int x = 0; x < 8; x++)
            {
                Label label = new Label
                {
                    Content = 1 + x,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(label, x + 1);
                Grid.SetColumn(label, 0);
                ButtonsGrid.Children.Add(label);
            }

            for (int x = 0; x < 8; x++)
            {
                labels[x] = new Label
                {
                    Content = 0,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(labels[x], x + 1);
                Grid.SetColumn(labels[x], 6);
                ButtonsGrid.Children.Add(labels[x]);
            }

            comboBox = new ComboBox
            {
                Name = "registerBox",
                FontSize = 16,
                Margin = new Thickness(5)
            };
            Grid.SetRow(comboBox, 1);
            Grid.SetColumn(comboBox, 1);
            comboBox.SelectedIndex = 0;
            for (int x = 0;x < 8; x++)
            {
                comboBox.Items.Add(x.ToString());
            }
            comboBox.SelectionChanged += (s, e) => { kodYazdir(); };
            bottomGrid.Children.Add(comboBox);
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                int buttonNumber = (int)clickedButton.Tag;
                int y = buttonNumber / 10;
                int x = buttonNumber % 10;
                if (states[x, y])
                {
                    states[x, y] = false;
                    buttons[x, y].Background = Brushes.White;
                    numbers[x] -= (int)Math.Pow(2, 4 - y);
                }
                else
                {
                    states[x, y] = true;
                    buttons[x, y].Background = Brushes.Black;
                    numbers[x] += (int)Math.Pow(2, 4 - y);
                }
                kodYazdir();
            }
        }

        private void kodYazdir()
        {
            String result = "LCDOUT $FE,";
            result += regs[Convert.ToUInt32(comboBox.SelectedItem)] + ",";
            for (int x = 0; x < 7; x++)
            {
                result += numbers[x].ToString() + ",";
                labels[x].Content = numbers[x].ToString();
            }
            result += numbers[7].ToString();
            labels[7].Content = numbers[7].ToString();
            resultLabel.Content = result;
        }

        private void kopyalaBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(resultLabel.Content.ToString());
        }

        private void sifirlaBtn_Click(Object sender, RoutedEventArgs e)
        {
            for (int  i = 0; i < 8; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    states[i,j] = false;
                    buttons[i, j].Background = Brushes.White;
                }
                numbers[i] = 0;
            }
            comboBox.SelectedIndex = 0;
            kodYazdir();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., display an error message to the user)
                MessageBox.Show("Failed to open link: " + ex.Message);
            }
            e.Handled = true;
        }
    }
}