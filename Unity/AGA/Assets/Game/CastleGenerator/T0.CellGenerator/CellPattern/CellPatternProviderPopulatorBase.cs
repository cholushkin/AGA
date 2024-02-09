using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CastleGenerator.Tier0
{
	public abstract class CellPatternProviderPopulatorBase : MonoBehaviour, ICellPatternProviderPopulator 
	{
		public abstract void Populate(CellPatternProvider targetPatternProvider);
	}
}
