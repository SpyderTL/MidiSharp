using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MidiSharp.Test
{
	[TestClass]
	public class MidiTests
	{
		[TestMethod]
		public async Task Initialize()
		{
			Midi.Refresh();
			Midi.Enable();

			await PlayAll(0, 0, 127, 2000, 0, Chord.Major(Midi.Note(Midi.Notes.E, 4)).Take(4));

			await Task.Delay(1000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task TestSync()
		{
			Midi.Refresh();
			Midi.Enable();

			var drums = async (int track) =>
			{
				await Play(track, 9, 127, 500, Midi.Drums.BassDrum);
				await Play(track, 9, 127, 500, Midi.Drums.SnareDrum);
				await Play(track, 9, 127, 500, Midi.Drums.BassDrum);
				await Play(track, 9, 127, 500, Midi.Drums.SnareDrum);
			};

			for (var i = 0; i < 64; i++)
			{
				await Task.WhenAll(
					drums(0),
					drums(1),
					drums(2),
					drums(3),
					drums(4),
					drums(5),
					drums(6),
					drums(7));
			}

			await Task.Delay(1000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task TheTime()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.BassLead);

			for (var x = 0; x < 2; x++)
			{
				await Play(0, 0, 127, 50, 200, Chord.Major(Midi.Note(Midi.Notes.E, 4)).Take(4));
				await Play(0, 0, 127, 50, 200, Chord.Major(Midi.Note(Midi.Notes.E, 5)).Take(4));
				await Play(0, 0, 127, 50, 200, Chord.Minor(Midi.Note(Midi.Notes.CSharp, 4)).Take(4));
				await Play(0, 0, 127, 50, 200, Chord.Minor(Midi.Note(Midi.Notes.CSharp, 5)).Take(4));
				await Play(0, 0, 127, 50, 200, Chord.Major(Midi.Note(Midi.Notes.D, 4)).Take(4));
				await Play(0, 0, 127, 50, 200, Chord.Major(Midi.Note(Midi.Notes.D, 5)).Take(4));
				await Play(0, 0, 127, 50, 200, Chord.Major(Midi.Note(Midi.Notes.D, 4)).Take(4));
				await Play(0, 0, 127, 50, 200, Chord.Major(Midi.Note(Midi.Notes.D, 5)).Take(4));
			}

			await Task.Delay(1000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task Chords()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.BassLead);

			for (var x = 0; x < 2; x++)
			{
				await PlayAll(0, 0, 127, 2000, 0, Chord.Major(Midi.Note(Midi.Notes.E, 4)).Take(4));
				await PlayAll(0, 0, 127, 2000, 0, Chord.Minor(Midi.Note(Midi.Notes.CSharp, 4)).Take(4));
				await PlayAll(0, 0, 127, 4000, 0, Chord.Major(Midi.Note(Midi.Notes.D, 4)).Take(4));
			}

			await Task.Delay(1000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task Chords2()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.GrandPiano);

			var notes = new int[]
			{
				Midi.Note(Midi.Notes.A, 3),
				Midi.Note(Midi.Notes.B, 3),
				Midi.Note(Midi.Notes.C, 4),
				Midi.Note(Midi.Notes.D, 4),
				Midi.Note(Midi.Notes.E, 4),
				Midi.Note(Midi.Notes.F, 4),
				Midi.Note(Midi.Notes.G, 4),
				Midi.Note(Midi.Notes.A, 4)
			};

			for (var x = 0; x < 8; x++)
			{
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);
				await PlayAll(0, 0, 127, 2000, 50, Chord.Major(notes[x] - 12).Take(4));
			}

			await Task.Delay(10000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task RandomChords()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.GrandPiano);

			var seed = Random.Shared.Next();
			var random = new Random(seed);

			Console.WriteLine($"Seed: {seed}");

			var note = random.Next(12);
			var chord = random.Next(2);
			var spread = random.Next(1, 11) * 25;
			var lastNote = note;
			var lastChord = chord;

			for (var x = 0; x < 8; x++)
			{
				while (note == lastNote &&
					chord == lastChord)
				{
					if (random.Next(2) == 0)
						note = random.Next(12);
					else
						chord = random.Next(2);
				}

				lastNote = note;
				lastChord = chord;

				System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]} spread: {spread}");

				Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);
				await PlayAll(0, 0, 127, 2000, spread, Chord.Select(chord, Midi.Note(note, 4)).Take(4));
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);

				while (note == lastNote &&
					chord == lastChord)
				{
					if (random.Next(2) == 0)
						note = random.Next(12);
					else
						chord = random.Next(2);
				}

				lastNote = note;
				lastChord = chord;

				System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]} spread: {spread}");

				Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);
				await PlayAll(0, 0, 127, 2000, spread, Chord.Select(chord, Midi.Note(note, 4)).Take(4));
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);

				while (note == lastNote &&
					chord == lastChord)
				{
					if (random.Next(2) == 0)
						note = random.Next(12);
					else
						chord = random.Next(2);
				}

				lastNote = note;
				lastChord = chord;

				System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]} spread: {spread}");

				Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);
				await PlayAll(0, 0, 127, 2000, spread, Chord.Select(chord, Midi.Note(note, 4)).Take(4));
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);

				while (note == lastNote &&
					chord == lastChord)
				{
					if (random.Next(2) == 0)
						note = random.Next(12);
					else
						chord = random.Next(2);
				}

				lastNote = note;
				lastChord = chord;

				System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]} spread: {spread}");

				Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);
				await PlayAll(0, 0, 127, 2000, spread, Chord.Select(chord, Midi.Note(note, 4)).Take(4));
			}

			await Task.Delay(10000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task RandomSequence()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.GrandPiano);

			var seed = Random.Shared.Next();
			var random = new Random(seed);

			System.Diagnostics.Debug.WriteLine($"Seed: {seed}");

			for (var z = 0; z < 64; z++)
			{
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
				Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

				var note = random.Next(12);
				var chord = random.Next(2);
				//var chord = random.Next(7);
				var sequence = Enumerable.Range(0, 8).Select(x => random.Next(16)).ToArray();
				var velocity = Enumerable.Range(0, 8).Select(x => random.Next(64, 96)).ToArray();

				System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

				for (var y = 0; y < 2; y++)
				{
					for (var x = 0; x < 8; x++)
					{
						await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
					}
				}
			}

			await Task.Delay(10000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task RandomSequence2()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.GrandPiano);
			//Midi.ProgramChange(0, Midi.Patches.Celesta);

			for (var a = 0; a < 8; a++)
			{
				var seed = Random.Shared.Next();
				//seed = 1514958894;
				//seed = 481331100;
				//seed = 1433140728;
				//seed = 1268093611;
				//seed = 1493092241;
				//seed = 692981443;
				//seed = 1812864831;
				//seed = 1542529204;
				//seed = 77486108;
				//seed = 1148009039;
				//seed = 1388716506;
				//seed = 567776082;
				//seed = 1860969591;
				//seed = 595956379;
				var random = new Random(seed);

				System.Diagnostics.Debug.WriteLine($"Seed: {seed}");

				var notes = Enumerable.Range(0, 6).Select(x => random.Next(12)).ToArray();
				var chords = Enumerable.Range(0, 6).Select(x => random.Next(2)).ToArray();
				var sequences = Enumerable.Range(0, 6).Select(x => Enumerable.Range(0, 8).Select(x => random.Next(16)).ToArray()).ToArray();
				var velocities = Enumerable.Range(0, 6).Select(x => Enumerable.Range(0, 8).Select(x => random.Next(64, 96)).ToArray()).ToArray();

				for (var z = 0; z < 2; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 0; z < 2; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 2; z < 4; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 2; z < 4; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 4; z < 5; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 3; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 5; z < 6; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 1; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 4; z < 5; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 3; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 5; z < 6; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 1; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}


				for (var z = 0; z < 2; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 0; z < 2; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 0; z < 1; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 0; z < 1; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 2; y++)
					{
						for (var x = 0; x < 8; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				for (var z = 0; z < 1; z++)
				{
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 0);
					Midi.ControlChange(0, Midi.Controls.SustainEnable, 127);

					var note = notes[z];
					var chord = chords[z];
					var sequence = sequences[z];
					var velocity = velocities[z];

					System.Diagnostics.Debug.WriteLine($"note: {Midi.NoteNames[note]} chord: {Chord.Names[chord]}");

					for (var y = 0; y < 1; y++)
					{
						for (var x = 0; x < 1; x++)
						{
							await Play(0, 0, velocity[x], 200, Chord.Select(chord, Midi.Note(note, 2)).Skip(sequence[x]).First());
						}
					}
				}

				await Task.Delay(10000);
			}

			Midi.Disable();
		}

		[TestMethod]
		public async Task Drums()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.BassLead);

			for (var x = 0; x < 2; x++)
			{
				await PlayAll(0, 0, 127, 2000, 0, Chord.Major(Midi.Note(Midi.Notes.E, 4)).Take(4));
				await PlayAll(0, 0, 127, 2000, 0, Chord.Minor(Midi.Note(Midi.Notes.CSharp, 4)).Take(4));
				await PlayAll(0, 0, 127, 4000, 0, Chord.Major(Midi.Note(Midi.Notes.D, 4)).Take(4));
			}

			for (var x = 0; x < 2; x++)
			{
				await Task.WhenAll(
					Task.Run(async () =>
					{
						await PlayAll(0, 0, 127, 2000, 0, Chord.Major(Midi.Note(Midi.Notes.E, 4)).Take(4));
						await PlayAll(0, 0, 127, 2000, 0, Chord.Minor(Midi.Note(Midi.Notes.CSharp, 4)).Take(4));
						await PlayAll(0, 0, 127, 4000, 0, Chord.Major(Midi.Note(Midi.Notes.D, 4)).Take(4));
					}),
					Task.Run(async () =>
					{
						for (var x = 0; x < 4; x++)
						{
							await Play(9, 9, 127, 500, Midi.Drums.BassDrum);
							await Play(9, 9, 127, 500, Midi.Drums.SnareDrum);
							await Play(9, 9, 127, 500, Midi.Drums.BassDrum);
							await Play(9, 9, 127, 500, Midi.Drums.SnareDrum);
						}
					}));
			}

			await Task.Delay(1000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task BillieJean()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.FingerBass);
			Midi.ProgramChange(1, Midi.Patches.SynthStrings);

			await Task.Delay(5000);

			var drums = async () =>
			{
				await Play(9, 9, 127, 125, 125,
					Midi.Drums.HiHat,
					Midi.Drums.HiHat,
					Midi.Drums.SnareDrum,
					Midi.Drums.HiHat,
					Midi.Drums.HiHat,
					Midi.Drums.HiHat,
					Midi.Drums.SnareDrum,
					Midi.Drums.HiHat,
					Midi.Drums.HiHat,
					Midi.Drums.HiHat,
					Midi.Drums.SnareDrum,
					Midi.Drums.HiHat,
					Midi.Drums.HiHat,
					Midi.Drums.HiHat,
					Midi.Drums.SnareDrum,
					Midi.Drums.HiHat);
			};

			var bassDrum = async () =>
			{
				await Play(10, 9, 127, 100, Midi.Drums.BassDrum);
				await Wait(10, 900);
				await Play(10, 9, 127, 100, Midi.Drums.BassDrum);
				await Wait(10, 900);
				await Play(10, 9, 127, 100, Midi.Drums.BassDrum);
				await Wait(10, 900);
				await Play(10, 9, 127, 100, Midi.Drums.BassDrum);
				await Wait(10, 900);
			};

			var bass = async () =>
			{
				await Play(0, 0, 127, 125, 125,
					Midi.Note(Midi.Notes.FSharp, 2),
					Midi.Note(Midi.Notes.CSharp, 3),
					Midi.Note(Midi.Notes.E, 3),
					Midi.Note(Midi.Notes.FSharp, 2),
					Midi.Note(Midi.Notes.E, 3),
					Midi.Note(Midi.Notes.CSharp, 3),
					Midi.Note(Midi.Notes.B, 2),
					Midi.Note(Midi.Notes.CSharp, 3),
					Midi.Note(Midi.Notes.FSharp, 2),
					Midi.Note(Midi.Notes.CSharp, 3),
					Midi.Note(Midi.Notes.E, 3),
					Midi.Note(Midi.Notes.FSharp, 2),
					Midi.Note(Midi.Notes.E, 3),
					Midi.Note(Midi.Notes.CSharp, 3),
					Midi.Note(Midi.Notes.B, 2),
					Midi.Note(Midi.Notes.CSharp, 3));
			};

			var bass2 = async () =>
			{
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.B, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.FSharp, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.A, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.B, 3));
				await Wait(0, 125);

				await Wait(0, 125);
				await Wait(0, 125);

				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.FSharp, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.E, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.FSharp, 3));
				await Wait(0, 125);

				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.B, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.FSharp, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.A, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.B, 3));
				await Wait(0, 125);

				await Wait(0, 125);
				await Wait(0, 125);

				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.FSharp, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.E, 3));
				await Wait(0, 125);
				await Play(0, 0, 127, 125, Midi.Note(Midi.Notes.FSharp, 3));
				await Wait(0, 125);
			};

			var bass3 = async () =>
			{
				await Play(0, 0, 127, 1750, Midi.Note(Midi.Notes.D, 3));
				await Play(0, 0, 127, 250, Midi.Note(Midi.Notes.E, 3));
				await Play(0, 0, 127, 1000, Midi.Note(Midi.Notes.FSharp, 3));
				await Play(0, 0, 127, 1000, Midi.Note(Midi.Notes.FSharp, 2));
			};

			var synth = async () =>
			{
				await PlayAll(1, 1, 127, 100, 0, Chord.Minor(Midi.Note(Midi.Notes.FSharp, 4)).Take(4));
				await Wait(1, 650);
				await PlayAll(1, 1, 127, 100, 0, Chord.Minor(Midi.Note(Midi.Notes.GSharp, 4)).Take(4));
				await Wait(1, 1150);
				await PlayAll(1, 1, 127, 100, 0, Chord.Major(Midi.Note(Midi.Notes.A, 4)).Take(4));
				await Wait(1, 650);
				await PlayAll(1, 1, 127, 100, 0, Chord.Minor(Midi.Note(Midi.Notes.GSharp, 4)).Take(4));
				await Wait(1, 1150);
			};

			var synth2 = async () =>
			{
				await PlayAll(1, 1, 127, 100, 0, Chord.Minor(Midi.Note(Midi.Notes.FSharp, 4)).Take(4));
				await Wait(1, 650);
				await PlayAll(1, 1, 127, 100, 0, Chord.Minor(Midi.Note(Midi.Notes.FSharp, 4)).Take(4));
				await Wait(1, 1150);

				await PlayAll(1, 1, 127, 100, 0, Chord.Minor(Midi.Note(Midi.Notes.FSharp, 4)).Take(4));
				await Wait(1, 650);
				await PlayAll(1, 1, 127, 100, 0, Chord.Minor(Midi.Note(Midi.Notes.FSharp, 4)).Take(4));
				await Wait(1, 1150);
			};

			var drumIntro = () => Task.WhenAll(bassDrum(), drums());

			var bassIntro = () => Task.WhenAll(bassDrum(), drums(), bass());

			var phrase3 = () => Task.WhenAll(
					bassDrum(),
					drums(),
					bass(),
					synth());

			var phrase4 = () => Task.WhenAll(
					bassDrum(),
					drums(),
					bass2(),
					synth2());

			var bassBreak = () => Task.WhenAll(
				bassDrum(),
				drums(),
				bass3());

			// Drum Intro
			await drumIntro();

			// Bass Intro
			for (var i = 0; i < 4; i++)
				await bassIntro();

			// Synth Intro
			await phrase3();
			await phrase3();

			// Vocal Intro
			/*
				She was more like a beauty queen
				From a movie scene
				I said, "Don't mind, but what do you mean
				I am the one
			*/
			await phrase3();
			await phrase3();

			/*
				Who will dance on the floor in the round?"
			*/
			await phrase4();

			/*
				She said I am the one
			*/
			await phrase3();

			/*
				Who will dance on the floor in the round
			*/
			await phrase4();

			await phrase3();

			/*
				She told me her name was Billie Jean
				As she caused a scene
				Then every head turned with eyes that dreamed of being the one
			*/
			await phrase3();
			await phrase3();

			/*
				Who will dance on the floor in the round
			*/
			await phrase4();

			await phrase3();

			/*
				People always told me, "Be careful of what you do.
				And don't go around breaking young girls' hearts."
				And mother always told me, "A-be careful of who you love,
			*/
			await bassBreak();
			await bassBreak();
			await bassBreak();

			await Task.Delay(1000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task FinalFantasy()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.PizzicatoStrings);
			Midi.ProgramChange(1, Midi.Patches.Strings2);
			Midi.ProgramChange(2, Midi.Patches.Harp);
			Midi.ProgramChange(3, Midi.Patches.Oboe);
			Midi.ProgramChange(4, Midi.Patches.FingerBass);

			var seed = Random.Shared.Next();

			//seed = 2;
			//seed = 5;
			//seed = 8;
			//seed = 9;
			//seed = 10;
			//seed = 11;
			//seed = 24;
			//seed = 27;

			//seed = 30;

			var random = new Random(seed);

			var note = Enumerable.Range(0, 4).Select(x => random.Next(0, 12)).ToArray();
			var octave = Enumerable.Range(0, 4).Select(x => random.Next(2, 6)).ToArray();
			var chord = Enumerable.Range(0, 4).Select(x => random.Next(0, 4)).ToArray();

			System.Diagnostics.Debug.WriteLine(seed);
			System.Diagnostics.Debug.WriteLine(string.Join(Environment.NewLine, Enumerable.Range(0, 4).Select(x => $"{Midi.NoteNames[note[x]]} {Chord.Names[chord[x]]}")));

			for (var i = 0; i < 4; i++)
			{
				for (var x = 0; x < note.Length - 1; x++)
				{
					switch (chord[x])
					{
						case 0:
							var majorNotes = Enumerable.Range(0, 4).Select(_ =>
								Chord.Major(Midi.Note(note[x], 4)).Skip(random.Next(4)).First());

							var majorNotes2 = Enumerable.Range(0, 4).Select(_ =>
								Midi.Note(note[x], 2) + (random.Next(0, 3) * 12));

							await Task.WhenAll(
								PlayAll(1, 1, 64, 4000, 0, Chord.Major(Midi.Note(note[x], 3)).Take(8).ToArray()),
								Play(3, 3, 127, 1000, 0, majorNotes),
								Play(4, 4, 127, 1000, 0, majorNotes2),
								Play(0, 0, 127, 125, 125, Chord.Major(Midi.Note(note[x], octave[x])).Take(9).ToArray()));
							break;

						case 1:
							var minorNotes = Enumerable.Range(0, 4).Select(_ =>
								Chord.Minor(Midi.Note(note[x], 4)).Skip(random.Next(4)).First());

							var minorNotes2 = Enumerable.Range(0, 4).Select(_ =>
								Midi.Note(note[x], 2) + (random.Next(0, 3) * 12));

							await Task.WhenAll(
								PlayAll(1, 1, 64, 4000, 0, Chord.Minor(Midi.Note(note[x], 3)).Take(8).ToArray()),
								Play(3, 3, 127, 1000, 0, minorNotes),
								Play(4, 4, 127, 1000, 0, minorNotes2),
								Play(0, 0, 127, 125, 125, Chord.Minor(Midi.Note(note[x], octave[x])).Take(9).ToArray()));
							break;

						case 2:
							var diminishedNotes = Enumerable.Range(0, 4).Select(_ =>
								Chord.Diminished(Midi.Note(note[x], 4)).Skip(random.Next(4)).First());

							var diminishedNotes2 = Enumerable.Range(0, 4).Select(_ =>
								Midi.Note(note[x], 2) + (random.Next(0, 3) * 12));

							await Task.WhenAll(
								PlayAll(1, 1, 64, 4000, 0, Chord.Minor(Midi.Note(note[x], 3)).Take(8).ToArray()),
								Play(3, 3, 127, 1000, 0, diminishedNotes),
								Play(4, 4, 127, 1000, 0, diminishedNotes2),
								Play(0, 0, 127, 125, 125, Chord.Minor(Midi.Note(note[x], octave[x])).Take(9).ToArray()));
							break;

						case 3:
							var suspendedNotes = Enumerable.Range(0, 4).Select(_ =>
								Chord.Suspended(Midi.Note(note[x], 4)).Skip(random.Next(4)).First());

							var suspendedNotes2 = Enumerable.Range(0, 4).Select(_ =>
								Midi.Note(note[x], 2) + (random.Next(0, 3) * 12));

							await Task.WhenAll(
								PlayAll(1, 1, 64, 4000, 0, Chord.Minor(Midi.Note(note[x], 3)).Take(8).ToArray()),
								Play(3, 3, 127, 1000, 0, suspendedNotes),
								Play(4, 4, 127, 1000, 0, suspendedNotes2),
								Play(0, 0, 127, 125, 125, Chord.Minor(Midi.Note(note[x], octave[x])).Take(9).ToArray()));
							break;
					}
				}

				var y = note.Length - 1;

				switch (chord[y])
				{
					case 0:
						var majorNotes = Chord.Major(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(0).Take(4)
							.Concat(Chord.Major(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(1).Take(4))
							.Concat(Chord.Major(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(2).Take(4))
							.Concat(Chord.Major(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(3).Take(4))
							.Concat(Chord.Major(Midi.Note(note[y], 4)).Skip(0).Take(4))
							.Concat(Chord.Major(Midi.Note(note[y], 4)).Skip(1).Take(4))
							.Concat(Chord.Major(Midi.Note(note[y], 4)).Skip(2).Take(4))
							.Concat(Chord.Major(Midi.Note(note[y], 4)).Skip(3).Take(4));

						await Task.WhenAll(
							PlayAll(1, 1, 64, 2000, 0, Chord.Major(Midi.Note(note[y], 3)).Take(8)),
							Play(3, 3, 127, 2000, 0, Chord.Major(Midi.Note(note[y], 3)).Take(8).Skip(random.Next(8)).First()),
							Play(4, 4, 127, 2000, Midi.Note(note[y], 2)),
							Play(2, 2, 127, 125, 0, majorNotes));
						break;

					case 1:
						var minorNotes = Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(0).Take(4)
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(1).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(2).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(3).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(0).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(1).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(2).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(3).Take(4));

						await Task.WhenAll(
							PlayAll(1, 1, 64, 2000, 0, Chord.Minor(Midi.Note(note[y], 3)).Take(8)),
							Play(3, 3, 127, 2000, Chord.Minor(Midi.Note(note[y], 3)).Take(8).Skip(random.Next(8)).First()),
							Play(4, 4, 127, 2000, Midi.Note(note[y], 2)),
							Play(2, 2, 127, 125, 0, minorNotes));
						break;

					case 2:
						var dimishedNotes = Chord.Diminished(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(0).Take(4)
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(1).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(2).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(3).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(0).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(1).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(2).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(3).Take(4));

						await Task.WhenAll(
							PlayAll(1, 1, 64, 2000, 0, Chord.Minor(Midi.Note(note[y], 3)).Take(8)),
							Play(3, 3, 127, 2000, Chord.Minor(Midi.Note(note[y], 3)).Take(8).Skip(random.Next(8)).First()),
							Play(4, 4, 127, 2000, Midi.Note(note[y], 2)),
							Play(2, 2, 127, 125, 0, dimishedNotes));
						break;

					case 3:
						var suspendedNotes = Chord.Diminished(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(0).Take(4)
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(1).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(2).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 2)).Take(16).Reverse().Skip(3).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(0).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(1).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(2).Take(4))
							.Concat(Chord.Minor(Midi.Note(note[y], 4)).Skip(3).Take(4));

						await Task.WhenAll(
							PlayAll(1, 1, 64, 2000, 0, Chord.Minor(Midi.Note(note[y], 3)).Take(8)),
							Play(3, 3, 127, 2000, Chord.Minor(Midi.Note(note[y], 3)).Take(8).Skip(random.Next(8)).First()),
							Play(4, 4, 127, 2000, Midi.Note(note[y], 2)),
							Play(2, 2, 127, 125, 0, suspendedNotes));
						break;
				}
			}

			await Task.Delay(5000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task FinalFantasy2()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.FingerBass);
			Midi.ProgramChange(1, Midi.Patches.Strings);
			Midi.ProgramChange(2, Midi.Patches.BrassSection);
			Midi.ProgramChange(15, Midi.Patches.Timpani);

			var root = Random.Shared.Next(12);

			await Play(0, 0, 127, 200, 0,
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root - 2, 3),
				Midi.Note(root - 2, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root, 3),
				Midi.Note(root - 2, 3),
				Midi.Note(root - 2, 3));

			var roots = Enumerable.Range(0, 8).Select(x => Random.Shared.Next(0, 12)).ToArray();
			var octaves = Enumerable.Range(0, 8).Select(x => Random.Shared.Next(3, 6)).ToArray();

			var bass = async (int rootNote) =>
			{
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3) + 12);
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3) + 12);
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3) + 12);
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3) + 12);
			};

			var bass2 = async (int rootNote) =>
			{
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Wait(0, 400);
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Play(0, 0, 127, 200, Midi.Note(rootNote, 3));
				await Wait(0, 400);
			};

			var bass3 = async (int rootNote) =>
			{
				await Play(0, 0, 127, 750, Midi.Note(rootNote, 2));
				await Play(0, 0, 127, 1250, Midi.Note(rootNote, 2));
			};

			var strings = async (int rootNote) =>
			{
				await Play(1, 1, 127, 200, Midi.Note(rootNote, 4));
				await Play(1, 1, 127, 200, Midi.Note(rootNote, 4));
				await Wait(1, 200);
				await Play(1, 1, 127, 200, Midi.Note(rootNote, 4));

				await Play(1, 1, 127, 200, Midi.Note(rootNote, 4) + 2);
				await Play(1, 1, 127, 200, Midi.Note(rootNote, 4) + 2);
				await Wait(1, 200);
				await Play(1, 1, 127, 200, Midi.Note(rootNote, 4) + 2);
			};

			var horns = async (int rootNote) =>
			{
				await Play(2, 2, 127, 100, Midi.Note(rootNote, 4) - 4);
				await Play(2, 2, 127, 100, Midi.Note(rootNote, 4) - 3);
				await Play(2, 2, 127, 100, Midi.Note(rootNote, 4) - 2);
				await Play(2, 2, 127, 100, Midi.Note(rootNote, 4) - 1);
				await Play(2, 2, 127, 800, Midi.Note(rootNote, 4) - 0);
			};

			var horns2 = async (int rootNote) =>
			{
				await Wait(2, 400);
				await Play(2, 2, 127, 200, Midi.Note(rootNote, 4));
				await Wait(2, 400);
				await Play(2, 2, 127, 200, Midi.Note(rootNote, 4));
			};

			var drums = async (int rootNote) =>
			{
				await Play(15, 15, 127, 750, Midi.Note(rootNote, 3));
				await Play(15, 15, 127, 1250, Midi.Note(rootNote, 3));
			};

			for (var j = 0; j < 2; j++)
			{
				for (var i = 0; i < 2; i++)
				{
					await Task.WhenAll(bass(roots[0]), strings(roots[0]), horns(roots[0]));
					await Task.WhenAll(bass(roots[1]), strings(roots[1]), horns(roots[1]));
					await Task.WhenAll(bass(roots[2]), strings(roots[2]), horns(roots[2]));
					await Task.WhenAll(bass(roots[2]), strings(roots[2]), horns(roots[2]));
				}

				await Task.WhenAll(bass(roots[3]), strings(roots[3]));
				await Task.WhenAll(bass(roots[4]), strings(roots[4]));
				await Task.WhenAll(bass(roots[5]), strings(roots[5]));
				await Task.WhenAll(bass(roots[5]), strings(roots[5]));

				await Task.WhenAll(bass2(roots[6]), strings(roots[6]), horns2(roots[6]));
				await Task.WhenAll(bass2(roots[6]), strings(roots[6]), horns2(roots[6]));
				await Task.WhenAll(bass2(roots[7]), strings(roots[7]), horns2(roots[7]));
				await Task.WhenAll(bass2(roots[5]), strings(roots[5]), horns2(roots[5]));

				await Task.WhenAll(bass3(roots[0]), drums(roots[0]));
				await Task.WhenAll(bass3(roots[0]), drums(roots[0]));

				//await Wait(2000);
			}

			await Task.Delay(5000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task Stars()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.CalliopeLead);
			Midi.ProgramChange(1, Midi.Patches.ChoirAahs);

			var root = Enumerable.Range(0, 4).Select(_ => Random.Shared.Next(12)).ToArray();
			var chords = Enumerable.Range(0, 4).Select(_ => Random.Shared.Next(4)).ToArray();
			var chordNotes = Enumerable.Range(0, 4).Select(x => Chord.Select(chords[x], root[x]).Take(8).ToArray()).ToArray();
			var notes = Enumerable.Range(0, 4).Select(x => Enumerable.Range(0, 8).Select(_ => chordNotes[x][Random.Shared.Next(8)]).ToArray()).ToArray();

			for (int loop2 = 0; loop2 < 2; loop2++)
			{
				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 0; i < 2; i++)
					{
						for (int j = 0; j < 2; j++)
						{
							await Play(0, 0, 127, 175, 0, notes[i].Select(x => Midi.Note(x, 3)));
						}
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 0; i < 2; i++)
					{
						for (int j = 0; j < 2; j++)
						{
							await Task.WhenAll(
								Play(0, 0, 127, 175, 0, notes[i].Select(x => Midi.Note(x, 3))),
								PlayAll(1, 1, 64, 8 * 175, 0, chordNotes[i].Select(x => Midi.Note(x, 3))));
						}
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 2; i < 4; i++)
					{
						for (int j = 0; j < 2; j++)
						{
							await Play(0, 0, 127, 175, 0, notes[i].Select(x => Midi.Note(x, 3)));
						}
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 2; i < 4; i++)
					{
						for (int j = 0; j < 2; j++)
						{
							await Task.WhenAll(
								Play(0, 0, 127, 175, 0, notes[i].Select(x => Midi.Note(x, 3))),
								PlayAll(1, 1, 64, 8 * 175, 0, chordNotes[i].Select(x => Midi.Note(x, 3))));
						}
					}
				}
			}

			await Task.Delay(5000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task Stars2()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.CalliopeLead);
			Midi.ProgramChange(1, Midi.Patches.ChoirAahs);

			var seed = Random.Shared.Next();

			//seed = 0;
			//seed = 1;
			//seed = 2;
			//seed = 3;
			//seed = 4;
			//seed = 5;
			//seed = 11;

			//seed = 11;

			var random = new Random(seed);

			var root = Enumerable.Range(0, 4).Select(_ => random.Next(12)).ToArray();
			var chords = Enumerable.Range(0, 4).Select(_ => random.Next(2)).ToArray();
			var chordNotes = Enumerable.Range(0, 4).Select(x => Chord.Select(chords[x], root[x]).Take(8).ToArray()).ToArray();
			var scaleNotes = Enumerable.Range(0, 4).Select(x => Scale.Select(chords[x], root[x]).Take(16).ToArray()).ToArray();
			var notes = Enumerable.Range(0, 4).Select(x => Enumerable.Range(0, 8).Select(_ => scaleNotes[x][random.Next(8)]).ToArray()).ToArray();

			var beat = 175;

			System.Diagnostics.Debug.WriteLine(seed);
			System.Diagnostics.Debug.WriteLine(string.Join(Environment.NewLine, Enumerable.Range(0, 4).Select(x => $"{Midi.NoteNames[root[x]]} {Chord.Names[chords[x]]}")));

			var drums = async () =>
			{
				await Play(9, 9, 127, 4 * beat, Midi.Drums.BassDrum);
				await Play(9, 9, 127, 4 * beat, Midi.Drums.SplashCymbal);
				await Play(9, 9, 127, 4 * beat, Midi.Drums.BassDrum);
				await Play(9, 9, 127, 4 * beat, Midi.Drums.SplashCymbal);
			};

			for (int loop2 = 0; loop2 < 2; loop2++)
			{
				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 0; i < 2; i++)
					{
						await Play(0, 0, 127, beat, 0, notes[i].Select(x => Midi.Note(x, 4)));
						await Play(0, 0, 127, beat, 0, notes[i].Select(x => Midi.Note(x, 4)));
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 0; i < 2; i++)
					{
						await Task.WhenAll(
							Play(0, 0, 127, beat, 0, notes[i].Concat(notes[i]).Select(x => Midi.Note(x, 4))),
							PlayAll(1, 1, 64, 16 * beat, 0, chordNotes[i].Select(x => Midi.Note(x, 3))));
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 0; i < 2; i++)
					{
						await Task.WhenAll(
							Play(0, 0, 127, beat, 0, notes[i].Concat(notes[i]).Select(x => Midi.Note(x, 4))),
							PlayAll(1, 1, 64, 16 * beat, 0, chordNotes[i].Select(x => Midi.Note(x, 3))),
							drums());
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 2; i < 4; i++)
					{
						await Play(0, 0, 127, beat, 0, notes[i].Select(x => Midi.Note(x, 4)));
						await Play(0, 0, 127, beat, 0, notes[i].Select(x => Midi.Note(x, 4)));
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 2; i < 4; i++)
					{
						await Task.WhenAll(
							Play(0, 0, 127, beat, 0, notes[i].Concat(notes[i]).Select(x => Midi.Note(x, 4))),
							PlayAll(1, 1, 64, 16 * beat, 0, chordNotes[i].Select(x => Midi.Note(x, 3))));
					}
				}

				for (int loop = 0; loop < 2; loop++)
				{
					for (int i = 2; i < 4; i++)
					{
						await Task.WhenAll(
							Play(0, 0, 127, beat, 0, notes[i].Concat(notes[i]).Select(x => Midi.Note(x, 4))),
							PlayAll(1, 1, 64, 16 * beat, 0, chordNotes[i].Select(x => Midi.Note(x, 3))),
							drums());
					}
				}
			}

			await Task.Delay(5000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task Movie()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.FrenchHorn);
			Midi.ProgramChange(1, Midi.Patches.ChoirAahs);
			Midi.ProgramChange(2, Midi.Patches.SweepPad);
			Midi.ProgramChange(3, Midi.Patches.Trumpet);
			Midi.ProgramChange(4, Midi.Patches.Strings2);

			var seed = Random.Shared.Next();

			seed = 0;

			System.Diagnostics.Debug.WriteLine(seed);

			var random = new Random(seed);

			var beat = 1000;

			for (int loop = 0; loop < 4; loop++)
			{
				var baseNote = random.Next(12);

				var note = random.Next(12);
				var chord = random.Next(5);

				while (note == baseNote)
					note = random.Next(12);

				System.Diagnostics.Debug.WriteLine(string.Join(Environment.NewLine, $"{Midi.NoteNames[note]} {Chord.Names[chord]}"));
				System.Diagnostics.Debug.WriteLine(string.Join(Environment.NewLine, $"{Midi.NoteNames[baseNote]}"));

				for (int i = 0; i < 2; i++)
				{
					await Task.WhenAll(
						PlayAll(0, 0, 96, beat * 4, 0, Chord.Select(chord, Midi.Note(note, 4)).Take(4)),
						PlayAll(1, 1, 96, beat * 4, 0, Chord.Select(chord, Midi.Note(note, 4)).Take(8)),
						Play(2, 2, 96, beat * 4, Midi.Note(note, 2)),
						Play(3, 3, 127, 250, 0,
							Chord.Select(chord, Midi.Note(note, 4)).Skip(1).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(0).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(2).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(1).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(3).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(2).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(4).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(3).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(5).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(4).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(6).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(5).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(7).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(6).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(8).First(),
							Chord.Select(chord, Midi.Note(note, 4)).Skip(7).First()
							)
						);

					await Task.WhenAll(
						Task.Run(async () =>
						{
							await PlayAll(0, 0, 127, beat * 2, 0, Chord.Suspended(Midi.Note(baseNote, 4)).Take(4));
							await PlayAll(0, 0, 112, beat * 2, 0, Chord.Major(Midi.Note(baseNote, 4)).Take(4));
						}),
						Task.Run(async () =>
						{
							await PlayAll(1, 1, 127, beat * 2, 0, Chord.Suspended(Midi.Note(baseNote, 4)).Take(8));
							await PlayAll(1, 1, 112, beat * 2, 0, Chord.Major(Midi.Note(baseNote, 4)).Take(8));
						}),
						Play(2, 2, 127, beat * 4, Midi.Note(baseNote, 2)),
						Task.Run(async () =>
						{
							await PlayAll(4, 4, 96, beat * 2, 0, Chord.Suspended(Midi.Note(baseNote, 6)).Take(2));
							await PlayAll(4, 4, 72, beat * 2, 0, Chord.Major(Midi.Note(baseNote, 6)).Take(2));
						})
						);
				}
			}

			await Task.Delay(5000);

			Midi.Disable();
		}

		[TestMethod]
		public async Task Groove()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.FingerBass);

			var seed = Random.Shared.Next();

			//seed = 0;
			//seed = 2;
			//seed = 3;
			//seed = 4;

			var random = new Random(seed);

			System.Diagnostics.Debug.WriteLine(seed);

			var time = 2000;

			var randomTime = (int remaining) =>
			{
				var t = random.Next(1, 9) * (1000 / 16);

				if (t > remaining)
					t = remaining;

				return t;
			};

			var randomPattern = () =>
			{
				var times = new List<int>();

				while (times.Sum() < time)
					times.Add(randomTime(time - times.Sum()));

				return times;
			};

			var drums = async () =>
			{
				await Play(9, 9, 127, 500, Midi.Drums.BassDrum);
				await Play(9, 9, 127, 500, Midi.Drums.SnareDrum);
				await Play(9, 9, 127, 500, Midi.Drums.BassDrum);
				await Play(9, 9, 127, 500, Midi.Drums.SnareDrum);
			};

			var pattern = Enumerable.Range(0, 4).Select(_ => randomPattern()).ToArray();

			var root = random.Next(12);
			var chord = random.Next(5);

			//var notes = Enumerable.Range(0, 16).Select(_ => Chord.Select(chord, root).Skip(random.Next(5)).First()).ToArray();
			var notes = Enumerable.Range(0, 16).Select(_ => Scale.Select(chord, root).Skip(random.Next(5)).First()).ToArray();

			for (var j = 0; j < 4; j++)
			{
				var note = 0;
				for (var i = 0; i < 2; i++)
				{
					await Task.WhenAll(Task.Run(async () =>
					{
						var noteOn = true;

						foreach (var t in pattern[i])
						{
							if (noteOn)
							{
								await Play(0, 0, 127, t, Midi.Note(notes[note++], 2));
								//await Play(0, 127, t, Midi.Note(Midi.Notes.E, 2));
								noteOn = false;
							}
							else
							{
								await Wait(0, t);

								noteOn = true;
							}
						}
					}),
					drums());
				}
			}

			//await Play(0, 127, 1000, 0, Chord.Major(Midi.Note(Midi.Notes.C, 3)).Take(4));
			//await Play(0, 127, 1000, 0, Chord.Major(Midi.Note(Midi.Notes.C, 3)).Take(3).Reverse());

			//await Play(0, 127, 1000, 0, Chord.Major(Midi.Note(Midi.Notes.A, 2)).Take(4));
			//await Play(0, 127, 1000, 0, Chord.Major(Midi.Note(Midi.Notes.A, 2)).Take(3).Reverse());

			//await Play(0, 127, 1000, 0, Chord.Minor(Midi.Note(Midi.Notes.D, 3)).Take(4));
			//await Play(0, 127, 1000, 0, Chord.Minor(Midi.Note(Midi.Notes.D, 3)).Take(3).Reverse());

			//await Play(0, 127, 1000, 0, Chord.Major(Midi.Note(Midi.Notes.G, 2)).Take(4));
			//await Play(0, 127, 1000, 0, Chord.Major(Midi.Note(Midi.Notes.G, 2)).Take(3).Reverse());

			//await PlayAll(0, 127, 1000, Chord.Major(Midi.Note(Midi.Notes.C, 4)).Take(3));
			//await PlayAll(0, 127, 1000, Chord.Major(Midi.Note(Midi.Notes.A, 4)).Take(4));
			//await PlayAll(0, 127, 1000, Chord.Minor(Midi.Note(Midi.Notes.D, 4)).Take(3));
			//await PlayAll(0, 127, 1000, Chord.Major(Midi.Note(Midi.Notes.G, 4)).Take(3));

			await Task.Delay(1000);

			Midi.Disable();

		}

		[TestMethod]
		public async Task Melody()
		{
			Midi.Refresh();
			Midi.Enable();

			Midi.ProgramChange(0, Midi.Patches.SynthVoice);

			var seed = Random.Shared.Next();

			var random = new Random(seed);

			for (var i = 0; i < 4; i++)
			{
				var root = random.Next(12);
				var chord = random.Next(5);

				await PlayAll(0, 0, 127, 2000, 0, Chord.Select(chord, Midi.Note(root, 3)).Take(8));

				root = random.Next(12);
				chord = random.Next(5);

				await PlayAll(0, 0, 127, 2000, 0, Chord.Select(chord, Midi.Note(root, 3)).Take(8));

				root = random.Next(12);
				chord = random.Next(5);

				await PlayAll(0, 0, 127, 4000, 0, Chord.Major(Midi.Note(root, 3)).Take(8));
			}

			await Task.Delay(5000);

			Midi.Disable();
		}

		private static int[] TrackTimers = new int[16];

		private async Task Wait(int track, int ticks)
		{
			var lastTickCount = Environment.TickCount;

			TrackTimers[track] += ticks;

			while (TrackTimers[track] > 0)
			{
				await Task.Delay(10);
				//await Task.Yield();

				var tickCount = Environment.TickCount;

				var delta = tickCount - lastTickCount;

				lastTickCount = tickCount;

				TrackTimers[track] -= delta;
			}
		}

		private async Task Play(int track, int channel, int velocity, int duration, int note)
		{
			Midi.NoteOn(channel, note, velocity);

			await Wait(track, duration);

			Midi.NoteOff(channel, note, velocity);
		}

		private Task PlayAll(int track, int channel, int velocity, int duration, int spread, params int[] notes)
		{
			return PlayAll(track, channel, velocity, duration, spread, (IEnumerable<int>)notes);
		}

		private async Task PlayAll(int track, int channel, int velocity, int duration, int spread, IEnumerable<int> notes)
		{
			var delay = 0;

			foreach (var note in notes)
			{
				Midi.NoteOn(channel, note, velocity);

				if (spread != 0)
					await Wait(track, spread);

				delay += spread;
			}

			await Wait(track, duration - delay);

			foreach (var note in notes)
				Midi.NoteOff(channel, note, velocity);
		}

		private Task Play(int track, int channel, int velocity, int duration, int delay, params int[] notes)
		{
			return Play(track, channel, velocity, duration, delay, (IEnumerable<int>)notes);
		}

		private async Task Play(int track, int channel, int velocity, int duration, int delay, IEnumerable<int> notes)
		{
			foreach (var note in notes)
			{
				await Play(track, channel, velocity, duration, note);
				await Wait(track, delay);
			}
		}
	}
}