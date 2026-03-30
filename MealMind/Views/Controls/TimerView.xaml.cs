using MealMind.Models;
using System.Collections.ObjectModel;

// Audio plugin used to play the timer alert sound stored in Resources/Raw
using Plugin.Maui.Audio;

namespace MealMind.Views.Controls;

public partial class TimerView : ContentView
{
    // Dispatcher timer used to tick once per second while a timer sequence is running
    private readonly IDispatcherTimer _timer;

    // Collection of timer items displayed in the timer queue
    private readonly ObservableCollection<RecipeTimerItem> _timers;

    // Tracks the next unique timer ID assigned to a new timer row
    private int _nextTimerId = 1;

    // Tracks whether the timer sequence is currently running or paused
    private bool _isRunning = false;

    // Default duration for a newly added timer (60 seconds / 1 minute)
    private const int DefaultTimerSeconds = 60;

    // Limit the queue size so the control stays manageable on screen
    private const int MaxTimers = 5;

    // Audio manager used to play the timer completion sound
    private readonly IAudioManager _audioManager = AudioManager.Current;

    public TimerView()
    {
        InitializeComponent();

        // Create the timer collection and bind it to the CollectionView in XAML
        _timers = new ObservableCollection<RecipeTimerItem>();
        TimerCollectionView.ItemsSource = _timers;

        // Create a 1-second repeating dispatcher timer for countdown updates
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;

        // Set the initial UI state
        UpdateMainDisplay();
        UpdatePlayPauseButton();
    }

    // Runs once per second while the timer is active
    private void OnTimerTick(object? sender, EventArgs e)
    {
        // Get the timer currently marked as active
        RecipeTimerItem? activeTimer = GetActiveTimer();

        // If no timer is active, try to move to the next available one
        if (activeTimer is null)
        {
            MoveToNextTimerOrStop();
            return;
        }

        // Reduce the current timer by one second while it still has time remaining
        if (activeTimer.RemainingSeconds > 0)
        {
            activeTimer.RemainingSeconds--;
            UpdateMainDisplay();
        }

        // Once the active timer reaches zero, mark it complete and move on
        if (activeTimer.RemainingSeconds <= 0)
        {
            activeTimer.RemainingSeconds = 0;
            activeTimer.IsActive = false;
            activeTimer.IsCompleted = true;

            UpdateMainDisplay();

            // Play an alert sound before switching to the next timer in the queue
            PlayAlert();

            MoveToNextTimerOrStop();
        }
    }

    // Toggles between starting and pausing the timer sequence
    private void OnPlayPauseClicked(object sender, EventArgs e)
    {
        if (_isRunning)
        {
            PauseTimerSequence();
        }
        else
        {
            StartTimerSequence();
        }
    }

    // Adds a new timer row to the queue
    private void OnAddTimerClicked(object sender, EventArgs e)
    {
        // Prevent adding more than the maximum allowed timers
        if (_timers.Count >= MaxTimers)
            return;

        // Create a new timer using the default duration
        RecipeTimerItem newTimer = new RecipeTimerItem(_nextTimerId++, DefaultTimerSeconds);
        _timers.Add(newTimer);

        // If this is the first timer in the queue, make it active immediately
        if (_timers.Count == 1)
        {
            newTimer.IsActive = true;
        }

        UpdateMainDisplay();
    }

    // Deletes a timer row from the queue
    private void OnDeleteTimerClicked(object sender, EventArgs e)
    {
        // Ensure the sender is a valid ImageButton with an ID in CommandParameter
        if (sender is not ImageButton button || button.CommandParameter is null)
            return;

        int timerId = Convert.ToInt32(button.CommandParameter);

        // Find the timer matching the selected row
        RecipeTimerItem? timerToDelete = _timers.FirstOrDefault(t => t.Id == timerId);

        if (timerToDelete is null)
            return;

        // Remembers incase the deleted timer was the active timer
        bool wasActive = timerToDelete.IsActive;

        _timers.Remove(timerToDelete);


        // If the active timer was removed, move to the next unfinished timer
        if (wasActive)
        {
            RecipeTimerItem? nextTimer = _timers.FirstOrDefault(t => !t.IsCompleted && t.RemainingSeconds > 0);

            if (nextTimer != null)
            {
                nextTimer.IsActive = true;
            }
            else
            {
                // If no timers remain, stop the sequence
                PauseTimerSequence();
            }
        }

        UpdateMainDisplay();
    }



