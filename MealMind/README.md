# MealMind – .NET MAUI Recipe App

## Overview

MealMind is a cross-platform recipe and meal planning application built using .NET MAUI. The app allows users to discover recipes, plan meals, and manage cooking workflows with features such as timers, favourites, and API integration.

This project was developed as part of a Cross-Platform Application Development module, with specific constraints including:

* No use of the .NET MAUI Community Toolkit
* Implementation of data binding
* Persistent storage
* Multiple pages and navigation
* External API integration (TheMealDB)

---

## Features

### Recipe Discovery

* Search for recipes using TheMealDB API
* View detailed recipe information including:

  * Ingredients
  * Instructions
  * Images

### Meal Planner

* Add recipes to a meal plan
* Organise meals across days
* Simple planning workflow for weekly use

### Cooking Timers

* Create multiple timers (up to 5)
* Start / pause / reset timers
* Audio alert when timer completes

### Favourites

* Save favourite recipes locally
* Quick access to frequently used meals

### Data Persistence

* Recipes, favourites, and settings are stored locally
* Data is retained between sessions

### Settings & Accessibility

* Light / Dark mode support
* Basic accessibility considerations

---

## Project Structure

```
MealMind/
│
├── Models/                # Data models (e.g., Recipe, RecipeTimerItem)
├── Views/
│   ├── Pages/            # Full pages (RecipeList, Details, Planner)
│   └── Controls/         # Reusable UI components (TimerView)
├── ViewModels/           # Handles UI logic and data binding
├── Services/             # API calls and data persistence
├── Resources/            # Images, fonts, styles
└── App.xaml              # App entry and global styles
```

---

## Technologies Used

* .NET MAUI – Cross-platform app framework
* C# – Application logic
* XAML – UI design
* TheMealDB API – Recipe data source
* Plugin.Maui.Audio – Timer alert sound

---

## Key Implementation Details

### Data Binding

The application uses MVVM principles:

* ViewModels expose properties and commands
* UI elements bind directly to ViewModel properties
* Observable collections update UI dynamically

### Timer System

* Uses IDispatcherTimer for real-time updates
* Supports multiple concurrent timers
* Each timer is represented by a RecipeTimerItem model

### API Integration

* HTTP requests fetch recipe data from TheMealDB
* JSON responses are parsed into C# models

### Local Storage

* Data is saved using local file storage
* Ensures persistence of user preferences and favourites

---

## How to Run

### Prerequisites

* Visual Studio 2022 or later
* .NET MAUI workload installed

### SetUp

1. Clone the repository
2. Open the solution in Visual Studio
3. Select a target platform (Windows / Android / etc.)
4. Build and run the application

Explore recipes, create meal plans, and start cooking your all ready to go.


### Usage

---

## Limitations

* No Community Toolkit usage
* Limited offline functionality without downloading recipies 
* (Favourites are stored locally, but new recipes require internet) 

---

## Future Improvements

* Connect to a larger recipe database for more variety
* Add user accounts and and base recommendations on multi-user preferences
* Improved filtering and search
* Nutritional information integration

---

## Author

Sean Mitchell
BSc Software Development Student
Atlantic Technological University (ATU)

---

## License

This project is for educational purposes only.

## AI Acknowledgement

AI tools (ChatGPT) were used during the development of this project as a supporting resource.

ChatGPT was primarily used to:
- Validate design ideas and identify potential logical issues early in development
- Assist in understanding C# and .NET MAUI concepts
- Clarify implementation approaches and reinforce learning of key programming principles

All core design decisions, implementation, and final code were completed independently. 
AI was used as a learning aid to support problem-solving and improve understanding, 
rather than generating complete solutions.

The use of AI contributed to improving the overall structure, correctness, and quality of the application, 
while maintaining full ownership of the work.


## Design & Planning

The application UI and user flow were planned prior to development using concept diagrams.

### Main Page Concept

This design outlines:
- The "Meal of the Day" feature
- Navigation buttons for core pages (Search, Favourites, Meal Plan, Pantry, Shopping List)
- A structured layout for quick access to key functionality

### Meal Feature View

This component was designed to:
- Display recipes dynamically
- Support horizontal navigation between meals
- Handle missing images with placeholders

### Search Page Design


The search system was designed to support:
- Ingredient-based filtering
- Category and area filters
- API-driven results display



### Design Documents (PDF)

* [Concept Designs](docs/PDF_Design_Planning/Concept_Design.pdf)
* [Main Menu Page](docs/PDF_Design_Planning/Main Menu Page Detailed.pdf)
* [Meal Feature Main Page](docs/PDF_Design_Planning/MealFeaturePresentation MainPage.pdf)
* [Meal Feature View](docs/PDF_Design_Planning/MealFeatureViewConcept.pdf)
* [Meal Plan Page](docs/PDF_Design_Planning/MealPlanPage.pdf)
* [Recipe View Page](docs/PDF_Design_Planning/RecipeView Page Concept.pdf)
* [Search Page](docs/PDF_Design_Planning/SearchPage concept.pdf)

These designs were used to:

* Plan user navigation and workflow
* Structure reusable UI components
* Define feature requirements before implementation

---

## Credits & Resources

* diagrams.net  
  Used for all UI layout design and planning. Essential for visualising application structure and user flow.

* pixabay.com  
  Source of the timer alert sound effect used in the cooking timer feature.

* freepik.com  
  Source of image assets used for UI elements.

* pngtree.com  
  Source of additional icon and UI image assets.

* canva.com  
  Used for logo design and visual branding elements.

* TheMealDB API  
  Used as the primary data source for retrieving recipe information.

* Plugin.Maui.Audio  
  Used to implement audio playback for timer alerts.

* Microsoft .NET MAUI  
  Framework used for building the cross-platform application.

---