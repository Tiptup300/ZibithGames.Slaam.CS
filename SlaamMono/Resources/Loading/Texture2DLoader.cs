﻿using Microsoft.Xna.Framework.Graphics;
using SlaamMono.Library.Logging;
using System;

namespace SlaamMono.Resources.Loading
{
    public class Texture2DLoader : IFileLoader<Texture2D>
    {
        private readonly ILogger _logger;
        private readonly IPixelFactory _pixelFactory;

        public Texture2DLoader(ILogger logger, IPixelFactory pixelFactory)
        {
            _logger = logger;
            _pixelFactory = pixelFactory;
        }

        public object Load(string filePath)
        {
            Texture2D output;

            try
            {
                output = SlaamGame.Content.Load<Texture2D>(filePath);
                _logger.Log($" - {filePath} Texture Loaded.");
            }
            catch (Exception ex)
            {
                output = _pixelFactory.BuildPixel();
                _logger.Log($"Texture \"{filePath}\" failed to load. Replaced with a blank pixel. Error: {ex.Message}");
            }
            return output;
        }
    }
}
