using System.ComponentModel;
using System.Linq;
using NetGore;
using NetGore.Graphics;
using NetGore.IO;
using SFML.Graphics;

namespace DemoGame
{
    /// <summary>
    /// Describes the body for a character, such as the size and what components to use to perform certain types
    /// of animations.
    /// </summary>
    public class BodyInfo
    {
        const string _bodyValueKey = "Body";
        const string _fallValueKey = "Fall";
        const string _idValueKey = "ID";
        const string _jumpValueKey = "Jump";
        const string _punchValueKey = "Punch";
        const string _sizeValueKey = "Size";
        const string _standValueKey = "Stand";
        const string _walkValueKey = "Walk";

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyInfo"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="IValueReader"/> to read the values from.</param>
        public BodyInfo(IValueReader reader)
        {
            ID = reader.ReadBodyID(_idValueKey);
            Body = reader.ReadString(_bodyValueKey);
            Fall = reader.ReadString(_fallValueKey);
            Jump = reader.ReadString(_jumpValueKey);
            Punch = reader.ReadString(_punchValueKey);
            Stand = reader.ReadString(_standValueKey);
            Walk = reader.ReadString(_walkValueKey);
            Size = reader.ReadVector2(_sizeValueKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyInfo"/> class.
        /// </summary>
        /// <param name="id">The <see cref="BodyID"/>.</param>
        public BodyInfo(BodyID id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="SkeletonBody"/> (when using <see cref="SkeletonCharacterSprite"/>) or
        /// sprite category (when using <see cref="GrhCharacterSprite"/>) to use for this body.
        /// </summary>
        /// <remarks>
        /// The setter should only be used in the editor.
        /// </remarks>
        [Browsable(true)]
        [Description(
            "The name of the SkeletonBody (when using SkeletonCharacterSprite) or sprite category (when using GrhCharacterSprite)" +
            " to use for this body.")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="SkeletonSet"/> (when using <see cref="SkeletonCharacterSprite"/>) or
        /// sprite category (when using <see cref="GrhCharacterSprite"/>) to use for this body when drawing
        /// the character falling.
        /// </summary>
        /// <remarks>
        /// The setter should only be used in the editor.
        /// </remarks>
        [Browsable(true)]
        [Description(
            "The name of the SkeletonSet (when using SkeletonCharacterSprite) or sprite category (when using GrhCharacterSprite)" +
            " to use for this body when drawing the character falling.")]
        public string Fall { get; set; }

        /// <summary>
        /// Gets the <see cref="BodyID"/> of this body.
        /// </summary>
        [Browsable(true)]
        [Description("The unique ID of this body.")]
        public BodyID ID { get; private set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="SkeletonSet"/> (when using <see cref="SkeletonCharacterSprite"/>) or
        /// sprite category (when using <see cref="GrhCharacterSprite"/>) to use for this body when drawing
        /// the character jumping.
        /// </summary>
        /// <remarks>
        /// The setter should only be used in the editor.
        /// </remarks>
        [Browsable(true)]
        [Description(
            "The name of the SkeletonSet (when using SkeletonCharacterSprite) or sprite category (when using GrhCharacterSprite)" +
            " to use for this body when drawing the character jumping.")]
        public string Jump { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="SkeletonSet"/> (when using <see cref="SkeletonCharacterSprite"/>) or
        /// sprite category (when using <see cref="GrhCharacterSprite"/>) to use for this body when drawing
        /// the character punching.
        /// </summary>
        /// <remarks>
        /// The setter should only be used in the editor.
        /// </remarks>
        [Browsable(true)]
        [Description(
            "The name of the SkeletonSet (when using SkeletonCharacterSprite) or sprite category (when using GrhCharacterSprite)" +
            " to use for this body when drawing the character punching.")]
        public string Punch { get; set; }

        /// <summary>
        /// Gets or sets the size of the body in pixels. While this should be roughly equal to the size of the body when
        /// drawn, it is not required at all.
        /// </summary>
        /// <remarks>
        /// The setter should only be used in the editor.
        /// </remarks>
        [Browsable(true)]
        [Description("The size of the body in pixels.")]
        public Vector2 Size { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="SkeletonSet"/> (when using <see cref="SkeletonCharacterSprite"/>) or
        /// sprite category (when using <see cref="GrhCharacterSprite"/>) to use for this body when drawing
        /// the character standing still.
        /// </summary>
        /// <remarks>
        /// The setter should only be used in the editor.
        /// </remarks>
        [Browsable(true)]
        [Description(
            "The name of the SkeletonSet (when using SkeletonCharacterSprite) or sprite category (when using GrhCharacterSprite)" +
            " to use for this body when drawing the character standing still.")]
        public string Stand { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="SkeletonSet"/> (when using <see cref="SkeletonCharacterSprite"/>) or
        /// sprite category (when using <see cref="GrhCharacterSprite"/>) to use for this body when drawing
        /// the character walking.
        /// </summary>
        /// <remarks>
        /// The setter should only be used in the editor.
        /// </remarks>
        [Browsable(true)]
        [Description(
            "The name of the SkeletonSet (when using SkeletonCharacterSprite) or sprite category (when using GrhCharacterSprite)" +
            " to use for this body when drawing the character walking.")]
        public string Walk { get; set; }

        /// <summary>
        /// Reads a <see cref="BodyInfo"/> from an <see cref="IValueReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="IValueReader"/> to read from.</param>
        /// <returns>The read <see cref="BodyInfo"/>.</returns>
        public static BodyInfo Read(IValueReader reader)
        {
            return new BodyInfo(reader);
        }

        /// <summary>
        /// Writes this <see cref="BodyInfo"/> to an <see cref="IValueWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="IValueWriter"/> to write to.</param>
        public void Write(IValueWriter writer)
        {
            writer.Write(_idValueKey, ID);
            writer.Write(_bodyValueKey, Body);
            writer.Write(_fallValueKey, Fall);
            writer.Write(_jumpValueKey, Jump);
            writer.Write(_punchValueKey, Punch);
            writer.Write(_standValueKey, Stand);
            writer.Write(_walkValueKey, Walk);
            writer.Write(_sizeValueKey, Size);
        }
    }
}