using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ViennaDotNet.PreviewGenerator
{
    record PreviewModel(
         int format_version, // always 1
         bool isNight,
         PreviewModel.SubChunk[] sub_chunks,
         PreviewModel.BlockEntity[] blockEntities,
         PreviewModel.Entity[] entities
    )
    {
        public record Position(
            int x,
            int y,
            int z
        )
        {
        }

        public record SubChunk(
            Position position,
            SubChunk.PaletteEntry[] block_palette,
            int[] blocks
        )
        {
            public record PaletteEntry(
                string name,
                int data
            )
            {
            }
        }

        public record BlockEntity(
            int type,
            Position position,
            JsonNbtConverter.JsonNbtTag data
        )
        {
        }

        public record Entity(
            string name,
            Entity.Position position,
            Entity.Rotation rotation,
            Entity.Position shadowPosition,
            float shadowSize,
            int overlayColor,
            int changeColor,
            int multiplicitiveTintChangeColor,
            Dictionary<string, object>? extraData,
            string skinData,
            bool isPersonaSkin
        )
        {
            public record Position(
                float x,
                float y,
                float z
            )
            {
            }

            public record Rotation(
                float x,
                float y
            )
            {
            }
        }
    }
}
