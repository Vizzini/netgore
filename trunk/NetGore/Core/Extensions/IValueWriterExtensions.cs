using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NetGore.Content;
using NetGore.IO;
using SFML.Graphics;

namespace NetGore
{
    /// <summary>
    /// Extensions for the <see cref="IValueWriter"/>.
    /// </summary>
    public static class IValueWriterExtensions
    {
        public static void Write(this IValueWriter writer, string name, BlendMode value)
        {
            writer.WriteEnum(name, value);
        }

        /// <summary>
        /// Writes a <see cref="VariableByte"/> the specified writer.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, VariableByte value)
        {
            WriteVariableValue(writer, name, value, writer.Write);
        }

        /// <summary>
        /// Writes a <see cref="VariableColor"/> the specified writer.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, VariableColor value)
        {
            WriteVariableValue(writer, name, value, writer.Write);
        }

        /// <summary>
        /// Writes a <see cref="VariableFloat"/> the specified writer.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, VariableFloat value)
        {
            WriteVariableValue(writer, name, value, writer.Write);
        }

        /// <summary>
        /// Writes a <see cref="VariableInt"/> the specified writer.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, VariableInt value)
        {
            WriteVariableValue(writer, name, value, writer.Write);
        }

        /// <summary>
        /// Writes a <see cref="VariableSByte"/> the specified writer.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, VariableSByte value)
        {
            WriteVariableValue(writer, name, value, writer.Write);
        }

        /// <summary>
        /// Writes a <see cref="VariableShort"/> the specified writer.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, VariableShort value)
        {
            WriteVariableValue(writer, name, value, writer.Write);
        }

        /// <summary>
        /// Writes a <see cref="VariableUShort"/> the specified writer.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, VariableUShort value)
        {
            WriteVariableValue(writer, name, value, writer.Write);
        }

        public static void Write(this IValueWriter writer, string name, SpriteCategory value)
        {
            writer.Write(name, value.ToString());
        }

        public static void Write(this IValueWriter writer, string name, ContentAssetName value)
        {
            writer.Write(name, value.Value);
        }

        public static void Write(this IValueWriter writer, string name, SpriteTitle value)
        {
            writer.Write(name, value.ToString());
        }

        public static void Write(this IValueWriter writer, string name, SpriteCategorization value)
        {
            writer.Write(name, value.ToString());
        }

        /// <summary>
        /// Writes an unsigned integer with the specified range to an IValueWriter.
        /// </summary>
        /// <param name="writer">IValueWriter to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <param name="name">Name of the value to write.</param>
        /// <param name="minValue">Minimum (inclusive) value that the written value can be.</param>
        /// <param name="maxValue">Maximum (inclusive) value that the written value can be.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than <paramref name="maxValue"/>
        /// or less than <paramref name="minValue"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than <paramref name="minValue"/>.</exception>
        public static void Write(this IValueWriter writer, string name, uint value, uint minValue, uint maxValue)
        {
            if (value < minValue || value > maxValue)
                throw new ArgumentOutOfRangeException("value", "Value parameter must be between minValue and maxValue.");
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException("maxValue", "MaxValue must be greater than or equal to MinValue.");

            // Find the number of bits required for the range of desired values
            var maxWriteValue = maxValue - minValue;
            var bitsRequired = BitOps.RequiredBits(maxWriteValue);

            // Subtract the minimum value from the value since we want to write how high above the minimum value
            // the value is, not the actual value
            var offsetFromMin = value - minValue;
            writer.Write(name, offsetFromMin, bitsRequired);

            Debug.Assert((value - minValue) <= maxWriteValue);
            Debug.Assert((1 << bitsRequired) >= maxWriteValue);
        }

        /// <summary>
        /// Writes a signed integer with the specified range to an IValueWriter.
        /// </summary>
        /// <param name="writer">IValueWriter to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <param name="name">Name of the value to write.</param>
        /// <param name="minValue">Minimum (inclusive) value that the written value can be.</param>
        /// <param name="maxValue">Maximum (inclusive) value that the written value can be.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than <paramref name="maxValue"/>
        /// or less than <paramref name="minValue"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than <paramref name="minValue"/>.</exception>
        public static void Write(this IValueWriter writer, string name, int value, int minValue, int maxValue)
        {
            if (value < minValue || value > maxValue)
                throw new ArgumentOutOfRangeException("value", "Value parameter must be between minValue and maxValue.");
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException("maxValue", "MaxValue must be greater than or equal to MinValue.");

            // Find the number of bits required for the range of desired values
            var maxWriteValue = (uint)(maxValue - minValue);
            var bitsRequired = BitOps.RequiredBits(maxWriteValue);

            // Subtract the minimum value from the value since we want to write how high above the minimum value
            // the value is, not the actual value
            var offsetFromMin = (uint)(value - minValue);
            writer.Write(name, offsetFromMin, bitsRequired);

            Debug.Assert((value - minValue) <= maxWriteValue);
            Debug.Assert((1 << bitsRequired) >= maxWriteValue);
        }