    // Adds one minute to a timer row
    private void OnIncreaseTimerClicked(object sender, EventArgs e)
    {
        if (sender is not ImageButton button || button.CommandParameter is null)
            return;

        int timerId = Convert.ToInt32(button.CommandParameter);
        AdjustTimer(timerId, 60);
    }

    // Subtracts one minute from a timer row
    private void OnDecreaseTimerClicked(object sender, EventArgs e)
    {
        if (sender is not ImageButton button || button.CommandParameter is null)
            return;

        int timerId = Convert.ToInt32(button.CommandParameter);
        AdjustTimer(timerId, -60);
    }


    // Shared helper used by the plus/minus row buttons
    private void AdjustTimer(int timerId, int secondsDelta)
    {
        RecipeTimerItem? timerItem = _timers.FirstOrDefault(t => t.Id == timerId);

        if (timerItem is null)
            return;

        // Update both the remaining time and stored duration
        // Stop at zero so the timer cannot go negative
        timerItem.RemainingSeconds = Math.Max(0, timerItem.RemainingSeconds + secondsDelta);
        timerItem.DurationSeconds = Math.Max(0, timerItem.DurationSeconds + secondsDelta);

        UpdateMainDisplay();
    }



    // Starts the timer sequence or resumes it from pause
    private void StartTimerSequence()
    {
        if (_timers.Count == 0)
            return;

        RecipeTimerItem? activeTimer = GetActiveTimer();

        // If no timer is currently active, find the next unfinished timer and activate it
        if (activeTimer is null)
        {
            activeTimer = _timers.FirstOrDefault(t => !t.IsCompleted && t.RemainingSeconds > 0);
            if (activeTimer != null)
            {
                ClearActiveFlags();
                activeTimer.IsActive = true;
            }
        }

        // If no valid timer exists, there is nothing to start
        if (activeTimer is null)
            return;

        _isRunning = true;
        _timer.Start();
        UpdatePlayPauseButton();
        UpdateMainDisplay();
    }

    // Pauses the timer sequence without clearing the queue
    private void PauseTimerSequence()
    {
        _timer.Stop();
        _isRunning = false;
        UpdatePlayPauseButton();
    }

    // Moves the sequence onto the next unfinished timer, or stops if none remain
    private void MoveToNextTimerOrStop()
    {
        RecipeTimerItem? nextTimer = _timers.FirstOrDefault(t => !t.IsCompleted && t.RemainingSeconds > 0);

        // Ensure only one timer can be active at a time
        ClearActiveFlags();

        if (nextTimer != null)
        {
            nextTimer.IsActive = true;
            UpdateMainDisplay();
        }
        else
        {
            PauseTimerSequence();
            UpdateMainDisplay();
        }
    }

    // Returns the timer currently marked as active
    private RecipeTimerItem? GetActiveTimer()
    {
        return _timers.FirstOrDefault(t => t.IsActive);
    }

    // Clears the active flag from every timer in the queue
    private void ClearActiveFlags()
    {
        foreach (RecipeTimerItem timer in _timers)
        {
            timer.IsActive = false;
        }
    }

    // Updates the large display label to show the current active timer
    private void UpdateMainDisplay()
    {
        RecipeTimerItem? activeTimer = GetActiveTimer();

        // If no timer is active, show a default zeroed display
        if (activeTimer is null)
        {
            CurrentTimerLabel.Text = "00:00";
            return;
        }

        // Format the active timer as mm:ss
        CurrentTimerLabel.Text = TimeSpan
            .FromSeconds(activeTimer.RemainingSeconds)
            .ToString(@"mm\:ss");
    }

    // Swaps the play/pause button icon depending on timer state
    private void UpdatePlayPauseButton()
    {
        PlayPauseButton.Source = _isRunning ? "pause_icon.png" : "play_icon.png";
    }

    // Plays the alert sound three times when a timer completes
    private async void PlayAlert()
    {
        try
        {
            for (int i = 0; i < 3; i++)
            {
                // Load the MP3 from Resources/Raw each time the sound is played
                using var stream = await FileSystem.OpenAppPackageFileAsync("timer_alert.mp3");
                var player = _audioManager.CreatePlayer(stream);

                player.Play();

                // Use the player's duration where available, otherwise fall back to 1.5 seconds
                var duration = player.Duration > 0
                    ? TimeSpan.FromSeconds(player.Duration)
                    : TimeSpan.FromMilliseconds(1500);

                await Task.Delay(duration);
            }
        }
        catch
        {
            // Silent Fail:
            //  if audio fails, the timer still continues normally
        }
    }
}
