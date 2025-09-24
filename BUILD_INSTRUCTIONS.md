# Activity Tracker - Build and Deployment Instructions

## Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 with .NET MAUI workload OR
- Visual Studio Code with .NET MAUI extension

## Building for Android

### Debug Build
```bash
dotnet build -t:Run -f net9.0-android
```

### Release Build (APK)
```bash
dotnet publish -f net9.0-android -c Release
```

The APK will be generated in:
`bin\Release\net9.0-android\publish\`

### Creating a Signed APK for Distribution

1. Generate a keystore (one time only):
```bash
keytool -genkey -v -keystore myapp.keystore -alias myapp -keyalg RSA -keysize 2048 -validity 10000
```

2. Add to your .csproj file:
```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
    <AndroidKeyStore>True</AndroidKeyStore>
    <AndroidSigningKeyStore>BabyTime.keystore</AndroidSigningKeyStore>
    <AndroidSigningKeyAlias>BabyTime</AndroidSigningKeyAlias>
    <AndroidSigningKeyPass></AndroidSigningKeyPass>
    <AndroidSigningStorePass></AndroidSigningStorePass>
</PropertyGroup>
```

3. Build signed APK:
```bash
dotnet publish -f net9.0-android -c Release /p:AndroidSigningKeyPass=4NuNrfrp4A3ptbCdUHzk /p:AndroidSigningStorePass=4NuNrfrp4A3ptbCdUHzk
```

## Installing on Android Device

### Method 1: Direct APK Installation
1. Enable "Install from Unknown Sources" in Android settings
2. Copy the APK to your device
3. Open the APK file on the device to install

### Method 2: Using ADB
```bash
adb install path/to/your.apk
```

## Publishing to Google Play Store

1. Create a Google Play Developer account ($25 one-time fee)
2. Create an app listing in Google Play Console
3. Build a signed AAB (Android App Bundle):
```bash
dotnet publish -f net9.0-android -c Release /p:AndroidPackageFormat=aab
```
4. Upload the AAB file to Google Play Console
5. Fill in all required information (description, screenshots, etc.)
6. Submit for review

## Features Implemented

- **Input Page**: Time selection with wheel-like pickers for hours and minutes (pre-set to current Sofia, Bulgaria time)
- **Activity Selection**: Dropdown with "Eat" and "Sleep" options
- **Add Button**: Saves activity with success notification (green rectangle for 5 seconds)
- **Today Page**: Shows all activities for the current day with time and activity type
- **Statistics Page**: Shows last 7 days statistics with breakdown by activity type
- **SQLite Database**: Local storage on device
- **Bottom Navigation**: Three tabs for easy navigation

## Notes
- Time is automatically set to Sofia, Bulgaria timezone
- Database is stored locally on the device
- App supports Android 5.0 (API 21) and above