using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ReactiveUI;

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
    public ViewModel()
    {
        _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(400), DispatcherPriority.Background, DoWork);
        
        _itemsCache.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();
    }
    
    [ObservableProperty]
    private object? selectedItem;

    private CancellationTokenSource? cts;

    private DispatcherTimer _timer;
    

    private readonly SourceList<object> _itemsCache = new SourceList<object>();

    private readonly ReadOnlyObservableCollection<object> _items;
    
    public ReadOnlyObservableCollection<object> Items => _items;

    [RelayCommand]
    public void Start()
    {
        _timer.Start();
    }
    
    [RelayCommand]
    private void Stop()
    {
        _timer.Stop();
    }

    private void DoWork(object? sender, EventArgs e)
    {
        var newItem = Guid.NewGuid();
        _itemsCache.Insert(0, newItem);
        SelectedItem = newItem;
    }
}