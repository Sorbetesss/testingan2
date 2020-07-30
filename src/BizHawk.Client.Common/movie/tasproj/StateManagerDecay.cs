﻿/****************************************************************************************
	
	Algorithm by r57shell & feos, 2018

	_zeros is the key to GREENZONE DECAY PATTERN.

	In a 16 element example, we evaluate these bitwise numbers to count zeros on the right.
	First element is always assumed to be 16, which has all 4 bits set to 0. Each right zero
	means that we lower the priority of a state that goes at that index. Priority changes
	depending on current frame and amount of states. States with biggest priority get erased
	first. With a 4-bit pattern and no initial gap between states, total frame coverage is
	about 5 times state count.
	
	Initial state gap can screw up our patterns, so do all the calculations like the gap
	isn't there, and take it back into account afterwards. The algo only works with integral
	greenzone, so we make it think it is integral by reducing the frame numbers. Before any
	decay logic starts for each state, we check if it has a marker on it (in which case we
	don't drop it) or appears inside the state gap (in which case we forcibly drop it). This
	step doesn't involve numbers reduction.

	_zeros values are essentially the values of rshiftby here:
	bitwise view     frame    rshiftby priority
	  00010000         0         4         1
	  00000001         1         0        15
	  00000010         2         1         7
	  00000011         3         0        13
	  00000100         4         2         3
	  00000101         5         0        11
	  00000110         6         1         5
	  00000111         7         0         9
	  00001000         8         3         1
	  00001001         9         0         7
	  00001010        10         1         3
	  00001011        11         0         5
	  00001100        12         2         1
	  00001101        13         0         3
	  00001110        14         1         1
	  00001111        15         0         1

*****************************************************************************************/
using System.Collections.Generic;

namespace BizHawk.Client.Common
{
	// TODO: interface me
	internal class StateManagerDecay
	{
		private readonly ITasMovie _movie;
		private readonly TasStateManager _tsm;

		private List<int> _zeros;		// number of ending zeros in binary representation of the index
		private int _bits;				// max number of bits for which to calculate _zeros
		private int _mask;				// to mask index into _zeros, to prevent accessing out of range

		private int _step;				// initial gap between states

		public StateManagerDecay(ITasMovie movie, TasStateManager tsm)
		{
			_movie = movie;
			_tsm = tsm;
		}

		/// <summary>
		/// This will strategically remove states based on their alignment with the state gap (and its multiples) and their distance from the current frame.
		/// </summary>
		public void Trigger(int currentEmulatedFrame, int statesToDecay)
		{
			int baseStateIndex = _tsm.GetStateIndexByFrame(currentEmulatedFrame);
			int baseStateFrame = _tsm.GetStateFrameByIndex(baseStateIndex) / _step; // reduce to step integral TODO: do we actually want this?
			//      key: priority  value: frame
			List<KeyValuePair<int, int>> decayPriorities = new List<KeyValuePair<int, int>>();

			for (int currentStateIndex = 1; currentStateIndex < _tsm.Count; currentStateIndex++)
			{
				int currentFrame = _tsm.GetStateFrameByIndex(currentStateIndex);

				if (_movie.Markers.IsMarker(currentFrame + 1))
					continue;
				if (currentFrame + 1 == _movie.LastEditedFrame)
					continue;
				
				// not aligned to state gap at all
				if (currentFrame % _step > 0) 
				{
					if (_tsm.Remove(currentFrame))
						statesToDecay--;
					if (statesToDecay == 0)
						return;
					continue;
				}
				
				// reduce to step integral for the decay logic
				currentFrame /= _step;
				int zeroCount = _zeros[currentFrame & _mask];
				int priority = (baseStateFrame - currentFrame) >> zeroCount;
				decayPriorities.Add(new KeyValuePair<int, int>(priority, currentFrame * _step));
			}

			// optimization: if we are only removing 1 state, don't bother sorting the whole list
			if (statesToDecay == 1)
			{
				int highestPriority = decayPriorities[0].Key;
				int toRemove = decayPriorities[0].Value;
				for (int i = 1; i < decayPriorities.Count; i++)
				{
					if (decayPriorities[i].Key > highestPriority)
					{
						highestPriority = decayPriorities[i].Key;
						toRemove = decayPriorities[i].Value;
					}
				}
				if (!_tsm.Remove(toRemove))
					throw new System.Exception("Failed to remove state."); // should never happen
				return;
			}
			else
			{
				// reverse sort; high priority to remove comes first
				decayPriorities.Sort((p2, p1) => p1.Key.CompareTo(p2.Key));
			}

			int index = 0;
			while (statesToDecay > 0 && index < decayPriorities.Count)
			{
				if (_tsm.Remove(decayPriorities[index].Value))
					statesToDecay--;
				index++;
			}
				
			// we're very sorry about failing to find states to remove, but we can't go beyond capacity, so remove at least something
			while (statesToDecay > 0)
			{
				if (_tsm.Remove(_tsm.GetStateFrameByIndex(1)))
					statesToDecay--;
				else // This should never happen, but just in case, we don't want to let memory usage continue to climb.
					throw new System.Exception("Failed to remove states.");
			}
		}

		public void UpdateSettings(int step, int bits)
		{
			_step = step;
			_bits = bits;
			_mask = (1 << _bits) - 1;
			_zeros = new List<int> { _bits };

			for (int i = 1; i < (1 << _bits); i++)
			{
				_zeros.Add(0);

				for (int j = i; j > 0; j >>= 1)
				{
					if ((j & 1) > 0)
					{
						break;
					}

					_zeros[i]++;
				}
			}
		}
	}
}
