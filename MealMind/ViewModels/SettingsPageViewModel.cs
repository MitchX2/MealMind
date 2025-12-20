using MealMind.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealMind.Models.Settings;

namespace MealMind.ViewModels
{


    // SettingsPageViewModel
    // ViewModel for the Settings page.
    // Loads persisted app settings and exposes bindable properties for the UI.
    // Changes are saved and applied immediately via SettingsService.
    public sealed class SettingsPageViewModel : BindableObject
    {


        // SettingsService handles persistence and runtime application (theme, accessibility).
        // _model represents the full persisted AppSettings object.
        private readonly SettingsService _settings = App.Services.Settings;
        private AppSettings _model;


        // Loads saved settings and initializes bindable properties
        // so the Settings page reflects the current app configuration.
        public SettingsPageViewModel()
        {
            _model = _settings.Load();

            _theme = _model.Theme;
            _highContrastTouchZones = _model.HighContrastTouchZones;
            _showInstructionCheckboxes = _model.ShowInstructionCheckboxes;
        }


        // Selected app theme (System / Light / Dark).
        // Changing this persists the value and updates the active resource dictionary.
        private AppThemePreference _theme;
        public AppThemePreference Theme
        {
            get => _theme;
            set
            {
                if (_theme == value) return;
                _theme = value;
                OnPropertyChanged();
                PersistAndApply();
            }
        }


        // Accessibility option that enables high-contrast touch overlays
        // (used by navigation and feature buttons).
        private bool _highContrastTouchZones;
        public bool HighContrastTouchZones
        {
            get => _highContrastTouchZones;
            set
            {
                if (_highContrastTouchZones == value) return;
                _highContrastTouchZones = value;
                OnPropertyChanged();
                PersistAndApply();
            }
        }


        // Option to show checkboxes next to each instruction step in recipes.
        private bool _showInstructionCheckboxes;
        public bool ShowInstructionCheckboxes
        {
            get => _showInstructionCheckboxes;
            set
            {
                if (_showInstructionCheckboxes == value) return;
                _showInstructionCheckboxes = value;
                OnPropertyChanged();
                PersistAndApply();
            }
        }


        // Persists the current settings model and applies changes immediately.
        // Keeps settings logic centralized in SettingsService.
        private void PersistAndApply()
        {
            _model.Theme = Theme;
            _model.HighContrastTouchZones = HighContrastTouchZones;
            _model.ShowInstructionCheckboxes = ShowInstructionCheckboxes;

            _settings.SaveAndApply(_model);
        }
    }
}
