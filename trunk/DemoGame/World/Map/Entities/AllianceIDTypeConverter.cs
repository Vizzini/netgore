using System.ComponentModel;
using System.Linq;
using NetGore;

namespace DemoGame
{
    /// <summary>
    /// A <see cref="TypeConverter"/> for the <see cref="AllianceID"/> type.
    /// </summary>
    public class AllianceIDTypeConverter : SimpleTypeConverter<AllianceID>
    {
        /// <summary>
        /// When overridden in the derived class, converts the <paramref name="value"/> to the output
        /// type.
        /// </summary>
        /// <param name="value">A string containing the value to parse.</param>
        /// <param name="wasConverted">Contains true if the parsing was successful, or false if the
        /// parsing failed.</param>
        /// <returns>The object parsed from the <paramref name="value"/>.</returns>
        protected override AllianceID ConvertFromString(string value, out bool wasConverted)
        {
            int i;
            wasConverted = int.TryParse(value, out i);

            if (wasConverted)
                return new AllianceID(i);

            return new AllianceID(1);
        }
    }
}