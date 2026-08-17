# CPMCD.Dotvanta.Component

Reusable UI components for **.NET MAUI** applications.

The library provides common form controls, date/DOB pickers, dropdowns, popups and tab controls with reusable XAML APIs and theme resources.

## Features

- `CustomEntry`
- `BorderlessField`
- `CustomDatePicker`
- `CustomDobPicker`
- `CustomDropdown`
- `CustomPopup`
- `CustomTabView`
- Popup service
- Validation support
- Light/Dark theme resources
- Reusable XAML controls
- `AppThemeBinding` based theme support

## Installation

```bash
dotnet add package CPMCD.Dotvanta.Component
```

## Target Framework

Current package target:

```text
net9.0-android
```

A compatible .NET MAUI workload and Android target environment are required.

## Register the Theme

Before using the controls, register the Dotvanta component theme.

For example, from `MauiProgram.cs` or after `Application.Current` is available:

```csharp
using CPMCD.Dotvanta.Component;

CpmcdComponentTheme.Register(Application.Current);
```

This loads the component color resources used by the controls.

## XAML Namespace

Add:

```xml
xmlns:cpmcd="clr-namespace:CPMCD.Dotvanta.Component.Controls;assembly=CPMCD.Dotvanta.Component"
```

## CustomEntry

A reusable entry control for application forms.

```xml
<cpmcd:CustomEntry
    Label="Email"
    Placeholder="Enter email"
    Text="{Binding Email}" />
```

Use it when you need a consistent form field across multiple pages.

## BorderlessField

```xml
<cpmcd:BorderlessField
    Label="Email"
    Placeholder="Enter email"
    Text="{Binding Email}"
    ErrorText="{Binding EmailError}" />
```

Useful for modern borderless form layouts and validation messages.

## CustomDatePicker

```xml
<cpmcd:CustomDatePicker
    Label="Joining Date"
    SelectedDate="{Binding JoiningDate}"
    MinimumDate="1950-01-01" />
```

You can constrain the selectable date range using minimum and maximum dates.

## CustomDobPicker

Designed specifically for date-of-birth scenarios.

```xml
<cpmcd:CustomDobPicker
    Label="Date of Birth"
    DateOfBirth="{Binding Dob}"
    MinimumAge="18"
    MaximumAge="100"
    ShowAge="True" />
```

The control supports age constraints and can expose the calculated age.

Code-behind event example:

```csharp
dobPicker.DateOfBirthChanged += (sender, dob) =>
{
    // Handle DOB change
};
```

Calculated age:

```csharp
var age = dobPicker.CalculatedAge;
```

## CustomDropdown

```xml
<cpmcd:CustomDropdown
    Label="City"
    ItemsSource="{Binding Cities}"
    SelectedItem="{Binding SelectedCity}" />
```

For object collections:

```xml
<cpmcd:CustomDropdown
    Label="City"
    ItemsSource="{Binding Cities}"
    SelectedItem="{Binding SelectedCity}"
    ItemDisplayBinding="{Binding Name}" />
```

## CustomPopup

The popup service displays a reusable overlay on a page.

The page root should be a `Grid` so the popup can be displayed above the page content.

```csharp
var popup = new CustomPopup
{
    Title = "Delete Item?",
    PrimaryButtonText = "Delete",
    SecondaryButtonText = "Cancel"
};

popup.PrimaryClicked += (sender, args) =>
{
    // Delete logic

    CpmcdPopupService.Hide(this, popup);
};

popup.SecondaryClicked += (sender, args) =>
{
    CpmcdPopupService.Hide(this, popup);
};

CpmcdPopupService.Show(this, popup);
```

## CustomTabView

Supports horizontal and vertical tab layouts.

```csharp
tabView.Orientation = CpmcdTabOrientation.Horizontal;

tabView.Items = new ObservableCollection<CpmcdTabItem>
{
    new CpmcdTabItem
    {
        Title = "Profile",
        Content = profileView
    },
    new CpmcdTabItem
    {
        Title = "Settings",
        Content = settingsView
    }
};

tabView.TabChanged += (sender, index) =>
{
    // Handle tab change
};
```

## Theme Resources

The package contains theme resources under:

```text
Resources/Styles/Colors.xaml
```

The controls are designed to support light/dark application themes through MAUI theme bindings.

## Recommended Usage

Keep business logic outside the controls.

Recommended architecture:

```text
Page
 └── ViewModel
      └── Application/Domain Service
           └── API / Repository
```

Use the controls as presentation-layer components.

## Compatibility

The package is currently Android-focused because its project target is:

```xml
<TargetFrameworks>net9.0-android</TargetFrameworks>
```

If iOS, MacCatalyst or Windows support is required, the project should be multi-targeted and tested against those MAUI platforms before publishing a cross-platform release.

## Build

```bash
dotnet restore
dotnet build -c Release
dotnet pack -c Release
```

## NuGet Notes

For NuGet package README rendering, include the README in the `.csproj` package metadata:

```xml
<PackageReadmeFile>README.md</PackageReadmeFile>
```

and:

```xml
<ItemGroup>
  <None Include="README.md"
        Pack="true"
        PackagePath="\" />
</ItemGroup>
```

## Author

**CPMCD : Faisal Raza Khan**

## License

Add the final project license before public distribution.
