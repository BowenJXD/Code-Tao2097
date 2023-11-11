using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(fileName = "New BagItemConfig", menuName = "Data/SO/BagItemConfig")]
    public class SO_BagItemConfig : ScriptableObject
    {
        public E_BagItemType types;
        public E_BagItemType defaultType;

        [SerializeField] private int rowCount;
        [SerializeField] private int columnCount;
        [SerializeField] private float spacing;

        public int defaultCanUseCount;
        public float cellSize;
        public int RowCount => rowCount;

        public Vector2 ViewSize => new Vector2(CalcLength(rowCount), CalcLength(columnCount));
        private float CalcLength(int count) => count * cellSize + (count - 1) * spacing;
        public int PageSize => rowCount * columnCount;
        public Vector2 GetPos(int index)
        {
            var x = index % rowCount;
            var y = index / rowCount;
            return new Vector2(x * cellSize + x * spacing, y * -cellSize - y * spacing);
        }
        public Vector2 CellSize => Vector2.one * cellSize;
    }
}