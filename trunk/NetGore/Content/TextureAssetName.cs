using System;
using System.ComponentModel;
using System.Linq;
using NetGore.IO;

namespace NetGore.Content
{
    /// <summary>
    /// An immutable string that represents the name, or virtual path, to a texture content asset.
    /// </summary>
    [TypeConverter(typeof(TextureAssetNameConverter))]
    public sealed class TextureAssetName : ContentAssetName
    {
        const string _rootDir = ContentPaths.GrhsFolder + PathSeparator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureAssetName"/> class.
        /// </summary>
        /// <param name="textureName">Name of the texture asset.</param>
        public TextureAssetName(string textureName) : base(GetContentAssetName(textureName))
        {
        }

        static string GetContentAssetName(string textureName)
        {
            // Only prefix the root if we need to
            if (textureName.StartsWith(_rootDir, StringComparison.OrdinalIgnoreCase))
                return textureName;

            return _rootDir + textureName;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Value))
                return string.Empty;

            if (Value.Length > _rootDir.Length)
                return Value.Substring(_rootDir.Length);

            return Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="TextureAssetName"/>.
        /// </summary>
        /// <param name="textureName">Name of the texture asset.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TextureAssetName(string textureName)
        {
            return new TextureAssetName(textureName);
        }
    }
}