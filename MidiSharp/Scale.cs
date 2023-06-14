using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiSharp
{
	public static class Scale
	{
		public static IEnumerable<int> Major(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;
			}
		}

		public static IEnumerable<int> Minor(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 2;
			}
		}

		public static IEnumerable<int> Chromatic(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note++;
			}
		}

		public static IEnumerable<int> HarmonicMajor(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 3;

				if (note <= 127)
					yield return note;

				note += 1;
			}
		}

		public static IEnumerable<int> HarmonicMinor(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 3;

				if (note <= 127)
					yield return note;

				note += 1;
			}
		}

		public static IEnumerable<int> Select(int chord, int root)
		{
			switch (chord)
			{
				case 0:
					return Major(root);

				case 1:
					return Minor(root);

				case 2:
					return Chromatic(root);

				case 3:
					return HarmonicMajor(root);

				case 4:
					return HarmonicMinor(root);

				default:
					return Chromatic(root);
			}
		}

		public static readonly string[] Names = new string[]
		{
			"Major",
			"Minor",
			"Chromatic",
			"Harmonic Major",
			"Harmonic Minor"
		};
	}
}
