using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication10;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ((ViewModel)DataContext!).Start();
    }
}

public partial class ViewModel : ObservableObject
{
    [ObservableProperty]
    private object? selectedItem;

    private CancellationTokenSource? cts;
    public ObservableCollection<object> Items { get; } = new();

    [RelayCommand]
    public void Start()
    {
        cts = new CancellationTokenSource();
        Task.Run(async () => await Work(cts.Token));
    }
    
    [RelayCommand]
    private void Stop()
    {
        cts?.Cancel();
    }

    private async Task Work(CancellationToken ct)
    {
        for (int i = 0; i < 50; i++)
        {
            if(ct.IsCancellationRequested)
                return;
            Items.Insert(0, Guid.NewGuid());
            SelectedItem = Items[0];
            await Task.Delay(400, ct);
        }
    }
}