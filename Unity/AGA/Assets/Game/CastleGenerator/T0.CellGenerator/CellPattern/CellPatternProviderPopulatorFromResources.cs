using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CastleGenerator.Tier0
{
	public class CellPatternProviderPopulatorFromResources : CellPatternProviderPopulatorBase
	{
		public string ResourcesPath;
        
		public override void Populate(CellPatternProvider targetPatternProvider)
		{
			var patternsList = Resources.LoadAll<CellPattern>(ResourcesPath).ToList();
			if (patternsList.Count > 0)
				targetPatternProvider.Populate(patternsList);
		}
	}
    
}

