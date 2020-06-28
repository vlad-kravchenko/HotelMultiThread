using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace HotelMultiThread
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        Random rand = new Random();
        List<Room> rooms = new List<Room>();
        DateTime global = DateTime.Now;
        int rows = 5, cols = 10;

        public MainWindow()
        {
            InitializeComponent();

            SetupGrid();
        }

        private void timerTick(object sender, EventArgs e)
        {
            global = global.AddDays(1);
            tbDate.Text = global.ToShortDateString();
            foreach (var block in MainGrid.Children.Cast<Panel>().Select(p => p.Children.Cast<TextBlock>().ToList()[1]))
            {
                Panel parent = block.Parent as Panel;
                Room room = (block.Tag as Room);
                if (room == null) continue;
                if (room.Empty == false)
                {
                    room.Now = global;
                    double left = room.Left;
                    if (left > 0)
                    {
                        block.Text = room.Left.ToString();
                        parent.Background = Brushes.Red;
                    }
                    else
                    {
                        block.Text = "";
                        parent.Background = Brushes.Green;
                        room.Empty = true;
                    }
                }
                else
                {
                    block.Text = "";
                    parent.Background = Brushes.Green;
                }
            }

            if (DateTime.Now.Second % 2 == 0)
            {
                var emptyRooms = rooms.Where(r => r.Empty).ToList();
                var room = emptyRooms[rand.Next(0, emptyRooms.Count())];
                room.Empty = false;
                room.Now = global;
                room.CheckIn = global;
                room.CheckOut = global.AddDays(rand.Next(3, 20));
            }
        }

        private void SetupGrid()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
            rooms = new List<Room>();
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < rows; i++) MainGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < cols; i++) MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            int k = 1;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Room room = new Room
                    {
                        Number = k,
                        Empty = true
                    };
                    rooms.Add(room);

                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Vertical;
                    MainGrid.Children.Add(panel);
                    Grid.SetRow(panel, row);
                    Grid.SetColumn(panel, col);

                    TextBlock tbNumber = new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        FontSize = this.Height / 20,
                        TextWrapping = TextWrapping.Wrap,
                        Text = k.ToString()
                    };
                    TextBlock tbLeft = new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = this.Height / 13,
                        TextWrapping = TextWrapping.Wrap,
                        Text = "",
                        Tag = room
                    };
                    panel.Children.Add(tbNumber);
                    panel.Children.Add(tbLeft);
                    panel.Background = Brushes.Green;
                    k++;
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var block in MainGrid.Children.Cast<Panel>().Select(p => p.Children.Cast<TextBlock>().FirstOrDefault()))
            {
                block.FontSize = this.Height / 20;
            }
            foreach (var block in MainGrid.Children.Cast<Panel>().Select(p => p.Children.Cast<TextBlock>().ToList()[1]))
            {
                block.FontSize = this.Height / 13;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = 0, col = 0;
            GetClickCoordinates(out row, out col);
            if (!InRange(row, col)) return;

            Panel panel = MainGrid.Children.Cast<Panel>().FirstOrDefault(p => Grid.GetRow(p) == row && Grid.GetColumn(p) == col);
            if (panel == null) return;
            Room room = panel.Children.Cast<TextBlock>().ToList()[1].Tag as Room;
            if (room == null) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                room.Empty = false;
                room.Now = global;
                room.CheckIn = global;
                room.CheckOut = global.AddDays(rand.Next(3, 20));
            }
            else
            {
                room.Empty = true;
            }
        }

        private bool InRange(int row, int col)
        {
            return row < rows && row > -1 && col < cols && col > -1;
        }

        private void GetClickCoordinates(out int row, out int col)
        {
            col = row = 0;
            var point = Mouse.GetPosition(MainGrid);
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;
            foreach (var rowDefinition in MainGrid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }
            foreach (var columnDefinition in MainGrid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            SetupGrid();
        }
    }
}