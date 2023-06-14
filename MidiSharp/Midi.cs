using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiSharp
{
	public static class Midi
	{
		[DllImport("winmm.dll")]
		static extern uint midiOutOpen(out IntPtr lphMidiOut, uint uDeviceID, IntPtr dwCallback, IntPtr dwInstance, uint dwFlags);

		[DllImport("winmm.dll")]
		static extern uint midiOutShortMsg(IntPtr hMidiOut, uint dwMsg);

		[DllImport("winmm.dll")]
		static extern uint midiOutClose(IntPtr hMidiOut);

		[DllImport("winmm.dll", SetLastError = true)]
		static extern uint midiOutGetNumDevs();

		[DllImport("winmm.dll", SetLastError = true)]
		static extern MMRESULT midiOutGetDevCaps(UIntPtr uDeviceID, ref MIDIOUTCAPS lpMidiOutCaps, uint cbMidiOutCaps);

		delegate void MidiCallBack(IntPtr handle, uint msg, uint instance, uint param1, uint param2);

		static IntPtr Handle;

		public static int Device;

		public static Tuple<uint, string>[] Devices = new Tuple<uint, string>[0];

		public static void Refresh()
		{
			var count = midiOutGetNumDevs();

			Devices = new Tuple<uint, string>[count + 1];

			var caps = new MIDIOUTCAPS();

			midiOutGetDevCaps((UIntPtr)0xffffffff, ref caps, (uint)Marshal.SizeOf(typeof(MIDIOUTCAPS)));

			Devices[0] = new Tuple<uint, string>(0xffffffff, caps.szPname);

			for (uint x = 0; x < count; x++)
			{
				midiOutGetDevCaps((UIntPtr)x, ref caps, (uint)Marshal.SizeOf(typeof(MIDIOUTCAPS)));

				Devices[x + 1] = new Tuple<uint, string>(x, caps.szPname);
			}
		}

		public static void Enable()
		{
			var result = midiOutOpen(out Handle, Devices[Device].Item1, IntPtr.Zero, IntPtr.Zero, 0);
		}

		public static void NoteOn(int channel, int note, int velocity)
		{
			var result = midiOutShortMsg(Handle, 0x90u | (uint)channel | ((uint)note << 8) | ((uint)velocity << 16));
		}

		public static void NoteOff(int channel, int note, int velocity)
		{
			var result = midiOutShortMsg(Handle, 0x80u | (uint)channel | ((uint)note << 8) | ((uint)velocity << 16));
		}

		public static void KeyPressure(int channel, int note, int velocity)
		{
			var result = midiOutShortMsg(Handle, 0xA0u | (uint)channel | ((uint)note << 8) | ((uint)velocity << 16));
		}

		public static void ProgramChange(int channel, int patch)
		{
			var result = midiOutShortMsg(Handle, 0xC0u | (uint)channel | ((uint)patch << 8));
		}

		public static void ChannelPressure(int channel, int pressure)
		{
			var result = midiOutShortMsg(Handle, 0xD0u | (uint)channel | ((uint)pressure << 8));
		}

		public static void ControlChange(int channel, int control, int value)
		{
			var result = midiOutShortMsg(Handle, 0xB0u | (uint)channel | ((uint)control << 8) | ((uint)value << 16));
		}

		public static void PitchBend(int channel, int value)
		{
			var result = midiOutShortMsg(Handle, 0xE0u | (uint)channel | (uint)(value << 8));
		}

		public static void Disable()
		{
			var result = midiOutClose(Handle);
		}

		public static int Note(int note, int octave)
		{
			return (octave * 12) + note;
		}

		public static class Notes
		{
			public const int C = 0;
			public const int CSharp = 1;
			public const int DFlat = 1;
			public const int D = 2;
			public const int DSharp = 3;
			public const int EFlat = 3;
			public const int E = 4;
			public const int F = 5;
			public const int FSharp = 6;
			public const int GFlat = 6;
			public const int G = 7;
			public const int GSharp = 8;
			public const int AFlat = 8;
			public const int A = 9;
			public const int ASharp = 10;
			public const int BFlat = 10;
			public const int B = 11;
		}

		public static readonly string[] NoteNames = new string[]
		{
			"C",
			"CSharp",
			"D",
			"DSharp",
			"E",
			"F",
			"FSharp",
			"G",
			"GSharp",
			"A",
			"ASharp",
			"B"
		};

		public static readonly string[] FlatNoteNames = new string[]
		{
			"C",
			"DFlat",
			"D",
			"EFlat",
			"E",
			"F",
			"GFlat",
			"G",
			"AFlat",
			"A",
			"BFlat",
			"B"
		};

		public static class Controls
		{
			public const int Bank = 0x00;
			public const int Modulation = 0x01;
			public const int Breath = 0x02;
			public const int Foot = 0x04;
			public const int Portamento = 0x05;
			public const int Volume = 0x07;
			public const int Balance = 0x07;
			public const int Pan = 0x0A;
			public const int Expression = 0x0B;
			public const int SustainEnable = 0x40;
			public const int PortamentoEnable = 0x041;
			public const int SostenutoEnable = 0x42;
			public const int SoftPedalEnable = 0x43;
			public const int LegatoPedalEnable = 0x044;
			public const int HoldEnable = 0x45;
			public const int PortamentoControl = 0x54;
			public const int Reverb = 0x5B;
			public const int Tremolo = 0x5C;
			public const int Chorus = 0x5D;
			public const int Detune = 0x5E;
			public const int Phaser = 0x5F;
		}

		public static class Patches
		{
			public const int GrandPiano = 0;
			public const int BrightPiano = 1;
			public const int ElectricPiano = 2;
			public const int HonkyTonkPiano = 3;
			public const int ElectricPiano2 = 4;
			public const int ElectricPiano3 = 5;
			public const int Harpsichord = 6;
			public const int Clavi = 7;
			public const int Celesta = 8;
			public const int Glockenspiel = 9;
			public const int MusicBox = 10;
			public const int Vibraphone = 11;
			public const int Marimba = 12;
			public const int Xylophone = 13;
			public const int TubularBells = 14;
			public const int Dulcimer = 15;
			public const int DrawbarOrgan = 16;
			public const int PercussiveOrgan = 17;
			public const int RockOrgan = 18;
			public const int ChurchOrgan = 19;
			public const int ReedOrgan = 20;
			public const int AcousticBass = 32;
			public const int FingerBass = 33;
			public const int PickBass = 34;
			public const int FretlessBass = 35;
			public const int SlapBass = 36;
			public const int SlapBass2 = 37;
			public const int SynthBass = 38;
			public const int SynthBass2 = 39;
			public const int Contrabass = 43;
			public const int TremoloStrings = 44;
			public const int PizzicatoStrings = 45;
			public const int Harp = 46;
			public const int Timpani = 47;
			public const int Strings = 48;
			public const int Strings2 = 49;
			public const int SynthStrings = 50;
			public const int SynthStrings2 = 51;
			public const int ChoirAahs = 52;
			public const int VoiceOohs = 53;
			public const int SynthVoice = 54;
			public const int OrchestraHit = 55;
			public const int Trumpet = 56;
			public const int Trombone = 57;
			public const int Tuba = 58;
			public const int MutedTrumpet = 59;
			public const int FrenchHorn = 60;
			public const int BrassSection = 61;
			public const int SynthBrass = 62;
			public const int SynthBrass2 = 63;
			public const int Oboe = 68;
			public const int Piccolo = 72;
			public const int Flute = 73;
			public const int Recorder = 74;
			public const int PanFlute = 75;
			public const int BlownBottle = 76;
			public const int Shakuhachi = 77;
			public const int Whistle = 78;
			public const int Ocarina = 79;
			public const int SquareLead = 80;
			public const int SawtoothLead = 81;
			public const int CalliopeLead = 82;
			public const int ChiffLead = 83;
			public const int CharangLead = 84;
			public const int VoiceLead = 85;
			public const int FifthsLead = 86;
			public const int BassLead = 87;
			public const int NewAgePad = 88;
			public const int WarmPad = 89;
			public const int PolysynthPad = 90;
			public const int ChoirPad = 91;
			public const int BowedPad = 92;
			public const int MetallicPad = 93;
			public const int HaloPad = 94;
			public const int SweepPad = 95;
		}

		public static class Drums
		{
			public const int BassDrum = 35;
			public const int BassDrum2 = 36;
			public const int SnareDrum = 38;
			public const int SnareDrum2 = 40;
			public const int HiHat = 42;
			public const int OpenHiHat = 46;
			public const int HandClap = 39;
			public const int SplashCymbal = 55;
			public const int CrashCymbal = 49;
			public const int CrashCymbal2 = 57;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct MIDIOUTCAPS
		{
			public ushort wMid;
			public ushort wPid;
			public uint vDriverVersion;     //MMVERSION
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;
			public ushort wTechnology;
			public ushort wVoices;
			public ushort wNotes;
			public ushort wChannelMask;
			public uint dwSupport;
		}

		// values for wTechnology field of MIDIOUTCAPS structure
		const ushort MOD_MIDIPORT = 1;     // output port
		const ushort MOD_SYNTH = 2;        // generic internal synth
		const ushort MOD_SQSYNTH = 3;      // square wave internal synth
		const ushort MOD_FMSYNTH = 4;      // FM internal synth
		const ushort MOD_MAPPER = 5;       // MIDI mapper
		const ushort MOD_WAVETABLE = 6;    // hardware wavetable synth
		const ushort MOD_SWSYNTH = 7;      // software synth

		// flags for dwSupport field of MIDIOUTCAPS structure
		const uint MIDICAPS_VOLUME = 1;      // supports volume control
		const uint MIDICAPS_LRVOLUME = 2;    // separate left-right volume control
		const uint MIDICAPS_CACHE = 4;
		const uint MIDICAPS_STREAM = 8;      // driver supports midiStreamOut directly

		enum MMRESULT : uint
		{
			MMSYSERR_NOERROR = 0,
			MMSYSERR_ERROR = 1,
			MMSYSERR_BADDEVICEID = 2,
			MMSYSERR_NOTENABLED = 3,
			MMSYSERR_ALLOCATED = 4,
			MMSYSERR_INVALHANDLE = 5,
			MMSYSERR_NODRIVER = 6,
			MMSYSERR_NOMEM = 7,
			MMSYSERR_NOTSUPPORTED = 8,
			MMSYSERR_BADERRNUM = 9,
			MMSYSERR_INVALFLAG = 10,
			MMSYSERR_INVALPARAM = 11,
			MMSYSERR_HANDLEBUSY = 12,
			MMSYSERR_INVALIDALIAS = 13,
			MMSYSERR_BADDB = 14,
			MMSYSERR_KEYNOTFOUND = 15,
			MMSYSERR_READERROR = 16,
			MMSYSERR_WRITEERROR = 17,
			MMSYSERR_DELETEERROR = 18,
			MMSYSERR_VALNOTFOUND = 19,
			MMSYSERR_NODRIVERCB = 20,
			WAVERR_BADFORMAT = 32,
			WAVERR_STILLPLAYING = 33,
			WAVERR_UNPREPARED = 34
		}
	}
}
