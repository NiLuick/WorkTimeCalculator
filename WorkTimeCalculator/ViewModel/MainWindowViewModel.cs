using WorkTimeCalculator.MVVMHelper;
using WorkTimeCalculator.Service;

namespace WorkTimeCalculator.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private readonly WorkTimeCalculationService _calculationService = new();

    private string _startText = string.Empty;
    private string _endText = string.Empty;
    private string _overtimeText = string.Empty;
    private string _statusText = string.Empty;
    private bool _isWorkTimeMode = true;

    public MainWindowViewModel()
    {
        TurnOnCommand = new RelayCommand(() => StartText = DateTime.Now.ToString("HH:mm"));
        NowCommand = new RelayCommand(() => EndText = DateTime.Now.ToString("HH:mm"));
        CalculateCommand = new RelayCommand(Calculate);
    }

    public string StartText { get => _startText; set => SetProperty(ref _startText, value); }
    public string EndText { get => _endText; set => SetProperty(ref _endText, value); }
    public string OvertimeText { get => _overtimeText; private set => SetProperty(ref _overtimeText, value); }
    public string StatusText { get => _statusText; private set => SetProperty(ref _statusText, value); }
    public bool IsWorkTimeMode { get => _isWorkTimeMode; set { if (SetProperty(ref _isWorkTimeMode, value)) { OnPropertyChanged(nameof(IsGoHomeTimeMode)); } } }
    public bool IsGoHomeTimeMode { get => !_isWorkTimeMode; set => IsWorkTimeMode = !value; }

    public RelayCommand TurnOnCommand { get; }
    public RelayCommand NowCommand { get; }
    public RelayCommand CalculateCommand { get; }

    private void Calculate()
    {
        if (!TimeSpan.TryParse(StartText, out var start))
        {
            StatusText = "Ungültige Startzeit (Format HH:mm).";
            OvertimeText = string.Empty;
            return;
        }

        if (IsWorkTimeMode)
        {
            if (!TimeSpan.TryParse(EndText, out var end))
            {
                StatusText = "Ungültige Endzeit (Format HH:mm).";
                OvertimeText = string.Empty;
                return;
            }

            var overtime = _calculationService.CalculateOvertime(start, end);
            var sign = overtime < TimeSpan.Zero ? "-" : "+";
            OvertimeText = $"{sign}{overtime.Duration():hh\\:mm}";
        }
        else
        {
            var goHome = _calculationService.CalculateGoHomeTime(start);
            OvertimeText = goHome.ToString(@"hh\:mm");
        }

        StatusText = string.Empty;
    }
    
    private static DateTime GetSystemBootTime()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return DateTime.Now - uptime;
    }
}