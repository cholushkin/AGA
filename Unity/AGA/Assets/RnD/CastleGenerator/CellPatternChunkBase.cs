using UnityEngine;
using UnityEngine.Events;


namespace CastleGenerator
{

	public abstract class CellPatternChunkBase : MonoBehaviour
	{
		public UnityEvent OnGenerate;

		public Vector2Int ChunkSize;

		public abstract void Generate();
		public abstract void Set(int col, int row, bool value);
		public abstract bool Get(int col, int row);

	}
}