using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Info.Map
{
    public class MapFileInfoExtractor : IEntityInfoExtractor<MapFile>
    {
        private static MapItemInfoExtractor _itemExtractor = new();
        public string[] GetInfo(MapFile entity)
        {
            return
            [
                $"Unk_0x04: 0x{entity.Unk_0x04:X2}",
                $"Items: {entity.Items.Count}",
                $"First item: {entity.FirstItem}",
                ..entity.Items.Select(DumpWithIndex)
            ];
        }

        private string DumpWithIndex(MapItem source, int index)
        {
            return string.Join(Environment.NewLine,[$"  Index: {index}", .._itemExtractor.GetInfo(source).Select(p => $"  {p}")]);
        }
    }
}
