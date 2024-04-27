﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using TheXDS.Vivianne.Helpers;

namespace TheXDS.Vivianne.Views;

/// <summary>
/// Business logic for FcePreviewView.xaml
/// </summary>
public partial class FcePreviewView : UserControl
{
    public FcePreviewView()
    {
        InitializeComponent();
        PreviewMouseMove += Window_PreviewMouseMove;
    }

    Point from;
    private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        var till = e.GetPosition(sender as IInputElement);
        double dx = till.X - from.X;
        double dy = till.Y - from.Y;
        from = till;

        var distance = dx * dx + dy * dy;
        if (distance <= 0d) return;
        if (e.MouseDevice.LeftButton is MouseButtonState.Pressed)
        {
            var angle = (distance / ptcMain.FieldOfView) % 45d;
            ptcMain.Rotate(new(dx * 2, dy * 3, 0d), angle);
        }
    }
}
