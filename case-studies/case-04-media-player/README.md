# Media Player Case Study

A practical implementation of the **Liskov Substitution Principle** using a media player that handles different file types seamlessly.

## 🎯 Problem Statement

Design a media player that:

- Handles different media types (audio, video)
- Supports type-specific features (subtitles for video only)
- Works consistently without breaking when substituting different file types
- Maintains clean, extensible code structure

**Challenge**: How do we handle type-specific features without violating substitutability?

## 📋 LSP Rules Applied

### Core Rules ✅

- **No new exceptions** - Subclasses don't throw unexpected exceptions
- **Preconditions cannot be strengthened** - Child classes aren't more restrictive
- **Postconditions cannot be weakened** - Child classes deliver at least what parent promises
- **Invariants must be preserved** - Core behavioral contracts remain intact
- **History constraint** - New methods don't break existing behavior

### Variance Rules ✅

- **Contravariance** - Methods can accept broader input types
- **Covariance** - Methods can return more specific output types

## ✨ Solution Architecture

```
MediaFile (Abstract Base)
├── AudioFile (extends MediaFile)
└── VideoFile (extends MediaFile, implements ISubtitleCapable)

ISubtitleCapable (Interface)
└── Optional subtitle functionality

MediaPlayer (Client)
└── Works polymorphically with any MediaFile
```

### Key Design Decisions

1. **Common functionality in base class** - `play()`, `pause()`, `stop()`
2. **Type-specific enhancements** - Audio/video classes add their own features
3. **Optional capabilities via interfaces** - `ISubtitleCapable` for video-only features
4. **Polymorphic client** - `MediaPlayer` works with any `MediaFile` subtype

## 🔍 LSP Compliance Demonstrated

```csharp
// This works identically regardless of concrete type
MediaFile file = new AudioFile(...);  // or VideoFile(...)
player.LoadFile(file);
player.PlayCurrent(); // ✅ Same method, same behavior, no type checking
```

### What Makes This LSP-Compliant

- ✅ **Substitutability**: Both `AudioFile` and `VideoFile` work wherever `MediaFile` is expected
- ✅ **No type checking**: `MediaPlayer` never needs `if (file is VideoFile)` logic
- ✅ **Consistent behavior**: Core methods behave predictably across all types
- ✅ **Graceful feature handling**: Optional features fail gracefully, don't throw exceptions

### What Would Break LSP

```csharp
// ❌ BAD: Forces all media files to handle subtitles
public abstract class MediaFile
{
    public abstract void ShowSubtitles(); // AudioFile would throw exception!
}

// ❌ BAD: Changes expected behavior
public class AudioFile : MediaFile
{
    public override void Play()
    {
        throw new Exception("Requires headphones!"); // Unexpected exception!
    }
}
```

## 🚀 Running the Demo

```bash
dotnet run
```

**Expected Output:**

- Audio file loads and plays with audio-specific features
- Video file loads and plays with video-specific features
- Same `MediaPlayer` methods work for both types
- Subtitle operations work only where supported

## 📚 Key Takeaways

1. **Design for substitutability** - Child classes should be drop-in replacements
2. **Separate optional features** - Use interfaces for capabilities not all types need
3. **Enhance, don't restrict** - Subclasses can do more, but not less
4. **Test polymorphically** - If your client needs type checking, reconsider your design

## 🔗 Related SOLID Principles

- **SRP**: Each class has single responsibility (media playback vs subtitle management)
- **OCP**: Easy to add new media types without modifying existing code
- **ISP**: `ISubtitleCapable` segregates optional functionality
- **DIP**: `MediaPlayer` depends on `MediaFile` abstraction, not concrete types
