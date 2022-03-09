using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using System.IO;

namespace TrialHelper
{
	public class Trial
	{

		protected int story;
		protected int state = 0;
		protected string group;

		public Trial(int s, string g)
        {
			story = s;
			group = g;
        }

		public virtual (int s, string e) Next ()
        {
			state += 1;
			switch (state)
            {
				case 1:
					return (story, "neutral");
				case 2:
					return (6, "neutral");
				case 3:
					state -= 1;
					return (0, "neutral"); // need input lol
				case 4:
					return (story, group);
				default:
					return (-1, "");
			}
        }

		public (int s, string e) Response(int input)
        {
			if (state != 2) return (-2, "neutral"); // should not happen (at least we hope so)
			state += input + 1;
			return ((input == 1 ? 7 : 8), group);
		}

	}

	public class TrialSpecial : Trial
    {
		public TrialSpecial(int s, string g) : base(s, g)
		{
		}

		public override (int s, string e) Next ()
		{
			state += 1;
			if (state == 1) return (story, group);
			return (-1, "");
		}
	}
}
