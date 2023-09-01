using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication10;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}

public partial class ViewModel : ObservableObject
{
    [ObservableProperty]
    private object? selectedItem;

    private CancellationTokenSource? cts;
    public ObservableCollection<object> Items { get; } = new();
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(StartCommand), nameof(StopCommand))]
    private bool started;

    private bool CanStart() => !Started;
    
    [RelayCommand(CanExecute = nameof(CanStart))]
    private void Start()
    {
        Started = true;
        cts = new CancellationTokenSource();
        Task.Run(async () => await Work(cts.Token)).ContinueWith(async (t) => await AfterTask(), TaskScheduler.FromCurrentSynchronizationContext());
    }

    async Task AfterTask()
    {
        Started = false;
        await Task.CompletedTask;
    }

    private bool CanStop() => started;
    
    [RelayCommand(CanExecute = nameof(CanStop))]
    private void Stop()
    {
        Started = false;
        cts?.Cancel();
    }

    private async Task Work(CancellationToken ct)
    {
        for (int i = 0; i < 50; i++)
        {
            if(ct.IsCancellationRequested)
                return;
            Dispatcher.UIThread.Post(() =>
            {
                Items.Insert(0, Guid.NewGuid());
                SelectedItem = Items[0];
            });
            await Task.Delay(400, ct);
        }
    }
}