        /// <summary>
        /// Writes a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, Vector2 value)
        {
            if (writer.SupportsNameLookup)
            {
                // We are using name lookup, so we have to combine the values so we use only one name
                var x = Parser.Invariant.ToString(value.X);
                var y = Parser.Invariant.ToString(value.Y);
                writer.Write(name, x + "," + y);
            }
            else
            {
                // Not using name lookup, so just write them out
                writer.Write(null, value.X);
                writer.Write(null, value.Y);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, Vector3 value)
        {
            if (writer.SupportsNameLookup)
            {
                // We are using name lookup, so we have to combine the values so we use only one name
                var x = Parser.Invariant.ToString(value.X);
                var y = Parser.Invariant.ToString(value.Y);
                var z = Parser.Invariant.ToString(value.Z);
                writer.Write(name, x + "," + y + "," + z);
            }
            else
            {
                // Not using name lookup, so just write them out
                writer.Write(null, value.X);
                writer.Write(null, value.Y);
                writer.Write(null, value.Z);
            }
        }

        /// <summary>
        /// Writes a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="writer"><see cref="IValueWriter"/> to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, Vector4 value)
        {
            if (writer.SupportsNameLookup)
            {
                // We are using name lookup, so we have to combine the values so we use only one name
                var x = Parser.Invariant.ToString(value.X);
                var y = Parser.Invariant.ToString(value.Y);
                var z = Parser.Invariant.ToString(value.Z);
                var w = Parser.Invariant.ToString(value.W);
                writer.Write(name, x + "," + y + "," + z + "," + w);
            }
            else
            {
                // Not using name lookup, so just write them out
                writer.Write(null, value.X);
                writer.Write(null, value.Y);
                writer.Write(null, value.Z);
                writer.Write(null, value.W);
            }
        }

        /// <summary>
        /// Writes a Point.
        /// </summary>
        /// <param name="writer">IValueWriter to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, Point value)
        {
            if (writer.SupportsNameLookup)
            {
                // We are using name lookup, so we have to combine the values so we use only one name
                var x = Parser.Invariant.ToString(value.X);
                var y = Parser.Invariant.ToString(value.Y);
                writer.Write(name, x + "," + y);
            }
            else
            {
                // Not using name lookup, so just write them out
                writer.Write(null, value.X);
                writer.Write(null, value.Y);
            }
        }

        /// <summary>
        /// Writes a Rectangle.
        /// </summary>
        /// <param name="writer">IValueWriter to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, Rectangle value)
        {
            if (writer.SupportsNameLookup)
            {
                // We are using name lookup, so we have to combine the values so we use only one name
                var x = Parser.Invariant.ToString(value.X);
                var y = Parser.Invariant.ToString(value.Y);
                var w = Parser.Invariant.ToString(value.Width);
                var h = Parser.Invariant.ToString(value.Height);
                writer.Write(name, x + "," + y + "," + w + "," + h);
            }
            else
            {
                // Not using name lookup, so just write them out
                writer.Write(null, value.X);
                writer.Write(null, value.Y);
                writer.Write(null, value.Width);
                writer.Write(null, value.Height);
            }
        }

        /// <summary>
        /// Writes a Color.
        /// </summary>
        /// <param name="writer">IValueWriter to write to.</param>
        /// <param name="name">Unique name of the <paramref name="value"/> that will be used to distinguish it
        /// from other values when reading.</param>
        /// <param name="value">Value to write.</param>
        public static void Write(this IValueWriter writer, string name, Color value)
        {
            if (writer.SupportsNameLookup)
            {
                // We are using name lookup, so we have to combine the values so we use only one name
                const string delimiter = ",";
                var sb = new StringBuilder(16);
                sb.Append(Parser.Invariant.ToString(value.R));
                sb.Append(delimiter);
                sb.Append(Parser.Invariant.ToString(value.G));
                sb.Append(delimiter);
                sb.Append(Parser.Invariant.ToString(value.B));
                sb.Append(delimiter);
                sb.Append(Parser.Invariant.ToString(value.A));
                writer.Write(name, sb.ToString());
            }
            else
            {
                // Not using name lookup, so just write them out
                // ReSharper disable RedundantCast
                writer.Write(null, (byte)value.R);
                writer.Write(null, (byte)value.G);
                writer.Write(null, (byte)value.B);
                writer.Write(null, (byte)value.A);
                // ReSharper restore RedundantCast
            }
        }

        /// <summary>
        /// Helps write a variable value.
        /// </summary>
        /// <typeparam name="T">The internal type of the variable value.</typeparam>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="writeValue">Delegate used to write type <typeparamref name="T"/>.</param>
        static void WriteVariableValue<T>(IValueWriter writer, string name, IVariableValue<T> value, Action<string, T> writeValue)
        {
            if (writer.SupportsNodes && writer.SupportsNameLookup)
            {
                writer.WriteStartNode(name);
                writeValue("Min", value.Min);
                writeValue("Max", value.Max);
                writer.WriteEndNode(name);
            }
            else
            {
                writeValue("Min", value.Min);
                writeValue("Max", value.Max);
            }
        }
    }
}