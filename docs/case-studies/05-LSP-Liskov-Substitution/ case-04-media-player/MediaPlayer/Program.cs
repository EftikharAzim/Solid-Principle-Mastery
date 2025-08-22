namespace SOLIDPrinciples.LSP
{
    // Base class with core media functionality
    public abstract class MediaFile
    {
        public string FileName { get; protected set; }
        public string FilePath { get; protected set; }
        public bool IsPlaying { get; protected set; }
        public bool IsPaused { get; protected set; }

        protected MediaFile(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
            IsPlaying = false;
            IsPaused = false;
        }

        // Core methods that ALL media files must implement
        public virtual void Play()
        {
            IsPlaying = true;
            IsPaused = false;
            Console.WriteLine($"▶️ Playing {FileName}");
        }

        public virtual void Pause()
        {
            if (IsPlaying)
            {
                IsPaused = true;
                IsPlaying = false;
                Console.WriteLine($"⏸️ Paused {FileName}");
            }
        }

        public virtual void Stop()
        {
            IsPlaying = false;
            IsPaused = false;
            Console.WriteLine($"⏹️ Stopped {FileName}");
        }

        public virtual void GetInfo()
        {
            Console.WriteLine($"📄 File: {FileName}, Path: {FilePath}");
        }
    }

    // Audio file implementation
    public class AudioFile : MediaFile
    {
        public int Bitrate { get; private set; }
        public string Artist { get; private set; }

        public AudioFile(string fileName, string filePath, int bitrate, string artist)
            : base(fileName, filePath)
        {
            Bitrate = bitrate;
            Artist = artist;
        }

        public override void Play()
        {
            base.Play();
            Console.WriteLine($"🎵 Audio quality: {Bitrate}kbps");
        }

        public override void GetInfo()
        {
            base.GetInfo();
            Console.WriteLine($"🎤 Artist: {Artist}, Bitrate: {Bitrate}kbps");
        }

        // Audio-specific functionality
        public void AdjustVolume(int level)
        {
            Console.WriteLine($"🔊 Volume adjusted to {level}%");
        }
    }

    // Interface for subtitle-capable media (Segregation principle)
    public interface ISubtitleCapable
    {
        void ShowSubtitles();
        void HideSubtitles();
        void ChangeSubtitleLanguage(string language);
    }

    // Video file implementation
    public class VideoFile : MediaFile, ISubtitleCapable
    {
        public string Resolution { get; private set; }
        public int FrameRate { get; private set; }
        public bool SubtitlesEnabled { get; private set; }
        public string CurrentSubtitleLanguage { get; private set; }

        public VideoFile(string fileName, string filePath, string resolution, int frameRate)
            : base(fileName, filePath)
        {
            Resolution = resolution;
            FrameRate = frameRate;
            SubtitlesEnabled = false;
            CurrentSubtitleLanguage = "English";
        }

        public override void Play()
        {
            base.Play();
            Console.WriteLine($"🎬 Video quality: {Resolution} at {FrameRate}fps");
            if (SubtitlesEnabled)
            {
                Console.WriteLine($"📝 Subtitles: ON ({CurrentSubtitleLanguage})");
            }
        }

        public override void GetInfo()
        {
            base.GetInfo();
            Console.WriteLine($"🎥 Resolution: {Resolution}, Frame Rate: {FrameRate}fps");
        }

        // Video-specific functionality
        public void AdjustBrightness(int level)
        {
            Console.WriteLine($"🌟 Brightness adjusted to {level}%");
        }

        // ISubtitleCapable implementation
        public void ShowSubtitles()
        {
            SubtitlesEnabled = true;
            Console.WriteLine($"📝 Subtitles enabled ({CurrentSubtitleLanguage})");
        }

        public void HideSubtitles()
        {
            SubtitlesEnabled = false;
            Console.WriteLine("📝 Subtitles disabled");
        }

        public void ChangeSubtitleLanguage(string language)
        {
            CurrentSubtitleLanguage = language;
            Console.WriteLine($"📝 Subtitle language changed to {language}");
        }
    }

    // Media Player that works with any MediaFile (LSP compliance)
    public class MediaPlayer
    {
        private MediaFile currentFile;

        public void LoadFile(MediaFile file)
        {
            currentFile = file;
            Console.WriteLine($"📁 Loaded: {file.FileName}");
            file.GetInfo();
        }

        public void PlayCurrent()
        {
            if (currentFile != null)
            {
                currentFile.Play();
            }
            else
            {
                Console.WriteLine("❌ No file loaded");
            }
        }

        public void PauseCurrent()
        {
            currentFile?.Pause();
        }

        public void StopCurrent()
        {
            currentFile?.Stop();
        }

        // Handle subtitle-specific operations safely
        public void EnableSubtitles()
        {
            if (currentFile is ISubtitleCapable subtitleFile)
            {
                subtitleFile.ShowSubtitles();
            }
            else
            {
                Console.WriteLine("❌ Current file doesn't support subtitles");
            }
        }

        public void SetSubtitleLanguage(string language)
        {
            if (currentFile is ISubtitleCapable subtitleFile)
            {
                subtitleFile.ChangeSubtitleLanguage(language);
            }
            else
            {
                Console.WriteLine("❌ Current file doesn't support subtitles");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🎮 SOLID LSP Demo - Media Player");
            Console.WriteLine("=====================================\n");

            var player = new MediaPlayer();

            // Create different media files
            var audioFile = new AudioFile("song.mp3", "/music/song.mp3", 320, "The Beatles");
            var videoFile = new VideoFile("movie.mp4", "/videos/movie.mp4", "1080p", 60);

            Console.WriteLine("🎵 AUDIO FILE DEMO:");
            Console.WriteLine("-------------------");

            // LSP in action: MediaPlayer works with AudioFile
            player.LoadFile(audioFile);
            player.PlayCurrent();
            audioFile.AdjustVolume(75);
            player.PauseCurrent();
            player.StopCurrent();

            // Try subtitles on audio (should fail gracefully)
            player.EnableSubtitles();

            Console.WriteLine("\n🎬 VIDEO FILE DEMO:");
            Console.WriteLine("-------------------");

            // LSP in action: Same MediaPlayer works with VideoFile
            player.LoadFile(videoFile);
            player.PlayCurrent();
            videoFile.AdjustBrightness(80);

            // Subtitle functionality works only for video
            player.EnableSubtitles();
            player.SetSubtitleLanguage("Spanish");
            player.StopCurrent();

            Console.WriteLine("\n✅ LSP DEMONSTRATION:");
            Console.WriteLine("---------------------");
            Console.WriteLine("Notice how MediaPlayer.PlayCurrent() works identically");
            Console.WriteLine("with both AudioFile and VideoFile without knowing their types!");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
