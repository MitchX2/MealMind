using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;


// used for adaptable feature cards on home page



namespace MealMind.ViewModels
    {
        public class FeatureCardVm : BindableObject
        {
            public string Key { get; init; } = ""; // "motd", "plan", "fav"

            private string _badgeText = "";
            public string BadgeText
            {
                get => _badgeText;
                set { _badgeText = value; OnPropertyChanged(); }
            }

            private string _title = "";
            public string Title
            {
                get => _title;
                set { _title = value; OnPropertyChanged(); }
            }

            private string _ingredientsPreview = "";
            public string IngredientsPreview
            {
                get => _ingredientsPreview;
                set { _ingredientsPreview = value; OnPropertyChanged(); }
            }

            // Keep this if you want it for debugging / storage
            private string _imageUrl = "";
            public string ImageUrl
            {
                get => _imageUrl;
                set { _imageUrl = value; OnPropertyChanged(); }
            }

            // ✅ NEW: actual ImageSource used by MealFeatureView
            private ImageSource? _image;
            public ImageSource? Image
            {
                get => _image;
                set { _image = value; OnPropertyChanged(); }
            }

            public ICommand? TapCommand { get; set; }
            public object? TapCommandParameter { get; set; }
        }
    }
