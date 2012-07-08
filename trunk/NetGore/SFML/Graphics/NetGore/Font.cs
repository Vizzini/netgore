using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace SFML
{
    namespace Graphics
    {
        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Structure describing a glyph (a visual character)
        /// </summary>
        ////////////////////////////////////////////////////////////
        [StructLayout(LayoutKind.Sequential)]
        public struct Glyph : IEquatable<Glyph>
        {
            /// <summary>Offset to move horizontically to the next character</summary>
            public int Advance;

            /// <summary>Bounding rectangle of the glyph, in coordinates relative to the baseline</summary>
            public IntRect Rectangle;

            /// <summary>Texture coordinates of the glyph inside the font's image</summary>
            public FloatRect TexCoords;

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            public bool Equals(Glyph other)
            {
                return other.Advance == Advance && other.Rectangle.Equals(Rectangle) && other.TexCoords.Equals(TexCoords);
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="obj">Another object to compare to.</param>
            /// <returns>
            /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                return obj is Glyph && this == (Glyph)obj;
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var result = Advance;
                    result = (result * 397) ^ Rectangle.GetHashCode();
                    result = (result * 397) ^ TexCoords.GetHashCode();
                    return result;
                }
            }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="left">The left argument.</param>
            /// <param name="right">The right argument.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(Glyph left, Glyph right)
            {
                return left.Equals(right);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="left">The left argument.</param>
            /// <param name="right">The right argument.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(Glyph left, Glyph right)
            {
                return !left.Equals(right);
            }
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Font is the low-level class for loading and
        /// manipulating character fonts. This class is meant to
        /// be used by String2D
        /// </summary>
        ////////////////////////////////////////////////////////////
        public class Font : ObjectBase
        {
            ////////////////////////////////////////////////////////////
            static Font ourDefaultFont = null;
            readonly Dictionary<uint, Image> myImages = new Dictionary<uint, Image>();
            uint _defaultSize = 36;

            #region Imports

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern IntPtr sfFont_Copy(IntPtr Font);

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern IntPtr sfFont_CreateFromFile(string Filename);

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern unsafe IntPtr sfFont_CreateFromMemory(char* Data, uint SizeInBytes);

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern void sfFont_Destroy(IntPtr This);

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern IntPtr sfFont_GetDefaultFont();

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern Glyph sfFont_GetGlyph(IntPtr This, uint codePoint, uint characterSize, bool bold);

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern IntPtr sfFont_GetImage(IntPtr This, uint characterSize);

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern int sfFont_GetKerning(IntPtr This, uint first, uint second, uint characterSize);

            [DllImport("csfml-graphics-2", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern int sfFont_GetLineSpacing(IntPtr This, uint characterSize);

            #endregion

            /// <summary>
            /// Construct the font from a file
            /// </summary>
            /// <param name="filename">Font file to load</param>
            /// <exception cref="LoadingFailedException" />
            ////////////////////////////////////////////////////////////
            public Font(string filename) : base(IntPtr.Zero)
            {
                EnsureLoaded(filename);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Construct the font from a file in a stream
            /// </summary>
            /// <param name="stream">Stream containing the file contents</param>
            /// <exception cref="LoadingFailedException" />
            ////////////////////////////////////////////////////////////
            public Font(Stream stream) : base(IntPtr.Zero)
            {
                unsafe
                {
                    stream.Position = 0;
                    var StreamData = new byte[stream.Length];
                    var Read = (uint)stream.Read(StreamData, 0, StreamData.Length);
                    fixed (byte* dataPtr = StreamData)
                    {
                        SetThis(sfFont_CreateFromMemory((char*)dataPtr, Read));
                    }
                }

                if (This == IntPtr.Zero)
                    throw new LoadingFailedException("font");
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Construct the font from another font
            /// </summary>
            /// <param name="copy">Font to copy</param>
            ////////////////////////////////////////////////////////////
            public Font(Font copy) : base(sfFont_Copy(copy.This))
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Font"/> class.
            /// </summary>
            protected internal Font() : base(IntPtr.Zero)
            {
            }

            /// <summary>
            /// Internal constructor
            /// </summary>
            /// <param name="thisPtr">Pointer to the object in C library</param>
            ////////////////////////////////////////////////////////////
            Font(IntPtr thisPtr) : base(thisPtr)
            {
            }

            /// <summary>
            /// Default built-in font
            /// </summary>
            ////////////////////////////////////////////////////////////
            public static Font DefaultFont
            {
                get
                {
                    if (ourDefaultFont == null)
                        ourDefaultFont = new Font(sfFont_GetDefaultFont());

                    return ourDefaultFont;
                }
            }

            /// <summary>
            /// Gets or sets the default size used for this <see cref="Font"/>.
            /// </summary>
            public uint DefaultSize
            {
                get { return _defaultSize; }
                set { _defaultSize = value; }
            }

            /// <summary>
            /// Handle the destruction of the object
            /// </summary>
            /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
            ////////////////////////////////////////////////////////////
            protected override void Destroy(bool disposing)
            {
                if (this != ourDefaultFont)
                {
                    var t = ThisRaw;

                    if (!disposing)
                        Context.Global.SetActive(true);

                    if (t != IntPtr.Zero)
                        sfFont_Destroy(t);

                    if (disposing)
                    {
                        foreach (var image in myImages.Values)
                        {
                            image.Dispose();
                        }
                    }

                    myImages.Clear();

                    if (!disposing)
                        Context.Global.SetActive(false);
                }
            }

            /// <summary>
            /// Reloads the asset from file if it is not loaded.
            /// </summary>
            /// <param name="filename">Font file to load</param>
            /// <returns>True if already loaded; false if it had to reload.</returns>
            /// <exception cref="LoadingFailedException"/>
            protected internal bool EnsureLoaded(string filename)
            {
                // NOTE: Custom method
                if (ThisRaw != IntPtr.Zero)
                    return true;

                var ptr = sfFont_CreateFromFile(filename);
                SetThis(ptr);

                if (ThisRaw == IntPtr.Zero)
                    throw new LoadingFailedException("font", filename);

                return false;
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get a glyph in the font
            /// </summary>
            /// <param name="codePoint">Unicode code point of the character to get</param>
            /// <param name="characterSize">Character size</param>
            /// <param name="bold">Retrieve the bold version or the regular one?</param>
            /// <returns>The glyph corresponding to the character</returns>
            ////////////////////////////////////////////////////////////
            public Glyph GetGlyph(uint codePoint, uint characterSize, bool bold)
            {
                return sfFont_GetGlyph(This, codePoint, characterSize, bold);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get a glyph in the font
            /// </summary>
            /// <param name="codePoint">Unicode code point of the character to get</param>
            /// <param name="bold">Retrieve the bold version or the regular one?</param>
            /// <returns>The glyph corresponding to the character</returns>
            ////////////////////////////////////////////////////////////
            public Glyph GetGlyph(uint codePoint, bool bold)
            {
                // NOTE: Custom method
                return GetGlyph(codePoint, DefaultSize, bold);
            }

            /// <summary>
            /// Get the image containing the glyphs of a given size
            /// </summary>
            /// <param name="characterSize">Character size</param>
            /// <returns>Image storing the glyphs for the given size</returns>
            ////////////////////////////////////////////////////////////
            public Image GetImage(uint characterSize)
            {
                Image img;
                if (!myImages.TryGetValue(characterSize, out img))
                {
                    img = new Image(sfFont_GetImage(This, characterSize));
                    myImages[characterSize] = img;
                }

                return img;
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get the image containing the glyphs of a given size
            /// </summary>
            /// <returns>Image storing the glyphs for the given size</returns>
            ////////////////////////////////////////////////////////////
            public Image GetImage()
            {
                // NOTE: Custom method
                return GetImage(DefaultSize);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get the kerning offset between two glyphs
            /// </summary>
            /// <param name="first">Unicode code point of the first character</param>
            /// <param name="second">Unicode code point of the second character</param>
            /// <param name="characterSize">Character size</param>
            /// <returns>Kerning offset, in pixels</returns>
            ////////////////////////////////////////////////////////////
            public int GetKerning(uint first, uint second, uint characterSize)
            {
                return sfFont_GetKerning(This, first, second, characterSize);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get the kerning offset between two glyphs
            /// </summary>
            /// <param name="first">Unicode code point of the first character</param>
            /// <param name="second">Unicode code point of the second character</param>
            /// <returns>Kerning offset, in pixels</returns>
            ////////////////////////////////////////////////////////////
            public int GetKerning(uint first, uint second)
            {
                // NOTE: Custom method
                return GetKerning(first, second, DefaultSize);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get spacing between two consecutive lines
            /// </summary>
            /// <param name="characterSize">Character size</param>
            /// <returns>Line spacing, in pixels</returns>
            ////////////////////////////////////////////////////////////
            public int GetLineSpacing(uint characterSize)
            {
                return sfFont_GetLineSpacing(This, characterSize);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Get spacing between two consecutive lines
            /// </summary>
            /// <returns>Line spacing, in pixels</returns>
            ////////////////////////////////////////////////////////////
            public int GetLineSpacing()
            {
                // NOTE: Custom method
                return GetLineSpacing(DefaultSize);
            }

            ////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Provide a string describing the object
            /// </summary>
            /// <returns>String description of the object</returns>
            ////////////////////////////////////////////////////////////
            public override string ToString()
            {
                return "[Font]";
            }

            ////////////////////////////////////////////////////////////
        }
    }
}