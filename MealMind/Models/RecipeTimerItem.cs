using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MealMind.Models;

// Represents a single timer item in the timer queue
// Implements INotifyPropertyChanged so UI updates automatically when values change
public class RecipeTimerItem : INotifyPropertyChanged
{
    // Unique identifier used for UI actions (delete, increase, decrease)
    public int Id { get; set; }

    // Total duration originally assigned to the timer (in seconds)
    private int _durationSeconds;
    public int DurationSeconds
    {
        get => _durationSeconds;
        set
        {
            if (_durationSeconds != value)
            {
                _durationSeconds = value;
                OnPropertyChanged();
            }
        }
    }

    // Remaining time left on the timer (in seconds)
    private int _remainingSeconds;
    public int RemainingSeconds
    {
        get => _remainingSeconds;
        set
        {
            if (_remainingSeconds != value)
            {
                _remainingSeconds = value;

                // Notify both RemainingSeconds and the formatted display string
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeText));
            }
        }
    }

    // Indicates whether this timer is currently the active countdown timer
    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
    }

    // Indicates whether the timer has completed
    private bool _isCompleted;
    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }
    }

    // Read-only formatted time string used in the UI (mm:ss)
    public string TimeText => TimeSpan
        .FromSeconds(RemainingSeconds)
        .ToString(@"mm\:ss");

    // Constructor used when creating a new timer
    public RecipeTimerItem(int id, int durationSeconds)
    {
        Id = id;
        DurationSeconds = durationSeconds;
        RemainingSeconds = durationSeconds;

        // New timers start inactive and not completed
        IsActive = false;
        IsCompleted = false;
    }

    // Required event for INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to notify the UI that a property value has changed
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}