using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Info.Map
{
    public class MapItemInfoExtractor : IEntityInfoExtractor<MapItem>
    {
        public string[] GetInfo(MapItem entity)
        {
            return
            [
                $"MUS stream offset: 0x{entity.MusOffset:X8}",
                "=============================",
                $"Jumps: {entity.Jumps.Count}",
                ..entity.Jumps.SelectMany(DumpJump).Select(p => $"  {p}"),
            ];
        }

        private string[] DumpJump(MapJump jump, int index)
        {
            return
            [
                string.Empty,
                $"Jump to {jump.NextItem}",
                "------------------",
                $"Data: {string.Join(" ", jump.StateData.Select(p => p.ToString("X2")))}"
            ];
        }
    }
}
