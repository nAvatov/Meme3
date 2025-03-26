namespace _ProjectAssets.Scripts.Structures
{
    public struct ArrayPositionData
    {
        public int RowIndex;
        public int ColumnIndex;

        public ArrayPositionData(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }

        public string DebugPositionData()
        {
            return string.Format("RowIndex: {0}, ColumnIndex: {1}", RowIndex, ColumnIndex);
        }
    }
}