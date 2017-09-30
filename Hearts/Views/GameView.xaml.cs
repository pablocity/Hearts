﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hearts.Model;
using System.Drawing;
using System.Resources;
using System.Reflection;

namespace Hearts
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class GameView : Window
    {
        public Deck deck;
        
        public GameView()
        {
            InitializeComponent();

            deck = new Deck();
            //DataContext = this;
            //handCards.ItemsSource = deck.cards;
        }
    }
}
