using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria.Utilities;

#nullable enable

namespace Terraria.Audio
{
	public record struct LegacySoundStyle : ISoundStyle
	{
		private static readonly UnifiedRandom Random = new();

		private int[] styles = Array.Empty<int>();

		public int SoundId { get; set; }
		public SoundType Type { get; set; }
		public float Volume { get; set; } = 1f;
		public float Pitch { get; set; }
		public float PitchVariance { get; set; }

		public int Style {
			set {
				Array.Resize(ref styles, 1);
				
				styles[0] = value;
			}
		}

		public Span<int> Styles {
			get => styles;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				if (value.Length == 0)
					throw new ArgumentException("Styles array must not be empty.", nameof(value));

				Array.Resize(ref styles, value.Length);

				value.CopyTo(styles);
			}
		}

		public LegacySoundStyle(int soundId, int style = 0, SoundType type = SoundType.Sound)
			: this(soundId, stackalloc int[] { style }, type) { }
		
		public LegacySoundStyle(int soundId, ReadOnlySpan<int> styles, SoundType type = SoundType.Sound) {
			SoundId = soundId;
			this.styles = styles.ToArray();
			Type = type;

			Pitch = 0f;
			PitchVariance = 0f;
			Volume = 1f;
		}

		public LegacySoundStyle WithVolume(float volume) {
			var result = this;
			
			result.Volume = volume;

			return result;
		}

		public LegacySoundStyle WithPitchVariance(float pitchVariance) {
			var result = this;

			result.PitchVariance = pitchVariance;

			return result;
		}

		/*
		public bool Includes(int soundId, int style) {
			if (SoundId == soundId && style >= Style)
				return style < Style + Variations;

			return false;
		}
		*/

		public SoundEffect GetRandomSound()
			=> SoundEngine.GetTrackableSoundByStyleId(SoundId, GetRandomStyle());

		public float GetRandomPitch()
			=> MathHelper.Clamp(Pitch + ((Random.NextFloat() - 0.5f) * PitchVariance), -1f, 1f);

		internal int GetRandomStyle() {
			if (Styles == null || Styles.Length == 0)
				return 0;

			if (Styles.Length == 1)
				return Styles[0];

			return Styles[Main.rand.Next(Styles.Length)];
		}

		public static bool operator ==(ISoundStyle soundStyleA, LegacySoundStyle soundStyleB) => Equals(soundStyleA, soundStyleB);
		
		public static bool operator !=(ISoundStyle soundStyleA, LegacySoundStyle soundStyleB) => !Equals(soundStyleA, soundStyleB);
	}
}