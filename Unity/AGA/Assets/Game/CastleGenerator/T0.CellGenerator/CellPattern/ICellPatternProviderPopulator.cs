using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CastleGenerator.Tier0
{
	public interface ICellPatternProviderPopulator
	{
		void Populate(CellPatternProvider targetPatternProvider);
	}

}
