using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiSharp
{
	public static class Chord
	{
		public static IEnumerable<int> Major(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 4;

				if (note <= 127)
					yield return note;

				note += 3;

				if (note <= 127)
					yield return note;

				note += 5;
			}
		}

		public static IEnumerable<int> Minor(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 3;

				if (note <= 127)
					yield return note;

				note += 4;

				if (note <= 127)
					yield return note;

				note += 5;
			}
		}

		public static IEnumerable<int> Diminished(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 3;

				if (note <= 127)
					yield return note;

				note += 3;

				if (note <= 127)
					yield return note;

				note += 6;
			}
		}

		public static IEnumerable<int> Suspended(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 5;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 5;
			}
		}

		public static IEnumerable<int> Augmented(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 4;

				if (note <= 127)
					yield return note;

				note += 4;

				if (note <= 127)
					yield return note;

				note += 4;
			}
		}

		public static IEnumerable<int> Dream(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 5;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 1;

				if (note <= 127)
					yield return note;

				note += 5;
			}
		}

		public static IEnumerable<int> Elektra(int root)
		{
			var note = root;

			while (note <= 127)
			{
				yield return note;

				note += 7;

				if (note <= 127)
					yield return note;

				note += 2;

				if (note <= 127)
					yield return note;

				note += 4;

				if (note <= 127)
					yield return note;

				note += 3;

				if (note <= 127)
					yield return note;

				note += 8;
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
					return Diminished(root);

				case 3:
					return Suspended(root);

				case 4:
					return Augmented(root);

				case 5:
					return Dream(root);

				case 6:
					return Elektra(root);

				default:
					return Major(root);
			}
		}

		public static readonly string[] Names = new string[]
		{
			"Major",
			"Minor",
			"Diminished",
			"Suspended",
			"Augmented",
			"Dream",
			"Elektra"
		};
	}
}
